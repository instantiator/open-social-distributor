#!/bin/bash

set -e
set -o pipefail

echo "Building library tests..."
dotnet build test/DistributorLib.Tests/DistributorLib.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
echo

echo "Running library tests..."
dotnet test test/DistributorLib.Tests/DistributorLib.Tests.csproj --no-build --verbosity:normal
echo
