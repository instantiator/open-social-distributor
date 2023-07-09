#!/bin/bash

set -e
set -o pipefail

echo "Building unit tests..."
dotnet build test/DistributionFunction.Tests/DistributionFunction.Tests.csproj
