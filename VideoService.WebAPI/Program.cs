using Bli.Common;
using Bli.EventBus;
using Bli.Infrastructure.Hub;
using Bli.Infrastructure.Options;
using CommonInitializer;
using Microsoft.AspNetCore.SignalR;
using VideoService.WebAPI;


var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
// Add services to the container.
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "VideoService.WebAPI",
    LogFilePath = "D:/temp/IdentityService.log"
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VideoService.WebAPI", Version = "v1" });
    //c.AddAuthenticationHeader();
});
builder.Services.AddHttpClient();
builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection("FileServiceOptions"));
builder.Services.Configure<MongoDbSettings>(
builder.Configuration.GetSection("MongoDbSettings"));
//登录、注册的项目除了要启用WebApplicationBuilderExtensions中的初始化之外，还要如下的初始化
//不要用AddIdentity，而是用AddIdentityCore
//因为用AddIdentity会导致JWT机制不起作用，AddJwtBearer中回调不会被执行，因此总是Authentication校验失败
//https://github.com/aspnet/Identity/issues/1376

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseCors();//启用Cors
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}

app.UseForwardedHeaders();
app.UseEventBus(); 
//app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();