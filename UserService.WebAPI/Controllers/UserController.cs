using Microsoft.AspNetCore.Mvc;
using IdentityService.Domain;
using UserService.Domain;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Bli.Common;
using Microsoft.AspNetCore.Http;
using UserService.WebAPI.Controllers.Login.Request;
using UserService.Domain.Entities;
using Microsoft.Extensions.Options;
using CommonInitializer;
using System.Security.Claims;
using FluentValidation;
using System;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserService.Infrastructure.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserService.WebAPI.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IIdRepository repository;
        private readonly IdDomainService idService;
        private IValidator<AccountRequest> _validator;

        private readonly IHttpContextAccessor accessor;
        private readonly IConnectionMultiplexer redisConn;

        private readonly IConfiguration configuration;
        public UserController(IIdRepository repository, IdDomainService idService, 
             IValidator<AccountRequest> _validator, IConnectionMultiplexer redisConn, IHttpContextAccessor accessor,IConfiguration configuration)
        {
            this.idService = idService;
            this.repository = repository;
            this._validator = _validator;
            this.redisConn = redisConn;
            this.accessor = accessor;
            this.configuration = configuration;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<UserVm>> GetUsersByIds(string ints)
        {
            ints = ints.Substring(1,ints.Length - 2);
      
            List<long> ids = ints.Split(',').Select(x => long.Parse(x)).ToList();
            var us = await repository.FindUsersById(ids);
            return us.Select(x => new UserVm() { Id = x.Id, UserName = x.UserName, Avatar = x.Avatar }).ToList();
           
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<UserVm> GetById(long id)
        {
           var user = await repository.FindByIdAsync(id);
           return new UserVm() {Id = user.Id,Avatar=user.Avatar,UserName=user.UserName };
        }
        [HttpGet]
        public async Task<IResult> GetInfo()
        {
            var db = redisConn.GetDatabase();
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            UserVm userVm ;
            string? value = null;
            try
            {
                 value = await db.StringGetAsync(id.ToString());
                if(value is not null)
                {
  
                    userVm = JsonConvert.DeserializeObject<UserVm>(value);
                }else
                {
                    throw new RedisServerException("Key is null");
                }
                 
            }
            catch (RedisServerException e)
            {
               User user = await repository.FindByIdAsync(long.Parse(id));
               await db.StringSetAsync(id, JsonConvert.SerializeObject(user), TimeSpan.FromHours(1));
                userVm = new UserVm()
                {
                    Avatar = user.Avatar,
                    Id = user.Id,
                    UserName = user.UserName
                };
            }
            return Results.Json(new {result = true,data = userVm });
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> Login([FromBody] AccountRequest request)
        {
            var vres = await _validator.ValidateAsync(request);
            if (!vres.IsValid)
            {
                return Results.Json(new { error = VerifyCode.Account_Validator, result = false });
            }
            string? ip = accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if(ip is null)
            {
                return Results.Json(new { error = VerifyCode.Login_Fail, result = false });
            }
            var db = redisConn.GetDatabase();
            string? code = await db.StringGetAsync(ip);
            await db.KeyDeleteAsync(ip);
            if (code is null || !string.Equals(code,request.Code, StringComparison.OrdinalIgnoreCase))
            {
               return Results.Json(new { error = VerifyCode.Error_Code,result = false});
            }
            (var res,var token) = await idService.LoginByUserNameAndPwdAsync(request.Account,request.Password);
            if (res.IsLockedOut)
            {
               return Results.Json(new { error = VerifyCode.Account_Locked, result = false });
            }else if(res.Succeeded)
            {
               return Results.Json(new { error = VerifyCode.Account_Locked, result = true,token = token });
            }
             return Results.Json(new { error = VerifyCode.Login_Fail, result = false });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IResult> Create([FromBody] AccountRequest request)
        {
            var res = await _validator.ValidateAsync(request);
            if(!res.IsValid)
            {
                return Results.Json(new { error = VerifyCode.Account_Validator, result = false });
            }
            string? ip = accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (ip is null)
            {
                return Results.Json(new { error = VerifyCode.CreateFail, result = false });
            }
            var db = redisConn.GetDatabase();
            string? code = await db.StringGetAsync(ip);
            await db.KeyDeleteAsync(ip);
            if (code is null || !string.Equals(code,request.Code,StringComparison.OrdinalIgnoreCase))
            {
               return Results.Json(new { error = VerifyCode.Error_Code, result = false });
            }
            User? user =  await repository.FindByNameAsync(request.Account);
            if(user is not null)
            {
                return Results.Json(new { error = VerifyCode.UserName_Exist, result = false });
            }
            var idres = await repository.CreateAsync(new User(request.Account),request.Password);
            if(idres.Succeeded)
            {
                return Results.Json(new { error = VerifyCode.CreateSucceeded, result = true });
            }else
            {
                return Results.Json(new { error = VerifyCode.CreateFail, result = false });
            }
        }
    }
}
