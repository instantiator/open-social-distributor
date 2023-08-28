using DistributorLib;
using DistributorLib.Network;
using DistributorLib.Network.Implementations;
using DistributorLib.Post;

namespace DistributorLib.Tests;

public class DistributorTests
{
    [Fact]
    public async void DistributorAddNetwork_WithTrue_AddsAndInitialisesNetwork()
    {
        var distributor = new Distributor();
        await distributor.AddNetworkAsync(new ConsoleNetwork(), true);
        Assert.Single(distributor.Networks);
        Assert.True(distributor.Networks.Single().Initialised);
    }

    [Fact]
    public async void DistributorAddNetwork_WithFalse_AddsNetwork()
    {
        var distributor = new Distributor();
        await distributor.AddNetworkAsync(new ConsoleNetwork(), false);
        Assert.Single(distributor.Networks);
        Assert.False(distributor.Networks.Single().Initialised);
    }

    [Fact]
    public async void DistributorTestNetworks_InitialisesAndTestsNetworks()
    {
        var distributor = new Distributor();
        await distributor.AddNetworkAsync(new ConsoleNetwork(), false);
        var tests = await distributor.TestNetworksAsync();
        Assert.Single(tests); // one test result
        Assert.True(tests.Single().Value); // test passed
        Assert.True(distributor.Networks.Single().Initialised); // network initialised
    }

    [Fact]
    public async void DistributorPost_ReturnsPostResult()
    {
        var distributor = new Distributor();
        await distributor.AddNetworkAsync(new ConsoleNetwork(), true);

        var result = await distributor.PostAsync(new SimpleSocialMessage("Test Message"));
        Assert.Single(result);
        Assert.True(result.Single().Success);
        Assert.Equal(distributor.Networks.Single(), result.Single().Network);
        Assert.Equal("Test Message", result.Single().Message.Parts.Single().ToStringFor(NetworkType.Any));
    }
}
