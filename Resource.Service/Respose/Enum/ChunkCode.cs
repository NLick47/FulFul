using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resource.Daomain.Respose.Enum
{
    public enum ChunkCode
    {
        //缺少片段
        MissChunk = 301,
        //片段创建错误
        CreateError = 405,
        Succeed = 200,
        //表单不存在，可能是键过期了或者被直接调用了该接口
        FormExist = 404
    }
}
