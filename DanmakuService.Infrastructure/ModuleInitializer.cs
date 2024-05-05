using Bli.Common;
using DanmakuService.Domain;
using DanmakuService.Infrastructure.Service;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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
           services.AddSingleton<IDanService, ReDanmakuService>();
           services.AddScoped<IValidator<DanmakuRequst>, DanmakuRequstValidator>();
        }
    }
}
