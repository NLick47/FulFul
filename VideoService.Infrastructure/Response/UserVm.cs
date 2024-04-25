using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Infrastructure.Response
{
    public class UserVm
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }

        public UserVm Default()
        {
            this.UserName = "神秘用户";
            this.Avatar = "";
            this.Id = 0;
            return this;
        }

    }
}
