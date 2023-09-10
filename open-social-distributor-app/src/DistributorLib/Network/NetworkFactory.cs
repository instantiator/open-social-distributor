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

            case NetworkType.Facebook:
                var fb_code = parameters["code"];
                var fb_mode = Enum.Parse<FacebookNetwork.Mode>(parameters["mode"], true);
                switch (fb_mode) 
                {
                    case FacebookNetwork.Mode.Page:
                        var fb_pageId = parameters["page_id"];
                        var fb_pageToken = parameters["page_token"];
                        return new FacebookNetwork(fb_code, fb_mode, userToken: null, pageId: fb_pageId, pageToken: fb_pageToken);
                    case FacebookNetwork.Mode.User:
                        var fb_userToken = parameters["user_token"];
                        return new FacebookNetwork(fb_code, fb_mode, userToken: fb_userToken, pageId: null, pageToken: null);
                    default:
                        throw new NotImplementedException($"FacebookNetwork mode {fb_mode} not implemented");
                }

            case NetworkType.LinkedIn:
                var li_code = parameters["code"];
                var li_mode = Enum.Parse<LinkedInNetwork.Mode>(parameters["mode"], true);
                switch (li_mode) {
                    case LinkedInNetwork.Mode.Org:
                    var li_token = parameters["token"];
                    var li_org_id = parameters["org_id"];
                    return new LinkedInNetwork(li_code, li_mode, li_token, li_org_id);
                default:
                    throw new NotImplementedException($"LinkedInNetwork mode {li_mode} not implemented");
                }

            case NetworkType.Mastodon:
                var ma_code = parameters["code"];
                var ma_instance = parameters["instance"];
                var ma_token = parameters["token"];
                return new MastodonNetwork(ma_code, ma_instance, ma_token);
            
            case NetworkType.Any:
                throw new ArgumentException("\"Any\" is not a network type.");
            default:
                throw new ArgumentException($"Unable to create a network for type: {networkType}");
        }
    }

}