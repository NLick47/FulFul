using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Infrastructure.Response
{
    public class CommentRepose
    {
        public UserVm user { get; set; }
        public DateTime CreateTime { get;  set; }

        public string Content { get;  set; }

        public int Id { get;  set; }

        public List<Reply> replies { get;  set; } = new List<Reply>();
        public class Reply
        {
            public DateTime CreateTime { get;  set; }
            public string Content { get;  set; }
            public UserVm user { get; set; }
            public int Id { get;  set; }
        }
    }

    

}
