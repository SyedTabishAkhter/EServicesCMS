using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;
using EServicesCms.Common;
using EServicesCms.Models;
using System.IO;


namespace EServicesCms.Common
{
    /// <summary>
    /// Summary description for DownloadFile
    /// </summary>
    public class DownloadFile : IHttpHandler
    {

        private string output = string.Empty;
        private int templateId = 0;
        private string language = string.Empty;
        private string fileName = string.Empty;
        byte[] binaryData = null;
        public void ProcessRequest(HttpContext context)
        {
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {
                //if (Common.Helper.URL_Decode(context.Request.Url.ToString()) == false)
                //{
                //    context.Response.Write("URL_Decode Error");
                //    context.Response.End();
                //}

                if (string.IsNullOrEmpty(context.Request["templateId"]) == false)
                    templateId = Convert.ToInt32(context.Request["templateId"]);

                if (string.IsNullOrEmpty(context.Request["language"]) == false)
                    language = context.Request["language"];

                var iFile = iDbContext.ServiceTemplates.SingleOrDefault(x => x.TemplateId == templateId);
                if (iFile != null)
                {
                    if (language == "en")
                        fileName = iFile.FileUrl;
                    else
                        fileName = iFile.FileUrlAlt;
                }

                if (WebConfig.GetIntValue("ServiceTemplates_Source") == (int)TemplatesSources.Folder)
                {
                    binaryData = FileToByteArray(fileName);
                    if (binaryData == null)
                    {
                        string closeWindow = @"<html><head></head><body><script>alert('"+ DbManager.GetText("Services", "lblTEmplateNotFound", "Template Not Found") + "');window.open('', '_self', ''); window.close();</script></body></html>";
                        context.Response.Write(closeWindow);
                        return;
                    }
                    context.Response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");
                }
                else
                {
                    var iFileData = iDbContext.ServiceTemplatesDatas.SingleOrDefault(x => x.TemplateId == templateId && x.Language == language);
                    if (iFileData == null)
                    {
                        string closeWindow = @"<html><head></head><body><script>alert('" + DbManager.GetText("Services", "lblTEmplateNotFound", "Template Not Found") + "');window.open('', '_self', ''); window.close();</script></body></html>";
                        context.Response.Write(closeWindow);
                        return;
                    }

                    context.Response.AddHeader("content-disposition", "attachment; filename=\"" + fileName + "\"");

                    binaryData = Convert.FromBase64String(iFileData.FileData);
                }
                context.Response.Clear();
                context.Response.ClearContent();
                context.Response.ContentType = FileHelper.GetContentType(fileName);
                context.Response.BinaryWrite(binaryData);
                context.Response.End();
            }
            catch (Exception E)
            {
                string closeWindow = @"<html><head></head><body><script>alert('Error while download the file = " + E.Message + "');window.open('', '_self', ''); window.close();</script></body></html>";
                context.Response.Write(closeWindow);
                return;
            }
            finally
            {
                iDbContext.Dispose();
            }
        }
        public static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            string path = WebConfig.GetStringValue("TemplateUploadFilePath");

            if (File.Exists(path + fileName))
            {
                using (FileStream fs = File.OpenRead(path + fileName))
                {
                    var binaryReader = new BinaryReader(fs);
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }
            }
            return fileData;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}