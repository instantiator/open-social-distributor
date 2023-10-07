namespace DistributorLib.Post.Images;
public interface ISocialImage
{
    public string Source { get; }
    public Uri SourceUri { get; }
    public string? LocalPath { get; }
    public string? Description { get; }
    public string Filename { get; }
    public Task<Stream> GetStreamAsync();
    public Task<byte[]> GetBinaryAsync();
}
