using DanmakuService.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanmakuService.Domain
{
    public interface IDanService
    {
       Task AddDanmakuAsync(Danmaku danmaku);

        Task<List<Danmaku>> GetDanmakuAsync(string videoId,int max);
    }
}
