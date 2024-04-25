using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Infrastructure.Request
{
   public record AddCommentRequest(int videoId,string text);

    public class AddCommentRequestValidator : AbstractValidator<AddCommentRequest>
    {
      public  AddCommentRequestValidator()
        {
            RuleFor(x => x.text).NotEmpty().MaximumLength(280);
           
        }
    }
}
