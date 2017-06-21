param(

    [string] $ikvmc = "c:\tools\ikvm-8.1.5717.0\bin\ikvmc.exe",
    [string] $version = "2.3.5",
    [string] $pre = $null
)

New-Item bin -ItemType Directory -ErrorAction SilentlyContinue | Out-Null

$ikvm_args = @(
    "-target:library",
    "-sharedclassloader",
    "-keyfile:..\featureide.snk",
    "{", "-version:$version", "..\lib\org.sat4j.core.jar", "}",
    "{", "-version:$version", "..\lib\org.sat4j.pb.jar", "}"
)

Write-Output "Compiling jars" | Out-Host

Push-Location bin
& $ikvmc $ikvm_args
Pop-Location

Write-Output "Packing files" | Out-Host

if($pre -ne $null) {
    $version += "-" + $pre
}

nuget pack org.sat4j.core.nuspec -OutputDirectory bin -version $version $pack
nuget pack org.sat4j.pb.nuspec -OutputDirectory bin -version $version $pack
