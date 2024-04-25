using Bli.JWT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

namespace CommonInitializer
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
        {
            IServiceCollection services = builder.Services;
            
            IConfiguration configuration = builder.Configuration;
            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            services.RunModuleInitializers(assemblies);
            services.AddAllDbContexts(ctx =>
            {
                //连接字符串如果放到appsettings.json中，会有泄密的风险
                //如果放到UserSecrets中，每个项目都要配置，很麻烦
                //因此这里推荐放到环境变量中。
                //string connStr = configuration.GetValue<string>("DefaultDB:ConnStr");
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr"); ctx.UseSqlServer(connStr);
            }, assemblies);

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();
          
            JWTOptions jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>();
            services.Configure<JWTOptions>(configuration.GetSection("JWT"));
            builder.Services.AddJWTAuthentication(jwtOpt);
         
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });

            builder.Services.AddControllers(opt =>
            {
                // 统一设置路由前缀
                opt.UseCentralRoutePrefix(new RouteAttribute("api"));

            }).AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

            services.Configure<JsonOptions>(options =>
            {
                //设置时间格式。而非“2008-08-08T08:08:08”这样的格式
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            });
            
            services.AddCors(options =>
            {
                //更好的在Program.cs中用绑定方式读取配置的方法：https://github.com/dotnet/aspnetcore/issues/21491
                //不过比较麻烦。
                var urls = configuration.GetSection("Cors").Get<CorsSettings>().Origins;
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            }
           );
            
            string redisConnStr = configuration.GetValue<string>("Redis:ConnStr");
            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnStr);
            services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);
        }

        public static void ConfigureDbConfiguration(this WebApplicationBuilder builder)
        {
            builder.Host.ConfigureAppConfiguration((hostCtx, configBuilder) =>
            {
                //不能使用ConfigureAppConfiguration中的configBuilder去读取配置，否则就循环调用了，因此这里直接自己去读取配置文件
                // var configRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                //string connStr = configRoot.GetValue<string>("DefaultDB:ConnStr");
                string connStr = builder.Configuration.GetValue<string>("DefaultDB:ConnStr");
                configBuilder.AddDbConfiguration(() => new SqlConnection(connStr), reloadOnChange: true, reloadInterval: TimeSpan.FromSeconds(60));
            });
        }
    }
}