namespace DistributorLib.Post.Images;
public interface ISocialImage
{
    public string SourceUri { get; }
    public string? Description { get; }
    public Task<Stream> GetStreamAsync();
}
