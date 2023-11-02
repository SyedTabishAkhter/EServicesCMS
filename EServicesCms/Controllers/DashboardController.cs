using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EServicesCms.Common;
using EServicesCms.Models;

namespace EServicesCms.Controllers
{
    
    [SessionTimeout]
    public class DashboardController : BaseController
    {
        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");
            return View();
        }
        [HttpPost]
        public JsonResult AjaxGetServiceGuideRatings(DashboardFilters iFilter)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                iResult = new JsonResult();
                iResult.Data = new { result = true, d = DbManager.GetServiceGuideRatings(iFilter) };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AjaxGetServiceGuideByTypes_LINE(DashboardFilters iFilter)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                iResult = new JsonResult();
                iResult.Data = new { result = true, d = DbManager.GetServiceGuideByTypes_LINE(iFilter) };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AjaxGetServiceGuideVisitors(DashboardFilters iFilter)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                iResult = new JsonResult();
                iResult.Data = new { result = true, d = DbManager.GetServiceGuideVisitors(iFilter) };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AjaxGetServiceGuideByUserTypes(DashboardFilters iFilter)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                iResult = new JsonResult();
                iResult.Data = new { result = true, d = DbManager.GetServiceGuideByUserTypes(iFilter) };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AjaxGetServiceGuideByKeywords(DashboardFilters iFilter)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                var keywordsList = DbManager.GetServiceGuideByKeywords(iFilter);
                if (keywordsList != null && keywordsList.Count > 0)
                {
                    foreach(var keyword in keywordsList)
                    {
                        html += "<span data-weight='" + keyword.Value + "'>" + keyword.KPI + "</span>";
                    }
                }

                iResult = new JsonResult();
                iResult.Data = new { result = true, d = html };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AjaxGetServiceGuideByServices(DashboardFilters iFilter)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                iResult = new JsonResult();
                iResult.Data = new { result = true, d = DbManager.GetServiceGuideByServices(iFilter) };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}