using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class Collection
    {
      
        public long Id { get; set; }
        public List<CollectionItem> Item { get;private set; }

        public string Name { get;private set; }

        public DateTime CreateTime { get; private set; }

        public long CreateUserId { get; private set; }

        public bool IsVisible { get; private set; }

        public Collection(string Name,long CreateUserId)
        {
            this.Name = Name;
            this.CreateTime = DateTime.Now;
            this.CreateUserId = CreateUserId;
            this.IsVisible= true;
        }

        public Collection SetName(string newName)
        {
            this.Name = newName;
            return this;
        }

        public Collection SetVisible(bool value)
        {
            this.IsVisible  = value;
            return this;
        }
        public Collection AddItem(CollectionItem item)
        {
            this.Item.Add(item);    
            return this;
        }
    }
}
