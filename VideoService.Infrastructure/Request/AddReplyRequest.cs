using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Infrastructure.Request
{
    public record AddReplyRequest(int commId,string text);

    public class AddReplyRequestValidator : AbstractValidator<AddReplyRequest>
    {
        public AddReplyRequestValidator() {
            RuleFor(x => x.text).NotEmpty().MaximumLength(300);
        }   
    }


}
