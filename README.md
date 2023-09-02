# open-social-distributor

**UNDER DEVELOPMENT**

A tool for managing social posts across multiple platforms. Simple to configure.

## Documentation

* [Open Social Distributor](docs/index.md)

## Making posts

You can use the command-line interface (CLI) to make an ad-hoc post to any number of social networks, or launch a service to regularly post from a dataset of posts you provide.

See:

* [CLI options](docs/cli-options.md)
* [Configuration](docs/configuration.md)
* [Connection strings](docs/connection-strings.md)
* [Authorisation tokens](docs/auth-tokens.md)

## Scheduled posting

This tool will also offers 2 ways to set up a scheduled posting service:

1. As a service, running in Docker (in development)
2. As a CloudFormation stack, running on AWS (in development)

You could also automate invocations of the CLI any other way you please from your machine.

## TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)
- [ ] Social media features...

| Network | Post simple message | Message formatting | Images |
|-|-|-|-|
| Console | ✅ | ✅ | ✅ |
| Mastodon | ✅ | ✅ | |
| Facebook | ✅ | ✅ | |
| Discord | | | |
| Twitter | | | |
| LinkedIn | | | |

- [ ] CLI features
    - [x] More complex posting options
    - [x] Support for image upload
    - [x] Publish binaries of the CLI

- [ ] Scheduled posting for service and function
    - [ ] Define message store formats
    - [ ] Document message store formats

- [ ] `DistributionService` (Background Service)
    - [ ] Periodically post from CSV or JSON source (drawn from a URL or file)
    - [ ] Periodically follow more complex post selection rules
    - [x] Docker build and run for the service

- [ ] `DistributionFunction` (Lambda function)
    - [x] AWS CloudWatch stack sync for development
    - [ ] AWS CloudWatch stack publishing
    - [ ] Store state in config
    - [ ] Periodically post from CSV or JSON source (drawn from a URL, S3 bucket, or other source)
    - [ ] Periodically follow more complex post selection rules when invoked

- [ ] Offer Discord icons / updates for the status of other posts
