namespace DistributorLib.Network
{
    public class ConnectionTestResult
    {
        public ConnectionTestResult(ISocialNetwork network, bool success, string? actorId, string? message = null, Exception? exception = null)
        {
            Network = network;
            Success = success;
            Message = message;
            ActorId = actorId;
            Exception = exception;
        }

        public ISocialNetwork Network { get; private set; }
        public bool Success { get; private set; }
        public string? ActorId {get; private set; }
        public string? Message { get; private set; }
        public Exception? Exception { get; private set; }
    }
}