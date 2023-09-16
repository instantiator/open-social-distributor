# Mastodon

The process for obtaining a Mastodon access token is relatively simple as compared to the other social networks!

## Mastodon connection strings

```text
type=mastodon;code=<SHORTCODE>;instance=<INSTANCE>;token=<TOKEN>
```

A mastodon connection string describes a connection to an account on a specific mastodon server.

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `INSTANCE` - the mastodon server's domain name (eg. `mastodon.social`)
* `TOKEN` - access token, obtained through the process outlined below

Ensure that the _write_ permissions are granted for your access token.

## Access tokens

* Open **Settings** on your Mastodon server, and choose **Development**
* Create an application
* Select scopes:
  * `read:accounts`
  * `read:statuses`
  * `write:statuses`
* Retrieve the access token provided for your application
