
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
// Add services to the container.
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "MiddleService.WebAPI",
    LogFilePath = "e:/temp/MiddleService.log"
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
    //c.AddAuthenticationHeader();
});
builder.Services.AddDataProtection();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}
app.UseCors();
app.UseForwardedHeaders();

//app.UseHttpsRedirection();//������ForwardedHeaders�ܺõĹ���������webapi��ĿҲû��Ҫ�������
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();