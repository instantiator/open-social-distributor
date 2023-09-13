# Discord

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
