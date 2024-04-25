using Bli.JWT;
using DanmakuService.Domain;
using DanmakuService.Domain.Entity;
using DanmakuService.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace DanmakuService.WebApi
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class DanmakuController : Controller
    {
        private readonly IHubContext<DanmakuHub> _hubContext;

        private readonly IDanService danService;

        private readonly IValidator<DanmakuRequst> validator;

        private readonly ILogger<DanmakuController> logger;
        public DanmakuController(IHubContext<DanmakuHub> hubContext,IDanService danService, IValidator<DanmakuRequst> validator
        ,ILogger<DanmakuController> logger) {
            this._hubContext = hubContext;  
            this.danService = danService;
           this.validator = validator;
            this.logger = logger;
          
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Danlist(string id, int max = 2000)
        {
            return Ok(new { data = await danService.GetDanmakuAsync(id, max) } );
        }
        

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DanmakuRequst req)
        {
           if (!validator.Validate(req).IsValid)
            {
                return Ok(new { result = false });
            }
            long userId = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            try
            {
               var dan = new Danmaku
                    (
                        req.Text,
                        userId,
                        req.Id,
                        req.Time,
                        req.Type,
                        req.Color
                    );
                await danService.AddDanmakuAsync(dan);
                await _hubContext.Clients.Group(req.Id.ToString()).SendAsync("NewDanmaku", dan);
            }
            catch (Exception e)
            {
                logger.LogInterpolatedError($"弹幕发送",e);
                return Ok(new {result = false,mesg = "系统miss了你的弹幕" });
            }
            return Ok();
        }

       
    }
}
