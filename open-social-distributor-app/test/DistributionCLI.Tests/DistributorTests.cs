namespace DistributionCLI.Tests;

public class DistributorTests
{
    [Fact]
    public void AppInitialises()
    {
        DistributionCLI.Main("test");
    }
}