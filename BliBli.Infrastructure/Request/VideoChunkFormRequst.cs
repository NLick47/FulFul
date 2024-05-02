using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bli.Infrastructure.Request
{
    public class VideoChunkFormRequst
    {
        public int Index { get; set; }
        //所属表单hash
        public string FormHash { get; set; }
    }
}
