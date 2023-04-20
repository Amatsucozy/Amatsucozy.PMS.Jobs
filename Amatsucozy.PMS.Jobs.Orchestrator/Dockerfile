FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Amatsucozy.PMS.Jobs.Orchestrator/Amatsucozy.PMS.Jobs.Orchestrator.csproj", "Amatsucozy.PMS.Jobs.Orchestrator/"]
RUN dotnet restore "Amatsucozy.PMS.Jobs.Orchestrator/Amatsucozy.PMS.Jobs.Orchestrator.csproj"
COPY . .
WORKDIR "/src/Amatsucozy.PMS.Jobs.Orchestrator"
RUN dotnet build "Amatsucozy.PMS.Jobs.Orchestrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Amatsucozy.PMS.Jobs.Orchestrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Amatsucozy.PMS.Jobs.Orchestrator.dll"]
