using AsmResolver.PE.DotNet.Metadata;
using Bli.Common;
using FFmpeg.NET;
using FFmpeg.NET.Enums;
using FFmpeg.NET.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Diagnostics;
using System.Runtime.InteropServices;
using VideoService.Domain.Entities;
using VideoService.Domain.Entities.Enum;
using VideoService.Domain.Options;
using VideoService.Infrastructure;
using VideoService.WebAPI.Videos.Request;

namespace VideoService.WebAPI.BgService
{
    public class VideoSaveBackGroundTask : BackgroundService
    {
        private readonly IConnectionMultiplexer redisConn;

        private readonly VideoDbContext videoDbContext;

   
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
            this.videoDbContext = sp.GetRequiredService<VideoDbContext>();
            this.logger = sp.GetRequiredService<ILogger<VideoSaveBackGroundTask>>();
            this.fileService = sp.GetRequiredService<IOptionsSnapshot<FileServiceOptions>>();
            this.redisConn = sp.GetRequiredService<IConnectionMultiplexer>();
            transcodingSavePath = Path.Combine(fileService.Value.RootPath,configuration.GetValue<string>("FilePath:TrVSavePath"));
            playerPath = Path.Combine(fileService.Value.RootPath, configuration.GetValue<string>("FilePath:TSChunck"));
            ffmpegKeyPath = Path.Combine(fileService.Value.RootPath, configuration.GetValue<string>("FilePath:FfmpegKey"));
            getKeyUrl = configuration.GetValue<string>("FilePath:KeyUrl");
            RawVideoPath = configuration.GetValue<string>("FilePath:VideoPath");
            plyaerUrl = configuration.GetValue<string>("FilePath:TSChunck");
            _database = new MongoClient(sp.GetRequiredService<IOptionsSnapshot<MongoDbSettings>>().
                Value.ConnectionString).GetDatabase(sp.GetRequiredService<IOptionsSnapshot<MongoDbSettings>>().
                Value.DatabaseName);    
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = redisConn.GetDatabase();
            ffmpeg.Error += OnError;
            while (!stoppingToken.IsCancellationRequested)
            {
                var readyVideo = await db.SetMembersAsync("video_transcoding_ready");
                foreach(var fileName in readyVideo)
                {
                    string input_file = Path.Combine(fileService.Value.RootPath, RawVideoPath, fileName, fileName);
                    string out_folder = Path.Combine(fileService.Value.RootPath, transcodingSavePath, fileName);
                    try
                    {
                        var meta = await ffmpeg.GetMetaDataAsync(new InputFile(input_file), stoppingToken);

                        float w = float.Parse(meta.VideoData.FrameSize.Split("x")[0]);
                        float h = float.Parse(meta.VideoData.FrameSize.Split("x")[1]);
                     
                        List<VideoResouce> resouces = new List<VideoResouce>();
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

                                resouces.Add(await VideoResource(ouut_Hfilepath, 500 * 1024, Domain.Entities.Enum.VideoSize.Hd720, true,  stoppingToken));
                                resouces.Add(await VideoResource(ouut_Lfilepath, 500 * 1024, Domain.Entities.Enum.VideoSize.Hd480,false, stoppingToken));
                            }else
                            {
                                string ouut_Lfilepath = Path.Combine(out_folder, Guid.NewGuid() + ".mp4");
                                resouces.Add(await VideoResource(ouut_Lfilepath, 500 * 1024, Domain.Entities.Enum.VideoSize.Hd480, false, stoppingToken));
                            }                          
                        }
                        else
                        {
                            string out_path = Path.Combine(out_folder, Guid.NewGuid() + ".mp4");

                            await ffmpeg.ConvertAsync(new InputFile(input_file),
                            new OutputFile(out_path), stoppingToken);
                            resouces.Add(await VideoResource(out_path, 500 * 1024, Domain.Entities.Enum.VideoSize.Hd480, false, stoppingToken));

                        }
                        
                        var form = JsonConvert.DeserializeObject<UploadVideoFormRequst>(await db.HashGetAsync(fileName.ToString(),"From")) ;
                        var add = new Video
                           (
                           form.Title,
                           form.Description,
                           form.UserId.Value,
                           form.Cover,
                           form.VideoType, meta.Duration.TotalMilliseconds,
                           resouces
                           );
                        await videoDbContext.Video.AddAsync(add);

                        await   videoDbContext.SaveChangesAsync();
                       _database.CreateCollection("dan"+add.Id);
                    }
                    catch (Exception e)
                    {
                        logger.LogInterpolatedError($"转码失败",e);
                    }
                    finally
                    {
                        await db.SetRemoveAsync("video_transcoding_ready", fileName.ToString());
                        await db.HashDeleteAsync(fileName.ToString(), "From");
                        Directory.Delete(Path.Combine(fileService.Value.RootPath, RawVideoPath, fileName), true);
                    }
                    
                }
                await Task.Delay(3000);
            }
        }

        private async Task<VideoResouce> VideoResource(string inputFilePath, int pieceSize, Domain.Entities.Enum.VideoSize videoSize, bool isAddKey, CancellationToken token)
        {
            var video = await ffmpeg.GetMetaDataAsync(new InputFile(inputFilePath), token);
            int numberOfPieces = (int)MathF.Ceiling(video.FileInfo.Length / (float)pieceSize);
            int slice_time = (int)video.Duration.TotalSeconds / numberOfPieces;
            string forder = Guid.NewGuid().ToString();
            string outfile_path = Path.Combine(playerPath, forder);
            Directory.CreateDirectory(outfile_path); 

            VideoResouce resource;
            string ffmpegArguments;

            if (isAddKey)
            {
                var (key, iv) = KeyHlsHelper.GenerateRandomKeyAndIV();
                string keyInfoFileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + ".keyinfo";
                string keyfile_path = Path.Combine(ffmpegKeyPath, keyInfoFileName);

                resource = new VideoResouce(Path.Combine(plyaerUrl, forder, "playlist.m3u8"), inputFilePath, key, videoSize);
                KeyHlsHelper.GenerateKeyInfoFile(keyfile_path, getKeyUrl + resource.Id, key, iv);

                ffmpegArguments = $"-i \"{inputFilePath}\" -c copy -hls_time {slice_time} -hls_key_info_file \"{keyfile_path}\" -hls_list_size 0 -hls_playlist_type vod -hls_segment_filename \"{Path.Combine(outfile_path, "file%03d.ts")}\" \"{Path.Combine(outfile_path, "playlist.m3u8")}\"";
            }
            else
            {
                ffmpegArguments = $"-i \"{inputFilePath}\" -c copy -hls_time {slice_time} -hls_list_size 0 -hls_playlist_type vod -hls_segment_filename \"{Path.Combine(outfile_path, "file%03d.ts")}\" \"{Path.Combine(outfile_path, "playlist.m3u8")}\"";
                resource = new VideoResouce(Path.Combine(plyaerUrl, forder, "playlist.m3u8"), inputFilePath, string.Empty,videoSize);
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
