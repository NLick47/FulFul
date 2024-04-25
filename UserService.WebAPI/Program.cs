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
//��¼��ע�����Ŀ����Ҫ����WebApplicationBuilderExtensions�еĳ�ʼ��֮�⣬��Ҫ���µĳ�ʼ��
//��Ҫ��AddIdentity��������AddIdentityCore
//��Ϊ��AddIdentity�ᵼ��JWT���Ʋ������ã�AddJwtBearer�лص����ᱻִ�У��������AuthenticationУ��ʧ��
//https://github.com/aspnet/Identity/issues/1376
IdentityBuilder idBuilder = builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    //�����趨RequireUniqueEmail��������������Ϊ��
    //options.User.RequireUniqueEmail = true;
    //�������У���GenerateEmailConfirmationTokenAsync��֤������
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
app.UseCors();//����Cors
app.UseForwardedHeaders();

//app.UseHttpsRedirection();//������ForwardedHeaders�ܺõĹ���������webapi��ĿҲû��Ҫ�������
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();