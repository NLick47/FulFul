using FluentValidation;
using Microsoft.AspNetCore.Http;
using VideoService.Domain.Entities.Enum;

namespace VideoService.WebAPI.Videos.Request
{
    public class UploadVideoFormRequst
    {
        public long? UserId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
     
        public VideoType VideoType { get; set; }

       
        public int SliceCount { get; set; }

        public string? Cover { get; set; }
       
    }
   

    public class UploadVideoFormRequstValidator : AbstractValidator<UploadVideoFormRequst>
    {
        UploadVideoFormRequstValidator()
        {
            RuleFor(x => x.Title).MaximumLength(15).MinimumLength(6).NotEmpty();
            RuleFor(x => x.Description).MaximumLength(300);
            RuleFor(x=>x.VideoType).NotEmpty();
            RuleFor(x => x.SliceCount).Must(x => x >=1 && x<=100);
          
        }
    }
}
