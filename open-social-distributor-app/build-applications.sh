#!/bin/bash

set -e
set -o pipefail

echo "Building functions..."
HUSH_ASYNC_WARNING="/nowarn:CS1998"
dotnet build src/DistributionFunction/DistributionFunction.csproj $HUSH_ASYNC_WARNING
dotnet build src/DistributionService/DistributionService.csproj $HUSH_ASYNC_WARNING
dotnet build src/DistributionCLI/DistributionCLI.csproj $HUSH_ASYNC_WARNING
