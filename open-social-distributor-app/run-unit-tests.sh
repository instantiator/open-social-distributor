#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Runs all unit tests for the specified project (or all projects if none specified).

Options:
    -p <project>   --project <project>     Project to test: lib, cli, function, service
    -h             --help                  Prints this help message and exits
EOF
}

# defaults
PROJECT="all"

# parameters
while [ -n "$1" ]; do
  case $1 in
  -p | --project)
    shift
    PROJECT=$1
    ;;
  -h | --help)
    usage
    exit 0
    ;;
  *)
    echo -e "Unknown option $1...\n"
    usage
    exit 1
    ;;
  esac
  shift
done

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building projects..."
dotnet build test/DistributorLib.Tests/DistributorLib.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
dotnet build test/DistributionCLI.Tests/DistributionCLI.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
dotnet build test/DistributionFunction.Tests/DistributionFunction.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
dotnet build test/DistributionService.Tests/DistributionService.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
echo

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "lib" ]; then
  echo "Running unit tests for lib..."
  dotnet test test/DistributorLib.Tests/DistributorLib.Tests.csproj --no-build --verbosity:normal
  echo
fi

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "cli" ]; then
  echo "Running unit tests for cli..."
  dotnet test test/DistributionCLI.Tests/DistributionCLI.Tests.csproj --no-build --verbosity:normal
  echo
fi

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "function" ]; then
  echo "Running unit tests for function..."
  dotnet test test/DistributionFunction.Tests/DistributionFunction.Tests.csproj --no-build --verbosity:normal
  echo
fi

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "service" ]; then
  echo "Running unit tests for service..."
  dotnet test test/DistributionService.Tests/DistributionService.Tests.csproj --no-build --verbosity:normal
  echo
fi
