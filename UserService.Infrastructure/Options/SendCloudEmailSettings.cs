﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Options
{
    public class SendCloudEmailSettings
    {
        public string ApiUser { get; set; }
        public string ApiKey { get; set; }
        public string From { get; set; }
    }
}
