using AsmResolver.PE.DotNet.Metadata;
using Bli.Common;
using Bli.EventBus;
using Bli.Infrastructure.Options;
using Bli.Infrastructure.Request;
using FFmpeg.NET;
using FFmpeg.NET.Enums;
using FFmpeg.NET.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using Resource.WebApi.Entity;
using StackExchange.Redis;
using System.Diagnostics;
using System.Runtime.InteropServices;


using static Resource.WebApi.Entity.Video;


namespace Resource.WebApi.BgService
{
    public class VideoSaveBackGroundTask : BackgroundService
    {
        private readonly IConnectionMultiplexer redisConn;

        private readonly Engine ffmpeg  = new Engine(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? $"/usr/bin/ffmpeg"
                    : $"{AppDomain.CurrentDomain.BaseDirectory}ffmpeg.exe");

        private readonly IOptionsSnapshot<FileServiceOptions> fileService;

        private readonly ILogger<VideoSaveBackGroundTask> logger;

        private readonly IServiceScope serviceScope;


        private readonly string transcodingSavePath;

        private readonly string playerPath;

        private readonly string ffmpegKeyPath;

        private readonly string getKeyUrl;

        private readonly string RawVideoPath;

        private readonly string plyaerUrl;

        private readonly IMongoDatabase _database;

        private readonly IEventBus eventBus;

        private readonly ConversionOptions hightOptions = new ConversionOptions
        {
            VideoAspectRatio = VideoAspectRatio.R16_9,
            VideoSize = FFmpeg.NET.Enums.VideoSize.Hd720,
            VideoBitRate = 400,
        };

        private readonly ConversionOptions lowOptions = new ConversionOptions
        {
            VideoAspectRatio = VideoAspectRatio.R16_9,
            VideoSize = FFmpeg.NET.Enums.VideoSize.Hd480,
            VideoBitRate = 1000
        };

        public VideoSaveBackGroundTask(IServiceScopeFactory spf,IConfiguration configuration)
        {
            this.serviceScope = spf.CreateScope();
            var sp = serviceScope.ServiceProvider;
          
            this.eventBus = sp.GetRequiredService<IEventBus>();
            this.logger = sp.GetRequiredService<ILogger<VideoSaveBackGroundTask>>();
            this.fileService = sp.GetRequiredService<IOptionsSnapshot<FileServiceOptions>>();
            this.redisConn = sp.GetRequiredService<IConnectionMultiplexer>();
            transcodingSavePath = Path.Combine(fileService.Value.RootPath,configuration.GetValue<string>("FilePath:TrVSavePath"));
            playerPath = Path.Combine(fileService.Value.RootPath, configuration.GetValue<string>("FilePath:TSChunck"));
            ffmpegKeyPath = Path.Combine(fileService.Value.RootPath, configuration.GetValue<string>("FilePath:FfmpegKey"));
            getKeyUrl = configuration.GetValue<string>("FilePath:KeyUrl");
            RawVideoPath = configuration.GetValue<string>("FilePath:VideoPath");
            plyaerUrl = configuration.GetValue<string>("FilePath:TSChunck");
           
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = redisConn.GetDatabase();
            ffmpeg.Error += OnError;
            while (!stoppingToken.IsCancellationRequested)
            {
                var readyVideo = await db.SetMembersAsync("video_transcoding_ready");
                UploadVideoFormRequst videoform = default;
                foreach (var fileName in readyVideo)
                {
                    await db.KeyExpireAsync(fileName.ToString(), TimeSpan.FromMinutes(30));
                    bool flag = true;
                    string input_file = Path.Combine(fileService.Value.RootPath, RawVideoPath, fileName, fileName);
                    string out_folder = Path.Combine(fileService.Value.RootPath, transcodingSavePath, fileName);
                    try
                    {
                        var meta = await ffmpeg.GetMetaDataAsync(new InputFile(input_file), stoppingToken);

                        //拿到的字符格式是 180 x 120
                        float w = float.Parse(meta.VideoData.FrameSize.Split("x")[0]);
                        float h = float.Parse(meta.VideoData.FrameSize.Split("x")[1]);
                     
                        List<Resouce> resouces = new List<Resouce>();
                        Directory.CreateDirectory(out_folder);
                       
                        if (Math.Abs((w / h) -(16.0f/9.0f)) < 0.001)
                        {
                            if((w * h) >= 1080 * 720)
                            {
                                string ouut_Hfilepath = Path.Combine(out_folder,  "720.mp4");
                                string ouut_Lfilepath = Path.Combine(out_folder, "480.mp4");
                                await ffmpeg.ConvertAsync(new InputFile(input_file),
                                    new OutputFile(ouut_Hfilepath), hightOptions, stoppingToken);

                                await ffmpeg.ConvertAsync(new InputFile(input_file),
                                    new OutputFile(ouut_Lfilepath), lowOptions, stoppingToken);

                                resouces.Add(await VideoResource(ouut_Hfilepath, 500 * 1024, Bli.Infrastructure.Enum.VideoSize.Hd720, true,  stoppingToken));
                                resouces.Add(await VideoResource(ouut_Lfilepath, 500 * 1024, Bli.Infrastructure.Enum.VideoSize.Hd480,false, stoppingToken));
                            }else
                            {
                                string ouut_Lfilepath = Path.Combine(out_folder, Guid.NewGuid() + ".mp4");
                                resouces.Add(await VideoResource(ouut_Lfilepath, 500 * 1024, Bli.Infrastructure.Enum.VideoSize.Hd480, false, stoppingToken));
                            }                          
                        }
                        else
                        {
                            string out_path = Path.Combine(out_folder, Guid.NewGuid() + ".mp4");

                            await ffmpeg.ConvertAsync(new InputFile(input_file),
                            new OutputFile(out_path), stoppingToken);
                            resouces.Add(await VideoResource(out_path, 500 * 1024, Bli.Infrastructure.Enum.VideoSize.Hd480, false, stoppingToken));
                        }

                        videoform = JsonConvert.DeserializeObject<UploadVideoFormRequst>(await db.HashGetAsync(fileName.ToString(),"Form")) ;
                        var add = new Video
                           (
                           videoform.Title,
                           videoform.Description,
                           videoform.UserId.Value,
                           videoform.Cover,
                           videoform.VideoType, meta.Duration.TotalMilliseconds,
                           resouces
                           );

                        eventBus.Publish("Video.Create", add);
                    }
                    catch (Exception e)
                    {
                        flag = false;
                        logger.LogInterpolatedError($"转码失败",e);
                    }
                    finally
                    {
                        await db.KeyDeleteAsync(fileName.ToString());
                        //发布转码结果
                        eventBus.Publish("Resource.Notification",new { video = videoform, success = flag });
                        await db.SetRemoveAsync("video_transcoding_ready", fileName.ToString());
                       
                    } 
                }
                await Task.Delay(3000);
            }
        }

        private async Task<Resouce> VideoResource(string inputFilePath, int pieceSize, Bli.Infrastructure.Enum.VideoSize videoSize, bool isAddKey, CancellationToken token)
        {
            var video = await ffmpeg.GetMetaDataAsync(new InputFile(inputFilePath), token);
            int numberOfPieces = (int)MathF.Ceiling(video.FileInfo.Length / (float)pieceSize);
            int slice_time = (int)video.Duration.TotalSeconds / numberOfPieces;
            string forder = Guid.NewGuid().ToString();
            string outfile_path = Path.Combine(playerPath, forder);
            Directory.CreateDirectory(outfile_path);

            Resouce resource;
            string ffmpegArguments;

            if (isAddKey)
            {
                var (key, iv) = KeyHlsHelper.GenerateRandomKeyAndIV();
                string keyInfoFileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + ".keyinfo";
                string keyfile_path = Path.Combine(ffmpegKeyPath, keyInfoFileName);

                resource = new Resouce(Path.Combine(plyaerUrl, forder, "playlist.m3u8"), inputFilePath, key, videoSize);
                KeyHlsHelper.GenerateKeyInfoFile(keyfile_path, getKeyUrl + resource.Id, key, iv);

                ffmpegArguments = $"-i \"{inputFilePath}\" -c copy -hls_time {slice_time} -hls_key_info_file \"{keyfile_path}\" -hls_list_size 0 -hls_playlist_type vod -hls_segment_filename \"{Path.Combine(outfile_path, "file%03d.ts")}\" \"{Path.Combine(outfile_path, "playlist.m3u8")}\"";
            }
            else
            {
                ffmpegArguments = $"-i \"{inputFilePath}\" -c copy -hls_time {slice_time} -hls_list_size 0 -hls_playlist_type vod -hls_segment_filename \"{Path.Combine(outfile_path, "file%03d.ts")}\" \"{Path.Combine(outfile_path, "playlist.m3u8")}\"";
                resource = new Resouce(Path.Combine(plyaerUrl, forder, "playlist.m3u8"), inputFilePath, string.Empty,videoSize);
            }

            ExecuteFFmpegCommand(ffmpegArguments);
            return resource;
        }

        private void OnError(object sender, ConversionErrorEventArgs e)
        {
            throw new Exception(e.Exception.ToString());
        }


        public static void ExecuteFFmpegCommand(string arguments)
        {
            using Process process = new Process();
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception(stderr);
            }
        }
    }
}
