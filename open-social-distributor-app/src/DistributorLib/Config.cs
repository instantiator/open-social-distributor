using DistributorLib.Network;

namespace DistributorLib;

public class Config
{
    public NetworkLists networks = new NetworkLists();
}

public class NetworkLists
{
    public IEnumerable<NetworkConnectionString> enabled { get; set; } = new List<NetworkConnectionString>();
    public IEnumerable<NetworkConnectionString> disabled { get; set; } = new List<NetworkConnectionString>();
}
