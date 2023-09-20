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

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "lib" ]; then
    dotnet build test/DistributorLib.Tests/DistributorLib.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
    dotnet test test/DistributorLib.Tests/DistributorLib.Tests.csproj --no-build --verbosity:normal
fi

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "cli" ]; then
    dotnet build test/DistributionCLI.Tests/DistributionCLI.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
    dotnet test test/DistributionCLI.Tests/DistributionCLI.Tests.csproj --no-build --verbosity:normal
fi

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "function" ]; then
    dotnet build test/DistributionFunction.Tests/DistributionFunction.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
    dotnet test test/DistributionFunction.Tests/DistributionFunction.Tests.csproj --no-build --verbosity:normal
fi

if [ "$PROJECT" = "all" ] || [ "$PROJECT" = "service" ]; then
    dotnet build test/DistributionService.Tests/DistributionService.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet
    dotnet test test/DistributionService.Tests/DistributionService.Tests.csproj --no-build --verbosity:normal
fi
