using DistributorLib.Post;

namespace DistributorLib;

public interface ISocialNetwork : IAsyncDisposable
{
    bool Initialised { get; }
    string ShortCode { get; }
    string NetworkName { get; }
    string? NetworkAccountId { get; }
    Task<PostResult> PostAsync(ISocialMessage message);

    Task InitAsync();
}
