#Requires -Version 5.1

param(
  [Parameter()]
  [string] $version = "2.3.6",
  [Parameter()]
  [string] $assemblyversion = "2.0.0",
  [Parameter()]
  [string] $pre = $null,

  [Parameter()]
  [switch] $all
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'

$target = "bin"

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
  $file = "$env:TEMP/${name}-${version}.jar"
  if (!(Test-Path $file)) {
    Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.core/$version/org.ow2.sat4j.core-$version.jar" -OutFile $file
  }
}

function build-assembly {
  param (
    [Parameter(Mandatory)]
    [ValidateSet("net461", "netcoreapp3.1")]
    [string] $tfm
  )

  $tgt = New-Item $target/$tfm -ItemType Directory -Force
  Copy-Item $env:TEMP/org.sat4j.core-${version}.jar -Destination "$tgt/org.sat4j.core.jar"
  Copy-Item $env:TEMP/org.sat4j.pb-${version}.jar -Destination "$tgt/org.sat4j.pb.jar"
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

Write-Output "Downloading jars" | Out-Host
get-jar -name org.sat4j.core
get-jar -name org.sat4j.pb


if ($pre) {
  $version += "-" + $pre
}


Write-Output "Compiling jars" | Out-Host

build-assembly -tfm net461

if ($all) {
  build-assembly -tfm netcoreapp3.1
}

Write-Output "Packing files" | Out-Host


nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure

nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure
