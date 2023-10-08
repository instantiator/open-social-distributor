#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building library..."
dotnet clean src/DistributorLib/DistributorLib.csproj
dotnet build src/DistributorLib/DistributorLib.csproj $HUSH_ASYNC_WARNING
echo

echo "Building CLI..."
dotnet clean src/DistributionCLI/DistributionCLI.csproj
dotnet build src/DistributionCLI/DistributionCLI.csproj $HUSH_ASYNC_WARNING
echo

echo "Building AWS function..."
dotnet clean src/DistributionFunction/DistributionFunction.csproj
dotnet build src/DistributionFunction/DistributionFunction.csproj $HUSH_ASYNC_WARNING
echo

echo "Building service..."
dotnet clean src/DistributionService/DistributionService.csproj
dotnet build src/DistributionService/DistributionService.csproj $HUSH_ASYNC_WARNING
echo

