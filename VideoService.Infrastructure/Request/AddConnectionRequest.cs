using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.WebAPI.Videos.Request;

namespace VideoService.Infrastructure.Request
{
    public record AddConnectionRequest(int videoId, string name);

    public class AddConnectionRequstValidator : AbstractValidator<AddConnectionRequest>
    {
        public AddConnectionRequstValidator()
        {
            RuleFor(x => x.name).NotEmpty().MaximumLength(8);
        }
    }
}
