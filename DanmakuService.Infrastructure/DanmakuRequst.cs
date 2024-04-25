using DanmakuService.Domain.Entity.Enum;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanmakuService.Infrastructure
{
    public class DanmakuRequst
    {
        public string Text { get; set; }

        public float Time { get; set; }

        public DanPostionType Type { get; set; }

        public int Color { get; set; }

        public string Author { get; set; }

        public int Id { get; set; }

        
    }
    public class DanmakuRequstValidator : AbstractValidator<DanmakuRequst>
    {
        public DanmakuRequstValidator()
        {
            RuleFor(x => x.Text).NotEmpty().MaximumLength(15);
        }
    }

}
