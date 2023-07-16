#!/bin/bash

set -e
set -o pipefail

echo "Building unit tests..."
HUSH_ASYNC_WARNING="/nowarn:CS1998"
dotnet build test/DistributorLib.Tests/DistributorLib.Tests.csproj $HUSH_ASYNC_WARNING
dotnet build test/DistributionFunction.Tests/DistributionFunction.Tests.csproj $HUSH_ASYNC_WARNING
