using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;

namespace EServicesCms.Common
{
    class MimeType
    {
        public string Extension { get; set; }
        public string Type { get; set; }
    }

    public class FileHelper
    {
        private ArrayList _Files = null;

        public FileHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        private static List<MimeType> mimeTypes = new List<MimeType>();

        static FileHelper()
        {
            #region --> MimeTypes

            mimeTypes.Add(new MimeType() { Extension = "323", Type = "text/h323" });
            mimeTypes.Add(new MimeType() { Extension = "acx", Type = "application/internet-property-stream" });
            mimeTypes.Add(new MimeType() { Extension = "ai", Type = "application/postscript" });
            mimeTypes.Add(new MimeType() { Extension = "aif", Type = "audio/x-aiff" });
            mimeTypes.Add(new MimeType() { Extension = "aifc", Type = "audio/x-aiff" });
            mimeTypes.Add(new MimeType() { Extension = "aiff", Type = "audio/x-aiff" });
            mimeTypes.Add(new MimeType() { Extension = "asf", Type = "video/x-ms-asf" });
            mimeTypes.Add(new MimeType() { Extension = "asr", Type = "video/x-ms-asf" });
            mimeTypes.Add(new MimeType() { Extension = "asx", Type = "video/x-ms-asf" });
            mimeTypes.Add(new MimeType() { Extension = "au", Type = "audio/basic" });
            mimeTypes.Add(new MimeType() { Extension = "avi", Type = "video/x-msvideo" });
            mimeTypes.Add(new MimeType() { Extension = "axs", Type = "application/olescript" });
            mimeTypes.Add(new MimeType() { Extension = "bas", Type = "text/plain" });
            mimeTypes.Add(new MimeType() { Extension = "bcpio", Type = "application/x-bcpio" });
            mimeTypes.Add(new MimeType() { Extension = "bin", Type = "application/octet-stream" });
            mimeTypes.Add(new MimeType() { Extension = "bmp", Type = "image/bmp" });
            mimeTypes.Add(new MimeType() { Extension = "c", Type = "text/plain" });
            mimeTypes.Add(new MimeType() { Extension = "cat", Type = "application/vnd.ms-pkiseccat" });
            mimeTypes.Add(new MimeType() { Extension = "cdf", Type = "application/x-cdf" });
            mimeTypes.Add(new MimeType() { Extension = "cer", Type = "application/x-x509-ca-cert" });
            mimeTypes.Add(new MimeType() { Extension = "class", Type = "application/octet-stream" });
            mimeTypes.Add(new MimeType() { Extension = "clp", Type = "application/x-msclip" });
            mimeTypes.Add(new MimeType() { Extension = "cmx", Type = "image/x-cmx" });
            mimeTypes.Add(new MimeType() { Extension = "cod", Type = "image/cis-cod" });
            mimeTypes.Add(new MimeType() { Extension = "cpio", Type = "application/x-cpio" });
            mimeTypes.Add(new MimeType() { Extension = "crd", Type = "application/x-mscardfile" });
            mimeTypes.Add(new MimeType() { Extension = "crl", Type = "application/pkix-crl" });
            mimeTypes.Add(new MimeType() { Extension = "crt", Type = "application/x-x509-ca-cert" });
            mimeTypes.Add(new MimeType() { Extension = "csh", Type = "application/x-csh" });
            mimeTypes.Add(new MimeType() { Extension = "css", Type = "text/css" });
            mimeTypes.Add(new MimeType() { Extension = "dcr", Type = "application/x-director" });
            mimeTypes.Add(new MimeType() { Extension = "der", Type = "application/x-x509-ca-cert" });
            mimeTypes.Add(new MimeType() { Extension = "dir", Type = "application/x-director" });
            mimeTypes.Add(new MimeType() { Extension = "dll", Type = "application/x-msdownload" });
            mimeTypes.Add(new MimeType() { Extension = "dms", Type = "application/octet-stream" });
            mimeTypes.Add(new MimeType() { Extension = "doc", Type = "application/msword" });
            mimeTypes.Add(new MimeType() { Extension = "docx", Type = "application/msword" });
            mimeTypes.Add(new MimeType() { Extension = "dot", Type = "application/msword" });
            mimeTypes.Add(new MimeType() { Extension = "dvi", Type = "application/x-dvi" });
            mimeTypes.Add(new MimeType() { Extension = "dxr", Type = "application/x-director" });
            mimeTypes.Add(new MimeType() { Extension = "eps", Type = "application/postscript" });
            mimeTypes.Add(new MimeType() { Extension = "etx", Type = "text/x-setext" });
            mimeTypes.Add(new MimeType() { Extension = "evy", Type = "application/envoy" });
            mimeTypes.Add(new MimeType() { Extension = "exe", Type = "application/octet-stream" });
            mimeTypes.Add(new MimeType() { Extension = "fif", Type = "application/fractals" });
            mimeTypes.Add(new MimeType() { Extension = "flr", Type = "x-world/x-vrml" });
            mimeTypes.Add(new MimeType() { Extension = "gif", Type = "image/gif" });
            mimeTypes.Add(new MimeType() { Extension = "gtar", Type = "application/x-gtar" });
            mimeTypes.Add(new MimeType() { Extension = "gz", Type = "application/x-gzip" });
            mimeTypes.Add(new MimeType() { Extension = "h", Type = "text/plain" });
            mimeTypes.Add(new MimeType() { Extension = "hdf", Type = "application/x-hdf" });
            mimeTypes.Add(new MimeType() { Extension = "hlp", Type = "application/winhlp" });
            mimeTypes.Add(new MimeType() { Extension = "hqx", Type = "application/mac-binhex40" });
            mimeTypes.Add(new MimeType() { Extension = "hta", Type = "application/hta" });
            mimeTypes.Add(new MimeType() { Extension = "htc", Type = "text/x-component" });
            mimeTypes.Add(new MimeType() { Extension = "htm", Type = "text/html" });
            mimeTypes.Add(new MimeType() { Extension = "html", Type = "text/html" });
            mimeTypes.Add(new MimeType() { Extension = "htt", Type = "text/webviewhtml" });
            mimeTypes.Add(new MimeType() { Extension = "ico", Type = "image/x-icon" });
            mimeTypes.Add(new MimeType() { Extension = "ief", Type = "image/ief" });
            mimeTypes.Add(new MimeType() { Extension = "iii", Type = "application/x-iphone" });
            mimeTypes.Add(new MimeType() { Extension = "ins", Type = "application/x-internet-signup" });
            mimeTypes.Add(new MimeType() { Extension = "isp", Type = "application/x-internet-signup" });
            mimeTypes.Add(new MimeType() { Extension = "jfif", Type = "image/pipeg" });
            mimeTypes.Add(new MimeType() { Extension = "jpe", Type = "image/jpeg" });
            mimeTypes.Add(new MimeType() { Extension = "jpeg", Type = "image/jpeg" });
            mimeTypes.Add(new MimeType() { Extension = "jpg", Type = "image/jpeg" });
            mimeTypes.Add(new MimeType() { Extension = "js", Type = "application/x-javascript" });
            mimeTypes.Add(new MimeType() { Extension = "latex", Type = "application/x-latex" });
            mimeTypes.Add(new MimeType() { Extension = "lha", Type = "application/octet-stream" });
            mimeTypes.Add(new MimeType() { Extension = "lsf", Type = "video/x-la-asf" });
            mimeTypes.Add(new MimeType() { Extension = "lsx", Type = "video/x-la-asf" });
            mimeTypes.Add(new MimeType() { Extension = "lzh", Type = "application/octet-stream" });
            mimeTypes.Add(new MimeType() { Extension = "m13", Type = "application/x-msmediaview" });
            mimeTypes.Add(new MimeType() { Extension = "m14", Type = "application/x-msmediaview" });
            mimeTypes.Add(new MimeType() { Extension = "m3u", Type = "audio/x-mpegurl" });
            mimeTypes.Add(new MimeType() { Extension = "man", Type = "application/x-troff-man" });
            mimeTypes.Add(new MimeType() { Extension = "mdb", Type = "application/x-msaccess" });
            mimeTypes.Add(new MimeType() { Extension = "me", Type = "application/x-troff-me" });
            mimeTypes.Add(new MimeType() { Extension = "mht", Type = "message/rfc822" });
            mimeTypes.Add(new MimeType() { Extension = "mhtml", Type = "message/rfc822" });
            mimeTypes.Add(new MimeType() { Extension = "mid", Type = "audio/mid" });
            mimeTypes.Add(new MimeType() { Extension = "mny", Type = "application/x-msmoney" });
            mimeTypes.Add(new MimeType() { Extension = "mov", Type = "video/quicktime" });
            mimeTypes.Add(new MimeType() { Extension = "movie", Type = "video/x-sgi-movie" });
            mimeTypes.Add(new MimeType() { Extension = "mp2", Type = "video/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "mp3", Type = "audio/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "mpa", Type = "video/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "mpe", Type = "video/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "mpeg", Type = "video/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "mpg", Type = "video/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "mpp", Type = "application/vnd.ms-project" });
            mimeTypes.Add(new MimeType() { Extension = "mpv2", Type = "video/mpeg" });
            mimeTypes.Add(new MimeType() { Extension = "ms", Type = "application/x-troff-ms" });
            mimeTypes.Add(new MimeType() { Extension = "mvb", Type = "application/x-msmediaview" });
            mimeTypes.Add(new MimeType() { Extension = "nws", Type = "message/rfc822" });
            mimeTypes.Add(new MimeType() { Extension = "oda", Type = "application/oda" });
            mimeTypes.Add(new MimeType() { Extension = "p10", Type = "application/pkcs10" });
            mimeTypes.Add(new MimeType() { Extension = "p12", Type = "application/x-pkcs12" });
            mimeTypes.Add(new MimeType() { Extension = "p7b", Type = "application/x-pkcs7-certificates" });
            mimeTypes.Add(new MimeType() { Extension = "p7c", Type = "application/x-pkcs7-mime" });
            mimeTypes.Add(new MimeType() { Extension = "p7m", Type = "application/x-pkcs7-mime" });
            mimeTypes.Add(new MimeType() { Extension = "p7r", Type = "application/x-pkcs7-certreqresp" });
            mimeTypes.Add(new MimeType() { Extension = "p7s", Type = "application/x-pkcs7-signature" });
            mimeTypes.Add(new MimeType() { Extension = "pbm", Type = "image/x-portable-bitmap" });
            mimeTypes.Add(new MimeType() { Extension = "pdf", Type = "application/pdf" });
            mimeTypes.Add(new MimeType() { Extension = "pfx", Type = "application/x-pkcs12" });
            mimeTypes.Add(new MimeType() { Extension = "pgm", Type = "image/x-portable-graymap" });
            mimeTypes.Add(new MimeType() { Extension = "pko", Type = "application/ynd.ms-pkipko" });
            mimeTypes.Add(new MimeType() { Extension = "pma", Type = "application/x-perfmon" });
            mimeTypes.Add(new MimeType() { Extension = "pmc", Type = "application/x-perfmon" });
            mimeTypes.Add(new MimeType() { Extension = "pml", Type = "application/x-perfmon" });
            mimeTypes.Add(new MimeType() { Extension = "pmr", Type = "application/x-perfmon" });
            mimeTypes.Add(new MimeType() { Extension = "pmw", Type = "application/x-perfmon" });
            mimeTypes.Add(new MimeType() { Extension = "pnm", Type = "image/x-portable-anymap" });
            mimeTypes.Add(new MimeType() { Extension = "pot,", Type = "application/vnd.ms-powerpoint" });
            mimeTypes.Add(new MimeType() { Extension = "ppm", Type = "image/x-portable-pixmap" });
            mimeTypes.Add(new MimeType() { Extension = "pps", Type = "application/vnd.ms-powerpoint" });
            mimeTypes.Add(new MimeType() { Extension = "ppt", Type = "application/vnd.ms-powerpoint" });
            mimeTypes.Add(new MimeType() { Extension = "pptx", Type = "application/vnd.ms-powerpoint" });
            mimeTypes.Add(new MimeType() { Extension = "prf", Type = "application/pics-rules" });
            mimeTypes.Add(new MimeType() { Extension = "ps", Type = "application/postscript" });
            mimeTypes.Add(new MimeType() { Extension = "pub", Type = "application/x-mspublisher" });
            mimeTypes.Add(new MimeType() { Extension = "qt", Type = "video/quicktime" });
            mimeTypes.Add(new MimeType() { Extension = "ra", Type = "audio/x-pn-realaudio" });
            mimeTypes.Add(new MimeType() { Extension = "ram", Type = "audio/x-pn-realaudio" });
            mimeTypes.Add(new MimeType() { Extension = "ras", Type = "image/x-cmu-raster" });
            mimeTypes.Add(new MimeType() { Extension = "rgb", Type = "image/x-rgb" });
            mimeTypes.Add(new MimeType() { Extension = "rmi", Type = "audio/mid" });
            mimeTypes.Add(new MimeType() { Extension = "roff", Type = "application/x-troff" });
            mimeTypes.Add(new MimeType() { Extension = "rtf", Type = "application/rtf" });
            mimeTypes.Add(new MimeType() { Extension = "rtx", Type = "text/richtext" });
            mimeTypes.Add(new MimeType() { Extension = "scd", Type = "application/x-msschedule" });
            mimeTypes.Add(new MimeType() { Extension = "sct", Type = "text/scriptlet" });
            mimeTypes.Add(new MimeType() { Extension = "setpay", Type = "application/set-payment-initiation" });
            mimeTypes.Add(new MimeType() { Extension = "setreg", Type = "application/set-registration-initiation" });
            mimeTypes.Add(new MimeType() { Extension = "sh", Type = "application/x-sh" });
            mimeTypes.Add(new MimeType() { Extension = "shar", Type = "application/x-shar" });
            mimeTypes.Add(new MimeType() { Extension = "sit", Type = "application/x-stuffit" });
            mimeTypes.Add(new MimeType() { Extension = "snd", Type = "audio/basic" });
            mimeTypes.Add(new MimeType() { Extension = "spc", Type = "application/x-pkcs7-certificates" });
            mimeTypes.Add(new MimeType() { Extension = "spl", Type = "application/futuresplash" });
            mimeTypes.Add(new MimeType() { Extension = "src", Type = "application/x-wais-source" });
            mimeTypes.Add(new MimeType() { Extension = "sst", Type = "application/vnd.ms-pkicertstore" });
            mimeTypes.Add(new MimeType() { Extension = "stl", Type = "application/vnd.ms-pkistl" });
            mimeTypes.Add(new MimeType() { Extension = "stm", Type = "text/html" });
            mimeTypes.Add(new MimeType() { Extension = "svg", Type = "image/svg+xml" });
            mimeTypes.Add(new MimeType() { Extension = "sv4cpio", Type = "application/x-sv4cpio" });
            mimeTypes.Add(new MimeType() { Extension = "sv4crc", Type = "application/x-sv4crc" });
            mimeTypes.Add(new MimeType() { Extension = "swf", Type = "application/x-shockwave-flash" });
            mimeTypes.Add(new MimeType() { Extension = "t", Type = "application/x-troff" });
            mimeTypes.Add(new MimeType() { Extension = "tar", Type = "application/x-tar" });
            mimeTypes.Add(new MimeType() { Extension = "tcl", Type = "application/x-tcl" });
            mimeTypes.Add(new MimeType() { Extension = "tex", Type = "application/x-tex" });
            mimeTypes.Add(new MimeType() { Extension = "texi", Type = "application/x-texinfo" });
            mimeTypes.Add(new MimeType() { Extension = "texinfo", Type = "application/x-texinfo" });
            mimeTypes.Add(new MimeType() { Extension = "tgz", Type = "application/x-compressed" });
            mimeTypes.Add(new MimeType() { Extension = "tif", Type = "image/tiff" });
            mimeTypes.Add(new MimeType() { Extension = "tiff", Type = "image/tiff" });
            mimeTypes.Add(new MimeType() { Extension = "tr", Type = "application/x-troff" });
            mimeTypes.Add(new MimeType() { Extension = "trm", Type = "application/x-msterminal" });
            mimeTypes.Add(new MimeType() { Extension = "tsv", Type = "text/tab-separated-values" });
            mimeTypes.Add(new MimeType() { Extension = "txt", Type = "text/plain" });
            mimeTypes.Add(new MimeType() { Extension = "uls", Type = "text/iuls" });
            mimeTypes.Add(new MimeType() { Extension = "ustar", Type = "application/x-ustar" });
            mimeTypes.Add(new MimeType() { Extension = "vcf", Type = "text/x-vcard" });
            mimeTypes.Add(new MimeType() { Extension = "vrml", Type = "x-world/x-vrml" });
            mimeTypes.Add(new MimeType() { Extension = "wav", Type = "audio/x-wav" });
            mimeTypes.Add(new MimeType() { Extension = "wcm", Type = "application/vnd.ms-works" });
            mimeTypes.Add(new MimeType() { Extension = "wdb", Type = "application/vnd.ms-works" });
            mimeTypes.Add(new MimeType() { Extension = "wks", Type = "application/vnd.ms-works" });
            mimeTypes.Add(new MimeType() { Extension = "wmf", Type = "application/x-msmetafile" });
            mimeTypes.Add(new MimeType() { Extension = "wps", Type = "application/vnd.ms-works" });
            mimeTypes.Add(new MimeType() { Extension = "wri", Type = "application/x-mswrite" });
            mimeTypes.Add(new MimeType() { Extension = "wrl", Type = "x-world/x-vrml" });
            mimeTypes.Add(new MimeType() { Extension = "wrz", Type = "x-world/x-vrml" });
            mimeTypes.Add(new MimeType() { Extension = "xaf", Type = "x-world/x-vrml" });
            mimeTypes.Add(new MimeType() { Extension = "xbm", Type = "image/x-xbitmap" });
            mimeTypes.Add(new MimeType() { Extension = "xla", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xlc", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xlm", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xls", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xlsx", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xlt", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xlw", Type = "application/vnd.ms-excel" });
            mimeTypes.Add(new MimeType() { Extension = "xof", Type = "x-world/x-vrml" });
            mimeTypes.Add(new MimeType() { Extension = "xpm", Type = "image/x-xpixmap" });
            mimeTypes.Add(new MimeType() { Extension = "xwd", Type = "image/x-xwindowdump" });
            mimeTypes.Add(new MimeType() { Extension = "z", Type = "application/x-compress" });
            mimeTypes.Add(new MimeType() { Extension = "zip", Type = "application/zip" });
            mimeTypes.Add(new MimeType() { Extension = "rar", Type = "application/zip" });

            #endregion
        }
        public static void ReadFileInfo(string basePath, out string fileName, out int contentLength, out byte[] binaryData)
        {
            fileName = string.Empty;
            contentLength = 0;
            binaryData = null;
            try
            {
                if (System.IO.File.Exists(basePath) == true)
                {
                    FileInfo fi = new FileInfo(basePath);
                    fileName = System.IO.Path.GetFileName(fi.FullName);
                    long nFileLen = fi.Length;
                    contentLength = Convert.ToInt32(nFileLen);
                    FileStream rFile = new FileStream(basePath, FileMode.Open);
                    binaryData = new byte[nFileLen];
                    rFile.Read(binaryData, 0, (int)nFileLen);
                    rFile.Close();
                    rFile.Dispose();
                }
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return;
        }
        public static void AppendToTempFile(string tempFilename, byte[] data)
        {
            try
            {
                //This method appends block of data to the end of the temporary file.
                FileStream fs = new FileStream(WebConfig.GetStringValue("TemporaryFileStorageLocation") + tempFilename, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(data);
                bw.Close();
                fs.Close();
            }
            catch (Exception err)
            {
                throw new Exception("AppendToTempFile: " + err.Message);
            }
        }
        public static void DeleteFile(string basePath)
        {
            try
            {
                if (System.IO.File.Exists(basePath) == true)
                {
                    System.IO.File.Delete(basePath);
                }
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return;
        }
        public static string CalculateFileSize(double FileInBytes)
        {
            string strSize = "00";
            if (FileInBytes < 1024)
                strSize = FileInBytes + " B";//Byte
            else if (FileInBytes > 1024 & FileInBytes < 1048576)
                strSize = Math.Round((FileInBytes / 1024), 2) + " KB";//Kilobyte
            else if (FileInBytes > 1048576 & FileInBytes < 107341824)
                strSize = Math.Round((FileInBytes / 1024) / 1024, 2) + " MB";//Megabyte
            else if (FileInBytes > 107341824 & FileInBytes < 1099511627776)
                strSize = Math.Round(((FileInBytes / 1024) / 1024) / 1024, 2) + " GB";//Gigabyte
            else
                strSize = Math.Round((((FileInBytes / 1024) / 1024) / 1024) / 1024, 2) + " TB";//Terabyte
            return strSize;
        }
        public static string GetContentType(string fileName)
        {
            if (fileName == null)
                throw new ArgumentException("Paramater fileName cannot be null");

            fileName = fileName.ToLower().Trim();
            string mimeType = "application/octet-stream";

            if (!fileName.Contains("."))
                throw new ArgumentException("Paramater fileName does not have extension");

            string extension = fileName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[1];
            extension = extension.Trim();

            MimeType mime = mimeTypes.Where(m => m.Extension.Equals(extension)).SingleOrDefault();
            if (mime == null)
                return mimeType;
            else
                return mime.Type;
        }
        public static int GetAttachmentType(string typE)
        {
            int returN = 1;
            try
            {
                switch (typE)
                {
                    case "application/msword":
                    case "application/pdf":
                    case "application/vnd.ms-excel":
                    case "application/vnd.ms-powerpoint":
                    case "application/zip":
                    case "text/plain":
                        returN = 0;
                        break;
                    default:
                        returN = 1;
                        break;
                }
            }
            catch //(Exception Exp)
            {

            }
            return returN;
        }
        public static string GetAttachmentTypeIcon(string typE)
        {
            var i = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            string returN = string.Empty;
            try
            {
                switch (typE)
                {
                    case "video/x-msvideo":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/icon_avi.png") + "' align='absmiddle' alt='Video File' />";
                        break;

                    case "application/msword":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/word.gif") + "'  align='absmiddle' alt='Document File' />";
                        break;

                    case "application/pdf":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/pdf.gif") + "'  align='absmiddle' alt='PDF File' />";
                        break;

                    case "application/vnd.ms-excel":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/excel.gif") + "'  align='absmiddle' alt='Excel File' />";
                        break;

                    case "application/vnd.ms-powerpoint":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/PPT.gif") + "'  align='absmiddle' alt='Presentation File' />";
                        break;

                    case "image/bmp":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/BMP.gif") + "'  align='absmiddle' alt='Bitmap File' />";
                        break;

                    case "image/DWG":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/DWG.gif") + "'  align='absmiddle' alt='Drawing File' />";
                        break;

                    case "application/x-shockwave-flash":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/FLASH.gif") + "'  align='absmiddle' alt='Drawing File' />";
                        break;

                    case "image/jpeg":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/JPEG.gif") + "'  align='absmiddle' alt='Jpg File' />";
                        break;

                    case "image/gif":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/GIF.gif") + "'  align='absmiddle' alt='GIF File' />";
                        break;

                    case "image/tiff":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/TIFF.gif") + "' align='absmiddle' alt='TIFF File' />";
                        break;

                    case "application/zip":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/WINZIP.gif") + "'  align='absmiddle' alt='WINZIP File' />";
                        break;

                    case "text/plain":
                        returN = "<img border='0' src='" + i.Content("~/Content/images/TEXT.gif") + "'  align='absmiddle' alt='TEXT File' />";
                        break;

                    default:
                        returN = "<img border='0' src='" + i.Content("~/Content/images/Attachment_nor.gif") + "'  align='absmiddle' alt='" + typE + " File' />";
                        break;
                }
            }
            catch (Exception Exp)
            {
                returN = Exp.Message.ToString();
            }
            return returN;
        }

        public static string GetInputTypeIcon(string typE)
        {
            var i = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            string returN = string.Empty;
            try
            {
                switch (typE)
                {
                    case "text":
                    case "number":
                    case "textarea":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/1.gif") + "' align='absmiddle' alt='"+ typE + "' title='" + typE + "' />";
                        break;

                    case "email":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/2.gif") + "'  align='absmiddle' alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "select":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/4.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "file":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/6.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "radio":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/7.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "checkbox":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/8.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "date":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/9.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "password":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/10.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "tel":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/11.gif") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "url":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/12.png") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    case "map":
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/13.png") + "' align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;

                    default:
                        returN = "<img border='0' width='17' height='17' src='" + i.Content("~/Content/images/0.png") + "'  align='absmiddle'  alt='" + typE + "' title='" + typE + "' />";
                        break;
                }
            }
            catch (Exception Exp)
            {
                returN = Exp.Message.ToString();
            }
            return returN;
        }
    }
}