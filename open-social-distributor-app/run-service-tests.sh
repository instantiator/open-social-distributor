#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building service tests..."
dotnet build test/DistributionService.Tests/DistributionService.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
echo

echo "Running service tests..."
dotnet test test/DistributionService.Tests/DistributionService.Tests.csproj --no-build --verbosity:normal
echo
