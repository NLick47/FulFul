//using Bli.EventBus;
//using UserService.Domain;
//using UserService.Infrastructure;

//namespace UserService.WebAPI.Events
//{
//    [EventName("UserService.User.Created")]
//    public class UserCreatedEventHandler : JsonIntegrationEventHandler<UserCreatedEvent>
//    {
//        private readonly ISmsSender smsSender;
//        private readonly IEmailSender emailSender;
//        public UserCreatedEventHandler(ISmsSender smsSender, IEmailSender emailSender)
//        {
//            this.smsSender = smsSender;
//            this.emailSender = emailSender;
//        }
//        public override async Task HandleJson(string eventName, UserCreatedEvent? eventData)
//        {
//            //return await emailSender.SendAsync(eventData.);

//        }
//    }
//}
