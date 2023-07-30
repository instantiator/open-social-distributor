namespace DistributorLib.Post;
public interface ISocialMessage
{
    Dictionary<SocialMessagePart, string> Parts { get; }
    IEnumerable<ISocialImage>? Images { get; }
}
