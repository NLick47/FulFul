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
//��¼��ע�����Ŀ����Ҫ����WebApplicationBuilderExtensions�еĳ�ʼ��֮�⣬��Ҫ���µĳ�ʼ��
//��Ҫ��AddIdentity��������AddIdentityCore
//��Ϊ��AddIdentity�ᵼ��JWT���Ʋ������ã�AddJwtBearer�лص����ᱻִ�У��������AuthenticationУ��ʧ��
//https://github.com/aspnet/Identity/issues/1376

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseCors();//����Cors
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}

app.UseForwardedHeaders();
app.UseEventBus(); 
//app.UseHttpsRedirection();//������ForwardedHeaders�ܺõĹ���������webapi��ĿҲû��Ҫ�������
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();