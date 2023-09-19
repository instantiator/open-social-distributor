using DistributorLib.Post;
using DistributorLib.Post.Images;

namespace Integration.Tests;

public class MessageHelper
{
    public static ISocialMessage CreateSimplestMessage()
    {
        var parts = new List<SocialMessageContent>
        {
            new SocialMessageContent("Hello this is a test")
        };
        var images = new List<ISocialImage>()
        {
            SocialImageFactory.FromUri("file://TestData/TestImages/mastodon-logo.png", "the Mastodon logo")
        };
        return new SimpleSocialMessage(parts, images);
    }
}
