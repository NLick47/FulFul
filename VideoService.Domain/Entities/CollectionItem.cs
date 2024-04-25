using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class CollectionItem
    {
       
        public int Id { get;private set; }

        public CollectionItem(int videoId)
        {
            VideoId = videoId;
        }
        public Video? Video { get;private set; }

        public Collection? Collection { get;private set; }
        public long? CollectionId { get;private set; }

        public int VideoId { get; private set; }
    }
}
