using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bli.Common
{
    public class KeyHlsHelper
    {

        public static (string KeyHex, string IVHex) GenerateRandomKeyAndIV()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[16]; // 128位密钥
                byte[] iv = new byte[16]; // 128位IV

                rng.GetBytes(key);
                rng.GetBytes(iv);

                string keyHex = BitConverter.ToString(key).Replace("-", string.Empty).ToLower();
                string ivHex = BitConverter.ToString(iv).Replace("-", string.Empty).ToLower();

                return (keyHex, ivHex);
            }
        }

        public static void GenerateKeyInfoFile(string keyInfoFilePath, string keyUrl, string keyHex, string ivHex)
        {
            // 生成key文件和keyinfo文件的内容
            string keyFilePath = keyInfoFilePath.Replace(".keyinfo", ".key");

            // 将16进制的key转换为字节并保存为.key文件
            byte[] keyBytes = ConvertHexStringToByteArray(keyHex);
            File.WriteAllBytes(keyFilePath, keyBytes);

            // 生成enc.keyinfo文件内容
            var keyInfoContent = $"{keyUrl}\n{keyFilePath}\n{ivHex}";
            File.WriteAllText(keyInfoFilePath, keyInfoContent);
        }

        private static byte[] ConvertHexStringToByteArray(string hexString)
        {
            int numberChars = hexString.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }


        
    }
}
