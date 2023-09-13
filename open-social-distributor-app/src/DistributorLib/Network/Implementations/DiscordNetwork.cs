using Discord;
using Discord.WebSocket;
using DistributorLib.Extensions;
using DistributorLib.Post;
using DistributorLib.Post.Formatters;
using Newtonsoft.Json;

namespace DistributorLib.Network.Implementations;

public class DiscordNetwork : AbstractNetwork
{
    DiscordSocketClient? client;

    private List<string> log = new List<string>();

    private string token;
    private ulong guildId;
    private ulong channelId;

    public DiscordNetwork(string shortcode, string token, ulong guildId, ulong channelId) 
        : base(NetworkType.Discord, shortcode, "Discord", PostFormatVariantFactory.Discord)
    {
        this.token = token;
        this.guildId = guildId;
        this.channelId = channelId;
    }

    protected override async Task DisposeClientAsync()
    {
        if (client != null)
        {
            await client.DisposeAsync();
            client = null;
        }
    }

    protected override async Task InitClientAsync()
    {
        client = new DiscordSocketClient();
        client.Log += (msg) =>
        {
            log.Add($"{msg.Severity}: {msg.Message}");
            return Task.CompletedTask;
        };
    }

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message)
    {
        throw new NotImplementedException();
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        log.Clear();
        await client!.LoginAsync(TokenType.Bot, token);

        
        await client!.StartAsync();
        await TaskEx.WaitUntil(() => client!.Status == UserStatus.Online, 100, 10000);
        var onlineOk = client!.Status == UserStatus.Online;

        await TaskEx.WaitUntil(() => client!.ConnectionState == ConnectionState.Connected, 100, 10000);
        var connectedOk = client!.ConnectionState == ConnectionState.Connected;

        var guild = client!.GetGuild(guildId);
        var foundGuildOk = guild != null;

        var channel = await client!.GetChannelAsync(channelId);
        var foundChannelOk = channel != null;

        var info = await client!.GetApplicationInfoAsync();
        var owner = info.Owner.Username + "#" + info.Owner.Discriminator;
        var id = $"{info.Id}";
        var name = $"{info.Name}";

        var summary = new
        {
            connected = connectedOk,
            online = onlineOk,
            guildOk = foundGuildOk,
            channelOk = foundChannelOk,
            id,
            name,
            owner,
            guild = guildId,
            channel = channelId
        };

        var aok = 
            connectedOk && 
            onlineOk && 
            foundChannelOk;

        return new ConnectionTestResult(this, aok, id, JsonConvert.SerializeObject(summary) + '\n' + string.Join("\n", log));
    }
}
