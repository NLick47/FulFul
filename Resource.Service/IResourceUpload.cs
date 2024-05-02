using Bli.Infrastructure.Request;
using Microsoft.AspNetCore.Http;
using Resource.Daomain.Respose;


namespace Resource.Daomain
{
    public interface IResourceUpload
    {
       Task<FormRespose>  UploadFromAsync(IFormFile file, UploadVideoFormRequst res);

       Task<ChunkRespose> UploadCoverChunkSave(IFormFile file, VideoChunkFormRequst res);
    }
}
