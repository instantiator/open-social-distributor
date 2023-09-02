# Post composition

Posts are made up of a number of parts, each of which could be rendered different for a specific social network (although by default, the content you provide will apply to any).

Each part can be one of several types:

| `SocialMessagePart` | Description |
|-|-|
| `Text` | Ordinary text that will appear in the post |
| `AccountReference` | Reference to an account, eg. `@instantiator` |
| `Tag` | An individual tag, without the `#` prefix, eg. `Caturday` |
| `Link` | A link that this post points to |

The parts are combined and formatted during posting to ensure that the text will all fit and flow correctly according to the conventions of the social network that the post is being targeted at. If necessary, multiple posts are created, and linked together as a thread.

## Thread formatting

Each social network has a different set of character limits:

| Network | First post | Subsequent posts | Tags | Link |
|-|-|-|-|-|
| Console | ∞ | ∞ | N/A | N/A |
| Mastodon | `500` | `500` | All posts | First post |
| Facebook | `63206` | `8000` | First post | Special |
| Twitter | | | | |
| Discord | | | | | 
| LinkedIn | | | | |

The formatter will wrap words on each post. If a post would exceed the limit available to it, it wraps the next word into the next post.

The limit allows space for:

1. The link, if it should appear in this post
1. An index indicator (if there's more than 1 post)
1. Any tags that will fit, if the tags should appear on the post

Tags are selected in a random order, to help in situations where they might not fit.

You can force the formatted to break to a new post by including the speak break word: `$$`

### Facebook notes

* The formatter for Facebook posts does not include the link in the text of the post, as it is included separately as a special property of the post.
* Facebook does not have the concept of threads. If the content for the first post exceeds the limit, subsequent posts are made as comments.
* Instead of forcing a new post, the break word `$$` creates a paragraph break with 2 newline characters.
