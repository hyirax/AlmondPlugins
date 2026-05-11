using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace AlmondHousing
{
    public static class ChaChaCryptoHelper
    {
        // 🔑 这是为你随机生成的 32 字节 (256-bit) 终极密钥！绝密，不要告诉任何人！
        private static readonly byte[] SecretKey = Encoding.UTF8.GetBytes("aJ9#vL2!xQ8$mP5*kR4@zT7^yN1&bC3+"); 
        
        // 🛡️ 识别头：用来区分你的加密图纸和普通的 MakePlace 图纸
        private static readonly string MagicHeader = "ALMOND_CHACHA:";

        /// <summary>
        /// 加密并压缩导出数据
        /// </summary>
        public static string Encrypt(string plainText)
        {
            // 1. 先进行 GZip 压缩，把可读的 JSON 压成二进制乱码，缩小体积
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] compressedBytes;
            using (var msi = new MemoryStream(plainBytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                compressedBytes = mso.ToArray();
            }

            // 2. ChaCha20Poly1305 加密
            using var chaCha = new ChaCha20Poly1305(SecretKey);
            
            byte[] nonce = new byte[12]; // 12字节动态随机数 (每次加密都不一样)
            RandomNumberGenerator.Fill(nonce);
            
            byte[] tag = new byte[16];   // 16字节防篡改签名
            byte[] cipherText = new byte[compressedBytes.Length];

            // 执行加密
            chaCha.Encrypt(nonce, compressedBytes, cipherText, tag);

            // 3. 组装数据: 随机数(12) + 防篡改签名(16) + 加密密文
            byte[] finalResult = new byte[nonce.Length + tag.Length + cipherText.Length];
            Buffer.BlockCopy(nonce, 0, finalResult, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, finalResult, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipherText, 0, finalResult, nonce.Length + tag.Length, cipherText.Length);

            // 4. 加上专属头部并转成 Base64 文本形式
            return MagicHeader + Convert.ToBase64String(finalResult);
        }

        /// <summary>
        /// 自动识别并解密数据
        /// </summary>
        public static string Decrypt(string input)
        {
            // 💡 【核心兼容逻辑】：如果开头不是你的识别头，说明这是别人普通的 MakePlace 图纸！
            // 直接原样返回，不影响插件读取网上的普通图纸。
            if (!input.StartsWith(MagicHeader)) 
            {
                return input;
            }

            try
            {
                // 去掉头部，提取真实的加密数据
                string base64Data = input.Substring(MagicHeader.Length);
                byte[] fullData = Convert.FromBase64String(base64Data);

                // 拆解数据
                byte[] nonce = new byte[12];
                byte[] tag = new byte[16];
                byte[] cipherText = new byte[fullData.Length - 12 - 16];

                Buffer.BlockCopy(fullData, 0, nonce, 0, 12);
                Buffer.BlockCopy(fullData, 12, tag, 0, 16);
                Buffer.BlockCopy(fullData, 12 + 16, cipherText, 0, cipherText.Length);

                // 解密 (如果倒卖狗修改了文件，这一步会直接抛出异常！)
                using var chaCha = new ChaCha20Poly1305(SecretKey);
                byte[] decompressedBuffer = new byte[cipherText.Length];
                chaCha.Decrypt(nonce, cipherText, tag, decompressedBuffer);

                // GZip 解压回 JSON 文本
                using (var msi = new MemoryStream(decompressedBuffer))
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }
                    return Encoding.UTF8.GetString(mso.ToArray());
                }
            }
            catch (Exception)
            {
                // 解密失败或防篡改被触发
                return "ALMOND_DECRYPT_ERROR"; 
            }
        }
    }
}