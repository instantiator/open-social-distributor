namespace DistributionCLI.Tests;

public class ApplicationTests
{
    [Fact]
    public void AppInitialises()
    {
        DistributionCLI.Main("test");
    }

}