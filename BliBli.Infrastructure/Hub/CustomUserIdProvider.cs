using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bli.Infrastructure.Hub
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        /**
         * 获取连接的用户id，转码结果的通知
         */
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
