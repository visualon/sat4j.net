#Requires -Version 5.1

param(

    [string] $version = "2.3.5",
    [string] $assemblyversion = "2.0.0",
    [string] $pre = $null
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

New-Item bin -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

$baseUri = "https://repository.ow2.org/nexus/service/local/repositories/releases/content/org/ow2/sat4j"


Write-Output "Downloading jars" | Out-Host
Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.core/$version/org.ow2.sat4j.core-$version.jar" -OutFile bin/org.ow2.sat4j.core.jar
Invoke-WebRequest -URI "$baseUri/org.ow2.sat4j.pb/$version/org.ow2.sat4j.pb-$version.jar" -OutFile bin/org.ow2.sat4j.core.jar

if($pre) {
    $version += "-" + $pre
}

$ikvm_args = @(
    "-target:library",
    "-classloader:ikvm.runtime.AppDomainAssemblyClassLoader",
    "-keyfile:..\featureide.snk",
    "-version:$assemblyversion",
    "-fileversion:$version",
    "{", , "..\bin\org.sat4j.core.jar", "}",
    "{", "..\bin\org.sat4j.pb.jar", "}"
)

Write-Output "Compiling jars" | Out-Host

Push-Location bin
ikvmc $ikvm_args
Pop-Location

Write-Output "Packing files" | Out-Host


nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version
nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version
