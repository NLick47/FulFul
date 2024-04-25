using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Net.Http;

namespace Bli.Common
{
    public static class VerificationCode
    {
        public static string GetRodomCode()
        {
            char code;
            string checkCode = string.Empty;
            Random rd = new Random();

            for (int i = 0; i < 4; i++)
            {
                int num = rd.Next();
                int _temp;

                if (num % 2 == 0)
                {
                    _temp = ('0' + (char)(num % 10));
                    if (_temp == 48 || _temp == 49)
                    {
                        _temp += rd.Next(2, 9);
                    }
                }
                else
                {
                    _temp = ('A' + (char)(num % 10));
                    if (rd.Next(0, 2) == 0)
                    {
                        _temp = (char)(_temp + 32);
                    }
                    if (_temp == 66 || _temp == 73 || _temp == 79 || _temp == 108 || _temp == 111)
                    {
                        _temp++;
                    }
                }
                code = (char)_temp;
                checkCode += code;
            }
            return checkCode;
        }
    
        public static (string code,string base64) CreateCodeForBase64(string code)
        {
            int codeWeight = 80;
            int codeHeight = 22;
            int fontSize = 16;
            Random rd = new Random();
            string checkCode = code; 
            Bitmap image = new Bitmap(codeWeight, codeHeight); //构建画图
            Graphics g = Graphics.FromImage(image); //构建画布
            g.Clear(Color.White); //清空背景色
            Color[] color = new Color[] { Color.Red, Color.Black, Color.Green, Color.Blue };
            string[] font = new string[] { "宋体", "黑体", "楷体" };

            //画噪音线
            for (int i = 0; i < 6; i++)
            {
                int x1 = rd.Next(image.Width);
                int x2 = rd.Next(image.Width);
                int y1 = rd.Next(image.Height);
                int y2 = rd.Next(image.Height);
                g.DrawLine(new Pen(color[rd.Next(color.Length)]), new Point(x1, y1), new Point(x2, y2));
            }

            //画验证码
            for (int i = 0; i < checkCode.Length; i++)
            {
                Color clr = color[rd.Next(color.Length)];
                Font ft = new Font(font[rd.Next(font.Length)], fontSize);
                g.DrawString(checkCode[i].ToString(), ft, new SolidBrush(clr), (float)i * 18 + 2, 0);
            }

            //画噪音点
            for (int i = 0; i < 200; i++)
            {
                int x = rd.Next(image.Width);
                int y = rd.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(rd.Next()));
            }

            //画边框线
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            MemoryStream ms = new MemoryStream();
            string? base64 = default;
            try
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                //context.Session[ConstantValues.VerCodeSessionName] = checkCode; //将验证码保存到Session中
                //context.Response.ContentType = "Image/Gif";
                //context.Response.ClearContent();
                //context.Response.BinaryWrite(ms.ToArray());
                base64 = Convert.ToBase64String(ms.ToArray());
            }
            finally
            {
                image.Dispose();
                g.Dispose();
            }
            return (checkCode,base64);
        }

       
    }
}