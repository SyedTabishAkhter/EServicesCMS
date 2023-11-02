using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EServicesCms.Models;

namespace EServicesCms.Controllers
{
    public class BaseController : Controller
    {
        public MOFPortalEntities iDbContext = new MOFPortalEntities();
        public Models.User iUser = null;
        public BaseController()
        {
            try
            {
                ViewBag.iUser = Common.Security.GetUser();
                ViewBag.SessionExpiryDuration = System.Web.HttpContext.Current.Session.Timeout * 60;
            }
            catch (Exception E)
            {
                throw E;
            }
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
                var ControllerName = (string)filterContext.RouteData.Values["controller"];
                var ActionName = (string)filterContext.RouteData.Values["action"];
                var LineNumber = new System.Diagnostics.StackTrace(filterContext.Exception, true).GetFrame(0).GetFileLineNumber();

                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        filterContext.Exception.Message,
                        filterContext.Exception.StackTrace,
                        ControllerName,
                        ActionName,
                        LineNumber
                    }
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                Exception exception = filterContext.Exception;
                filterContext.ExceptionHandled = true;
                var Result = this.View("Error", new HandleErrorInfo(exception,
                    filterContext.RouteData.Values["controller"].ToString(),
                    filterContext.RouteData.Values["action"].ToString()));
                filterContext.Result = Result;
            }
        }
    }
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["iUser"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class UrlDecode : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string requestUrl = filterContext.HttpContext.Request.Url.OriginalString.ToString().Replace(":443","").Replace(":200", "");
            HttpContext.Current.Trace.Write(requestUrl);

            ////string AbsolutePath = filterContext.HttpContext.Request.Url.AbsolutePath.ToString();
            ////HttpContext.Current.Trace.Write(AbsolutePath);

            ////string AbsoluteUri = filterContext.HttpContext.Request.Url.AbsoluteUri.ToString();
            ////HttpContext.Current.Trace.Write(AbsoluteUri);

            ////string PathAndQuery = filterContext.HttpContext.Request.Url.PathAndQuery.ToString();
            ////HttpContext.Current.Trace.Write(PathAndQuery);

            if (Common.Helper.URL_Decode(requestUrl) == false)
            {
                HttpContext.Current.Trace.Write("Decode Failed");
                filterContext.Result = new RedirectResult("~/Account/Index");
                return;
            }
        }
    }
}