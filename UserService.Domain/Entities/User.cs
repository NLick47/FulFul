using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;

namespace UserService.Domain.Entities
{
    public class User : IdentityUser<long>
    {
        public DateTime CreationTime { get; init; }

        public DateTime? DeletionTime { get; private set; }

        public bool IsDeleted { get; private set; }

        //经验值
        public int Exp { get; private set; }

        //头像地址
        public string? Avatar { get; private set; }

        //硬币
        public int Coin { get; private set; }

        public User ChangeAvatar(string avatar)
        {
            this.Avatar = avatar;
            return this;
        }

        public User AugmentExp(int exp)
        {
            if (exp > 0) Exp += exp;
            return this;
        }

        public User AugmentCoin()
        {
            Coin += 1;
            return this;
        }

        public User(string userName) : base(userName)
        {
           
            CreationTime = DateTime.Now;
        }

        public void SoftDelete()
        {
            this.IsDeleted = true;
            this.DeletionTime = DateTime.Now;
        }
    }
}
