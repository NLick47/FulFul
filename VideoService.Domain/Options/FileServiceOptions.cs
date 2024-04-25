using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Options
{
    public class FileServiceOptions
    {
        public string RootPath { get; set; }

        //视频可用扩展名
        public string[] VideoExtensions { get; set; }

        //图片可用扩展名
        public string[] ImgExtensions { get; set; }

        //视频的最大byte
        public int VideoMaxSize { get; set; }

        //图片的最大byte
        public int ImagingMaxSize { get; set; }
    }
}
