namespace Resource.WebApi.Entity
{
    public class RemoveFail
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public string EventName { get; set; }

        public string ErrorMessage { get; set; }

        public RemoveFail(int VideoId,string EventName, string ErrorMessage)
        {
            this.VideoId = VideoId;
            this.CreateTime = DateTime.Now;
            this.EventName = EventName;
            this.ErrorMessage = ErrorMessage;
        }
        public DateTime CreateTime { get; set; }
    }
}
