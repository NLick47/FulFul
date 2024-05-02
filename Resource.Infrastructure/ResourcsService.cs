using Bli.Common;
using Bli.EventBus;
using Bli.Infrastructure.Entity;
using Bli.Infrastructure.Options;
using Bli.Infrastructure.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resource.Daomain;
using Resource.Daomain.Respose;
using StackExchange.Redis;



namespace Resource.Infrastructure
{
    public class ResourcsService : IResourceUpload
    {
        private readonly IOptionsSnapshot<FileServiceOptions> fileServer;
        private readonly IConfiguration configuration;
        private readonly IConnectionMultiplexer redisConn;
        private readonly ILogger<ResourcsService> logger;
        private readonly IEventBus eventBus;


        public ResourcsService(IOptionsSnapshot<FileServiceOptions> fileServer, IConfiguration configuration,
            IConnectionMultiplexer redisConn, ILogger<ResourcsService> logger, IEventBus eventBus)
        {
            this.configuration = configuration;
            this.fileServer =  fileServer;
            this.redisConn = redisConn; 
            this.logger = logger;  
            this.eventBus= eventBus;
        }

        public async Task<ChunkRespose> UploadCoverChunkSave(IFormFile file, VideoChunkFormRequst res)
        {
            var db = redisConn.GetDatabase();
            bool exist_key = await db.HashExistsAsync(res.FormHash, "Form");
            if (!exist_key)
            {
                return new ChunkRespose() { Code = Daomain.Respose.Enum.ChunkCode.FormExist };
            }

            //检查片段是否连续
            for (int i = 0; i < res.Index; i++)
            {
                bool flag = await db.HashExistsAsync(res.FormHash, i.ToString());
                if (!flag)
                {
                    return new ChunkRespose() { Code = Daomain.Respose.Enum.ChunkCode.MissChunk,MissingIndex = i };
                }
            }
            //当前片段已存在，表示已经上传了要找到下一片缺少的index
            exist_key = await db.HashExistsAsync(res.FormHash, res.Index);
            string save_path = Path.Combine(fileServer.Value.RootPath, $"{configuration.GetValue<string>("FilePath:ChunckVideoSavePath")}/{res.FormHash}");
            if (!Directory.Exists(save_path))
            {
                Directory.CreateDirectory(save_path);
            }
            string file_savepath = Path.Combine(save_path, $"{res.Index}");
            var form = JsonConvert.DeserializeObject<UploadVideoFormRequst>(await db.HashGetAsync(res.FormHash, "Form"));
            if (exist_key)
            {
                int i = res.Index + 1;
                for (; i < form.SliceCount; i++)
                {
                    bool flag = await db.HashExistsAsync(res.FormHash, i.ToString());
                    if (!flag)
                    {
                        break;
                    }
                }
                return new ChunkRespose() { Code = Daomain.Respose.Enum.ChunkCode.MissChunk,MissingIndex = i};
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
                return new ChunkRespose() { Code = Daomain.Respose.Enum.ChunkCode.CreateError,MissingIndex = res.Index };
            }
            bool r = await db.HashSetAsync(res.FormHash, res.Index, file_savepath);
            //片段全部上传完成，发送一个合并事件
            if(r && res.Index +1 == form.SliceCount)
            {
                dynamic s = new Medium { };
                eventBus.Publish("Resource.Completed",new { formHash = res.FormHash });
            }
            return new ChunkRespose() { Code = Daomain.Respose.Enum.ChunkCode.Succeed};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">视频封面</param>
        /// <param name="res"></param>
        /// <returns></returns>

        public async Task<FormRespose> UploadFromAsync(IFormFile file, UploadVideoFormRequst res)
        {
            var json = JsonConvert.SerializeObject(res);
            string form_hash = HashHelper.ComputeSha256Hash(json + file.FileName);

            var db = redisConn.GetDatabase();
            
            //表单存在可能是上次没上传完成，返回true让前端再调用一次片段上传接口寻找缺失
            bool exist =  await db.HashExistsAsync(form_hash,"Form");
            if(exist) return new FormRespose() { Formhas = form_hash, Result = true };

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
                logger.LogInterpolatedError($"视频表单上传出错", e);
                return new FormRespose() { Formhas = string.Empty,Result = false };
            }
            res.Cover = Path.Combine($"{configuration.GetValue<string>("FilePath:ConverVideoSavePath")}", form_hash, file_name);
           
            await db.HashSetAsync(form_hash, "Form", JsonConvert.SerializeObject(res));
            await db.KeyExpireAsync(form_hash,TimeSpan.FromMinutes(30));
            return new FormRespose() {Formhas = form_hash,Result = true };
        }
    }
}
