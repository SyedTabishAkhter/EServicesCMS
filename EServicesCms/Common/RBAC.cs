using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace EServicesCms.Common
{
    public class RBAC
    {
        public RBAC()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        internal class AIB
        {
            public void AIBEventLog(string strMessage, int nMsgType)
            {
                string logName = "Application";
                if (!EventLog.SourceExists("PPSACL"))
                    EventLog.CreateEventSource("PPSACL", logName);
                else
                    logName = EventLog.LogNameFromSourceName("PPSACL", ".");
                EventLog eventLog = new EventLog();
                eventLog.Source = "PPSACL";
                eventLog.Log = logName;
                EventLogEntryType type = nMsgType != 0 ? (nMsgType != 2 ? EventLogEntryType.Information : EventLogEntryType.Warning) : EventLogEntryType.Error;
                eventLog.WriteEntry(strMessage, type);
            }

            public double fnDateDifferenceWithCurrentDate(string strDateToCompare)
            {
                string[] strArray = strDateToCompare.Split(',');
                if (strArray.Length < 1 || strArray.Length != 7)
                {
                    double num;
                    return num = -1.0;
                }
                return (DateTime.Now - new DateTime(Convert.ToInt32(strArray[0]), Convert.ToInt32(strArray[1]), Convert.ToInt32(strArray[2]), Convert.ToInt32(strArray[3]), Convert.ToInt32(strArray[4]), Convert.ToInt32(strArray[5]), Convert.ToInt32(strArray[6]))).TotalDays;
            }

            public bool CheckSQLElementsInString(string source)
            {
                string Key = "DB_CheckForSQLElementInSQL";
                string[] tokens = new string[33]
          {
            "INSERT INTO",
            "INSERT TOP",
            "UPDATE",
            "UPDATE TOP",
            "DELETE FROM",
            "DELETE TOP",
            "CREATE TABLE",
            "CREATE DATABASE",
            "CREATE STORED PROCEDURE",
            "CREATE FUNCTION",
            "CREATE SCHEMA",
            "CREATE TRIGGERS",
            "CREATE VIEW",
            "ALTER TABLE",
            "ALTER INDEX",
            "ALTER TRIGGER",
            "ALTER VIEW",
            "ALTER DATABASE",
            "ALTER USER",
            "ALTER LOGIN",
            "ALTER SCHEMA",
            "ALTER ROUTE",
            "DROP DATABASE",
            "DROP TABLE",
            "TRUNCATE TABLE",            
            "DROP INDEX",
            "DROP PROCEDURE",
            "CREATE PROCEDURE",
            "CREATE FUNCTION",
            "ALTER FUNCTION",
            "DROP FUNCTION",
            "DROP USER",
            "CREATE USER"
          };
                string str = this.GetConfigurationValue(Key, false) ?? "";
                if (str.Length <= 0)
                    str = "1";
                if (str == "0")
                    return false;
                return this.CheckTokens(source, tokens);
            }

            public bool CheckTokens(string source, string[] tokens)
            {
                if (tokens.Length <= 0)
                    return false;
                source = source.ToLower();
                foreach (string str in tokens)
                {
                    if (source.Contains(str.ToLower()))
                        return true;
                }
                return false;
            }
            public string AIBAppend(string strOriginal, string strToAppend)
            {
                StringBuilder stringBuilder = new StringBuilder(strOriginal);
                stringBuilder.Append(strToAppend);
                return stringBuilder.ToString();
            }
            public string AIBDecrypt(string strToDecrypt, string strPassPhrase, string strSaltValue, string strHashAlgorithm, int nPasswordIterations, string strInitVector, int nKeySize)
            {
                string str = "";
                MemoryStream memoryStream = (MemoryStream)null;
                CryptoStream cryptoStream = (CryptoStream)null;
                try
                {
                    byte[] bytes1 = Encoding.ASCII.GetBytes(strInitVector);
                    byte[] bytes2 = Encoding.ASCII.GetBytes(strSaltValue);
                    byte[] buffer = Convert.FromBase64String(strToDecrypt);
                    byte[] bytes3 = new PasswordDeriveBytes(strPassPhrase, bytes2, strHashAlgorithm, nPasswordIterations).GetBytes(nKeySize / 8);
                    RijndaelManaged rijndaelManaged = new RijndaelManaged();
                    rijndaelManaged.Mode = CipherMode.CBC;
                    ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes3, bytes1);
                    memoryStream = new MemoryStream(buffer);
                    cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
                    byte[] numArray = new byte[buffer.Length];
                    int count = cryptoStream.Read(numArray, 0, numArray.Length);
                    str = Encoding.UTF8.GetString(numArray, 0, count);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    memoryStream.Close();
                    cryptoStream.Close();
                }
                return str;
            }
            public string AIBEncrypt(string strStringToEncrypt, string strPassPhrase, string strSaltValue, string strHashAlgorithm, int nPasswordIterations, string strInitVector, int nKeySize)
            {
                MemoryStream memoryStream = (MemoryStream)null;
                CryptoStream cryptoStream = (CryptoStream)null;
                byte[] inArray;
                try
                {
                    byte[] bytes1 = Encoding.ASCII.GetBytes(strInitVector);
                    byte[] bytes2 = Encoding.ASCII.GetBytes(strSaltValue);
                    byte[] bytes3 = Encoding.UTF8.GetBytes(strStringToEncrypt);
                    byte[] bytes4 = new PasswordDeriveBytes(strPassPhrase, bytes2, strHashAlgorithm, nPasswordIterations).GetBytes(nKeySize / 8);
                    RijndaelManaged rijndaelManaged = new RijndaelManaged();
                    rijndaelManaged.Mode = CipherMode.CBC;
                    ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(bytes4, bytes1);
                    memoryStream = new MemoryStream();
                    cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
                    cryptoStream.Write(bytes3, 0, bytes3.Length);
                    cryptoStream.FlushFinalBlock();
                    inArray = memoryStream.ToArray();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    memoryStream.Close();
                    cryptoStream.Close();
                }
                return Convert.ToBase64String(inArray);
            }
            public string GetConfigurationValueObselete(string strKeyName, bool bRequired)
            {
                string str = "";
                try
                {
                    str = (string)new AppSettingsReader().GetValue(strKeyName, typeof(string));
                }
                catch (Exception ex)
                {
                    if (bRequired)
                        throw ex;
                }
                return str;
            }

            public string GetConfigurationValue(string Key, bool bRequired)
            {
                string str1 = string.Empty;
                string str2 = string.Empty;
                try
                {
                    if (ConfigurationManager.AppSettings[Key] != null)
                        return ConfigurationManager.AppSettings[Key];
                    if (bRequired)
                        throw new ApplicationException(this.AIBAppend(this.AIBAppend(" Please configure ", Key), " in configuration file "));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return str1;
            }
            public string GetTimeStampEx()
            {
                string strOriginal = "";
                string strToAppend = ";";
                return this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(this.AIBAppend(strOriginal, DateTime.Now.Year.ToString()), strToAppend), DateTime.Now.Month.ToString()), strToAppend), DateTime.Now.Day.ToString()), strToAppend), DateTime.Now.Hour.ToString()), strToAppend), DateTime.Now.Minute.ToString()), strToAppend), DateTime.Now.Second.ToString()), strToAppend), DateTime.Now.Millisecond.ToString());
            }

        }
        public string AIB_DataEncryption(string strDataToEncrypt)
        {
            string str1 = string.Empty;
            string str2;
            try
            {
                str2 = new AIB().AIBEncrypt(strDataToEncrypt, "DATATOENCRYPT", "9849483364", "SHA1", 2, "@1B2c3D4e5F6g7H8", 128);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return str2;
        }

        public string AIB_DataDecryption(string strDataToDecrypt)
        {
            string str1 = string.Empty;
            string str2;
            try
            {
                str2 = new AIB().AIBDecrypt(strDataToDecrypt, "DATATOENCRYPT", "9849483364", "SHA1", 2, "@1B2c3D4e5F6g7H8", 128);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return str2;
        }

        public string AIB_EncodingUrl(string strURL)
        {
            string str1 = string.Empty;
            int num1 = 0;
            string str2 = string.Empty;
            string str3 = string.Empty;
            string str4 = string.Empty;
            string strStringToEncrypt = HttpContext.Current.Request.UserHostAddress.ToString();
            string str5 = string.Empty;
            string str6 = string.Empty;
            AIB AIB = new AIB();
            string configurationValue = AIB.GetConfigurationValue("EventLog", false);
            foreach (object obj in (Array)HttpUtility.UrlEncodeToBytes(strURL.ToUpper()))
                num1 = num1 + Convert.ToInt32(obj.ToString()) ^ 1867;
            string strOriginal1 = strURL + "&mvisions=";
            int num2 = num1 ^ 1867;
            string timeStampEx = AIB.GetTimeStampEx();
            if (configurationValue == "1")
                AIB.AIBEventLog("Inside AIB_EncodingUrl with url " + strURL + ". The Client IP Address was found to be " + strStringToEncrypt + ". The Timestamp was " + timeStampEx, 1);
            string strToAppend1 = AIB.AIBEncrypt(timeStampEx, "9849483364", "9849483364", "SHA1", 2, "@1B2c3D4e5F6g7H8", 128);
            string strToAppend2 = AIB.AIBEncrypt(strStringToEncrypt, "9849483364", "9849483364", "SHA1", 2, "@1B2c3D4e5F6g7H8", 128);
            string strOriginal2 = AIB.AIBAppend(strOriginal1, num2.ToString());
            string strOriginal3 = AIB.AIBAppend(strOriginal2, "|");
            string strOriginal4 = AIB.AIBAppend(strOriginal3, strToAppend1);
            string strOriginal5 = AIB.AIBAppend(strOriginal4, "|");
            string strOriginal6 = AIB.AIBAppend(strOriginal5, strToAppend2);
            return AIB.AIBAppend(strOriginal6, "1");
        }

        public bool AIB_DecodingUrl(string strEncodedURL)
        {
            string str1 = string.Empty;
            int num1 = 0;
            string str2 = string.Empty;
            string str3 = string.Empty;
            bool flag1 = false;
            string str4 = string.Empty;
            string str5 = string.Empty;
            string str6 = string.Empty;
            string str7 = string.Empty;
            string str8 = string.Empty;
            string str9 = HttpContext.Current.Request.UserHostAddress.ToString();
            string str10 = string.Empty;
            string str11 = string.Empty;
            string str12 = string.Empty;
            string str13 = string.Empty;
            string str14 = string.Empty;
            AIB AIB = new AIB();
            string configurationValue = AIB.GetConfigurationValue("EventLog", false);
            if (configurationValue == "1")
                AIB.AIBEventLog("Inside AIB_DecodingUrl with Encodedurl " + strEncodedURL, 1);
            strEncodedURL = strEncodedURL.Remove(strEncodedURL.Length - 1);
            if (configurationValue == "1")
                AIB.AIBEventLog("After chopping the last character Encodedurl is " + strEncodedURL, 1);
            string str15 = AIB.GetConfigurationValue("ACL_EnableUrlEncoding", false) ?? string.Empty;
            if (str15.Length <= 0)
                str15 = "1";
            if (str15 == "0")
                return true;
            string str16 = AIB.GetConfigurationValue("ACL_CheckForSQLElementInURL", false) ?? string.Empty;
            if (str16.Length <= 0)
                str16 = "1";
            if (str16 == "1")
            {
                if (AIB.CheckSQLElementsInString(strEncodedURL))
                    return false;
            }
            string str17 = AIB.GetConfigurationValue("ACL_CheckForSQLElementInURL", false) ?? string.Empty;
            if (str17.Length <= 0)
                str17 = "0";
            long num2 = Convert.ToInt64(str17);
            string str18 = AIB.GetConfigurationValue("ACL_IPCheckEnableInUrlEncoding", false) ?? string.Empty;
            if (str18.Length <= 0)
                str18 = "1";
            string[] strArray1 = strEncodedURL.Split('&');
            if (strArray1.Length < 1)
            {
                if (configurationValue == "1")
                    AIB.AIBEventLog(" bIsValid is false because length of Array szArray is less than 1. strEncodedURL = " + strEncodedURL, 1);
                return false;
            }
            foreach (string str19 in strArray1)
            {
                if (str19.Length > 8)
                {
                    str2 = str19.Substring(0, 8);
                    if (str2 == "mvisions")
                    {
                        flag1 = true;
                        str3 = str19.Remove(0, 9);
                    }
                }
                else
                {
                    str2 = string.Empty;
                    str3 = string.Empty;
                }
                if (!flag1)
                    str1 = str1.Length > 0 ? str1 + "&" + str19 : str1 + str19;
            }
            if (str2.Length <= 0 || str3.Length <= 0)
            {
                if (configurationValue == "1")
                    AIB.AIBEventLog(" bIsValid is false because either strParamName is empty or strParamValue is empty. strParamName = " + str2 + " strParamValue = " + str3, 1);
                return false;
            }
            string str20 = str3;
            string[] strArray2 = str20.Split('|');
            if (strArray2 == null)
            {
                if (configurationValue == "1")
                    AIB.AIBEventLog("bIsValid is false because szKeyArray is null", 1);
                return false;
            }
            if (strArray2.Length != 3)
            {
                if (configurationValue == "1")
                    AIB.AIBEventLog(" bIsValid is false because length of szKeyArray not equal to 3. strKeyValue = " + str20, 1);
                return false;
            }
            int num3 = Convert.ToInt32(strArray2[0]);
            string strToDecrypt1 = strArray2[1];
            string strToDecrypt2 = strArray2[2];
            string strDateToCompare = AIB.AIBDecrypt(strToDecrypt1, "9849483364", "9849483364", "SHA1", 2, "@1B2c3D4e5F6g7H8", 128);
            string str21 = AIB.AIBDecrypt(strToDecrypt2, "9849483364", "9849483364", "SHA1", 2, "@1B2c3D4e5F6g7H8", 128);
            if (configurationValue == "1")
                AIB.AIBEventLog("Inside AIB_DecodingUrl with url " + strEncodedURL + ". The Client IP Address was found to be " + str9 + ". The Decrpted IP Address was " + str21, 1);
            if (num3 <= 0)
            {
                if (configurationValue == "1")
                    AIB.AIBEventLog(" bIsValid is false because the value of nReturnDecodedValue is less than or equal to 0  " + (object)num3, 1);
                return false;
            }
            foreach (object obj in (Array)HttpUtility.UrlEncodeToBytes(str1.ToUpper()))
                num1 = num1 + Convert.ToInt32(obj.ToString()) ^ 1867;
            int num4 = num1 ^ 1867;
            if (num3 == num4)
            {
                bool flag2 = true;
                if (str18 == "1")
                {
                    if (str9 == str21)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        if (configurationValue == "1")
                            AIB.AIBEventLog(" bIsValid is false because strClientIPAddress is not equal to strDecryptedIP. strClientIPAddress = " + str9 + " and strDecryptedIP = " + str21, 1);
                        return false;
                    }
                }
                if (num2 != 0L)
                {
                    double num5 = AIB.fnDateDifferenceWithCurrentDate(strDateToCompare);
                    if (num5 == -1.0)
                    {
                        if (configurationValue == "1")
                            AIB.AIBEventLog(string.Concat(new object[4]
              {
                (object) " bIsValid is false because dCalculatedDuration is equal -1. dCalculatedDuration = ",
                (object) num5,
                (object) " strDecryptedTimeStamp = ",
                (object) strDateToCompare
              }), 1);
                        return false;
                    }
                    if (num5 <= (double)num2)
                    {
                        flag2 = true;
                    }
                    else
                    {
                        if (configurationValue == "1")
                            AIB.AIBEventLog(string.Concat(new object[4]
              {
                (object) " bIsValid is false because dCalculatedDuration is less than equal to lUrlExpiryDuration. dCalculatedDuration = ",
                (object) num5,
                (object) " and lUrlExpiryDuration = ",
                (object) num2
              }), 1);
                        return false;
                    }
                }
                return flag2;
            }
            if (configurationValue == "1")
                AIB.AIBEventLog(string.Concat(new object[4]
        {
          (object) " bIsValid is false because nReturnDecodedValue is not equal to nDecodedValue. nReturnDecodedValue = ",
          (object) num3,
          (object) " and nDecodedValue = ",
          (object) num4
        }), 1);
            return false;
        }
    }
}
