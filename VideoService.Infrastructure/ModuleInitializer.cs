using Bli.Common;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VideoService.Domain;
using VideoService.Infrastructure.Request;

namespace VideoService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IVideoCommentService, VideoCommentService>();
            services.AddScoped<VideoUploadService>();   
            services.AddScoped<VideoCommentRepository>();
            services.AddScoped<CommentRepository>();
            services.AddScoped<IValidator<AddCommentRequest>, AddCommentRequestValidator>();
            services.AddScoped<IValidator<AddConnectionRequest>, AddConnectionRequstValidator>();
            services.AddScoped<IValidator<AddReplyRequest>, AddReplyRequestValidator>();
            
        }
    }
}
