FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY ["src/DistributorLib/DistributorLib.csproj", "src/DistributorLib/"]
COPY ["src/DistributionService/DistributionService.csproj", "src/DistributionService/"]
COPY ["test/DistributorLib.Tests/DistributorLib.Tests.csproj", "test/DistributorLib.Tests/"]
COPY ["test/DistributionService.Tests/DistributionService.Tests.csproj", "test/DistributionService.Tests/"]

RUN dotnet restore src/DistributorLib/DistributorLib.csproj
RUN dotnet restore src/DistributionService/DistributionService.csproj
RUN dotnet restore test/DistributorLib.Tests/DistributorLib.Tests.csproj
RUN dotnet restore test/DistributionService.Tests/DistributionService.Tests.csproj

COPY . .

WORKDIR "/"
RUN dotnet test test/DistributorLib.Tests/DistributorLib.Tests.csproj
RUN dotnet test test/DistributionService.Tests/DistributionService.Tests.csproj

WORKDIR "/"
RUN dotnet build "src/DistributionService/DistributionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/DistributionService/DistributionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistributionService.dll"]
