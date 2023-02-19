FROM mcr.microsoft.com/dotnet/aspnet:6.0  AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./Notification.Host/Notification.Host.csproj .
RUN dotnet restore Notification.Host.csproj
COPY . .
WORKDIR /src/Notification.Host
RUN dotnet build "Notification.Host.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.Host.csproj" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Host.dll"]