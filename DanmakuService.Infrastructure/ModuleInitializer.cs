using Bli.Common;
using DanmakuService.Domain;
using DanmakuService.Infrastructure.Service;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanmakuService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
           services.AddScoped<IDanService, ReDanmakuService>();
           services.AddScoped<IValidator<DanmakuRequst>, DanmakuRequstValidator>();
        }
    }
}
