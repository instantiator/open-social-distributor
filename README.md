# open-social-distributor

**UNDER DEVELOPMENT**

A tool for managing social posts across multiple platforms. Simple to configure. Offers:

* A CLI for basic configuration and message posting
* A scheduled posting service, as a Docker container or as an AWS stack

## CLI options

```text
DistributionCLI <verb> <options...>
```

### Verbs

```text
  post       Post a message to one or more social networks.
  test       Test all connections in the configuration file.
  help       Display more information on a specific command.
  version    Display version information.
```

## Config format

Configuration is provided through a json file, with details of various connections and settings.

```jsonc
{
    "networks": [
        "type=console;code=<SHORTCODE>",
        "type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>",
        // etc.
    ]
}
```

| Path | Notes | Documentation |
|-|-|-|
| `$.networks` | Each entry in the `networks` list is a connection string describing how to connect to a single social network account. | [connection strings](docs/connection-strings.md) |

## Authorisation tokens

Each social network has its own miserable process for obtaining access tokens that will grant you sufficient permission to post to accounts or pages.

| Network | Notes | Documentation |
|-|-|-|
| Console | Not really a network. No tokens required. | |
| Mastodon | Define an app, get an access token. Remarkably simple. | [mastodon-notes.md](docs/mastodon-notes.md) |
| Facebook | Define an app, get a user access token, exchange it for a page access token. | [facebook-notes.md](docs/facebook-notes.md) |
| LinkedIn | TBC | |
| Twitter | TBC | |
| Discord | TBC | |

## Build and deploy scripts

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
| `docker-build-service.sh` | Under development |
| `docker-run-service.sh` | Under development |

## Config support

Testing scripts are found at the root of the `open-social-distributor-app` directory. The CLI supports configuration tests. Scripts are provided as a shortcut to build and invoke the CLI:

| Script | Purpose |
|-|-|
| `test-connections.sh` | Builds and invokes the CLI to test the connections defined in a configuration file |

## Unit tests

Testing scripts are found at the root of the `open-social-distributor-app` directory.

| Script | Purpose |
|-|-|
| `test-unit-all.sh` | builds and runs the unit tests for each project |
| `test-unit-lib.sh` | builds and runs unit tests for `DistributorLib` |
| `test-unit-cli.sh` | builds and runs the CLI unit tests |
| `test-unit-function.sh` | builds and runs the lambda function unit tests |
| `test-unit-service.sh` | builds and runs the service unit tests |

## TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)
- [ ] Social media features...

| Network | Post simple message | Message formatting | Image upload |
|-|-|-|-|
| Console | ✅ | ✅ | |
| Discord | | | |
| Twitter | | | |
| Mastodon | ✅ | ✅ | |
| Facebook | ✅ | ✅ | |
| LinkedIn | | | |

- [ ] CLI features
    - [ ] More complex posting options
    - [ ] Support for image upload
    - [ ] Publish binaries of the CLI
    - [ ] Docker build, test, and run for the CLI

- [ ] Scheduled posting for service and function
    - [ ] Define message store formats
    - [ ] Document message store formats

- [ ] `DistributionService` (Background Service)
    - [ ] Periodically post from a CSV (drawn from a URL)
    - [ ] Periodically follow more complex post selection rules
    - [ ] Docker build and run for the service

- [ ] `DistributionFunction` (Lambda function)
    - [ ] AWS CloudWatch stack publishing
    - [ ] Store state in config
    - [ ] Periodically post from a CSV (drawn from a URL) when invoked
    - [ ] Periodically follow more complex post selection rules when invoked

- [ ] Offer Discord icons / updates for the status of other posts
