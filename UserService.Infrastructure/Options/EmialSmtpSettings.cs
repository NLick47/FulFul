using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Options
{
    public class EmialSmtpSettings
    {
        public SendCloudEmailSettings[] smtp { get; set; }
    }
}
