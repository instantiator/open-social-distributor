#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building CLI..."
dotnet build src/DistributionCLI/DistributionCLI.csproj $HUSH_ASYNC_WARNING
echo

for OS in win linux osx
do
    echo "Publishing $OS release..."
    mkdir -p "release/${OS}-x64"
    dotnet publish src/DistributionCLI/DistributionCLI.csproj \
        -c Release \
        --os $OS \
        /p:PublishSingleFile=true \
        /p:CopyOutputSymbolsToPublishDirectory=false \
        --self-contained false \
        --output "release/${OS}-x64"
    chmod +x release/${OS}-x64/*
    echo
done
