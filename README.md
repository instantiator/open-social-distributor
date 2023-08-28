# open-social-distributor

**UNDER DEVELOPMENT**

A tool for managing social posts across multiple platforms. Simple to configure. Offers a CLI for basic configuration and message posting. Optionally run a scheduled service as a Docker container or as an AWS Lambda stack.

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
| `build-applications.sh` | Builds the CLI, lambda function, and service projects |
| `dev-sync-app.sh` | Builds and synchronises the project code with an AWS CloudFormation stack |

## Config tests

Testing scripts are found at the root of the `open-social-distributor-app` directory. The CLI supports configuration tests. Scripts are provided as a shortcut to build and invoke the CLI:

| Script | Purpose |
|-|-|
| `test-connections.sh` | Builds and invokes the CLI to test the connections defined in a configuration file |

## Unit tests

Testing scripts are found at the root of the `open-social-distributor-app` directory.

| Script | Purpose |
|-|-|
| `open-social-distributor-app/run-all-tests.sh` | builds and runs all the unit tests |
| `open-social-distributor-app/run-library-tests.sh` | builds and runs unit tests for `DistributorLib` |
| `open-social-distributor-app/run-cli-tests.sh` | builds and runs the CLI unit tests |
| `open-social-distributor-app/run-function-tests.sh` | builds and runs the lambda function unit tests |
| `open-social-distributor-app/run-service-tests.sh` | builds and runs the service unit tests |

## TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)
- [ ] Publishing code for social networks
    - [x] Console _(not really a network)_
    - [ ] Discord
    - [ ] Twitter
    - [x] Mastodon
    - [x] Facebook
    - [ ] LinkedIn
- [ ] Add CLI features
    - [ ] More complex posting options
    - [ ] Support for image upload
    - [ ] Publish binaries of the CLI
- [ ] Develop scheduled posting projects
    - [ ] DistributionService
    - [ ] DistributionFunction
    - [ ] Docker build and run for the service
    - [ ] Docker build and run for the CLI
- [ ] Document scheduled post structure
- [ ] Offer Discord icons / updates for the status of other posts
