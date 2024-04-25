using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bli.Common
{
    public interface IModuleInitializer
    {
        public void Initialize(IServiceCollection services);
    }
}
