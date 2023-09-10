using DistributorLib;
using DistributorLib.Network;

namespace DistributorLib.Post;
public class PostResult
{
    public PostResult(ISocialNetwork network, ISocialMessage message, bool success, IEnumerable<string>? postIds, string? error = null, Exception? exception = null)
    {
        Network = network;
        Message = message;
        Success = success;
        PostIds = postIds;
        Error = error;
        Exception = exception;
        Created = DateTime.Now;
    }

    public DateTime Created { get; private set; }
    public ISocialNetwork Network { get; private set; }
    public ISocialMessage Message { get; private set; }
    public bool Success { get; private set; }
    public IEnumerable<string>? PostIds { get; private set; }
    public string? Error {get; private set; }
    public Exception? Exception { get; private set; }
}