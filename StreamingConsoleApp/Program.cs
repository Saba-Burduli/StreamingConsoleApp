using LiveStreamingServerNet;
using FxResources.Microsoft.Extensions.Logging;
using System.Net;
using LiveStreamingServerNet.StreamProcessor.FFmpeg.Contracts;
using LiveStreamingServerNet.StreamProcessor.Hls;
using LiveStreamingServerNet.StreamProcessor.Hls.Configurations;
using LiveStreamingServerNet.StreamProcessor.Hls.Contracts;
using LiveStreamingServerNet.StreamProcessor.Installer;
using LiveStreamingServerNet.StreamProcessor.Utilities;
using Microsoft.Extensions.Logging;

//2mile RTMP Config :
using var server = LiveStreamingServerBuilder.Create()
    .ConfigureLogging(options => options.AddConsole())
    .Build();

await server.RunAsync(new IPEndPoint(IPAddress.Any, 1935));

//RTMP + HLS Config :
 // var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Output");
 //
 // using var liveStreamingServer = LiveStreamingServerBuilder.Create()
 //     .ConfigureRtmpServer(options=>options
 //         .AddStreamProcessor()
 //         .AddAdaptiveHlsTranscoder(config =>
 //         {
 //             config.FFmpegPath = ExecutableFinder.FindExecutableFromPATH("ffmpeg")!;
 //             config.FFprobePath = ExecutableFinder.FindExecutableFromPATH("ffprobe")!;
 //             config.OutputPathResolver = new HlsOutputPathResolver(outputDirectory);
 //             
 //             config.DownsamplingFilters = new DownsamplingFilter[]
 //             {
 //                 new DownsamplingFilter(
 //                     Name: "360p",
 //                     Height: 360,
 //                     MaxVideoBitrate: "600k",
 //                     MaxAudioBitrate: "64k"
 //                     ),
 //                 new  DownsamplingFilter(
 //                     Name:"480p",
 //                     Height: 480,
 //                     MaxVideoBitrate: "1500k",
 //                     MaxAudioBitrate: "128k"
 //                     ),
 //                 new DownsamplingFilter(
 //                     Name: "720p",
 //                     Height: 720,
 //                     MaxVideoBitrate: "3000k",
 //                     MaxAudioBitrate: "256k"
 //                     ),
 //                 new DownsamplingFilter(
 //                     Name: "1080p ",
 //                     Height: 1080,
 //                     MaxVideoBitrate: "15000k ",
 //                     MaxAudioBitrate: "384k"
 //                 ),
 //                 
 //                 new DownsamplingFilter(
 //                     Name: "1440p",
 //                     Height: 1080,
 //                     MaxVideoBitrate: "30000k",
 //                     MaxAudioBitrate: "384k"
 //                 ),
 //                 
 //                 new DownsamplingFilter(
 //                     Name: "2160p",
 //                     Height: 2160,
 //                     MaxVideoBitrate: "85000k",
 //                     MaxAudioBitrate: "384k"
 //                 )
 //             };
 //         })
 //     )
 //     .ConfigureLogging(options=>options.AddConsole())
 //     .Build();
 //     
 //     await liveStreamingServer.RunAsync(new IPEndPoint(IPAddress.Any, 1935));
 //
 //     public class HlsOutputPathResolver : IFFmpegOutputPathResolver
 //     {
 //         private readonly string _outputPath;
 //
 //         public HlsOutputPathResolver(string outputPath)
 //         {
 //             _outputPath = outputPath;
 //         }
 //
 //         public ValueTask<string> ResolveOutputPath(IServiceProvider services, Guid contextIdentifier, string streamPath,
 //             IReadOnlyDictionary<string, string> streamArguments)
 //         {
 //             return ValueTask.FromResult((Path.Combine(_outputPath, streamPath.Trim('/'),"output.m3u8")));
 //         }
 //     }
 //

    // public static class Program
    // {
    //     public static async Task Main(string[] args)
    //     {
    //         var builder = WebApplication.CreateBuilder(args);
    //
    //         builder.Services.AddLiveStreamingServer();
    //
    //         builder.Services.AddCors(options =>
    //             options.AddDefaultPolicy(policy =>
    //                 policy.AllowAnyHeader()
    //                       .AllowAnyOrigin()
    //                       .AllowAnyMethod()
    //             )
    //         );
    //
    //         var app = builder.Build();
    //
    //         app.UseCors();
    //
    //         // Given that the scheme is https, the port is 7138, and the stream path is live/demo,
    //         // the HLS stream will be available at https://localhost:7138/live/demo/output.m3u8
    //         app.UseHlsFiles();
    //
    //         await app.RunAsync();
    //     }
    //
    //     private static IServiceCollection AddLiveStreamingServer(this IServiceCollection services)
    //     {
    //         return services.AddLiveStreamingServer(
    //             new IPEndPoint(IPAddress.Any, 1935),
    //             options => options
    //                 .Configure(options => options.EnableGopCaching = false)
    //                 .AddStreamProcessor(options =>
    //                 {
    //                     options.AddStreamProcessorEventHandler(svc =>
    //                         new StreamProcessorEventListener(
    //                             svc.GetRequiredService<ILogger<StreamProcessorEventListener>>()));
    //                 })
    //                 .AddAdaptiveHlsTranscoder(options =>
    //                 {
    //                     options.FFmpegPath = ExecutableFinder.FindExecutableFromPATH("ffmpeg")!;
    //                     options.FFprobePath = ExecutableFinder.FindExecutableFromPATH("ffprobe")!;
    //
    //                     options.DownsamplingFilters =
    //                     [
    //                         new DownsamplingFilter(
    //                             Name: "360p",
    //                             Height: 360,
    //                             MaxVideoBitrate: "600k",
    //                             MaxAudioBitrate: "64k"
    //                         ),
    //
    //                         new DownsamplingFilter(
    //                             Name: "480p",
    //                             Height: 480,
    //                             MaxVideoBitrate: "1500k",
    //                             MaxAudioBitrate: "128k"
    //                         ),
    //
    //                         new DownsamplingFilter(
    //                             Name: "720p",
    //                             Height: 720,
    //                             MaxVideoBitrate: "3000k",
    //                             MaxAudioBitrate: "256k"
    //                         )
    //                     ];
    //
    //                     // Hardware acceleration 
    //                     // options.VideoDecodingArguments = "-hwaccel auto -c:v h264_cuvid";
    //                     // options.VideoEncodingArguments = "-c:v h264_nvenc -g 30";
    //                 })
    //         );
    //     }
    //
    //     private class StreamProcessorEventListener : IStreamProcessorEventHandler
    //     {
    //         private readonly ILogger _logger;
    //
    //         public StreamProcessorEventListener(ILogger<StreamProcessorEventListener> logger)
    //         {
    //             _logger = logger;
    //         }
    //
    //         public Task OnStreamProcessorStartedAsync(IEventContext context, string processor, Guid identifier, uint clientId, string inputPath, string outputPath, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
    //         {
    //             _logger.LogInformation($"[{identifier}] Streaming processor {processor} started: {inputPath} -> {outputPath}");
    //             return Task.CompletedTask;
    //         }
    //
    //         public Task OnStreamProcessorStoppedAsync(IEventContext context, string processor, Guid identifier, uint clientId, string inputPath, string outputPath, string streamPath, IReadOnlyDictionary<string, string> streamArguments)
    //         {
    //             _logger.LogInformation($"[{identifier}] Streaming processor {processor} stopped: {inputPath} -> {outputPath}");
    //             return Task.CompletedTask;
    //         }
    //     }
    // }