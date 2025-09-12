using System.ComponentModel;
using LiveStreamingServerNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using LiveStreamingServerNet.Rtmp;
using LiveStreamingServerNet.StreamProcessor.AspNetCore.Configurations;
using LiveStreamingServerNet.StreamProcessor.AspNetCore.Installer;
using LiveStreamingServerNet.StreamProcessor.Contracts;
using LiveStreamingServerNet.StreamProcessor.Hls.Contracts;
using LiveStreamingServerNet.StreamProcessor.Installer;
using LiveStreamingServerNet.Utilities.Contracts;
using System.Net;
using LiveStreamingServerNet.Networking;
using LiveStreamingServerNet.Rtmp.Server.Contracts;
using LiveStreamingServerNet.StreamProcessor.Hls;
using LiveStreamingServerNet.StreamProcessor.Hls.Configurations;
using LiveStreamingServerNet.StreamProcessor.Utilities;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StreamingConsoleApp;

//2 Mile RTMP Config (Project Name: LiveStreamingServer.BaseDemo) :
/*
using var server = LiveStreamingServerBuilder.Create()
    .ConfigureLogging(options => options.AddConsole())
    .Build();

await server.RunAsync(new IPEndPoint(IPAddress.Any, 1935));
*/

    