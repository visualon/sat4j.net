#Requires -Version 5.1

$ErrorActionPreference = 'Stop';

# renovate: datasource=nuget depName=IKVM
$IKVM_VERSION="8.2.0-prerelease.809"

$url = "https://github.com/ikvm-revived/ikvm/releases/download/${IKVM_VERSION}/IKVM-${IKVM_VERSION}-tools-net461-any.zip"
$file = "./bin/ikvm-${IKVM_VERSION}.zip"
$target = "./bin"

New-Item $target -ItemType Directory -Force | Out-Null

if (!(Test-Path $file)) {
  Invoke-WebRequest $url -OutFile $file
  Expand-Archive -Path $file -DestinationPath $target
}

./bin/ikvm -version
