using Bli.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using VideoService.Domain.Options;
using System.IO;
using VideoService.Infrastructure.Request;
using VideoService.WebAPI.Videos.Request;
using Microsoft.Extensions.Logging;
using VideoService.Domain.Entities.Enum;
using VideoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace VideoService.Infrastructure
{
    public class VideoUploadService
    {
        private readonly IConnectionMultiplexer redisConn;

        private readonly IOptionsSnapshot<FileServiceOptions> fileServer;
        private readonly ILogger<VideoUploadService> logger;

        private readonly VideoDbContext videodb;

        private readonly IConfiguration configuration;

        public VideoUploadService(IConnectionMultiplexer redisConn, IOptionsSnapshot<FileServiceOptions> fileServer, VideoDbContext video,
            IConfiguration configuration)
        {
            this.redisConn = redisConn;
            this.fileServer = fileServer;
            this.videodb = video;
            this.configuration = configuration;
        }
        //上传表单
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">视频封面</param>
        /// <param name="res"></param>
        /// <returns></returns>
        public async Task<IResult> UploadFromAsync(IFormFile file,UploadVideoFormRequst res)
        {
          
            string img_has = HashHelper.ComputeSha256Hash(file.OpenReadStream());
       
            var json = JsonConvert.SerializeObject(res);
            string form_hash = HashHelper.ComputeSha256Hash(json);
            string folder = Path.Combine(fileServer.Value.RootPath, $"{configuration.GetValue<string>("FilePath:ConverVideoSavePath")}", form_hash);
            string file_name = $"{Guid.NewGuid()}.{file.FileName.Split('.')[^1]}";
            try
            {
               
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string save_path = Path.Combine(folder, file_name);
               
                using var write = new FileStream(save_path, FileMode.Create, FileAccess.Write);
                using var steam = file.OpenReadStream();
                await steam.CopyToAsync(write);
                write.Close(); 
            }
            catch (Exception e)
            {
                logger.LogInterpolatedError($"视频表单上传出错",e);
                return Results.Json(new { datetime = DateTime.Now,result = false,keep = false });
            }
            res.Cover = Path.Combine($"{configuration.GetValue<string>("FilePath:ConverVideoSavePath")}", form_hash, file_name);
            var db = redisConn.GetDatabase();
         
            await db.HashSetAsync(form_hash, "From", JsonConvert.SerializeObject(res));
            await db.KeyExpireAsync(form_hash, TimeSpan.FromMinutes(30));
            return Results.Json(new { formhas = form_hash, datetime = DateTime.Now,result = true,keep = true });
        }

      

        public async Task<IResult> UploadCoverChunkSave(IFormFile file, VideoChunkFormRequst res)
        {
            var db = redisConn.GetDatabase();
            bool exist_key = await db.HashExistsAsync(res.FormHash, "From");
            if (!exist_key)
            {
                return Results.Json(new {result = false,code = 320});
            }

            //检查片段是否连续
            for (int i = 0;i<res.Index;i++)
            {
                bool flag = await db.HashExistsAsync(res.FormHash,i.ToString());
                if (!flag)
                {
                    return Results.Json(new {result = false, code = 321,lastIndex = i });
                }
            }
           

            exist_key = await db.HashExistsAsync(res.FormHash, res.Index);
            string save_path = Path.Combine(fileServer.Value.RootPath, $"{configuration.GetValue<string>("FilePath:ChunckVideoSavePath")}/{res.FormHash}");
            if (!Directory.Exists(save_path))
            {
                Directory.CreateDirectory(save_path);
            }
            string file_savepath = Path.Combine(save_path, $"{res.Index}");
            if (exist_key)
            {
                //寻找下一段缺失a
                var form = JsonConvert.DeserializeObject<UploadVideoFormRequst>(await db.HashGetAsync(res.FormHash, "From"));
                int i = res.Index + 1;
                for (; i < form.SliceCount; i++)
                {
                    bool flag = await db.HashExistsAsync(res.FormHash, i.ToString());
                    if(!flag)
                    {
                        break;
                    }
                }
                return Results.Json(new { result = false, code = 322, nextIndex = i });
            }
            try
            {
                using var stream = new FileStream(file_savepath, FileMode.CreateNew, FileAccess.Write);
                file.OpenReadStream();
                await file.CopyToAsync(stream);
               
            }
            catch (IOException e)
            {
                logger.LogInterpolatedError($"视频片段写入失败", e);
                return Results.Json(new { result = false });
            }
            bool r = await db.HashSetAsync(res.FormHash, res.Index, file_savepath);
            return Results.Json(new { result = r, datetime = DateTime.Now});
        }

        public async Task<IResult> CompleteDoSave(string formHash)
        {
            var redisValueDic = new Dictionary<int, Task<RedisValue>>();
            var db = redisConn.GetDatabase();
            bool exit = await db.HashExistsAsync(formHash, "From");
            if (!exit)
            {
                return Results.Json(new { Error = "from is null" });
            }
            UploadVideoFormRequst? res = JsonConvert.DeserializeObject<UploadVideoFormRequst>(await db.HashGetAsync(formHash, "From"));
            if (res is null)
            {
                return Results.Json(new { result = false, Error = "布里布里？" });
            }
            
            var complete_path = Path.Combine(fileServer.Value.RootPath, $"{configuration.GetValue<string>("FilePath:VideoPath")}/{formHash}");
            var file_path = Path.Combine(complete_path, $"{formHash}");
            if (!Directory.Exists(complete_path))
            {
                Directory.CreateDirectory(complete_path);
                var batch = db.CreateBatch();
                for (int i = 0; i < res.SliceCount; i++)
                {
                    var path = batch.HashGetAsync(formHash, i);
                    redisValueDic.Add(i, path);
                }
                batch.Execute();
                try
                {
                    using (var fws = new FileStream(file_path, FileMode.CreateNew, FileAccess.Write))
                    {
                        for (int i = 0; i < redisValueDic.Count; i++)
                        {
                            var path = (await redisValueDic[i]).ToString();
                            using (var frs = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                byte[] buffer = new byte[1024];
                                int r;
                                while ((r = await frs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fws.WriteAsync(buffer, 0, r);
                                    
                                }
                            }
                            await fws.FlushAsync();
                        }
                        
                    }
                    batch = db.CreateBatch();
                    for (int i = 0; i < res.SliceCount; i++)
                    {
                        var resbool = batch.HashDeleteAsync(formHash, i);
                    }
                    batch.Execute();

                    await db.SetAddAsync("video_transcoding_ready", formHash);
                    return Results.Json(new { result = true });
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] This is an error message." + e);
                    logger.LogInterpolatedError($"视频合并失败", e);
                    return Results.Json(new { result = false });
                }
            }
           
            return Results.Json(new { result = false });

        }
    }
}
