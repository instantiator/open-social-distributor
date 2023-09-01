#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building function tests..."
dotnet build test/DistributionFunction.Tests/DistributionFunction.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal
echo

echo "Running function tests..."
dotnet test test/DistributionFunction.Tests/DistributionFunction.Tests.csproj --no-build --verbosity:normal
echo
