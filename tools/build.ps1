#Requires -Version 5.1

param(
  [Parameter()]
  [string] $version
)

# renovate: datasource=maven depName=sat4j packageName=org.ow2.sat4j:org.ow2.sat4j.pom
$SAT4J_VERSION = "2.3.6"


$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'

$target = "bin"

if ($env:GITHUB_REF_TYPE -eq 'tag' ) {
  $version = $env:GITHUB_REF_NAME
}

if (!$version) {
  $version = $SAT4J_VERSION
}

# trim leading v
$version = $version -replace "^v?", ""

$major, $minor, $patch = $version.Split('-')[0].Split('.')
$jarVersion = $version
$assemblyversion = "$major.0.0.0"

if ($patch.Length -ge 3) {
  $patch = $patch.Substring(0, $patch.Length - 2)
}

$jarVersion = "$major.$minor.$patch"

if (Test-Path $target) {
  Remove-Item $target -Recurse -Force
}
New-Item $target -ItemType Directory -Force | Out-Null

function ThrowOnNativeFailure {
  if (-not $?) {
    throw 'Native Failure'
  }
}


$baseUri = "https://repository.ow2.org/nexus/content/repositories/releases/org/ow2/sat4j"

function get-jar {
  param (
    [Parameter(Mandatory)]
    [ValidateSet("core", "pb")]
    [string] $name
  )
  $file = "$env:TEMP/org.sat4j.${name}-${jarVersion}.jar"
  if (!(Test-Path $file)) {
    Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.$name/$jarVersion/org.ow2.sat4j.$name-$jarVersion.jar" -OutFile $file
  }
}

function copy-jar {
  param (
    [Parameter(Mandatory)]
    [ValidateSet("core", "pb")]
    [string] $name
  )
  Copy-Item $env:TEMP/org.sat4j.$name-${jarVersion}.jar -Destination "$tgt/org.sat4j.$name.jar"
}

function build-assembly {
  param (
    [Parameter(Mandatory)]
    [ValidateSet("net461", "netcoreapp3.1")]
    [string] $tfm
  )

  $tgt = New-Item $target/$tfm -ItemType Directory -Force
  copy-jar -name core
  copy-jar -name pb
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

Write-Output "Downloading jars for version $jarVersion" | Out-Host
get-jar -name core
get-jar -name pb

Write-Output "Compiling jars for version $version" | Out-Host

build-assembly -tfm net461
build-assembly -tfm netcoreapp3.1

Write-Output "Packing files" | Out-Host


nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure

nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure
