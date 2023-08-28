#!/bin/bash

set -e
set -o pipefail

usage() {
  cat << EOF
Exchange a short-lived user access token for a long-lived user access token.

Options:
    -a <id>       --app-id <id>          The app ID
    -s <secret>   --app-secret <secret>  The app secret
    -t <token>    --token <token>        A short-lived user access token
    -h            --help                 Prints this help message and exits
EOF
}

# parameters
while [ -n "$1" ]; do
  case $1 in
  -a | --app-id)
    shift
    APP_ID=$1
    ;;
  -s | --app-secret)
    shift
    APP_SECRET=$1
    ;;
  -t | --token)
    shift
    SHORT_USER_TOKEN=$1
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

if [ -z "$APP_ID" ]; then
  echo "Please provide the app ID."
  echo
  usage
  exit 1
fi

if [ -z "$APP_SECRET" ]; then
  echo "Please provide the app secret."
  echo
  usage
  exit 1
fi

if [ -z "$SHORT_USER_TOKEN" ]; then
  echo "Please provide a short-lived user token."
  echo
  usage
  exit 1
fi

DATA=$(curl -s -X GET "https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=$APP_ID&client_secret=$APP_SECRET&fb_exchange_token=$SHORT_USER_TOKEN")
echo $DATA | jq .
