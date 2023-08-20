#!/bin/bash

set -e
set -o pipefail

echo "Building CLI tests..."
dotnet build test/DistributionCLI.Tests/DistributionCLI.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
echo

echo "Running CLI tests..."
dotnet test test/DistributionCLI.Tests/DistributionCLI.Tests.csproj --no-build --verbosity:normal
echo
