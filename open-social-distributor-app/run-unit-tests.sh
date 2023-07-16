#!/bin/bash

echo "Running unit tests..."
dotnet test test/DistributorLib.Tests/DistributorLib.Tests.csproj
dotnet test test/DistributionFunction.Tests/DistributionFunction.Tests.csproj
