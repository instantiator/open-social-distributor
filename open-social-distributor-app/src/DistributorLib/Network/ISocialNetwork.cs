using DistributorLib.Post;

namespace DistributorLib.Network;

public interface ISocialNetwork : IAsyncDisposable
{
    bool Initialised { get; }

    bool DryRunPosting { get; set; }
    string ShortCode { get; }
    NetworkType NetworkType { get; }
    string NetworkName { get; }
    Task<ConnectionTestResult> TestConnectionAsync();
    Task<PostResult> PostAsync(ISocialMessage message);
    Task InitAsync();
}
