﻿param(

    [string] $ikvmc = "c:\tools\ikvm-8.1.5717.0\bin\ikvmc.exe",
    [string] $version = "2.3.5",
    [string] $assemblyversion = "2.0.0",
    [string] $pre = $null
)

New-Item bin -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

if($pre) {
    $version += "-" + $pre
}

$ikvm_args = @(
    "-target:library",
    "-classloader:ikvm.runtime.AppDomainAssemblyClassLoader",
    "-keyfile:..\featureide.snk",
    "-version:$assemblyversion",
    "-fileversion:$version",
    "{", , "..\lib\org.sat4j.core.jar", "}",
    "{", "..\lib\org.sat4j.pb.jar", "}"
)

Write-Output "Compiling jars" | Out-Host

Push-Location bin
& $ikvmc $ikvm_args
Pop-Location

Write-Output "Packing files" | Out-Host


nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version
nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version
