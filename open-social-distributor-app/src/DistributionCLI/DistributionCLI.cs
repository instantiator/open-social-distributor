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

    [Verb("test", HelpText = "Test all connections in the configuration file.")]
    public class TestOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to the config file.")]
        public string? ConfigPath { get; set; }
    }

    public static void Main(params string[] args)
    {
        Parser.Default
            .ParseArguments<PostOptions, TestOptions>(args)
            .MapResult(
                (PostOptions opts) => ExecutePost(opts),
                (TestOptions opts) => ExecuteTest(opts),
                errs => 1);
    }

    public static int ExecuteTest(TestOptions options)
    {
        var json = File.ReadAllText(options.ConfigPath!);
        var config = JsonConvert.DeserializeObject<Config>(json);
        var networks = NetworkFactory.FromConfig(config!);
        Console.WriteLine($"Identified {networks.Count()} networks.");

        var distributor = new Distributor(networks);
        var testResults = distributor.TestNetworksAsync().Result;
        var i = 0;
        foreach (var result in testResults)
        {
            Console.WriteLine($"{++i} - {result.Key.ShortCode} ({result.Key.NetworkType}): {(result.Value.Success ? "OK" : "fail")}");
        }

        var fails = testResults.Where(r => !r.Value.Success);
        if (fails.Count() > 0)
        {
            Console.WriteLine($"{fails.Count()} issues found:");
            foreach (var result in fails)
            {
                var message = result.Value.Message;
                var exceptionType = result.Value.Exception?.GetType().Name;
                var report = "";
                if (!string.IsNullOrWhiteSpace(message)) {
                    report += message;
                }
                if (!string.IsNullOrWhiteSpace(exceptionType)) {
                    if (!string.IsNullOrWhiteSpace(report)) report += " ";
                    report += $"({exceptionType})";
                }
                Console.WriteLine($"❌ - {result.Key.ShortCode} ({result.Key.NetworkType}): {report}");
                if (result.Value.Network is FacebookNetwork)
                {
                    Console.WriteLine($"Facebook token: {(result.Value.Network as FacebookNetwork)!.pageToken}");
                }
            }
        }
        return testResults.All(r => r.Value.Success) ? 0 : 1;
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
