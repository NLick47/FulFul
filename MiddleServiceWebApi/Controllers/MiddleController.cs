
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
        private readonly JWTOptions optionsSnapshot;
        private readonly ITokenService tokenService;
        private readonly IConnectionMultiplexer redisConn;
        public MiddleController()
        {

        }
        [HttpGet]
        public IResult GetCode()
        {
            string code = VerificationCode.GetRodomCode();
            Claim claim = new Claim(ClaimTypes.SerialNumber, code);
            string token = tokenService.BuildToken(new Claim[] { claim}, optionsSnapshot);
            return Results.Json(new { baseimg = VerificationCode.CreateCodeForBase64(code).base64, token = token });
        }
    }
}
