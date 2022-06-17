#Requires -Version 5.1

param(
  [Parameter()]
  [string] $version = "2.3.600"
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'

$target = "bin"

if ($env:GITHUB_REF_TYPE -eq 'tag' ) {
  $version = $env:GITHUB_REF_NAME
}

$parts = $version.Split('.')

$sat4jVersion = $version
$assemblyversion = "$($parts[0]).0.0.0"
$sat4jVersion = "$($parts[0]).$($parts[1]).$($parts[2].Substring(0, $parts[2].Length -2))"

if (Test-Path $target) {
  Remove-Item $target -Recurse -Force
}
New-Item $target -ItemType Directory -Force | Out-Null

function ThrowOnNativeFailure {
  if (-not $?) {
    throw 'Native Failure'
  }
}


$baseUri = "https://repository.ow2.org/nexus/service/local/repositories/releases/content/org/ow2/sat4j"

function get-jar {
  param (
    [Parameter(Mandatory)]
    [ValidateSet("org.sat4j.core", "org.sat4j.pb")]
    [string] $name
  )
  $file = "$env:TEMP/${name}-${sat4jVersion}.jar"
  if (!(Test-Path $file)) {
    Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.core/$sat4jVersion/org.ow2.sat4j.core-$sat4jVersion.jar" -OutFile $file
  }
}

function build-assembly {
  param (
    [Parameter(Mandatory)]
    [ValidateSet("net461", "netcoreapp3.1")]
    [string] $tfm
  )

  $tgt = New-Item $target/$tfm -ItemType Directory -Force
  Copy-Item $env:TEMP/org.sat4j.core-${sat4jVersion}.jar -Destination "$tgt/org.sat4j.core.jar"
  Copy-Item $env:TEMP/org.sat4j.pb-${sat4jVersion}.jar -Destination "$tgt/org.sat4j.pb.jar"
  Copy-Item tools/ikvm/$tfm/* -Destination $tgt -Recurse

  $ikvm_args = @(
    "-target:library",
    "-classloader:ikvm.runtime.AppDomainAssemblyClassLoader",
    "-keyfile:../../featureide.snk",
    "-version:$assemblyversion",
    "-fileversion:$version"
  )

  if ($tfm -eq "netcoreapp3.1") {
    $ikvm_args += "-nostdlib", "-r:./refs/*.dll"
  }


  $ikvm_args += "{", "org.sat4j.core.jar", "}"
  $ikvm_args += "{", "org.sat4j.pb.jar", "}"

  try {
    Push-Location $tgt
    . ./ikvmc $ikvm_args
    ThrowOnNativeFailure
  }
  finally {
    Pop-Location
  }
}

Write-Output "Downloading jars for version $sat4jVersion" | Out-Host
get-jar -name org.sat4j.core
get-jar -name org.sat4j.pb


if ($pre) {
  $version += "-" + $pre
}


Write-Output "Compiling jars for version $version" | Out-Host

build-assembly -tfm net461

if ($all) {
  build-assembly -tfm netcoreapp3.1
}

Write-Output "Packing files" | Out-Host


nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure

nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure
