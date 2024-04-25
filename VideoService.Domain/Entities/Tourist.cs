using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class TouristLike
    {
        public int Id { get; set; }
       
        public int Ip { get; set; }

       public int videoId { get; set; }

    }
}
