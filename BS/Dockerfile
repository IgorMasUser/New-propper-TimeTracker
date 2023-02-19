FROM mcr.microsoft.com/dotnet/aspnet:6.0  AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./BS/BS.csproj .
RUN dotnet restore BS.csproj
COPY . .
WORKDIR /src/BS
RUN dotnet build "BS.csproj" -o /app/build

FROM build AS publish
RUN dotnet publish "BS.csproj" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BS.dll"]