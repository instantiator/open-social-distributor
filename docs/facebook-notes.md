# Facebook

To post to pages you control, it's sufficient to create an app and request the appropriate scopes (see below) when generating your access tokens.

If you want to post to pages you do not own (ie. on behalf of other businesses or people), you'll need to pass additional approvals by Facebook to be granted advanced API permissions. _This capability is out of scope for this project._

**NB. Facebook removed support for posting to a user's personal feed in 2018.**

## Facebook connection strings

```text
type=facebook;code=<SHORTCODE>;mode=page;page_id=<PAGE_ID>;page_token=<PAGE_TOKEN>
type=facebook;code=<SHORTCODE>;mode=user;user_token=<USER_TOKEN>
```

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `MODE` - set this to: `Page` (case insensitive)

#### `Page` mode

* `PAGE_ID` - Id of the page to post to
* `PAGE_TOKEN` - A [page access token](https://developers.facebook.com/docs/pages/access-tokens) obtained through the process outlined in [Facebook notes](facebook-notes.md)


## Tools

* [Meta for Developers](https://developers.facebook.com/) (Facebook developer console)
* [Graph API Explorer](https://developers.facebook.com/tools/explorer?method=GET&path=me%3Ffields%3Did%2Cname&version=v17.0)
* [Access Token Debugger](https://developers.facebook.com/tools/debug/accesstoken/)

## Scripts

Some of the activities described below are implemented as scripts, found in this repository:

| Script | Purpose |
|-|-|
| `/scripts/fb-get-long-user-access-token.sh` | Exchange an app's short-lived user access token for a long-lived one. |
| `/scripts/fb-get-all-page-access-tokens.sh` | Retrieve all page access tokens for the given user. |

## Access tokens

| Duration | User token | Page token |
|-|-|-|
| Short-lived | 1 hour | 1 hour |
| Long-lived | 60 days | Indefinite |

NB. Although some tokens may last a long time, they can be [invalidated](https://developers.facebook.com/docs/pages/access-tokens#invalidate-a-token) if needs be.

### Get a user access token (explorer)

* Visit: [Graph API Explorer](https://developers.facebook.com/tools/explorer?method=GET&path=me%3Ffields%3Did%2Cname&version=v17.0)
* **Meta App:** Select your Facebook app's name
* **User or Page:** Select `User Token`
* Set permissions: `public_profile`
* Click **Generate Access Token** if the explorer hasn't provided one for you.

A popup dialog will prompt you to grant the public profile permission to the app.

See also: https://developers.facebook.com/docs/pages/access-tokens#get-a-page-access-token

## Get a long-lived user access token (CLI)

You can exchange a short-lived token for a long lived one:

```bash
curl -i -X GET "https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&
  client_id=APP-ID&
  client_secret=APP-SECRET&
  fb_exchange_token=SHORT-LIVED-USER-ACCESS-TOKEN"
```

## Get a page access token (explorer)

* Visit: [Graph API Explorer](https://developers.facebook.com/tools/explorer?method=GET&path=me%3Ffields%3Did%2Cname&version=v17.0)
* **Meta App:** Select your Facebook app's name
* **User or Page:** Select your page
* Set the following permissions:
  * `pages_manage_posts`
  * `pages_read_engagement`
  * `pages_show_list`
* Click **Generate Access Token** if the explorer hasn't provided one for you.

A popup dialog will prompt you to grant these page permissions to the app.

* If you provided a short-lived user access token, your page access token will be short-lived (1 hour).
* If you provided a long-lived user access token, you page access token will be long-lived (indefinite).

## Get a page access token (CLI)

You can exchange your user access token for a page access token:

```bash
curl -i -X GET "https://graph.facebook.com/PAGE-ID?
  fields=access_token&
  access_token=USER-ACCESS-TOKEN"
```

* If you provided a short-lived user access token, your page access token will be short-lived (1 hour).
* If you provided a long-lived user access token, you page access token will be long-lived (indefinite).

## Get all access tokens for your pages

```bash
curl -i -X GET "https://graph.facebook.com/USER-ID/accounts?
  fields=name,access_token&
  access_token=USER-ACCESS-TOKEN"
```

* If you provided a short-lived user access token, your page access token will be short-lived (1 hour).
* If you provided a long-lived user access token, you page access token will be long-lived (indefinite).
