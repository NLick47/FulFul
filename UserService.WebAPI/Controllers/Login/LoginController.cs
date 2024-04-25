using Bli.Common;
using IdentityService.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UserService.Domain;
using UserService.Domain.Entities;
using UserService.Infrastructure;
using UserService.WebAPI.Controllers.Login.Request;
using UserService.WebAPI.Events;

namespace UserService.WebAPI.Controllers.Login
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IIdRepository repository;
        private readonly IdDomainService idService;
        private readonly IConfiguration configuration;
      

        public LoginController(IIdRepository repository, IdDomainService idService, IConfiguration configuration)
        {
            this.idService = idService;
            this.repository = repository;
            this.configuration = configuration;

        }
        /// <summary>
        /// 弃用
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        /// 
        /**[AllowAnonymous]
        [NonAction]
        public async Task<ActionResult<string?>> Login(LoginAccountRequest req)
        {
            string? sess_code = HttpContext.Session.GetString("code");
            if(sess_code is null || sess_code != req.Code)
            {
               return StatusCode((int)HttpStatusCode.NotExtended, VerifyCode.Error_Code);
            }
            HttpContext.Session.Remove("code");
            bool isEmial = Validator.IsValidEmail(req.Account), isPhone = Validator.IsValidPhoneNumber(req.Account);
            if(isEmial | isPhone)
            {
               User? name = await repository.FindByNameAsync(req.UserName);
               if(name is not null)
                {
                    return StatusCode((int)HttpStatusCode.NotExtended, VerifyCode.Login.UserName_Exist);
                }
        
                if (isEmial)
                {
                    User? user =  await repository.FindByEmialAsync(req.Account);
                    if(user == null)
                    {
                        UserCreatedEvent @event = new UserCreatedEvent(Guid.NewGuid(),req.UserName,req.Password,req.Account,AccountType.Email);
                        return StatusCode((int)HttpStatusCode.OK,VerifyCode.Login.Auto_RegisterByEmial);
                        eventBus.Publish("UserService.User.Created", @event);
                    }

                  
                }
                if (isPhone)
                {
                    User? user = await repository.FindByPhoneNumberAsync(req.Account);
                    if(user == null)
                    {
                        UserCreatedEvent @event = new UserCreatedEvent(Guid.NewGuid(), req.UserName, req.Password, req.Account, AccountType.Phone);
                        return StatusCode((int)HttpStatusCode.OK, VerifyCode.Login.Auto_RegisterByPhone);
                        eventBus.Publish("UserService.User.Created", @event);
                    }

                    (var result, var token) = await idService.LoginByPhoneAndPwdAsync(req.Account, req.Password);
                    if(result.Succeeded)
                    {
                       return token;
                    }else if (result.IsLockedOut)
                    {
                        return StatusCode((int)HttpStatusCode.Locked, VerifyCode.Login.Account_Locked);
                    }
                } 
            }
            return StatusCode((int)HttpStatusCode.NotExtended, VerifyCode.Login.Login_Fail);
        }
         * 
         * 
         * 
         */

    }
}
