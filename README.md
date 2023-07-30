# open-social-distributor

**UNDER DEVELOPMENT**

A tool for managing social posts across multiple platforms. Simple to configure, with options to run through as a Docker container or as an AWS Lambda function.

## TODOs

- [ ] Social network configuration
- [ ] Publishing code for social networks
    - [ ] Discord
    - [ ] Twitter(?)
    - [ ] Mastodon
    - [ ] Facebook
    - [ ] LinkedIn
- [ ] Document CSV structure
- [ ] Have Discord indicate status of other posts
- [ ] Add scheduled event trigger
- [ ] Create CLI distribution option

## Build and deploy scripts

* `open-social-distributor-app/export-defaults.sh` - defaults for `PROFILE` (AWS CLI), and `STACK_NAME` (CloudFormation stack)
* `open-social-distributor-app/build-functions.sh` - builds all functions in this project
* `open-social-distributor-app/dev-sync-app.sh` - synchronises all functions in this project with an AWS CloudFormation stack

## Testing scripts

* `open-social-distributor-app/build-unit-tests.sh` - builds all unit tests in this project
* `open-social-distributor-app/run-unit-tests.sh` - runs all unit tests in this project
