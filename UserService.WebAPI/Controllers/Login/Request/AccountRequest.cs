using FluentValidation;

namespace UserService.WebAPI.Controllers.Login.Request
{
    public record AccountRequest(string Account, string Password, string Code);
    public class AccountRequestValidator : AbstractValidator<AccountRequest>
    {
        public AccountRequestValidator()
        {
            RuleFor(e => e.Account).NotNull().NotEmpty().MaximumLength(25).MinimumLength(2);
            RuleFor(e => e.Password).NotNull().NotEmpty().MaximumLength(25).MinimumLength(6);
            RuleFor(e => e.Code).NotNull().NotEmpty().Length(4);        
        }
    }
}
