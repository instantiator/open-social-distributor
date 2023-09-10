# Connection strings

## Config format

The configuration file contains a list of network connection strings, ie.

```jsonc
{
    "networks": [
        "type=console;code=<SHORTCODE>",
        "type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>",
        // etc.
    ]
}
```

Each entry in the `networks` list is a connection string describing how to connect to a single social network account.

Many connection strings require an authorisation token from the social network they connect to. See:

* [Authorisation tokens](auth-tokens.md)

### Console connection strings

```text
type=console
```

Console is the simplest of the 'social networks' - instead of connecting to a network, it simply prints any messages it has been asked to post to the console or log.

Console is the only "network" that does not require you to specify a shortcode. You should not create more than one instance of console.

### Mastodon connection strings

```text
type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>
```

A mastodon connection string describes a connection to an account on a specific mastodon server.

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `INSTANCE` - the mastodon server's domain name (eg. `mastodon.social`)
* `TOKEN` - access token, obtained through the process outlined in [Mastodon notes](mastodon-notes.md)

Ensure that the _write_ permissions are granted for your access token.

### Facebook connection strings

```text
type=facebook;code=<SHORTCODE>;mode=page;page_id=<PAGE_ID>;page_token=<PAGE_TOKEN>
type=facebook;code=<SHORTCODE>;mode=user;user_token=<USER_TOKEN>
```

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `MODE` - One of `Page|User` (case insensitive)

#### `Page` mode

* `PAGE_ID` - Id of the page to post to
* `PAGE_TOKEN` - A [page access token](https://developers.facebook.com/docs/pages/access-tokens) obtained through the process outlined in [Facebook notes](facebook-notes.md)

#### `User` mode (not yet implemented)

* `USER_TOKEN` - A user access token obtained through the process outlined in [Facebook notes](facebook-notes.md)

### LinkedIn connection strings

```text
type=linkedin;code=<SHORTCODE>;client_id=<CLIENT_ID>;client_secret=<CLIENT_SECRET>;mode=org;author_id=<ORG_ID>;token=<TOKEN>
type=linkedin;code=<SHORTCODE>;client_id=<CLIENT_ID>;client_secret=<CLIENT_SECRET>;mode=user;author_id=<USER_ID>;token=<TOKEN>
```

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `MODE` - One of `Org|User` (case insensitive)
* `CLIENT_ID` - Id of the LinkedIn application that the access token was created for (used to introspect/test the token)
* `CLIENT_SECRET` - Secret of the LinkedIn application that the access token was created for (used to introspect/test the token)
* `TOKEN` - An access token, obtained through the process outlined in [LinkedIn notes](linkedin-notes.md).
* 

#### `Org` mode

* `ORG_ID` - Id of the LinkedIn org (not the full urn)

#### `User` mode (not yet implemented)

* `USER_ID` - Id of the user (not the full urn)

## Twitter connection strings

*Under development...*

## Discord connection strings

*Under development...*

