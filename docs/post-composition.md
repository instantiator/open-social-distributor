# Post composition

Posts are made up of a number of parts, each of which could be rendered different for a specific social network (although by default, the content you provide will apply to any).

Each part can be one of several types:

| `SocialMessagePart` | Description |
|-|-|
| `Text` | Ordinary text |
| `AccountReference` | Reference to an account, eg. `@instantiator` |
| `Tag` | An individual tag, eg. `Caturday` |
| `Link` | A link that this post points to |

The parts are combined and formatted during posting to ensure that the text will all fit and flow correctly. If necessary, multiple posts are created, and linked together as a thread.

Some social networks, eg. Facebook, treat links as a special part of the post - and so the link itself is not included in the text of the post.
