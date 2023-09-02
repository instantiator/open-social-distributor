# CLI options

## Options

```text
DistributionCLI <verb> <options...>
```

### Verbs

```text
  post       Post a message to one or more social networks.
  test       Test all connections in the configuration file.
  help       Display more information on a specific command.
  version    Display version information.
```

## Building the CLI

* Run the `release-cli.sh` script to build the CLI.
* Binaries will be placed in the `release/` directory.

## Examples

Here's a simple example you can use to explore the CLI.

Now you can invoke it to `post` a simple message with a link, a few tags, and an image:

```bash
./release/osx-x64/DistributionCLI post \
  --config sample-config/console-only.json \
  --message "Meet the lads from Sev'ral Timez" \
  --images "https://static.wikia.nocookie.net/gravityfalls/images/9/92/S1e17_show_end.png/revision/latest/scale-to-width-down/1000?cb=20130412112159" \
  --image-descriptions "Creggy G., Greggy C., Leggy P., Chubby Z. and Deep Chris" \
  --tags "GravityFalls;SevralTimez" \
  --link "https://gravityfalls.fandom.com/wiki/Sev%27ral_Timez"
```

Here's the same example, but for a local file. To test this, you'll need to provide the path to a file on your local filesystem as the `file:///` uri provided to the `-i` (`--images`) option. 

This example uses single character options:

```bash
./release/osx-x64/DistributionCLI post \
  -c sample-config/console-only.json \
  -m "Meet the lads from Sev'ral Timez" \
  -i "file:///Users/lewiswestbury/Desktop/sev'ral timez.png" \
  -d "Creggy G., Greggy C., Leggy P., Chubby Z. and Deep Chris" \
  -t "GravityFalls;SevralTimez" \
  -l "https://gravityfalls.fandom.com/wiki/Sev%27ral_Timez"
```

**Tip:** You can include more than 1 image, and more than 1 image description by separating values with a semi-colon: `;`

