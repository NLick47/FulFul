using Bli.Common;
using Microsoft.Extensions.DependencyInjection;
using Resource.Daomain;

namespace Resource.Infrastructure
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IResourceUpload, ResourcsService>();
        }
    }
}
