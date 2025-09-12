FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StreamingConsoleApp.HlsDemo/StreamingConsoleApp.HlsDemo.csproj", "StreamingConsoleApp.HlsDemo/"]
RUN dotnet restore "StreamingConsoleApp.HlsDemo/StreamingConsoleApp.HlsDemo.csproj"
COPY . .
WORKDIR "/src/StreamingConsoleApp.HlsDemo"
RUN dotnet build "StreamingConsoleApp.HlsDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StreamingConsoleApp.HlsDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StreamingConsoleApp.HlsDemo.dll"]
