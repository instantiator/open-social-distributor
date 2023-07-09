#!/bin/bash

set -e
set -o pipefail

echo "Building functions..."
dotnet build src/DistributionFunction/DistributionFunction.csproj 
