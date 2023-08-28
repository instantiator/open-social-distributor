#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Exchange a user access token for all page access tokens.

You must provide a user access token with the following permissions:
- manage_pages
- pages_show_list
- pages_read_engagement

Options:
    -u <id>       --user-id <id>         The user ID
    -t <token>    --token <token>        The user access token
    -h            --help                 Prints this help message and exits
EOF
}

# parameters
while [ -n "$1" ]; do
  case $1 in
  -u | --user-id)
    shift
    USER_ID=$1
    ;;
  -t | --token)
    shift
    USER_TOKEN=$1
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

if [ -z "$USER_ID" ]; then
  echo "Please provide the user ID."
  echo
  usage
  exit 1
fi

if [ -z "$USER_TOKEN" ]; then
  echo "Please provide the user access token."
  echo
  usage
  exit 1
fi

DATA=$(curl -s -X GET "https://graph.facebook.com/$USER_ID/accounts?fields=name,access_token&access_token=$USER_TOKEN")
echo $DATA | jq .
