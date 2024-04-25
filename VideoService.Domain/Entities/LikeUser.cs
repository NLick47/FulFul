using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class LikeUser
    {
        public long id { get;private set; }

        public LikeUser(long id)
        {
            this.id = id;
        }
    }
}
