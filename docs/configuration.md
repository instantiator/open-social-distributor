# Configuration

Provide config as a json file, with details of various connections and settings. ie.

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
| `$.networks` | Each entry in the `networks` list is a connection string describing how to connect to a single social network account. | [connection strings](connection-strings.md) |

## Testing config

Use the `test-connections.sh` script to build and invoke the command line interface `test` on a given configuration file.
