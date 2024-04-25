using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoService.Infrastructure;
using Microsoft.Extensions;
using Microsoft.Extensions.Options;
using VideoService.Domain.Options;
using SixLabors.ImageSharp;

using VideoService.WebAPI.Videos.Request;
using System.Security.Claims;
using VideoService.Infrastructure.Request;

namespace VideoService.WebAPI.Controller
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly VideoUploadService videoUploadService;

        private readonly IOptionsSnapshot<FileServiceOptions> fileOptions;

        public FileController(VideoUploadService videoUploadService, IOptionsSnapshot<FileServiceOptions> fileOptions)
        {
            this.videoUploadService = videoUploadService;
            this.fileOptions = fileOptions;
        }

        [HttpPost]
        public async Task<IResult> UploadChunk(IFormFile file, [FromForm] VideoChunkFormRequst requst)
        {
            return await videoUploadService.UploadCoverChunkSave(file, requst);
        }
        [HttpPost]
        public async Task<IResult> FileCombine([FromForm]string formHash)
        {
            return await videoUploadService.CompleteDoSave(formHash);
        }

        [HttpPost]
        public async Task<IResult> UploadFrom(IFormFile file, [FromForm] UploadVideoFormRequst requst)
        {
            string ? exten = fileOptions.Value.ImgExtensions.SingleOrDefault(x => file.FileName.EndsWith(x));
            if (exten is null)
            {
                return Results.Json(new { result = false, error = "不支持该图片格式" });
            }
            if (file.Length > fileOptions.Value.ImagingMaxSize)
            {
                return Results.Json(new { result = false, error = "图片最大不超过" + fileOptions.Value.ImagingMaxSize });
            }
            try
            {
                using var stream = file.OpenReadStream();
                string mime = file.ContentType.ToLower();
                var imageInfo = Image.Identify(stream);
                if (imageInfo is null)
                {
                    throw new Exception("无效图片");
                }
                var bounds = imageInfo.Bounds;
                //if (!IsValidVideoSize(bounds.Width, bounds.Height))
                //{
                //    return Results.Json(new { result = false, error = "封面比例不符合规范" });
                //}
            }
            catch (Exception e)
            {
                return Results.Json(new { result = false, error = "图片无效" });
            }
            requst.UserId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            return await videoUploadService.UploadFromAsync(file, requst);
        }

        private bool IsValidVideoSize(int width, int height)
        {
            int[] validSizes = { 1920 * 1920, 1920 * 1080, 1920 * 1440 };
            return validSizes.Contains(width * height);
        }

        //public IResult GetSysExtension()
        //{
        //    //return Results.Json(new {video_extensions = fileOptions.Value.VideoExtensions,
        //    //    img_extensions = fileOptions.Value.ImgExtensions});
        //}

    }
}
