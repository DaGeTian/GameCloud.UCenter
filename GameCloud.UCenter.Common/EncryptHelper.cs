using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GameCloud.UCenter.Common.Extensions;

namespace GameCloud.UCenter.Common
{
    /// <summary>
    /// This class is responsible for encryption and decryption.
    /// </summary>
    public class EncryptHelper
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES encrypt method.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string str, string key)
        {
            key = key.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] rgbIv = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, dcsp.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
            cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// DES decrypt method.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string str, string key)
        {
            try
            {
                key = key.PadRight(8, ' ');
                byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] rgbIv = Keys;
                byte[] inputByteArray = Convert.FromBase64String(str);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();

                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// Generate SHA256 hashed value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SHA256(string str)
        {
            byte[] data = (new UnicodeEncoding()).GetBytes(str);
            byte[] result = (new SHA256Managed()).ComputeHash(data);
            string hashedValue = Convert.ToBase64String(result);

            return hashedValue;
        }

        /// <summary>
        /// This is the MD5 method.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            bytes = new MD5CryptoServiceProvider().ComputeHash(bytes);

            string retValue = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                retValue += bytes[i].ToString("x").PadLeft(2, '0');
            }

            return retValue;
        }

        public static string ComputeHash(string str)
        {
            return SHA256(str);
        }

        public static bool VerifyHash(string str, string hash )
        {
            return string.Equals(SHA256(str), hash);
        }
    }
}
