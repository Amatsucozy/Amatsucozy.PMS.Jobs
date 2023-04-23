FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Amatsucozy.PMS.Jobs.Commander/Amatsucozy.PMS.Jobs.Commander.csproj", "Amatsucozy.PMS.Jobs.Commander/"]
RUN dotnet restore "Amatsucozy.PMS.Jobs.Commander/Amatsucozy.PMS.Jobs.Commander.csproj"
COPY . .
WORKDIR "/src/Amatsucozy.PMS.Jobs.Commander"
RUN dotnet build "Amatsucozy.PMS.Jobs.Commander.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Amatsucozy.PMS.Jobs.Commander.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Amatsucozy.PMS.Jobs.Commander.dll"]
