using DistributorLib.Network;

namespace DistributorLib;

public class Config
{
    public IEnumerable<NetworkConnectionString> networks { get; set; } = new List<NetworkConnectionString>();
}