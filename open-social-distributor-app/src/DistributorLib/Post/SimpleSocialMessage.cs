namespace DistributorLib.Post;

public class SimpleSocialMessage : ISocialMessage
{
    public SimpleSocialMessage(string message, IEnumerable<ISocialImage>? images = null)
    {
        Message = message;
        Images = images;
    }

    public string Message { get; private set; }
    public IEnumerable<ISocialImage>? Images { get; private set; }
}