

using Bli.Common;
using CommonInitializer;


namespace DanmakuService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.ConfigureDbConfiguration();
            builder.ConfigureExtraServices(new InitializerOptions
            {
                EventBusQueueName = "VideoService.WebAPI",
                LogFilePath = "e:/temp/IdentityService.log"
            }); 
            // Add services to the container.
            builder.Services.AddAuthorization();
           var s= builder.Configuration.GetSection("MongoDbSettings");
            builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection("MongoDbSettings"));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            var app = builder.Build();
            app.UseRouting();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();
       
            app.MapHub<DanmakuHub>("/danmakuHub");
            app.MapControllers();
            app.Run();
        }
    }
}