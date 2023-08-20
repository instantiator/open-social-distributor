using DistributorLib.Post;

namespace DistributorLib.Network;

public interface ISocialNetwork : IAsyncDisposable
{
    bool Initialised { get; }
    string ShortCode { get; }
    NetworkType NetworkType { get; }
    string NetworkName { get; }
    string? NetworkAccountId { get; }
    Task<bool> TestConnectionAsync();
    Task<PostResult> PostAsync(ISocialMessage message);
    Task InitAsync();
}
