using Resource.Daomain.Respose.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Daomain.Respose
{
    public class ChunkRespose
    {
        //缺少的片段
        public int MissingIndex { get; set; }

        //当前片段状态码
        public ChunkCode Code { get; set; }
    }
}
