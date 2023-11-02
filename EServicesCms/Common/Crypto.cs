using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace EServicesCms.Common
{
    class Crypto
    {
        static string sKey = "MVISIONey@M0BILEApp@MOAR";
        public static string Decrypt(string encryptedMsg)
        {
            string msg = string.Empty;
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Padding = PaddingMode.Zeros;
            des.Mode = CipherMode.CBC;
            des.IV = Encoding.ASCII.GetBytes("MOAR@MOF");
            des.Key = Encoding.ASCII.GetBytes(sKey);
            // Create or open the specified file. 

            byte[] Data = HexToBytes(encryptedMsg);
            // Create a new MemoryStream using the passed  
            // array of encrypted data.
            MemoryStream msDecrypt = new MemoryStream(Data);

            // Create a CryptoStream using the MemoryStream  
            // and the passed key and initialization vector (IV).
            CryptoStream csDecrypt = new CryptoStream(
                                                        msDecrypt,
                                                        des.CreateDecryptor(des.Key, des.IV),
                                                        CryptoStreamMode.Read
                                                     );

            // Create buffer to hold the decrypted data. 
            byte[] fromEncrypt = new byte[Data.Length];

            // Read the decrypted data out of the crypto stream 
            // and place it into the temporary buffer.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            //Convert the buffer into a string and return it. 
            msg = new UTF8Encoding().GetString(fromEncrypt);

            return msg;
        }
        public static string Encrypt(string msg)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Padding = PaddingMode.Zeros;
            des.Mode = CipherMode.CBC;
            des.IV = Encoding.ASCII.GetBytes("MOAR@MOF");
            des.Key = Encoding.ASCII.GetBytes(sKey);
            byte[] toEncrypt = new UTF8Encoding().GetBytes(msg);
            String encMsg = string.Empty;
            // Create or open the specified file. 
            // Create a MemoryStream.
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(
                                                    mStream,
                                                    des.CreateEncryptor(des.Key, des.IV),
                                                    CryptoStreamMode.Write
                                                    );

            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            // Get an array of bytes from the  
            // MemoryStream that holds the  
            // encrypted data. 
            byte[] ret = mStream.ToArray();

            // Close the streams.
            cStream.Close();
            mStream.Close();
            encMsg = ToHex(ret);

            return encMsg;
        }
        public static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }
        public static byte[] HexToBytes(string str)
        {
            if (str.Length == 0 || str.Length % 2 != 0)
                throw new Exception("String length must be even");

            byte[] buffer = new byte[str.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte 
                c = str[sx];
                buffer[bx] = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte 
                c = str[++sx];
                buffer[bx] |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
            }

            return buffer;
        }
    }
}