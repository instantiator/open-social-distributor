# Connection strings

## Config format

The configuration file contains a list of network connection strings, ie.

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

Each entry in the `$.networks.enabled` list is a connection string describing how to connect to a single social network account.

Many connection strings require an authorisation token from the social network they connect to. See:

* [Authorisation tokens](auth-tokens.md)

## Social network connection strings

Each social network has its own connection string parameters:

* [Mastodon notes](mastodon-notes.md)
* [Facebook notes](facebook-notes.md)
* [LinkedIn notes](linkedin-notes.md)
* [Discord notes](discord-notes.md)
* Twitter notes

### Console connection strings

```text
type=console
```

Console is the simplest of the 'social networks' - instead of connecting to a network, it simply prints any messages it has been asked to post to the console or log.

Console is the only "network" that does not require you to specify a shortcode. You shouldn't need to create more than one instance of console.
