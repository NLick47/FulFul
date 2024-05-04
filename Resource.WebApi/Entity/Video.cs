using Bli.Infrastructure.Enum;

namespace Resource.WebApi.Entity
{
    public class Video
    {
        public Video(string title, string description, long createUserId, string coverUri, VideoType videoType,
           double videoSecond, List<Resouce> resouces)
        {
            this.Title = title;
            this.Description = description;
            this.CreateTime = DateTime.Now;
            this.CreateUserId = createUserId;
            this.CoverUri = coverUri.Replace("\\", "/");
            this.VideoType = videoType;
            this.VideoSecond = videoSecond;
            this.Resouces = resouces;
        }

        public string Title { get; set; }

        public DateTime CreateTime { get; set; }

        public long CreateUserId { get; set; }
        public string CoverUri { get; set; }
        public List<Resouce> Resouces { get; set; }

        public string Description { get; set; }

        

        public VideoType VideoType { get; set; }

        public double VideoSecond { get; set; }

        public class Resouce
        {
            public Resouce(string playerPath, string rawPath, string key, VideoSize videoSize)
            {
                Id = Guid.NewGuid().ToString().Replace("-", string.Empty);
                this.PlayerPath = playerPath.Replace("\\", "/");
                this.RawPath = rawPath;
                this.Key = key;
                this.VideoSize = videoSize;
            }
            public string Id { get; set; }
            public Video Video { get; set; }
            public int VideoId { get; set; }

          
            public string PlayerPath { get; private set; }

          
            public string RawPath { get; private set; }

            public Resouce SetRawPath(string path)
            {
                this.RawPath = path;
                return this;
            }

            public VideoSize VideoSize { get; private set; }

     
            public string? Key { get; private set; }
        }
    }
}
