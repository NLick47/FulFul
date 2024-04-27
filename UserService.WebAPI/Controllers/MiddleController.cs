
using Bli.Common;
using Bli.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Net;
using System.Security.Claims;

namespace MiddleServiceWebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MiddleController : ControllerBase
    {
       
        private readonly IConnectionMultiplexer redisConn;
        private readonly IHttpContextAccessor accessor;
        public MiddleController(IConnectionMultiplexer connection, IHttpContextAccessor accessor)
        {
            this.redisConn = connection;    
            this.accessor = accessor;
        }
        [HttpGet]
        public async Task<IResult> GetCode()
        {
            string? ip  = accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (ip is not null)
            {
                var db = redisConn.GetDatabase();
                (var img,var code) = VerificationCode.GetCaptcha();
                var b = await db.StringSetAsync(ip, code,TimeSpan.FromSeconds(120));
                return Results.Json(new { baseimg = img });
            }
            
            return Results.Json(new { error = "哦哦什么原因呢?" });

            //Claim claim = new Claim(ClaimTypes.SerialNumber, code);
            //string token = tokenService.BuildToken(new Claim[] { claim }, optionsSnapshot);
            //return Results.Json(new { baseimg = VerificationCode.CreateCodeForBase64(code).base64, token = token });
        }
    }
}
