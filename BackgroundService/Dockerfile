FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR /src
COPY ["global.json", ""]
#COPY *.csproj /
COPY ["src/BackgroundService.Host/BackgroundService.Host.csproj", "src/BackgroundService.Host/"]
RUN dotnet restore "src/BackgroundService.Host/BackgroundService.Host.csproj"
COPY . .

WORKDIR "/src/src/BackgroundService.Host"
RUN dotnet build "BackgroundService.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackgroundService.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#COPY --from=publish /app/ .
ENTRYPOINT ["dotnet", "BackgroundService.Host.dll"]