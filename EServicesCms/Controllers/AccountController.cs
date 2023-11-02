using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EServicesCms.Models;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;
using EServicesCms.Common;

namespace EServicesCms.Controllers
{
    public class AccountController : Controller
    {
        private MOFPortalEntities iDbContext = new MOFPortalEntities();

        public AccountController()
        {
        }
        public ActionResult Index()
        {
            this.Session.Abandon();
            if (Common.WebConfig.GetBoolValue("EnableSSO_Authentication"))
                return this.RedirectToAction("Login", "Auth");
            else
                return this.RedirectToAction("Login", "Account");
            return View();
        }
        [HttpPost]
        public JsonResult AjaxLogin(Models.AuthenticateRequest iObject)
        {
            iObject = iObject.TrimObject();
            Models.MOFPortalEntities iDbContext = new Models.MOFPortalEntities();
            try
            {
                string uid = Guid.NewGuid().ToString();

                string password = Common.Helper.EncodeToBase64String(iObject.Password.HtmlStrip());
                string UserName = iObject.UserName.HtmlStrip();
                var iRecord = iDbContext.Users.SingleOrDefault(x => x.UserName == UserName && x.Password == password);
                if (iRecord == null)
                {
                    return Json(new { Result = false, Message = DbManager.GetText("Login", "lblInvalidUserName","Invalid Username or bad password supplied.") }, JsonRequestBehavior.AllowGet);
                }
                else if (iRecord.IsActive == false)
                {
                    return Json(new { Result = false, Message = DbManager.GetText("Login", "lblInactiveAccount", "Your account is InActive.") }, JsonRequestBehavior.AllowGet);
                }
                else if (iRecord.IsDeleted == true)
                {
                    return Json(new { Result = false, Message = DbManager.GetText("Login", "lblDeletedAccount","Your account is deleted.") }, JsonRequestBehavior.AllowGet);
                }

                var iRoleGropus = DbManager.GetLkRoleObject(iRecord.RoleId.Value);

                Session["iUser"] = iRecord;
                Session["iUserRoleGroups"] = iRoleGropus;

                if (Security.isUserAuthorized("MOF_ESRV_SYSTEM") == false)
                {
                    return Json(new { Result = false, Message = DbManager.GetText("Login", "lblLoginFail", "Sorry! You do not have sufficient privilage to access the system.") }, JsonRequestBehavior.AllowGet);
                }

                Session["iClientIpAddress"] = GetClientIp();
                Session["LoginAt"] = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 7;
                    aut.ServiceId = null;
                    aut.CategoryId = null;
                    aut.UniqueId = iObject.UserName;
                    aut.Remarks = Security.GetUser().UserName + " LOGGED-IN to MofEServicesCms Portal";
                    aut.RowInsertDate = DateTime.Now;
                    aut.IpAddress = Common.Security.GetIpAddress();
                    aut.RowInsertedBy = Security.GetUser().UserName;
                    aut.IsDeleted = false;
                    iDbContext.UserScreenActions.AddOrUpdate(aut);
                    iDbContext.SaveChanges();
                }
            }
            catch (Exception E)
            {
                return Json(new { Result = false, Message = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Login", "lblLognSuccess","Hurray! Login Successful.") }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxForgotPassword(Models.AuthenticateRequest iObject)
        {
            Models.MOFPortalEntities iDbContext = new Models.MOFPortalEntities();
            try
            {
                var iRecord = iDbContext.Users.SingleOrDefault(x => x.Email == iObject.UserName && x.IsActive == true && x.IsDeleted == false);
                if (iRecord == null)
                {
                    return Json(new { Result = false, Message = DbManager.GetText("Login", "lblEmailNotRegistered") }, JsonRequestBehavior.AllowGet);
                }
                try
                {
                    string address = Common.WebConfig.GetStringValue("TemplatesPath") + "ForgotPassword0.html";
                    //
                    System.Net.WebClient client = new System.Net.WebClient();
                    string messageBody = client.DownloadString(address);
                    messageBody = messageBody.Replace("FULL_NAME", iRecord.FullName);
                    messageBody = messageBody.Replace("USER_NAME", iRecord.Email);
                    messageBody = messageBody.Replace("PASS_WORD", Crypto.Decrypt(iRecord.Password));
                    //
                    var emsg = new EmailMessage();
                    emsg.Receiver = iRecord.Email;
                    emsg.Subject = Common.DbManager.GetText("Login", "lblRecoverPassword","EServices CMS Password");
                    emsg.Sender = WebConfig.GetStringValue("SmtpUserName");
                    emsg.Message = messageBody;
                    emsg.cc = string.Empty;
                    //
                    var iStatus = EmailManager.sendEmail(emsg);
                    if (iStatus == EmailManager.EmailSentStatus.SUCCESS)
                    {

                    }
                }
                catch (Exception Exp)
                {

                }
            }
            catch (Exception E)
            {
                return Json(new { Result = false, Message = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Login", "lblPasswordSent") }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
            {
                var aut = new Models.UserScreenAction();
                aut.ActionId = 7;
                aut.ServiceId = null;
                aut.CategoryId = null;
                aut.UniqueId = Security.GetUser().UserName;
                aut.Remarks = Security.GetUser().UserName + " LOGGED-OUT to MofEServicesCms Portal";
                aut.RowInsertDate = DateTime.Now;
                aut.IpAddress = Common.Security.GetIpAddress();
                aut.RowInsertedBy = Security.GetUser().UserName;
                aut.IsDeleted = false;
                iDbContext.UserScreenActions.AddOrUpdate(aut);
                iDbContext.SaveChanges();
            }
            this.Session.Abandon();
            this.Session.Clear();

            if (Common.WebConfig.GetBoolValue("EnableSSO_Authentication"))
            {
                Response.Redirect(WebConfig.GetStringValue("SsoLogout_Url"), true);
            }    
            else
                return this.RedirectToAction("Index", "Account");

            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");
            System.Web.HttpContext.Current.Session["CurrentLanguage"] = Helper.CurrentLanguage();
            if (Common.WebConfig.GetBoolValue("EnableSSO_Authentication"))
                return this.RedirectToAction("Login", "Auth");           
            return View();
        }
        private string GetClientIp()
        {
            string ip = string.Empty;
            try
            {
                ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] ipRange = ip.Split(',');
                    string trueIP = ipRange[0].Trim();
                }
                else
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception exp)
            {
                ip = "127.0.0.1";
            }
            return ip;
        }

        [HttpGet]
        public JsonResult ChangeLanguage()
        {
            try
            {
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    System.Web.HttpContext.Current.Session["CurrentLanguage"] = (int)Language.Arabic;
                }
                else
                {
                    System.Web.HttpContext.Current.Session["CurrentLanguage"] = (int)Language.English;
                }
            }
            catch (Exception E)
            {
                return Json(new { Result = false, Message = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}