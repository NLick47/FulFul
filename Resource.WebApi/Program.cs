
using Bli.Common;
using Bli.Infrastructure.Options;
using Bli.JWT;
using CommonInitializer;
using Resource.WebApi;
using Resource.WebApi.BgService;
using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "Resource.WebAPI",
    LogFilePath = "D:/temp/Resource.WebAPI.log"
});
builder.Services.AddHttpClient();
builder.Services.AddHostedService<VideoSaveBackGroundTask>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VideoService.WebAPI", Version = "v1" });
    //c.AddAuthenticationHeader();
});
builder.Services.AddSignalR();
builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection("FileServiceOptions"));

var app = builder.Build();
if (builder.Environment.IsDevelopment())
{
    app.UseCors();//ÆôÓÃCors
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resource.WebAPI.WebAPI v1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<CombineNotificationHub>("/hubs/combinenNtificationHub");
app.MapControllers();
app.Run();