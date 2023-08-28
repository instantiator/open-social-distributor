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

```jsonc
{
    "networks": [
        "type=console",
        "type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>",
        // etc.
    ]
}
```

Each entry in the `networks` list is a connection string describing how to connect to a single social network account.

### Console connection strings

```text
type=console
```

Console is the simplest of the 'social networks' - instead of connecting to a network, it simply prints any messages it has been asked to post to the console or log.

### Mastodon connection strings

```text
type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>
```

A mastodon connection string describes a connection to an account on a specific mastodon server.

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `INSTANCE` - the mastodon server's domain name (eg. `mastodon.social`)
* `TOKEN` - access token, generated in mastodon developer options

Ensure that the _write_ permissions are granted for your access token.

## TODOs

- [x] Self-test during startup
- [x] Social network configuration (connection strings)
- [ ] Publishing code for social networks
    - [ ] Discord
    - [ ] Twitter(?)
    - [x] Mastodon
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
