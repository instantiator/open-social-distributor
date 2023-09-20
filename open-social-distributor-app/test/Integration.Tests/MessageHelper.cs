using DistributorLib.Post;
using DistributorLib.Post.Images;

namespace Integration.Tests;

public class MessageHelper
{
    public static ISocialMessage CreateSimplestMessage()
    {
        var parts = new List<SocialMessageContent>
        {
            new SocialMessageContent("Integration test message")
        };
        var images = new List<ISocialImage>()
        {
            SocialImageFactory.FromUri("file://TestData/TestImages/social-distributor-icon.png", "the Social Distributor icon")
        };
        return new SimpleSocialMessage(parts, images);
    }

    public static ISocialMessage CreateLongTextMessage()
    {
        var text = File.ReadAllText("TestData/the-500-mile-email.txt");
        return new SimpleSocialMessage(text);
    }

    public static ISocialMessage CreateComplexMessageWithBreakWords()
    {
        var text = File.ReadAllText("TestData/job-interview-with-a-cat.txt");
        var url = "https://brianbilston.com/category/some-poems/";

        var parts = new List<SocialMessageContent>
        {
            new SocialMessageContent(text),
            new SocialMessageContent(url, part: SocialMessagePart.Link),
        };

        var images = new List<ISocialImage>()
        {
            SocialImageFactory.FromUri("file://TestData/TestImages/cat-1.jpg", "A big floofy cat lays by a laptop on a wooden table outdoors"),
            SocialImageFactory.FromUri("file://TestData/TestImages/cat-2.jpg", "A big floofy cat curled up, overlaid with the golden spiral"),
            SocialImageFactory.FromUri("file://TestData/TestImages/cat-3.jpg", "Blue and black mottled cat street art"),
            SocialImageFactory.FromUri("file://TestData/TestImages/cat-4.jpg", "A big floofy white cat sits on a stairway looking upwards"),
            SocialImageFactory.FromUri("file://TestData/TestImages/cat-5.jpg", "Two cats sit by a log burner"),
        };
        return new SimpleSocialMessage(parts, images);
    }
}
