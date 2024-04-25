using UserService.Infrastructure;

namespace UserService.WebAPI.Events
{
 
    public record UserCreatedEvent(Guid Id, string UserName, string Password, string Account,AccountType Type);
}
