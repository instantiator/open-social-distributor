using DistributorLib.Post;
using DistributorLib.Post.Images;

namespace DistributorLib.Input;

public static class PostListConverterExtensions
{
    public static IEnumerable<ISocialMessage> ToSocialMessages(this PostListFormat list)
    {
        return list.Posts.Select(p =>
            new SimpleSocialMessage(
                parts: p.Parts.Select(p => new SocialMessageContent(p.Content, p.Part)),
                images: p.Images.Select(i => new SocialImage(i.Uri, i.Description))
            ));
    }
}
