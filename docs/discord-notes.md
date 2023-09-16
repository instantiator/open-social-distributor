# Discord

Discord has great support for bots, who can be invited to servers and channels and then post there.

## Discord connection strings

```text
type=discord;code=<SHORTCODE>;guild_id=<GUILD_ID>;channel_id=<CHANNEL_ID>;token=<ACCESS_TOKEN>
```

* `SHORTCODE` - a unique code to refer to the social network instance, this will appear in logs
* `GUILD_ID` - id of the Discord server to post to
* `CHANNEL_ID` - channel of the Discord server to post to
* `TOKEN` - Access token for the Discord bot belonging to the Discord app (see below)

## Create a bot and obtain an access token

To build a bot that can post into a server, you'll need to:

* Create an app in the [developers portal](https://discord.com/developers/applications)
* Add a bot (needn't be public) to the app (on the Bot page)
* Capture the bot's **access token** (you may need to regenerate the token)
* Use the URL Generator to create a url with the following permissions:
    * `Bot`
        * `Send Messages`
        * `Attach Files`
        * `Add Reactions`
        * `Read Message History`
        * `Use Slash Commands`
* Visit the url you have generated, and grant the bot access to your server

**NB.** There are also some interesting thread-based permissions. They are out of scope for this application _for now._

## Get the guild and channel ids

* Right click a server (guild) in your Discord client, and choose "Copy server id"
* Right click a channel in your Discord client, and choose "Copy channel id"
