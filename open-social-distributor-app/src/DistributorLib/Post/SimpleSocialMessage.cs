namespace DistributorLib.Post;

public class SimpleSocialMessage : ISocialMessage
{
    public SimpleSocialMessage(string message, IEnumerable<ISocialImage>? images = null)
    {
        Parts = new Dictionary<SocialMessagePart, string>() { { SocialMessagePart.Message, message } };
        Images = images;
    }

    public SimpleSocialMessage(Dictionary<SocialMessagePart, string> parts, IEnumerable<ISocialImage>? images = null)
    {
        Parts = parts;
        Images = images;
    }

    public Dictionary<SocialMessagePart, string> Parts { get; private set; }
    public IEnumerable<ISocialImage>? Images { get; private set; }
}