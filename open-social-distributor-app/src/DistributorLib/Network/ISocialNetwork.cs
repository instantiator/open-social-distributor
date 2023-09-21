using DistributorLib.Post;
using DistributorLib.Post.Assigners;
using DistributorLib.Post.Formatters;

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

    IImageAssigner Assigner { get; }
    IPostFormatter Formatter { get; }
    
    Task InitAsync();
}
