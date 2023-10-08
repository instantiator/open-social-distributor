# Post List format

Sample post lists can be found at:

* `sample-messages/blank-root.json`
* `sample-messages/single-message.jsonc`

## PostListFormat

| Key | Type | Usage |
|-|-|-|
| `$.posts` | List | A list of `PostFormat` objects |


## PostFormat

| Key | Type | Usage |
|-|-|-|
| `$.parts` | List | A list of `PostPartFormat` objects |
| `$.images` | List | A list of `PostImageFormat` objects |

## PostPartFormat

| Key | Type | Usage |
|-|-|-|
| `$.part` | `SocialMessagePart` (enum) | Indicate which part of message this piece of content represents |
| `$.content.any` | string | How this content should appear across any network (the fallback) |
| `$.content.mastodon` | string | How this content should appear on mastodon networks |
| `$.content.console` | string | How this content should appear when rendered for the console |
| `$.content.facebook` | string | How this content should appear on Facebook |
| `$.content.linkedin` | string | How this content should appear on LinkedIn |
| `$.content.discord` | string | How this content should appear on Discord |

### SocialMessagePart

* `Text` - a piece of text
* `AccountReference` - reference to a social media account
* `Tag` - a hashtag (with the preceding `#`)
* `Link` - the main link of the post

Text and account references will be combined to produce the main text of the message (and then reformatted into a thread if necessary).

Tags will be added at appropriate points by the formatter for the social network being posted to.

See also: [Post composition](post-composition.md)

## PostImageFormat

| Key | Type | Usage |
|-|-|-|
| `$.uri` | string | A uri or file path to the image |
| `$.description` | string | Description of the image (sometimes referred to as 'alt text') |

NB. `$.uri` file paths are interpreted as relative to the working directory from which the CLI was invoked. Provide absolute file paths if you need to be certain.
