using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Service
{
    class SendCloudResponseModel
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
