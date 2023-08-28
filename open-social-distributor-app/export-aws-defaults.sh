#!/bin/bash

set -e
set -o pipefail
set -o allexport

echo "Exporting default AWS values..."
PROFILE=sa-social-distributor
STACK_NAME=open-social-distributor
REGION=eu-west-2
