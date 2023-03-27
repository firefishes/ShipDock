using System;
using System.Security.Cryptography;
using System.Text;

namespace ShipDock
{
    public class RSACryptoHelper : IEncryptionHelper
    {
        /// <summary>
        /// 用RSA公钥 加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] data, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            byte[] encryptData = rsa.Encrypt(data, false);

            return encryptData;
        }

        public string Encrypt(string strText, string strPublicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(strPublicKey);
                byte[] byteText = Encoding.UTF8.GetBytes(strText);
                byte[] byteEntry = rsa.Encrypt(byteText, false);
                return Convert.ToBase64String(byteEntry);
            }

        }

        public string Decrypt(string strEntryText, string strPrivateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(strPrivateKey);
                byte[] byteEntry = Convert.FromBase64String(strEntryText);
                byte[] byteText = rsa.Decrypt(byteEntry, false);
                return Encoding.UTF8.GetString(byteText);
            }
        }
    }
}