#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building all tests..."
dotnet build test/DistributorLib.Tests/DistributorLib.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
dotnet build test/DistributionCLI.Tests/DistributionCLI.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
dotnet build test/DistributionFunction.Tests/DistributionFunction.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
dotnet build test/DistributionService.Tests/DistributionService.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal

echo "Running all tests..."
dotnet test test/DistributorLib.Tests/DistributorLib.Tests.csproj --no-build --verbosity:normal
dotnet test test/DistributionCLI.Tests/DistributionCLI.Tests.csproj --no-build --verbosity:normal
dotnet test test/DistributionFunction.Tests/DistributionFunction.Tests.csproj --no-build --verbosity:normal
dotnet test test/DistributionService.Tests/DistributionService.Tests.csproj --no-build --verbosity:normal
