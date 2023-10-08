# Post composition

## Simple message composition

The CLI permits the specification of simple messages, using the `post` verb,

```bash
DistributionCLI post <options>
```

The following options are defined for `post`:

```text
-c, --config-file           Required. Path to the config file.
-s, --source-file           Path or url to a source file containing posts.
-f, --filter                Regular expression filter for network short codes.
-m, --message               Simple message text.
-l, --link                  Link for this message.
-i, --images                URIs to images, semi-colon separated (;)
-d, --image-descriptions    Image descriptions, semi-colon separated (;)
-t, --tags                  A list of tags (without # prefix), semi-colon separated (;)
--help                      Display this help screen.
--version                   Display version information.
```

Use the `--message` or `--source-file` option to provide content to post.

## Source file

To provide a source file with posts, see: [Post list format](post-list-format.md)

## Message options

* Provide message text with the `-m` option
* Provide a link to accompany the message content with the `-l` option.
* Provide a list of paths to images for the post with the `-i` option.
* Provide descriptions of the images with the `-d` option.
* Provide a list of tags to accompany the message with the `-t` option.

Note that lists should be semi-colon (`;`) separated.

A simple example:

```bash
DistributionCLI post -c path/to/config.json -m "this is a test"
```

## Message composition

Posts are made up of a number of parts, each of which could be rendered different for a specific social network (although by default, the content you provide will apply to any).

Each part can be one of several types:

| `SocialMessagePart` | Description |
|-|-|
| `Text` | Ordinary text that will appear in the post |
| `AccountReference` | Reference to an account, eg. `@instantiator` |
| `Tag` | An individual tag, without the `#` prefix, eg. `Caturday` |
| `Link` | A link that this post points to |

The parts are combined and formatted during posting to ensure that the text will all fit and flow correctly according to the conventions of the social network that the post is being targeted at.

If necessary, multiple posts are created, and linked together as a thread.

## Thread formatting

Some social networks (Mastodon, Twitter) lend themselves conceptually to threading more easily than others.

Each social network has a different set of character limits.

If you manage to post a thread longer than the post limit for other networks (eg. a Facebook post longer than `63206` characters, somehow!), subsequent parts of the message will be posted as comments on the main post.

Some items (ie. tags, links) will either be moved to the first post, duplicated across all posts, or may be handled differently depending on the social network.

| Network | First post | Subsequent posts | Tags | Link | Images | Images per post | Image size limit |
|-|-|-|-|-|-|-|-|
| Mastodon | `500` | `500` | All posts | First post | Front-loaded | `4` | TBC |
| Discord | `2000` | `2000` | First post | First post |  First post | `10` | TBC |
| Facebook | `63206` | `8000` | First post | Special | First post | `∞` |`10Mb` |
| LinkedIn | `3000` | `1250` | First post | First post | First post | `20` | TBC |
| Twitter | `280` | `280` | All posts | First post | Front-loaded | `4` | TBC |

The formatter will wrap words on each post. If a post would exceed the limit available to it, it wraps the next word into the next post.

The limit allows space for:

1. The link, if it should appear in this post
1. An index indicator (if there's more than 1 post)
1. Any tags that will fit, if the tags should appear on the post

By default, tags will not exceed 50% of the allowed character space. If not all tags will fit on a post, a random selection of the available tags is used.

### Images

As described above, images are assigned to the various posts that make up a thread.

* For Mastodon, images will be assigned starting from the first post, at 4 per post.
* For Facebook, all images (unlimited) will be assigned to the first post.
* For LinkedIn, all images will be assigned to the first post, up to 9.
* For Discord, all images will be assigned to the first post, up to 10.

**A message that is to be posted across all 4 of these networks should be constrained by the lowest common denominator, ie. a maximum of 9 images.**

### Breaks

You can force the formatter to break to a new post by including the special break word: `$$`

### Facebook notes

* The formatter for Facebook posts does not include the link in the text of the post, as it is included separately as a special property of the post.
* Facebook does not have the concept of threads. If the content for the first post exceeds the limit, subsequent posts are made as comments.
* Instead of forcing a new post, the break word `$$` creates a paragraph break with 2 newline characters.

### LinkedIn notes

* LinedIn does not have the concept of threads. If the content for the first post exceeds the limit, subsequent posts are made as comments.
* Instead of forcing a new post, the break word `$$` creates a paragraph break with 2 newline characters.
