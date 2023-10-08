# CLI options

## Options

```text
DistributionCLI <verb> <options...>
```

### Verbs

```text
  post       Post one or more messages to one or more social networks.
  test       Test all connections in the configuration file.
  help       Display more information on a specific command.
  version    Display version information.
```

### Files

When referring to files, you may provide:

* A file URI, starting with `file://`, or
* A regular URL, starting with `https://`, or
* A local file path

## Building the CLI

* Run the `release-cli.sh` script to build the CLI.
* Binaries will be placed in the `release/` directory.

## Examples

Here are some simple examples you can use to explore the CLI...

### Simple post

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

Here's the same example, but for a local file. To test this, you'll need to provide the path to a file on your local filesystem as the `-i` (`--images`) option.

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

### Post from a source file

This example draws on a local file with a series of messages in it:

```bash
./release/osx-x64/DistributionCLI post \
  -c "sample-config/console-only.json" \
  -s "sample-messages/single-message.jsonc"
```

For details on the content of the messages source file format, see: [Post list format](post-list-format.md)

### Filtering

You can filter the selection of configured networks for a post or test with the `--filter` (`-f`) option. The filter parameter is a regular expression.

In this example, `personal$`, it filters for networks that _end_ with the word `personal`:

```bash
./release/osx-x64/DistributionCLI test \
  --config sample-config/private-all.json \
  --filter "personal$"
```
