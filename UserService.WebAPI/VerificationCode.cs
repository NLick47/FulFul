using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

public class VerificationCode
{
    private static string GenerateRandomText(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static readonly SKColor[] colors = new SKColor[]
{
    SKColor.Parse("#FF0000"), // 红色
    SKColor.Parse("#00FF00"), // 绿色
    SKColor.Parse("#0000FF"), // 蓝色
    
};
    public static (string base64,string code) GetCaptcha()
    {
        string randomText = GenerateRandomText(5); // 生成随机文本，这里生成 6 位随机字母和数字
        string captchaText = randomText;
        Random random = new Random();
        using (SKBitmap bitmap = new SKBitmap(200, 50))
        {
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                using (SKPaint textPaint = new SKPaint())
                {
                    textPaint.TextSize = 25;
                    textPaint.Typeface = SKTypeface.FromFile( "Fonts/BRITANIC.TTF");
                    float offsetX = 0; // 初始化偏移量


                    for (int i = 0; i < captchaText.Length; i++)
                    {
                        SKColor randomColor = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)); 
 

                        textPaint.Color = randomColor;
                        SKPoint textPosition = new SKPoint(10 + i * 25, 30); // 设置文字位置，每个字符水平间距为25
                        canvas.DrawText(captchaText[i].ToString(),textPosition, textPaint);
                    }

                    // 添加干扰线

                    for (int i = 0; i < 6; i++)
                    {
                        SKPoint startPoint = new SKPoint(random.Next(0, 200), random.Next(0, 50));
                        SKPoint endPoint = new SKPoint(random.Next(0, 200), random.Next(0, 50));

                        SKColor randomColor = colors[random.Next(colors.Length)];
                        SKPaint linePaint = new SKPaint
                        {
                            Color = randomColor,
                            StrokeWidth = 2
                        };
                        canvas.DrawLine(startPoint, endPoint, linePaint);
                    }

                    for(int i=0;i<12;i++)
                    {
                        SKColor randomColor = colors[random.Next(colors.Length)];
                        SKPoint pointPosition = new SKPoint(random.Next(0, 200), random.Next(0, 50));
                        SKPaint pointPaint = new SKPaint
                        {
                            Color = randomColor,
                            IsAntialias = true
                        };
                        canvas.DrawCircle(pointPosition, 2, pointPaint);
                    }
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);
                    byte[] imageBytes = stream.ToArray();

                    string base64Image = Convert.ToBase64String(imageBytes);
                    return  ( base64Image, randomText);
                }
            }
        }
    }

    
}
