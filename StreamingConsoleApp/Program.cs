using LiveStreamingServerNet;
using FxResources.Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.Extensions.Logging;

using var server = LiveStreamingServerBuilder.Create()
    .ConfigureLogging(options => options.AddConsole())
    .Build();

await server.RunAsync(new IPEndPoint(IPAddress.Any, 1935));