#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Runs all integration tests.

Options:
    -c <path>      --config <path>         Path to the configuration JSON
    -f <filter>    --filter <filter>       Filter to identify which tests to run
    -h             --help                  Prints this help message and exits
EOF
}

# defaults
FILTER=Integration.Tests

# parameters
while [ -n "$1" ]; do
  case $1 in
  -c | --config)
    shift
    CONFIG_PATH=$1
    ;;
  -f | --filter)
    shift
    FILTER=$1
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

if [ -z "$CONFIG_PATH" ]; then
  echo "No config file provided."
  echo
  usage
  exit 1
fi

HUSH_ASYNC_WARNING="/nowarn:CS1998"

echo "Building integration tests..."
dotnet build test/Integration.Tests/Integration.Tests.csproj $HUSH_ASYNC_WARNING --verbosity:quiet

ABSOLUTE_CONFIG_PATH="$(pwd)/$CONFIG_PATH"

echo "Running integration tests..."
dotnet test test/Integration.Tests/Integration.Tests.csproj \
    --environment CONFIG_PATH="$ABSOLUTE_CONFIG_PATH" \
    --filter "$FILTER" \
    --no-build --verbosity:normal
