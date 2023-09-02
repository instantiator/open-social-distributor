#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building library..."
dotnet build src/DistributorLib/DistributorLib.csproj $HUSH_ASYNC_WARNING
echo

echo "Building CLI..."
dotnet build src/DistributionCLI/DistributionCLI.csproj $HUSH_ASYNC_WARNING
echo

echo "Building AWS function..."
dotnet build src/DistributionFunction/DistributionFunction.csproj $HUSH_ASYNC_WARNING
echo

echo "Building service..."
dotnet build src/DistributionService/DistributionService.csproj $HUSH_ASYNC_WARNING
echo

