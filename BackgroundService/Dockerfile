FROM mcr.microsoft.com/dotnet/aspnet:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:latest AS build
WORKDIR "/BackgroundService"

COPY ["global.json", ""]
COPY *.csproj /BackgroundService/
#COPY ["BackgroundService/BackgroundService.Host.csproj", "BackgroundService"]
#RUN dotnet restore "BackgroundService/BackgroundService.Host.csproj"
RUN dotnet restore
COPY . .

#WORKDIR "/BackgroundService"
WORKDIR /BackgroundService/
#RUN dotnet build "BackgroundService.Host.csproj" -c Release -o /app/build
RUN dotnet build  -c Release -o /app/build


FROM build AS publish
#RUN dotnet publish "BackgroundService.Host.csproj" -c Release -o /app/publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#COPY --from=publish /app/ .
#COPY --from=build /app/ .
ENTRYPOINT ["dotnet", "BackgroundService.Host.dll"]