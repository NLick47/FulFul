using CommonInitializer;
using UserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using UserService.Infrastructure;
using UserService.Infrastructure.Options;
using UserService.Domain;
using UserService.Infrastructure.Service;
using UserService.WebAPI.Controllers.Login.Request;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureDbConfiguration();
// Add services to the container.
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "UserService.WebAPI",
    LogFilePath = "e:/temp/IdentityService.log"
});
//builder.Services.AddDbContext<IdDbContext>(opt => { string conntr = "Server=.;Database=mic;User Id=sa;Password=123456"; opt.UseSqlServer(conntr); });
//builder.ConfigureExtraServices(new InitializerOptions
//{
//    EventBusQueueName = "IdentityService.WebAPI",
//    LogFilePath = "e:/temp/IdentityService.log"
//});

var s = builder.Configuration.GetSection("SendCloudEmail");
builder.Services.Configure<EmialSmtpSettings>(s);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
    //c.AddAuthenticationHeader();
});

builder.Services.AddScoped<IValidator<AccountRequest>, AccountRequestValidator>();

//builder.Services.AddScoped<IEmailSender>();
builder.Services.AddDataProtection();
//登录、注册的项目除了要启用WebApplicationBuilderExtensions中的初始化之外，还要如下的初始化
//不要用AddIdentity，而是用AddIdentityCore
//因为用AddIdentity会导致JWT机制不起作用，AddJwtBearer中回调不会被执行，因此总是Authentication校验失败
//https://github.com/aspnet/Identity/issues/1376
IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    //不能设定RequireUniqueEmail，否则不允许邮箱为空
    //options.User.RequireUniqueEmail = true;
    //以下两行，把GenerateEmailConfirmationTokenAsync验证码缩短
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
}
);
    idBuilder = new IdentityBuilder(idBuilder.UserType, typeof(Role), builder.Services);
    idBuilder.AddEntityFrameworkStores<UserDbContext>().AddDefaultTokenProviders()
        .AddRoleValidator<RoleValidator<Role>>()
        .AddRoleManager<RoleManager<Role>>()
        .AddUserManager<IdUserManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}
//app.UseEventBus();
app.UseSession();
app.UseCors();//启用Cors
app.UseForwardedHeaders();

//app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，而且webapi项目也没必要配置这个
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();