# Open Social Distributor

**UNDER DEVELOPMENT**

This is a tool for managing social posts across multiple platforms. Simple to configure, and able to run as a command line client, or as a service in a Docker container or an AWS CloudFormation stack.

## Current state of development

See: [Outstanding TODOs](todos.md)

### Social networks supported

This project is in development. Additional capabilities are coming soon.

| Network | Posting | Threading | Link | Tags | Images |
|-|-|-|-|-|-|
| Console | ✅ | ✅ | ✅ | ✅ | ✅ |
| Mastodon | ✅ | ✅ | ✅ | ✅ | |
| Facebook | ✅ | ✅ | ✅ | ✅ | |
| Discord | | | | | |
| Twitter | | | | | |
| LinkedIn | | | | | |

### Features

| Component | Feature | Completion |
|-|-|-|
| CLI | Compose and post simple messages | ✅ |
| CLI | Json configuration file | ✅ |
| CLI | Binary release script | ✅ |
| Service | Launch in Docker | ✅ |
| Service | Support for multiple environments | ✅ |
| Service | Configuration mechanism | |
| Service | Schedule and post messages | |
| AWS Stack | Sync stack (for development) | ✅ |
| AWS Stack | Deploy stack (for releases) | |
| Service | Configuration mechanism | |
| AWS Stack | Schedule and post messages | |

## Making posts

You can use the command-line interface (CLI) to make an ad-hoc post to any number of social networks, or launch a service to regularly post from a dataset of posts you provide.

See:

* [CLI options](cli-options.md)
* [Configuration](configuration.md)
* [Connection strings](connection-strings.md)
* [Authorisation tokens](auth-tokens.md)

## Scheduled posting

This tool will also offer 2 ways to set up a scheduled posting service:

1. As a service, running in Docker (in development)
2. As a CloudFormation stack, running on AWS (in development)

You could also automate invocations of the CLI any other way you please from your machine, or build your own tool that uses `DistributorLib`.

## Using the CLI

You can use the command-line interface (CLI) to make an ad-hoc post to any number of social networks, or launch a service to regularly post from a dataset of posts you provide. See:

* [CLI options](cli-options.md)
* [Configuration](configuration.md)
* [Post composition](post-composition.md)

## Social networks

To add networks to your configuration, you'll need to obtain authorisation tokens with permission to post. See:

* [Connection strings](connection-strings.md)
* [Authorisation tokens](auth-tokens.md)

## Developer notes

### Config support

Support scripts are found at the root of the `open-social-distributor-app` directory. The CLI supports configuration tests. Scripts are provided as a shortcut to build and invoke the CLI:

| Script | Purpose |
|-|-|
| `test-connections.sh` | Builds and invokes the CLI to test the connections defined in a configuration file |

### Components

| Component | Purpose |
|-|-|
| `DistributionCLI` | A CLI for basic configuration and message posting |
| `DistributionFunction` | A scheduled posting function, run as an AWS CloudWatch application stack |
| `DistributionService` | A scheduled posting service, runs as a Docker container |
| `DistributorLib` | Shared code with functionality to support the above 3 use cases |

### Build and deploy scripts

Build and deploy scripts are found at the root of the `open-social-distributor-app` directory.

| Script | Purpose |
|-|-|
| `export-aws-defaults.sh` | Exports some default AWS environment variables. |
| `aws-sync-stack.sh` | Builds and synchronises the project code with an AWS CloudFormation stack |
| `aws-deploy-stack.sh` | Under development |

| Script | Purpose |
|-|-|
| `build-all.sh` | Builds dev versions of the CLI, lambda function, and service projects as .NET binaries |
| `release-cli.sh` | Builds release binaries of the CLI for Windows, OS X, and Linux |

| Script | Purpose |
|-|-|
| `docker-run-service.sh` | Builds, tests, and runs the service for the `int` or `prod` environment |

### Unit tests

Testing scripts are found at the root of the `open-social-distributor-app` directory.

| Script | Purpose |
|-|-|
| `test-unit-all.sh` | builds and runs the unit tests for each project |
| `test-unit-lib.sh` | builds and runs unit tests for `DistributorLib` |
| `test-unit-cli.sh` | builds and runs the CLI unit tests |
| `test-unit-function.sh` | builds and runs the lambda function unit tests |
| `test-unit-service.sh` | builds and runs the service unit tests |
