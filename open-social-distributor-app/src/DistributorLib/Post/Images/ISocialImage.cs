namespace DistributorLib.Post.Images;
public interface ISocialImage
{
    public string Source { get; }
    public Uri SourceUri { get; }
    public string? AbsoluteLocalPath { get; }
    public string? Description { get; }
    public Task<Stream> GetStreamAsync();
}
