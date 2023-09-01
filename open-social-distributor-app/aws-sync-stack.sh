#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Synchronises this app with an AWS CloudFormation stack.

Options:
    -a <profile>   --aws-profile <profile> Sets the profile to use (see: ~/.aws/config and aws-vault list)
    -s <stack>     --stack <stack>         Sets the stack name
    -h             --help                  Prints this help message and exits
EOF
}

# defaults
source ./export-aws-defaults.sh

# parameters
while [ -n "$1" ]; do
  case $1 in
  -a | --aws-profile)
    shift
    PROFILE=$1
    ;;
  -s | --stack)
    shift
    STACK_NAME=$1
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

if [ -z "$PROFILE" ]; then
  echo "No AWS profile provided."
  echo
  usage
  exit 1
fi

if [ -z "$STACK_NAME" ]; then
  echo "No stack name provided."
  echo
  usage
  exit 1
fi

echo "Profile: $PROFILE"
echo "Stack:   $STACK_NAME"

echo "Syncing app with stack..."
sam sync --stack-name $STACK_NAME --profile $PROFILE --region $REGION --watch
