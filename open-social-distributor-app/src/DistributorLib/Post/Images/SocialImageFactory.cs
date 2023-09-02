namespace DistributorLib.Post.Images;

public class SocialImageFactory
{
    public static ISocialImage FromUri(string uri, string? description)
    {
        return new SocialImage(uri, description);
    }

    public static IEnumerable<ISocialImage> FromUrisAndDescriptions(IEnumerable<string> uris, IEnumerable<string>? descriptions)
    {
        var results = new List<ISocialImage>();
        for (int i = 0; i < uris.Count(); i++)
        {
            var uri = uris.ElementAt(i);
            var description = descriptions?.Count() > i ? descriptions?.ElementAt(i) : null;
            results.Add(new SocialImage(uri, description));
        }
        return results;
    }

}
 