#!/bin/bash

set -e
set -o pipefail

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building integration tests..."
dotnet build test/Integration.Tests/Integration.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:minimal

echo "Running integration tests..."
dotnet test test/Integration.Tests/Integration.Tests.csproj --no-build --verbosity:normal
