using DistributorLib.Network.Implementations;

namespace DistributorLib.Network;

public class NetworkFactory
{
    public static IEnumerable<ISocialNetwork> FromConfig(Config config)
    {
        var networks = new List<ISocialNetwork>();
        var duplicates = networks.Where(n => networks.Count(nc => nc.ShortCode == n.ShortCode) > 1);
        if (duplicates.Count() > 0)
        {
            throw new ArgumentException($"Duplicate network shortcodes found in config: {string.Join(", ", duplicates.Select(d => d.ShortCode).Distinct())}");
        }
        return config.networks.Select(FromConnectionString);
    }

    public static ISocialNetwork FromConnectionString(NetworkConnectionString connection)
    {
        var parameters = connection.Parameters;
        var typeParam = parameters["type"];
        var networkType = Enum.Parse<NetworkType>(typeParam, true);

        switch (networkType) {
            case NetworkType.Console:
                return new ConsoleNetwork();
            case NetworkType.LinkedIn:
                throw new NotImplementedException();
            case NetworkType.Mastodon:
                var code = parameters["code"];
                var instance = parameters["instance"];
                var token = parameters["token"];
                return new MastodonNetwork(code, instance, token);
            case NetworkType.Any:
                throw new ArgumentException("\"Any\" is not a network type.");
            default:
                throw new ArgumentException($"Unable to create a network for type: {networkType}");
        }



    }



}