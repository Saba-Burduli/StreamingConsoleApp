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
using LiveStreamingServerNet.StreamProcessor.Hls;
using LiveStreamingServerNet.StreamProcessor.Hls.Configurations;
using Microsoft.AspNetCore.Mvc.Diagnostics;
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
    
// Full HlsDemo (Project Name : LiveStreamingServerNet.HlsDemo)
 public static class Program
    {
        public static async Task Main(string[] args)
        {
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "hls-output");
            new DirectoryInfo(outputDir).Create();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLiveStreamingServer(outputDir);

            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader()
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                )
            );

            var app = builder.Build();

            app.UseCors();

            // Given that the scheme is https, the port is 7138, and the stream path is live/demo,
            // the HLS stream will be available at https://localhost:7138/hls/live/demo/output.m3u8
            app.UseHlsFiles(new HlsServingOptions
            {
                Root = outputDir,
                RequestPath = "/hls"
            });

            await app.RunAsync();
        }
        private static IServiceCollection AddLiveStreamingServer(this IServiceCollection services, string outputDir)
        {
            return services.AddLiveStreamingServer(
                new IPEndPoint(IPAddress.Any, 1935),
                options => options
                    .Configure(options => options.EnableGopCaching = false)
                    .AddVideoCodecFilter(builder => builder.Include(VideoCodec.AVC).Include(VideoCodec.HEVC))
                    .AddAudioCodecFilter(builder => builder.Include(AudioCodec.AAC))
                    .AddStreamProcessor(options =>
                    {
                        options.AddStreamProcessorEventHandler(svc =>
                                new StreamProcessorEventListener(outputDir, svc.GetRequiredService<ILogger<StreamProcessorEventListener>>()));
                    })
                    .AddHlsTransmuxer(options => options.Configure(config => config.OutputPathResolver = new HlsOutputPathResolver(outputDir)))
            );
        }

        private class HlsOutputPathResolver : IHlsOutputPathResolver
        {
            private readonly string _outputDir;

            public HlsOutputPathResolver(string outputDir)
            {
                _outputDir = outputDir;
            }

            public ValueTask<string> ResolveOutputPath(IServiceProvider services, Guid contextIdentifier, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
            {
                return ValueTask.FromResult(Path.Combine(_outputDir, contextIdentifier.ToString(), "output.m3u8"));
            }
        }

        private class StreamProcessorEventListener : IStreamProcessorEventHandler
        {
            private readonly string _outputDir;
            private readonly ILogger _logger;

            public StreamProcessorEventListener(string outputDir, ILogger<StreamProcessorEventListener> logger)
            {
                _outputDir = outputDir;
                _logger = logger;
            }

            public Task OnStreamProcessorStartedAsync(IEventContext context, string processor, Guid identifier, uint clientId, string inputPath, string outputPath, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
            {
                outputPath = Path.GetRelativePath(_outputDir, outputPath);
                _logger.LogInformation($"[{identifier}] Streaming processor {processor} started: {inputPath} -> {outputPath}");
                return Task.CompletedTask;
            }

            public Task OnStreamProcessorStoppedAsync(IEventContext context, string processor, Guid identifier, uint clientId, string inputPath, string outputPath, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
            {
                outputPath = Path.GetRelativePath(_outputDir, outputPath);
                _logger.LogInformation($"[{identifier}] Streaming processor {processor} stopped: {inputPath} -> {outputPath}");
                return Task.CompletedTask;
            }
        }
    }
    