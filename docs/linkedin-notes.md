# LinkedIn

To post to organisation pages, you'll need create an app in the [Developers Portal](https://developer.linkedin.com/) and then request access to the Community Management API.

To be granted access, you'll need to link your app to a LinkedIn organisation, and then get that organisation verified.

* In App Settings, associate the app to your organisation's page
* Request access to the Community Management API
* Shortly after, you'll have access to an application form
* Submit a request for verification of your organisation

LinkedIn look for various things when verifying an organisation. It shouldn't be too painful, particularly if you're a registered organisation in your country. (We had some difficulty with [Front-Line Tech Ltd](https://front-line-tech.com), but there's an appeal process, and we were able to show additional documentation to help).

See also:

* [Associate an App with a LinkedIn Page](https://www.linkedin.com/help/linkedin/answer/a548360)

## Quotas

By default, your app will be allowed:

* 10 posts per user per day
* A cap of 100 posts per day

## LinkedIn connection strings

```text
type=linkedin;code=<SHORTCODE>;client_id=<CLIENT_ID>;client_secret=<CLIENT_SECRET>;mode=org;author_id=<ORG_ID>;token=<TOKEN>
type=linkedin;code=<SHORTCODE>;client_id=<CLIENT_ID>;client_secret=<CLIENT_SECRET>;mode=user;author_id=<PERSON_ID>;token=<TOKEN>
```

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `MODE` - One of `Org|User` (case insensitive)
* `CLIENT_ID` - Id of the LinkedIn application that the access token was created for (used to introspect/test the token)
* `CLIENT_SECRET` - Secret of the LinkedIn application that the access token was created for (used to introspect/test the token)
* `TOKEN` - An access token, obtained through the process outlined in [LinkedIn notes](linkedin-notes.md).

#### `Org` mode

* `ORG_ID` - The id portion of the LinkedIn URN for _the organization to post as_

#### `User` mode (not yet implemented)

* `PERSON_ID` - The id portion of the LinkedIn URN for _the person to post as_

Person ids look like a 10-character string of letters and numbers, eg. `vl4hy9pJ3S`

To obtain the `PERSON_ID`, put any value for`author_id` in the connection string, and run a test of your connection using the CLI.

```bash
DistributionCLI test -c your/config/file.json
```

Provided your token, client id and client secret are correct, the output from the test contains the person id you'll need for `User` mode.

## Tools

For more information, see: [LinkedIn OAuth Tools](https://www.linkedin.com/developers/tools/oauth)

* [LinkedIn Token Generator](https://www.linkedin.com/developers/tools/oauth/token-generator)
* [LinkedIn Token Inspector](https://www.linkedin.com/developers/tools/oauth/token-inspector)

## Obtain client id and secret

The client id and secret for your LinkedIn application are listed in the **Auth** tab of your Application's settings in the [Developers Portal](https://developer.linkedin.com/).

## Obtain an access token (the easy, opaque way)

A token grants an app permission to take the actions defined by the scopes on behalf of the user that granted that permission.

Once you have been granted access to the Community Management API, you can obtain a token:

* Visit the token generator
* Select your app
* Select the following permissions:
  * `w_organization_social`
  * `w_member_social`
  * `r_basicprofile`
* Generate your token

## Obtain an access token (the less easy, transparent way)

### Step 0 - authorise the OAuth Token Explorer redirect URL

* In your app settings, under Auth, under **OAuth 2.0 settings**, authorize the following url:
  `https://instantiator.dev/oauth-token-explorer/authCallback.html`

### Step 1 - get user authorisation

* Visit: [OAuth Token Explorer](https://instantiator.dev/oauth-token-explorer/)
* Click: **Get Started**
* Click: **Init for LinkedIn**
* For **Client id** provide: the client id, obtained earlier
* For **Scope** provide: `w_organization_social,w_member_social,r_basicprofile`

**NB.** `w_organization_social` is the scope for writing social posts on behalf of an organisation, and `w_member_social` is for individuals. New tokens issued for a LinkedIn application will automatically revoke old tokens, so request the full set of scopes you'll need together.

_You should be sent to a LinkedIn sign in page. Sign in, authorize the app, and you'll be redirected to the request access token page..._

### Step 2 - get an access token

The page is largely filled in for you.

* Click: **Init for LinkedIn** (to populate the Access URL)
* For **Client secret** provide: the client secret, obtained earlier
* Click: **Prepare request**
* Copy the content of the **Curl request** generated for you, and paste it into a shell terminal

_You should receive a response in the following format:_

```json
{"access_token":"<ACCESS-TOKEN>","expires_in":5183999,"refresh_token":"<REFRESH-TOKEN>","refresh_token_expires_in":31536059,"scope":"w_organization_social"}
```

From there, you can extract the access token.

NB. The expiry times are in seconds. `5183999` very roughly equates to about 60 days.
