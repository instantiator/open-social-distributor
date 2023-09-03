#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Tests all connection strings listed in a config file.

Options:
    -c <path>      --config <path>         Path to the configuration file
    -h             --help                  Prints this help message and exits
EOF
}

# parameters
while [ -n "$1" ]; do
  case $1 in
  -c | --config)
    shift
    CONFIG_PATH=$1
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

echo "Config file path: $CONFIG_PATH"
echo

echo "Building CLI..."
HUSH_ASYNC_WARNING="/nowarn:CS1998"
dotnet build src/DistributionCLI/DistributionCLI.csproj $HUSH_ASYNC_WARNING
echo

echo "Executing initialisation and self-test..."
dotnet run --project src/DistributionCLI/DistributionCLI.csproj test --all --config $CONFIG_PATH
