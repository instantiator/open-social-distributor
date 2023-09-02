using System.Net;

namespace DistributorLib.Post.Images;

public class SocialImage : ISocialImage
{
    public SocialImage(string uri, string? description)
    {
        SourceUri = uri;
        Description = description;
    }

    public string SourceUri { get; private set; }

    public string? Description { get; private set; }

    public async Task<Stream> GetStreamAsync()
    {
        var uri = new Uri(SourceUri);
        return await GetStreamAsync(uri);
    }

    public static async Task<Stream> GetStreamAsync(Uri uri) {
        if (uri.IsFile)
        {
            return File.OpenRead(uri.LocalPath);
        }
        else 
        {
            return await new HttpClient().GetStreamAsync(uri);
        }
    }

}