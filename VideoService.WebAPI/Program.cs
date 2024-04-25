using Bli.Common;
using CommonInitializer;



using VideoService.Domain.Options;
using VideoService.WebAPI.BgService;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
// Add services to the container.
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "VideoService.WebAPI",
    LogFilePath = "e:/temp/IdentityService.log"
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.
Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VideoService.WebAPI", Version = "v1" });
    //c.AddAuthenticationHeader();
});
builder.Services.AddHostedService<VideoSaveBackGroundTask>();
var s = builder.Configuration.GetSection("FileServiceOptions");
builder.Services.AddHttpClient();
builder.Services.Configure<FileServiceOptions>(s);

builder.Services.Configure<MongoDbSettings>(
builder.Configuration.GetSection("MongoDbSettings"));
//��¼��ע�����Ŀ����Ҫ����WebApplicationBuilderExtensions�еĳ�ʼ��֮�⣬��Ҫ���µĳ�ʼ��
//��Ҫ��AddIdentity��������AddIdentityCore
//��Ϊ��AddIdentity�ᵼ��JWT���Ʋ������ã�AddJwtBearer�лص����ᱻִ�У��������AuthenticationУ��ʧ��
//https://github.com/aspnet/Identity/issues/1376
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}

app.UseCors();//����Cors
app.UseForwardedHeaders();

//app.UseHttpsRedirection();//������ForwardedHeaders�ܺõĹ���������webapi��ĿҲû��Ҫ�������
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();