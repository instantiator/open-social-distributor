using Discord;
using Discord.Net.Rest;
using Discord.Rest;
using Discord.WebSocket;
using DistributorLib.Extensions;
using DistributorLib.Post;
using DistributorLib.Post.Assigners;
using DistributorLib.Post.Formatters;
using DistributorLib.Post.Images;
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
        : base(NetworkType.Discord, shortcode, "Discord", PostFormatVariantFactory.Discord, ImageAssignerVariantFactory.Discord)
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

    protected override async Task<PostResult> PostImplementationAsync(ISocialMessage message, IEnumerable<string> texts, IEnumerable<IEnumerable<ISocialImage>> images)
    {
        log.Clear();
        await client!.LoginAsync(TokenType.Bot, token);
        await client!.StartAsync();
        await TaskEx.WaitUntil(() => 
            client!.LoginState == LoginState.LoggedIn &&
            client!.Status == UserStatus.Online && 
            client!.ConnectionState == ConnectionState.Connected, 100, 10000);
        var channel = await GetTextChannelAsync();
        if (channel == null)
        {
            log.Add("Channel not found");
            return new PostResult(this, message, false, null, string.Join('\n', log));
        } 
        else
        {
            var responses = new List<RestUserMessage>();
            for (var t = 0; t < texts.Count(); t++)
            {
                var text = texts.ElementAt(t);
                if (!DryRunPosting)
                {
                    if (images.Count() > t && images.ElementAt(t).Count() > 0)
                    {
                        var files = images.ElementAt(t)
                            .Select(async i => new FileAttachment(
                                stream: await i.GetStreamAsync(), 
                                fileName: Path.GetFileName(i.AbsoluteLocalPath), 
                                description: i.Description))
                            .Select(f => f.Result);

                        var response = await channel.SendFilesAsync(files, text);
                        responses.Add(response);
                    }
                    else
                    {
                        var response = await channel.SendMessageAsync(text);
                        responses.Add(response);
                    }
                }
            }

            try 
            { 
                await client!.StopAsync(); 
                await client!.LogoutAsync(); 
            } 
            catch (Exception e) 
            { 
                log.Add($"Ignored exception during sign off: {e.GetType().Name}: {e.Message}"); 
            }

            var aok = responses.All(r => r != null);
            var ids = responses.Select(r => r?.Id.ToString());
            return new PostResult(this, message, aok, ids, string.Join('\n', log));
        }
    }

    private async Task<SocketTextChannel?> GetTextChannelAsync()
    {
        var guild = client!.GetGuild(guildId);
        SocketTextChannel? channel = null;

        await TaskEx.WaitUntil(() =>
        {
            channel = guild.GetTextChannel(channelId);
            return channel != null;
        }, 100, 10000);
        
        return channel;
    }

    protected override async Task<ConnectionTestResult> TestConnectionImplementationAsync()
    {
        log.Clear();
        await client!.LoginAsync(TokenType.Bot, token);
        await client!.StartAsync();
        await TaskEx.WaitUntil(() => 
            client!.LoginState == LoginState.LoggedIn &&
            client!.Status == UserStatus.Online && 
            client!.ConnectionState == ConnectionState.Connected, 100, 10000);

        var channel = await GetTextChannelAsync();
        var info = await client!.GetApplicationInfoAsync();
        var id = $"{info.Id}";
        var name = $"{info.Name}";

        var summary = new
        {
            name,
            guild = guildId,
            channel = channelId
        };

        return new ConnectionTestResult(this, true, id, JsonConvert.SerializeObject(summary));
    }
}

// var filename = Path.GetFileName(path);

// var emb = new EmbedBuilder()
//     .WithImageUrl($"attachment://{filename}")
//     .Build();

// await Context.Channel.SendFileAsync(path, null, false, emb);