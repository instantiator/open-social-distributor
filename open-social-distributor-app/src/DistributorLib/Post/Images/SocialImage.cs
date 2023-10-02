using System.Net;

namespace DistributorLib.Post.Images;

public class SocialImage : ISocialImage
{
    public SocialImage(string uri, string? description)
    {
        Source = uri;
        Description = description;
    }

    public string Source { get; private set; }

    public Uri SourceUri => new Uri(Source);

    public string? AbsoluteLocalPath => SourceUri.IsFile ? Path.GetFullPath(Source.Substring("file://".Length)) : null;

    public string Filename => SourceUri.Segments.Last();

    public string? Description { get; private set; }

    public async Task<Stream> GetStreamAsync()
    {
        if (SourceUri.IsFile)
        {
            return File.OpenRead(AbsoluteLocalPath!);
        }
        else 
        {
            return await new HttpClient().GetStreamAsync(SourceUri);
        }
    }

    public async Task<byte[]> GetBinaryAsync()
    {
        if (SourceUri.IsFile)
        {
            return await File.ReadAllBytesAsync(AbsoluteLocalPath!);
        }
        else 
        {
            return await new HttpClient().GetByteArrayAsync(SourceUri);
        }
    }
}
