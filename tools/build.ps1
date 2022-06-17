#Requires -Version 5.1

param(
  [string] $version = "2.3.6",
  [string] $assemblyversion = "2.0.0",
  [string] $pre = $null
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'
function ThrowOnNativeFailure {
  if (-not $?) {
    throw 'Native Failure'
  }
}

New-Item bin -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

$baseUri = "https://repository.ow2.org/nexus/service/local/repositories/releases/content/org/ow2/sat4j"


Write-Output "Downloading jars" | Out-Host
Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.core/$version/org.ow2.sat4j.core-$version.jar" -OutFile bin/org.sat4j.core.jar
Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.pb/$version/org.ow2.sat4j.pb-$version.jar" -OutFile bin/org.sat4j.pb.jar

if ($pre) {
  $version += "-" + $pre
}

$ikvm_args = @(
  "-target:library",
  "-classloader:ikvm.runtime.AppDomainAssemblyClassLoader",
  "-keyfile:..\featureide.snk",
  "-version:$assemblyversion",
  "-fileversion:$version",
  "{", , "org.sat4j.core.jar", "}",
  "{", "org.sat4j.pb.jar", "}"
)

Write-Output "Compiling jars" | Out-Host

try {
  Push-Location bin
  ../bin/ikvmc $ikvm_args
  ThrowOnNativeFailure
}
finally {
  Pop-Location
}

Write-Output "Packing files" | Out-Host


nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure

nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version
ThrowOnNativeFailure
