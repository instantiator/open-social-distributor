# open-social-distributor

**UNDER DEVELOPMENT**

A tool for managing social posts across multiple platforms. Simple to configure. Offers a CLI for basic configuration and message posting. Optionally run a scheduled service as a Docker container or as an AWS Lambda stack.

## CLI options

```text
-c, --config     Required. Path to the config file.
-m, --message    Required. Simple message text.
--help           Display this help screen.
--version        Display version information.
```

## Config format

Configuration is provided as a json file, with details of various connections and settings.

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
    - [ ] Console
    - [ ] Discord
    - [ ] Twitter
    - [x] Mastodon
    - [ ] Facebook
    - [ ] LinkedIn
- [ ] Document CSV structure
- [ ] Have Discord indicate status of other posts
- [ ] Add scheduled event trigger
- [ ] Create CLI distribution option
