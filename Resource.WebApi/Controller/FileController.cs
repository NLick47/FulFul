using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Bli.EventBus;
using Resource.Infrastructure;
using SixLabors.ImageSharp;
using Resource.Daomain.Respose;
using Microsoft.Data.SqlClient.Server;
using Bli.Infrastructure.Options;
using Bli.Infrastructure.Request;
using Resource.Daomain;

namespace Resource.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
 
    public class FileController : ControllerBase
    {
        private readonly IResourceUpload resourcs;

        private readonly IOptionsSnapshot<FileServiceOptions> fileOptions;

        private readonly IEventBus eventBus;
        public FileController(IResourceUpload videoUploadService, IOptionsSnapshot<FileServiceOptions> fileOptions,
            IEventBus eventBus)
        {
            this.resourcs = videoUploadService;
            this.fileOptions = fileOptions;
            this.eventBus = eventBus;
        }

        
        [HttpPost]
        public async Task<IActionResult> UploadChunk(IFormFile file, [FromForm] VideoChunkFormRequst requst)
        {
            var res = await resourcs.UploadCoverChunkSave(file, requst);
            return Ok(new { result = res.Code==Resource.Daomain.Respose.Enum.ChunkCode.Succeed,data = res });
        }

        [HttpPost]
        public async Task<IActionResult> UploadFrom(IFormFile file, [FromForm] UploadVideoFormRequst requst)
        {
            string ? exten = fileOptions.Value.ImgExtensions.SingleOrDefault(x => file.FileName.EndsWith(x));
            FormRespose formRespose= null;  
            if (exten is null)
            {
                return BadRequest(new { result = false, error = "不支持该图片格式" });
            }
            if (file.Length > fileOptions.Value.ImagingMaxSize)
            {
                return BadRequest(new { result = false, error = "图片最大不超过" + fileOptions.Value.ImagingMaxSize });
            }
            try
            {
                using var stream = file.OpenReadStream();
                string mime = file.ContentType.ToLower();
                var imageInfo = Image.Identify(stream);
                if (imageInfo is null)
                {
                    throw new Exception("上传失败");
                }
                var bounds = imageInfo.Bounds;
                requst.UserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                formRespose = await resourcs.UploadFromAsync(file, requst);
            }
            catch (Exception e)
            {
                return BadRequest(new { result = false, error = "上传失败" });
            }
            return Ok(new { result = true, data = formRespose });
        }
    }
}
