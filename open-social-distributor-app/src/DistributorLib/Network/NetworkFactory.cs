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
        return config.networks.enabled.Select(FromConnectionString);
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
                        var fb_pageToken = parameters["token"];
                        return new FacebookNetwork(fb_code, fb_mode, token: fb_pageToken, actorId: fb_pageId);
                    default:
                        throw new NotImplementedException($"FacebookNetwork mode {fb_mode} not implemented");
                }

            case NetworkType.LinkedIn:
                var li_code = parameters["code"];
                var li_mode = Enum.Parse<LinkedInNetwork.Mode>(parameters["mode"], true);
                var li_client_id = parameters["client_id"];
                var li_client_secret = parameters["client_secret"];
                var li_token = parameters["token"];
                var li_author_id = parameters["author_id"];
                return new LinkedInNetwork(li_code, li_mode, li_client_id, li_client_secret, li_token, li_author_id);

            case NetworkType.Mastodon:
                var ma_code = parameters["code"];
                var ma_instance = parameters["instance"];
                var ma_token = parameters["token"];
                return new MastodonNetwork(ma_code, ma_instance, ma_token);
            
            case NetworkType.Discord:
                var di_code = parameters["code"];
                var di_token = parameters["token"];
                var di_guildId = ulong.Parse(parameters["guild_id"]);
                var di_channelId = ulong.Parse(parameters["channel_id"]);
                return new DiscordNetwork(di_code, di_token, di_guildId, di_channelId);

            case NetworkType.Any:
                throw new ArgumentException("\"Any\" is not a network type.");
            default:
                throw new ArgumentException($"Unable to create a network for type: {networkType}");
        }
    }

}