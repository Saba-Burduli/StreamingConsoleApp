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


//RTMP + HLS Config (Project Name : LiveStreamingServerNet.Adaptive.HlsDemo) :
/*public static class Program
    { 
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLiveStreamingServer();

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader()
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                )
            );

            var app = builder.Build();

            app.UseCors();
             
            app.UseHlsFiles();

            await app.RunAsync();
        }
         
        private static IServiceCollection AddLiveStreamingServer(this IServiceCollection services)
        {
            return services.AddLiveStreamingServer(
                new IPEndPoint(IPAddress.Any, 1935),
                options => options
                    .Configure(options => options.EnableGopCaching = false)
                    .AddStreamProcessor(options=>
                    {
                        options.AddStreamProcessorEventHandler(svc =>
                            new StreamProcessorEventListener(
                                svc.GetRequiredService<ILogger>()));
                    })
                    .AddAdaptiveHlsTranscoder(options =>
                    {
                        options.FFmpegPath = ExecutableFinder.FindExecutableFromPATH("ffmpeg")!;
                        options.FFprobePath = ExecutableFinder.FindExecutableFromPATH("ffprobe")!;

                        options.DownsamplingFilters =
                        [
                            new DownsamplingFilter(
                                Name: "360p",
                                Height: 360,
                                MaxVideoBitrate: "600k",
                                MaxAudioBitrate: "64k"
                            ),

                            new DownsamplingFilter(
                                Name: "480p",
                                Height: 480,
                                MaxVideoBitrate: "1500k",
                                MaxAudioBitrate: "128k"
                            ),

                            new DownsamplingFilter(
                                Name: "720p",
                                Height: 720,
                                MaxVideoBitrate: "3000k",
                                MaxAudioBitrate: "256k"
                            )
                        ];

                        // Hardware acceleration 
                        // options.VideoDecodingArguments = "-hwaccel auto -c:v h264_cuvid";
                        // options.VideoEncodingArguments = "-c:v h264_nvenc -g 30";
                    })
            );
        }

        private class StreamProcessorEventListener : IStreamProcessorEventHandler
        {
            private readonly ILogger _logger;

            public StreamProcessorEventListener(ILogger<StreamProcessorEventListener> logger)
            {
                _logger = logger;
            }

            public Task OnStreamProcessorStartedAsync(IEventContext context, string processor, Guid identifier, uint clientId, string inputPath, string outputPath, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
            {
                _logger.LogInformation($"[{identifier}] Streaming processor {processor} started: {inputPath} -> {outputPath}");
                return Task.CompletedTask;
            }

            public Task OnStreamProcessorStoppedAsync(IEventContext context, string processor, Guid identifier, uint clientId, string inputPath, string outputPath, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
            {
                _logger.LogInformation($"[{identifier}] Streaming processor {processor} stopped: {inputPath} -> {outputPath}");
                return Task.CompletedTask;
            }
        }
    }*/
    

    
    
    // Project Name (LiveStreamingServerNet.OnDemandStreamCapturingDemo) For Capturing and Snapshot streamArguments
    
    /*public static class Program
    {
        public static async Task Main()
        {
            var builder = Host.CreateApplicationBuilder();

            var endPoint = new ServerEndPoint(new IPEndPoint(IPAddress.Any, 1935), false);
            builder.Services.AddLiveStreamingServer(endPoint, options =>
            {
                options.AddStreamProcessor()
                    .AddOnDemandStreamCapturer(options =>
                    {
                        options.FFmpegPath = ExecutableFinder.FindExecutableFromPATH("ffmpeg")!;
                    });
            });

            builder.Services.AddHostedService<StreamCapturer>();

            var app = builder.Build();
            await app.RunAsync();
        }
    }

    public class StreamCapturer : BackgroundService
    {
        private readonly IRtmpStreamInfoManager _streamInfoManager;
        private readonly IOnDemandStreamCapturer _capturer;
        private readonly ILogger<StreamCapturer> _logger;
        private readonly string _outputDir;

        private const int MaxConcurrency = 10;

        public StreamCapturer(IRtmpStreamInfoManager streamInfoManager, IOnDemandStreamCapturer capturer, ILogger<StreamCapturer> logger)
        {
            _streamInfoManager = streamInfoManager;
            _capturer = capturer;
            _logger = logger;

            _outputDir = Path.Combine(Directory.GetCurrentDirectory(), "capture-output");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var streamInfos = _streamInfoManager.GetStreamInfos();

                foreach (var streams in streamInfos.Chunk(MaxConcurrency))
                {
                    await Task.WhenAll(streams.Select(async s =>
                    {
                        try
                        {
                            var outputDir = Path.Combine(_outputDir, s.Publisher.Id.ToString());
                            new DirectoryInfo(outputDir).Create();

                            // Use extensions such as .png, .jpg, .bmp, .tiff, .webp
                            var snapshotOutputPath = Path.Combine(outputDir, "snapshot.png");
                            await _capturer.CaptureSnapshotAsync(
                                streamPath: s.StreamPath,
                                streamArguments: s.StreamArguments,
                                outputPath: Path.Combine(_outputDir, snapshotOutputPath),
                                height: 512,
                                cancellationToken: stoppingToken
                            );

                            _logger.LogInformation("Captured stream {StreamPath} to {OutputPath}", s.StreamPath, snapshotOutputPath);

                            // Use extensions such as .webp, .webm, .gif, .mp4
                            var webpOutputPath = Path.Combine(outputDir, "clip.webp");
                            await _capturer.CaptureClipAsync(
                                streamPath: s.StreamPath,
                                streamArguments: s.StreamArguments,
                                outputPath: Path.Combine(_outputDir, webpOutputPath),
                                options: new ClipCaptureOptions(TimeSpan.FromSeconds(3))
                                {
                                    Framerate = 24,
                                    Height = 512
                                },
                                cancellationToken: stoppingToken
                            );

                            _logger.LogInformation("Captured clip from stream {StreamPath} to {OutputPath}", s.StreamPath, webpOutputPath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error capturing stream {StreamPath}: {Exception}", s.StreamPath, ex);
                        }
                    }));
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }*/
    
    