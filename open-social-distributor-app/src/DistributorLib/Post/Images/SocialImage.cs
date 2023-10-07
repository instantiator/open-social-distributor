using System.Net;
using DistributorLib.Input;

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

    public string? LocalPath => UriReader.LocalPath(Source);

    public string Filename => SourceUri.Segments.Last();

    public string? Description { get; private set; }

    public async Task<Stream> GetStreamAsync() => await UriReader.GetStreamAsync(SourceUri);

    public async Task<byte[]> GetBinaryAsync() => await UriReader.ReadByteArrayAsync(SourceUri);
}
