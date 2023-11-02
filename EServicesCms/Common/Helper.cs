using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Xml;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace EServicesCms.Common
{
    public class Helper
    {
        public static string URL_Encode(string urlToEncode)
        {
            string returN = urlToEncode;
            string Url_Encode_ExcludePorts = "";
            try
            {
                Url_Encode_ExcludePorts = WebConfig.GetStringValue("Url_Encode_ExcludePorts");

                string[] arrRay = Url_Encode_ExcludePorts.Split(',');

                foreach(var iPort in arrRay)
                {
                    if (urlToEncode.Contains(iPort))
                        urlToEncode = urlToEncode.Replace(iPort, "");
                }
                urlToEncode = urlToEncode.Replace(":443", "").Replace(":200", "");

                RBAC aclMain = new RBAC();
                returN = aclMain.AIB_EncodingUrl(urlToEncode).Replace("&amp;","&");
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return returN;
        }
        public static bool URL_Decode(string urlToDecode)
        {
            bool returN = true;
            try
            {
                RBAC aclMain = new RBAC();
                returN = aclMain.AIB_DecodingUrl(urlToDecode);
            }
            catch
            {
                returN = false;
            }
            return returN;
        }
        public static DateTime ParseDate(string date)
        {
            DateTime? dtDate = null;
            string[] arrRay = null;

            try
            {
                if (date.Contains("+"))
                {
                    arrRay = date.Split('+');
                    if (arrRay != null)
                        date = arrRay[0].TrimEnd();
                }
                else if (date.Contains("-"))
                {
                    arrRay = date.Split('-');
                    if (arrRay != null)
                        date = arrRay[0].TrimEnd();
                }
                else
                {
                    string timeZonesToSkip = WebConfig.GetStringValue("TimeZoneToSkip");

                    arrRay = timeZonesToSkip.Split(',');

                    for (int v = 0; v < arrRay.Length; v++)
                    {
                        if (date.Contains(arrRay[v]) == true)
                            date = date.Replace(arrRay[v], string.Empty);
                    }
                }
                dtDate = DateTime.Parse(date);
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return dtDate.Value;
        }
        public static DateTime FormatDate(string date)
        {
            try
            {
                IFormatProvider provider = new System.Globalization.CultureInfo("en-GB");
                return Convert.ToDateTime(date, provider);
            }
            catch
            {
                string format = "ddd dd MMM h:mm tt yyyy";

                return DateTime.ParseExact(date, format, System.Globalization.CultureInfo.InvariantCulture);
            }
        }
        public static string FormatDateEN(DateTime date)
        {
            return string.Format("{0:dd/MM/yyyy}", date);
        }
        public static string FormatDateMDY(string date)
        {
            return string.Format("{0:MM/dd/yyyy}", FormatDate(date));
        }
        public static string FormatDateYMD(DateTime date)
        {
            return string.Format("{0:yyyy/MM/dd}", date);
        }
        public static bool IsNumeric(string input)
        {
            int number;
            return int.TryParse(input, out number);
        }
        public static string RefineString(string text)
        {
            string resultText = text;
            //RBAC.AIB i = new RBAC.AIB();
            try
            {
                if (!string.IsNullOrEmpty(resultText))
                {
                    resultText = resultText.Replace("'", "");
                    resultText = resultText.Replace("\"", "");
                    resultText = resultText.Replace("\"\"", "");
                    resultText = resultText.Replace("''", "");
                    resultText = resultText.Trim();
                    resultText = resultText.TrimStart();
                    resultText = resultText.TrimEnd();
                    resultText = resultText.Replace("\r", " ");
                    resultText = resultText.Replace("\t", " ");
                    resultText = Regex.Replace(resultText, @"^\s+", "");
                    resultText = Regex.Replace(resultText, @"\s+$", "");
                }
                else
                    resultText = string.Empty;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return resultText;
        }
        public static string IsNull<T>(T? value) where T : struct
        {
            if (value.HasValue)
                return string.Format("'{0}'", RefineString(value.ToString()).Replace("''", "NULL").Replace("'NULL'", "NULL"));
            else
                return "NULL";
        }
        public static string IsNullGet<T>(T? value) where T : struct
        {
            if (value.HasValue)
                return string.Format("{0}", RefineString(value.ToString()).Replace("''", "").Replace("'NULL'", ""));
            else
                return "";
        }
        public static T? NullIf<T>(T? value) where T : struct
        {
            if (value.HasValue)
                return (T?)value;
            else
                return (T?)null;
        }
        public static string IsNull(string value)
        {
            string retVal = "";
            if (string.IsNullOrEmpty(value))
                return "NULL";
            else
            {
                retVal = string.Format("{0}", RefineString(value));
                retVal = retVal.Replace("'", "").Replace("''", "NULL").Replace("'NULL'", "NULL");
            }
            return retVal;
        }
        public static string IsNullGet(string value)
        {
            string retVal = "";
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
            {
                retVal = string.Format("{0}", RefineString(value));
            }
            return retVal;
        }
        public static string IsNullCompare(string value)
        {
            string retVal = "";
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
            {
                retVal = string.Format("{0}", RefineString(value).ToUpper());
            }
            return retVal;
        }
        public static string EncodeString(string stringToEncode)
        {
            return Crypto.Encrypt(stringToEncode);
        }
        public static string DecodeString(string stringToDecode)
        {
            return Crypto.Decrypt(stringToDecode);
        }
        public static string ReadValueFromXmlFile(string xmlFilePath, string tagName)
        {
            string xmlValue = string.Empty;
            try
            {
                XDocument xdoc = XDocument.Load(xmlFilePath);
                var query = (from q in xdoc.Descendants(tagName)
                             select q).SingleOrDefault();

                if (query == null)
                    throw new ApplicationException(string.Format("XML Element = <{0}> Not Found in {1} file.", tagName, xmlFilePath));

                xmlValue = query.Value;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return xmlValue;
        }
        public static string DisplayDateFormat(DateTime date)
        {
            return string.Format("{0:" + WebConfig.GetStringValue("DisplayDateFormat") + "}", date);
        }
        public static DateTime SaveDateFormat(string date)
        {
            date = string.Format("{0:" + WebConfig.GetStringValue("SaveDateFormat") + "}", FormatDate(date));

            return DateTime.ParseExact(date, WebConfig.GetStringValue("SaveDateFormat"), new System.Globalization.CultureInfo("en-GB"));
        }
        public static string GetXmlString(System.Xml.XmlDocument xmlDoc)
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(sw);
            xw.Formatting = System.Xml.Formatting.Indented;
            xmlDoc.WriteTo(xw);
            return sw.ToString();
        }
        public static void DeleteFile(string strPath)
        {
            System.IO.FileInfo fi = new FileInfo(strPath);
            if (fi != null && fi.Exists == true)
            {
                TimeSpan min = new TimeSpan(0, 0, 10, 0, 0);
                if (fi.CreationTime < DateTime.Now.Subtract(min))
                {
                    fi.Delete();
                }
                //fi.Delete();
            }
            return;
        }
        public static void RemoveFiles(string strPath)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strPath);
            System.IO.FileInfo[] fiArr = di.GetFiles();
            foreach (System.IO.FileInfo fi in fiArr)
            {
                if (fi.Extension.ToString() == ".pdf")
                {
                    TimeSpan min = new TimeSpan(0, 0, 0, 5, 0);
                    if (fi.CreationTime < DateTime.Now.Subtract(min))
                    {
                        fi.Delete();
                    }
                }
            }
            return;
        }
        public static string CalculatePages(int _totalRecords)
        {
            string retHtml = "";

            int totalPages = 0;
            int _rowsPerPage = 15;
            totalPages = _totalRecords / _rowsPerPage;
            if (_totalRecords % _rowsPerPage != 0)
                totalPages++;

            retHtml += "<div class='row'>";
            retHtml += "    <div class='col-sm-12'>";
            retHtml += "        <div class='well well-sm' style='background-color:#f2e7c4;text-align:center;'> Total Records : {0} | Total Pages : {1} </div>";
            retHtml += "    </div>";
            retHtml += "</div>";

            retHtml = string.Format(retHtml, _totalRecords, totalPages);

            return retHtml;
        }
        public static void CalculateRecords(int _currentPage, int _rowsPerPage, out int _startRow, out int _endRow)
        {
            _startRow = 1;
            _endRow = 1;

            _startRow = ((_currentPage - 1) * _rowsPerPage + 1);
            if (_startRow <= 0)
                _startRow = 1;
            _endRow = ((_currentPage) * _rowsPerPage);
        }
        public static byte[] GetFile(string filePath)
        {
            WebClient req = null;
            HttpResponse response = null;

            req = new WebClient();
            response = HttpContext.Current.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Buffer = true;
            response.AddHeader("Content-Disposition", "attachment;filename=\"" + filePath + "\"");
            return req.DownloadData(WebConfig.GetStringValue("FileUploadPath") + filePath);
        }
        public static int GetCurrentCulture()
        {
            int lang = WebConfig.GetIntValue("DefalutCulture");
            try
            {
                if (HttpContext.Current.Session["CurrentCulture"] != null)
                    lang = Convert.ToInt32(HttpContext.Current.Session["CurrentCulture"].ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            return lang;
        }
        public static string GetCurrentDirection()
        {
            try
            {
                if (GetCurrentCulture() == 1)
                    return "ltr";
                else
                    return "rtl";
            }
            catch (Exception E)
            {
                throw E;
            }
        }
        public static void TrimDataTable(DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
                if (c.DataType == typeof(string))
                    foreach (DataRow r in dt.Rows)
                        try
                        {
                            string value = r[c.ColumnName].ToString();
                            if (string.IsNullOrEmpty(value))
                                continue;

                            r[c.ColumnName] = r[c.ColumnName].ToString().Trim().TrimStart().TrimEnd(
                                                   ' ',
                                                   '\t',
                                                   '\n',
                                                   '\v',
                                                   '\f',
                                                   '\r');
                        }
                        catch
                        { }
        }
        public static bool IsUrlExist(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "HEAD";
            webRequest.Timeout = 10000;
            try
            {
                var response = webRequest.GetResponse();
                /* response is `200 OK` */
                response.Close();
            }
            catch
            {
                /* Any other response */
                return false;
            }
            return true;
        }       
        public static string GetDbEntityValidationException(string source, DbEntityValidationException Exp)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var eve in Exp.EntityValidationErrors)
            {
                sb.Append(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    eve.Entry.Entity.GetType().Name, eve.Entry.State));
                foreach (var ve in eve.ValidationErrors)
                {
                    sb.Append(String.Format("- Property: \"{0}\", Error: \"{1}\"",
                        ve.PropertyName, ve.ErrorMessage));
                }
                sb.Append("<br />");
            }
            return sb.ToString();
        }
        public static string GetRandomCode()
        {
            Random random = new Random();
            string Celebritycode = "";

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Celebritycode = new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return Celebritycode;
        }
        public static int GetTotalPages(int _totalRecords, int _rowsPerPage)
        {
            int totalPages = 0;
            totalPages = _totalRecords / _rowsPerPage;
            if (_totalRecords % _rowsPerPage != 0)
                totalPages++;

            return totalPages;
        }
        public static string GenerateNumber()
        {
            string newNumber = string.Empty;

            newNumber = "CB-" + new Random().Next(1001, 99999);

            return newNumber;
        }
        public static string EncodeToBase64String(string stringToEncode)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(stringToEncode);
            return Convert.ToBase64String(encbuff);
        }
        public static string DecodeFromBase64String(string stringToDecode)
        {
            byte[] decbuff = Convert.FromBase64String(stringToDecode);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }
        public static string EncryptStringAES(string cipherText)
        {
            var key = Encoding.UTF8.GetBytes(WebConfig.GetStringValue("EncryptStringAES_Key"));
            var iv = Encoding.UTF8.GetBytes(WebConfig.GetStringValue("DecryptStringAES_Key"));
            var encrypted = EncryptStringToBytes(cipherText, key, iv);
            return Convert.ToBase64String(encrypted);
        }
        public static string DecryptStringAES(string cipherText)
        {
            string decriptedFromJavascript = "";
            try
            {
                var key = Encoding.UTF8.GetBytes(WebConfig.GetStringValue("EncryptStringAES_Key"));
                var iv = Encoding.UTF8.GetBytes(WebConfig.GetStringValue("DecryptStringAES_Key"));
                var encrypted = Convert.FromBase64String(cipherText);
                decriptedFromJavascript = DecryptStringFromBytes(encrypted, key, iv);
            }
            catch (Exception Exp)
            {
                decriptedFromJavascript = "ERROR - " + Exp.Message;
            }
            return string.Format(decriptedFromJavascript);
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.ECB;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.BlockSize = 128;
                rijAlg.KeySize = 128;
                rijAlg.IV = iv;
                rijAlg.Key = key;
                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }
        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.ECB;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.BlockSize = 128;
                rijAlg.KeySize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.  
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.  
            return encrypted;
        }
        public static int CurrentLanguage()
        {
            int returnValue = 0;
            try
            {
                if (System.Web.HttpContext.Current.Session["CurrentLanguage"] != null)
                {
                    returnValue = Convert.ToInt32(System.Web.HttpContext.Current.Session["CurrentLanguage"].ToString());
                }
                else
                {
                    returnValue =  WebConfig.GetIntValue("CurrentLanguage");
                    System.Web.HttpContext.Current.Session["CurrentLanguage"] = returnValue;
                }
            }
            catch { }
            return returnValue;
        }
        public static string GetPageAlertCodes(string viewId)
        {
            NameValueCollection nvc = null;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            string jScript = "";
            try
            {
                
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    nvc = CacheManager.GetApplicationCache().GetLabelCollectionsEn(viewId);
                }
                else
                {
                    nvc = CacheManager.GetApplicationCache().GetLabelCollectionsAr(viewId);
                }
                using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    writer.WriteStartObject();
                    foreach (string key in nvc.AllKeys)
                    {
                        writer.WritePropertyName(key);
                        writer.WriteValue(nvc[key]);
                    }
                    writer.WriteEndObject();
                }
                //jScript = "function ShowAlert(key){ var errors=%ERRORS%;if(errors[key] == undefined) Swal.fire({ icon: 'info', text: 'Please configure Info Code for  viewId = %viewId% and LabelId = ' + key }); else Swal.fire({ icon: 'info',confirmButtonText:ShowConfirm('OkButton'), text: errors[key] });}";
                jScript = "var screenErrors=%ERRORS%; function ShowAlert(key, mode){ if(screenErrors[key] == undefined) Swal.fire({ icon: 'info', text: 'Please configure Info Code for  viewId = %viewId% and LabelId = ' + key }); else { if (mode == '1'){return screenErrors[key];}else{ Swal.fire({ icon: 'info',confirmButtonText:screenErrors['OkButton'], text: screenErrors[key] });}}}";
                jScript = jScript.Replace("%ERRORS%", sb.ToString());
                jScript = jScript.Replace("%viewId%", viewId.ToString());
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return jScript;
        }
        public static string GetPageMultipleAlertCodes(string viewId)
        {
            NameValueCollection nvc = null;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            string jScript = "";
            string[] arrRay = null;
            try
            {
                using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
                {
                    writer.Formatting = Newtonsoft.Json.Formatting.Indented;
                    writer.WriteStartObject();

                    arrRay = viewId.Split(',');
                    foreach(var iView in arrRay)
                    {
                        if (Helper.CurrentLanguage() == (int)Language.English)
                        {
                            nvc = CacheManager.GetApplicationCache().GetLabelCollectionsEn(iView);
                        }
                        else
                        {
                            nvc = CacheManager.GetApplicationCache().GetLabelCollectionsAr(iView);
                        }
                        foreach (string key in nvc.AllKeys)
                        {
                            writer.WritePropertyName(key);
                            writer.WriteValue(nvc[key]);
                        }
                    }
                    writer.WriteEndObject();
                }
                //jScript = "function ShowAlert(key){ var errors=%ERRORS%;if(errors[key] == undefined) Swal.fire({ icon: 'info', text: 'Please configure Info Code for  viewId = %viewId% and LabelId = ' + key }); else Swal.fire({ icon: 'info',confirmButtonText:ShowConfirm('OkButton'), text: errors[key] });}";
                //jScript = "var screenErrors=%ERRORS%; function ShowAlert(key, mode){ if(screenErrors[key] == undefined) Swal.fire({ icon: 'info', text: 'Please configure Info Code for  viewId = %viewId% and LabelId = ' + key }); else { if (mode == '0'){Swal.fire({ icon: 'info',confirmButtonText:screenErrors['OkButton'], text: screenErrors[key] });}else{ screenErrors[key]}}}";
                jScript = "var screenErrors=%ERRORS%; function ShowAlert(key, mode){ if(screenErrors[key] == undefined) Swal.fire({ icon: 'info', text: 'Please configure Info Code for  viewId = %viewId% and LabelId = ' + key }); else { if (mode == '1'){return screenErrors[key];}else{ Swal.fire({ icon: 'info',confirmButtonText:screenErrors['OkButton'], text: screenErrors[key] });}}}";
                jScript = jScript.Replace("%ERRORS%", sb.ToString());
                jScript = jScript.Replace("%viewId%", viewId.ToString());
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return jScript;
        }
        
    }
}