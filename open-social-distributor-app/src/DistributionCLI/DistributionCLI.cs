using CommandLine;
using DistributorLib;
using DistributorLib.Network;
using DistributorLib.Network.Implementations;
using DistributorLib.Post;
using Newtonsoft.Json;

namespace DistributionCLI;

public class DistributionCLI
{
    [Verb("post", HelpText = "Post a message to one or more social networks.")]
    public class PostOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to the config file.")]
        public string? ConfigPath { get; set; }

        [Option('m', "message", Required = true, HelpText = "Simple message text.")]
        public string? Text { get; set; }
    }

    public static void Main(params string[] args)
    {
        Parser.Default
            .ParseArguments<PostOptions>(args)
            .MapResult(
                (PostOptions opts) => ExecutePost(opts),
                errs => 1);
    }

    public static int ExecutePost(PostOptions options)
    {
        var json = File.ReadAllText(options.ConfigPath!);
        var config = JsonConvert.DeserializeObject<Config>(json);
        var networks = NetworkFactory.FromConfig(config!);
        Console.WriteLine($"Identified {networks.Count()} networks.");

        var distributor = new Distributor(networks);

        var message = new SimpleSocialMessage(options.Text!);
        var result = distributor.PostAsync(message).Result;
        return 0;
    }
}
