# Configuration

Provide config as a json file, with details of various connections and settings. ie.

```jsonc
{
    "networks": {
        "enabled": [
            "type=console;code=<SHORTCODE>",
            "type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>",
            // etc.
        ],
        "disabled": []
    }
}
```

| Path | Notes | Documentation |
|-|-|-|
| `$.networks.enabled` | Each entry in the list is a connection string describing how to connect to a single social network account. | [connection strings](connection-strings.md) |
| `$.networks.disabled` | Connection strings to ignore |

## Testing config

Use the `test-connections.sh` script to build and invoke the command line interface `test` on a given configuration file.
