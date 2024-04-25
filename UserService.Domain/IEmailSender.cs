﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Domain
{
    public interface IEmailSender
    {
        public Task SendAsync(string toEmail, string subject, string body);
    }
}
