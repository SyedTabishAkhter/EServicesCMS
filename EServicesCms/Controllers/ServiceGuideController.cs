using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;
using EServicesCms.Common;
using System.Net;
using System.Configuration;
using RestSharp;

namespace EServicesCms.Controllers
{
    [SessionTimeout]
    public class ServiceGuideController : BaseController
    {
        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");
            return View();
        }

        #region --> Hints

        [UrlDecode]
        public ActionResult Hints()
        {
            var lookup = Common.DbManager.GetLkLookupIdList("LkServiceGuideHintTypes");
            ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionEng");
            return View();
        }

        [UrlDecode]
        [HttpPost]
        public JsonResult AjaxGetLkHints(Models.BaseRequest baseRequest)
        {
            string html = "";
            int nctr = 0;
            try
            {
                var result = Common.DbManager.GetServiceGuideHints(baseRequest.dummy);
                if (result != null && result.Count > 0)
                {
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>"+(nctr +1)+"</div>";
                        html += "<div class='title nameEn'>{1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_LOOKUP") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}','{2}');\" class='link'>Edit</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}');\" class='link diabled'>Edit</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_LOOKUP") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','{1}','{2}');\" class='link'>Delete</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}');\" class='link diabled'>Delete</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_VIEW_LOOKUPLIST") == true)
                        {
                            string url = Common.Helper.URL_Encode(Url.Action("List", "Lookup", null, Request.Url.Scheme, null) + "?id=" + i.HintId);
                            html += "<a href='" + url + "'  class='link'>View Actions</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}');\" class='link diabled'>List</a>";
                        }
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.HintId, i.DescriptionEng, i.DescriptionAlt);
                        nctr++;
                    }
                }
                else
                {
                    html += @"<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>
                                    <div class='card'>
                                        <div class='tag'>{0}</div>
                                        <div class='title'>{1}</div>
                                    </div>
                                </div>";
                    html = string.Format(html, DbManager.GetText("Lookup", "lblSorry", "Sorry !"), DbManager.GetText("Lookup", "lblSorryNoRecords", "No Records Found !"));
                }

                var serializer = Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer;
            }
            catch (Exception Exp)
            {
                return Json(new { success = false, result = Exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
        }

        [UrlDecode]
        [HttpPost]
        public JsonResult AjaxSaveHint(Models.ServiceGuideHint iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.ServiceGuideHints.SingleOrDefault(x => x.HintId == iObject.HintId);
                if (iRecord != null)
                {
                    ////var b = iDbContext.ServiceGuideHints.Where(x => x.IsDeleted == false && x.HintId != iRecord.HintId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault();
                    ////if (b != null)
                    ////    return Json(new { Result = false, Message = iObject.DescriptionEng + " Already Exists.", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.HintId = iObject.HintId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceGuideHints.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " updated ServiceGuideHints record = " + iRecord.HintId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Common.Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    //var b = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault();
                    //if (b != null)
                    //    return Json(new { Result = false, Message = iObject.DescriptionEng + " Already Exists.", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.ServiceGuideHint();

                    var i = iDbContext.ServiceGuideHints.Where(x => x.IsDeleted == false && x.TypeId == iObject.TypeId).OrderByDescending(x => x.SortOrder).ToList();
                    if (i != null && i.Count > 0)
                        Code = Convert.ToInt32(i[i.Count - 1].SortOrder) + 1;

                    TimeSpan span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                    string ReferenceNo = Convert.ToString((double)span.TotalSeconds).Replace(".", "");

                    iRecord.HintId = Convert.ToInt32(ReferenceNo);
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Common.Security.GetUser().UserName;
                    iRecord.SortOrder = Code;
                    iDbContext.ServiceGuideHints.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " added ServiceGuideHints record = " + iRecord.HintId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Common.Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception E)
            {
                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = "Service Guide Hint Saved Successfully.!", Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteHint(Models.ServiceGuideHint iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceGuideHints.SingleOrDefault(x => x.HintId == iObject.HintId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iDbContext.ServiceGuideHints.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.ServiceGuideHintActions.Where(x => x.IsDeleted == false && x.HintId == iRecord.HintId).ToList();
                    if (i != null && i.Count > 0)
                    {
                        foreach (var j in i)
                        {
                            j.IsDeleted = true;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Common.Security.GetUser().UserName;
                            iDbContext.ServiceGuideHintActions.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " deleted ServiceGuideHints record = " + iRecord.HintId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Common.Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = "Service Guide Hint Deleted Successfully.!", Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> Service Guide Content

        [UrlDecode]
        public ActionResult Content()
        {
            Models.ServiceGuide iObject = new Models.ServiceGuide();
            try
            {
                ViewBag.FormMode = "Edit";

                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");

                iObject = Common.DbManager.GetServiceGuideData();
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [HttpPost]
        public JsonResult SaveServiceGuideContent(Models.ServiceGuide iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    iObject = iObject.TrimObject();
                    //
                    var iRecord = iDbContext.ServiceGuides.Where(x=>x.IsDeleted == false).FirstOrDefault();
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iDbContext.ServiceGuides.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }
                    //
                    iObject.IsDeleted = false;
                    iObject.RowInsertDate = DateTime.Now;
                    iObject.RowInsertedBy = Security.GetUser().UserName;
                    iObject.IpAddress = Common.Security.GetIpAddress();
                    iDbContext.ServiceGuides.AddOrUpdate(iObject);
                    iDbContext.SaveChanges();
                    //
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.CategoryId = null;
                        aut.UniqueId = "Service Guide Update";
                        aut.Remarks = Security.GetUser().UserName + " UPDATED SERVICE GUIDE CONTENT ";
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Dispose();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblContentSave", "Service Guide Content Updated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> Service Guide Procedures

        [UrlDecode]
        public ActionResult Procedures()
        {
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetProceduresList(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetServiceGuideProcedures(iSearch, out totalRows);

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.TitleEng
                        ,c.TitleAlt
                        ,EditIcon((int)Guide.Procedures, c)
                        ,DeleteIcon((int)Guide.Procedures, c)
                    }
                ).ToList();
                return Json(new
                {
                    draw = iSearch.draw,
                    recordsTotal = totalRows,
                    recordsFiltered = totalRows,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetProceduresList", E.ToString());
                return Json(new { result = false, Error = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProcedureCards(Models.BaseRequest baseRequest)
        {
            string html = "";
            try
            {
                var result = iDbContext.ServiceGuideProcedures.Where(x => x.IsDeleted == false).OrderBy(x=> x.SortOrder).ToList();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.TitleEng : i.TitleAlt;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.RecordId + "' id='SortOrder" + i.RecordId + "' onchange=\"ChangeSortOrder(" + (int)Guide.Procedures + "," + i.RecordId + ",'SortOrder" + i.RecordId + "');\">";
                        for (int k = minSort; k <= maxSort; k++)
                        {
                            if (i.SortOrder == k)
                                sortOrderControl += "<option value='" + k + "' selected>" + k + "</option>";
                            else
                                sortOrderControl += "<option value='" + k + "'>" + k + "</option>";
                        }
                        sortOrderControl += "</select>";

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>{3}</div>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_PROCEDURE") == true)
                        {
                            html += "<div class='title nameEn'>" + sortOrderControl + " {1}</div>";
                        }
                        else
                        {
                            html += "<div class='title nameEn'>{1}</div>";
                        }
                        
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";

                        if (string.IsNullOrEmpty(i.UrlEng) == false)
                            html += "   <a title='{1}' class='link' href='" + i.UrlEng + "' target='_blank'><svg xmlns='http://www.w3.org/2000/svg' width='25' height='25' fill='currentColor' class='bi bi-youtube' viewBox='0 0 16 16'><path d='M8.051 1.999h.089c.822.003 4.987.033 6.11.335a2.01 2.01 0 0 1 1.415 1.42c.101.38.172.883.22 1.402l.01.104.022.26.008.104c.065.914.073 1.77.074 1.957v.075c-.001.194-.01 1.108-.082 2.06l-.008.105-.009.104c-.05.572-.124 1.14-.235 1.558a2.007 2.007 0 0 1-1.415 1.42c-1.16.312-5.569.334-6.18.335h-.142c-.309 0-1.587-.006-2.927-.052l-.17-.006-.087-.004-.171-.007-.171-.007c-1.11-.049-2.167-.128-2.654-.26a2.007 2.007 0 0 1-1.415-1.419c-.111-.417-.185-.986-.235-1.558L.09 9.82l-.008-.104A31.4 31.4 0 0 1 0 7.68v-.123c.002-.215.01-.958.064-1.778l.007-.103.003-.052.008-.104.022-.26.01-.104c.048-.519.119-1.023.22-1.402a2.007 2.007 0 0 1 1.415-1.42c.487-.13 1.544-.21 2.654-.26l.17-.007.172-.006.086-.003.171-.007A99.788 99.788 0 0 1 7.858 2h.193zM6.4 5.209v4.818l4.157-2.408L6.4 5.209z' /></svg></a>";

                        if (string.IsNullOrEmpty(i.UrlAlt) == false)
                            html += "   <a title='{2}' class='link' href='" + i.UrlAlt + "' target='_blank'><svg xmlns='http://www.w3.org/2000/svg' width='25' height='25' fill='currentColor' class='bi bi-youtube' viewBox='0 0 16 16'><path d='M8.051 1.999h.089c.822.003 4.987.033 6.11.335a2.01 2.01 0 0 1 1.415 1.42c.101.38.172.883.22 1.402l.01.104.022.26.008.104c.065.914.073 1.77.074 1.957v.075c-.001.194-.01 1.108-.082 2.06l-.008.105-.009.104c-.05.572-.124 1.14-.235 1.558a2.007 2.007 0 0 1-1.415 1.42c-1.16.312-5.569.334-6.18.335h-.142c-.309 0-1.587-.006-2.927-.052l-.17-.006-.087-.004-.171-.007-.171-.007c-1.11-.049-2.167-.128-2.654-.26a2.007 2.007 0 0 1-1.415-1.419c-.111-.417-.185-.986-.235-1.558L.09 9.82l-.008-.104A31.4 31.4 0 0 1 0 7.68v-.123c.002-.215.01-.958.064-1.778l.007-.103.003-.052.008-.104.022-.26.01-.104c.048-.519.119-1.023.22-1.402a2.007 2.007 0 0 1 1.415-1.42c.487-.13 1.544-.21 2.654-.26l.17-.007.172-.006.086-.003.171-.007A99.788 99.788 0 0 1 7.858 2h.193zM6.4 5.209v4.818l4.157-2.408L6.4 5.209z' /></svg></a>";

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_PROCEDURE") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_DELETE_PROCEDURE") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','"+ name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.RecordId, i.TitleEng, i.TitleAlt, i.SortOrder);
                    }
                }
                else
                {
                    html += @"<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>
                                    <div class='card'>
                                        <div class='tag'>{0}</div>
                                        <div class='title'>{1}</div>
                                    </div>
                                </div>";
                    html = string.Format(html, DbManager.GetText("Lookup", "lblSorry", "Sorry !"), DbManager.GetText("Lookup", "lblSorryNoRecords", "No Records Found !"));
                }

                var serializer = Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer;
            }
            catch (Exception Exp)
            {
                return Json(new { success = false, result = Exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveProcedure(Models.ServiceGuideProcedure j)
        {
            DbContextTransaction iTransaction = null;
            string mode = "ADDED";
            if (j.RecordId > 0)
                mode = "UPDATED";
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    if (mode == "UPDATED")
                    {
                        j.RowUpdateDate = DateTime.Now;
                        j.RowUpdatedBy = Security.GetUser().UserName;
                    }
                    else
                    {
                        j.RowInsertDate = DateTime.Now;
                        j.RowInsertedBy = Security.GetUser().UserName;
                    }
                    j.IsDeleted = false;
                    j.IpAddress = Security.GetIpAddress();
                    iDbContext.ServiceGuideProcedures.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.CategoryId = null;
                        aut.UniqueId = j.RecordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " " +mode + " SERVICE GUIDE PROCEDURE  = " + j.RecordId + "-" + j.TitleEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblProcedureSave", "Service Guide Procedure Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProcedure(int recordId)
        {
            try
            {
                var iData = iDbContext.ServiceGuideProcedures.Where(x => x.RecordId == recordId).FirstOrDefault();

                return Json(iData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }//

        [HttpPost]
        public JsonResult DeleteProcedures(int recordId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceGuideProcedures.SingleOrDefault(x => x.RecordId == recordId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideProcedures.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.UniqueId = recordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE GUIDE PROCEDURE = " + recordId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblProcedureDelete", "Service Guide Procedure Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

       

        #endregion

        #region -->  Service Guide Channels

        [UrlDecode]
        public ActionResult Channels()
        {
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");

                var lookup = Common.DbManager.GetLkLookupIdList("LkServiceGuideChannels");
                if (Helper.CurrentLanguage() == (int)Language.English)
                    ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionEng");
                else
                    ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionAlt");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetChannelsList(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetServiceGuideChannels(iSearch, out totalRows);

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.TitleEng
                        ,c.TitleAlt
                        ,EditIcon((int)Guide.Channels, c)
                        ,DeleteIcon((int)Guide.Channels, c)
                    }
                ).ToList();
                return Json(new
                {
                    draw = iSearch.draw,
                    recordsTotal = totalRows,
                    recordsFiltered = totalRows,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetChannelsList", E.ToString());
                return Json(new { result = false, Error = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetChannelCards(Models.BaseRequest baseRequest)
        {
            string html = "";
            string channelName = "";
            try
            {
                var result = iDbContext.ServiceGuideChannels.Where(x => x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    var lks = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookUpName.ToUpper() == "LkServiceGuideChannels".ToUpper()).FirstOrDefault();

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.TitleEng : i.TitleAlt;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.RecordId + "' id='SortOrder" + i.RecordId + "' onchange=\"ChangeSortOrder(" + (int)Guide.Channels + "," + i.RecordId + ",'SortOrder" + i.RecordId + "');\">";
                        for (int k = minSort; k <= maxSort; k++)
                        {
                            if (i.SortOrder == k)
                                sortOrderControl += "<option value='" + k + "' selected>" + k + "</option>";
                            else
                                sortOrderControl += "<option value='" + k + "'>" + k + "</option>";
                        }
                        sortOrderControl += "</select>";

                        var lkop = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == lks.LookupId && x.Code == i.TypeId.ToString()).FirstOrDefault();
                        if (lkop != null)
                            channelName = lkop.DescriptionEng;

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";

                        //if (string.IsNullOrEmpty(channelName) == false)
                        //    html += "<div class='circle'>"+ channelName.Substring(0, 1) + "</div>";

                        html += "<div class='circle'>" + i.SortOrder + "</div>";
                        html += "<div class='tag'>{3}</div>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_CHANNEL") == true)
                        {
                            html += "<div class='title nameEn'>" + sortOrderControl + " {1}</div>";
                        }
                        else
                        {
                            html += "<div class='title nameEn'>{1}</div>";
                        }
                        //html += "<div class='title nameEn'>" + sortOrderControl + ".{1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_CHANNEL") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_DELETE_CHANNEL") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','"+ name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.RecordId, i.TitleEng, i.TitleAlt, channelName);
                    }
                }
                else
                {
                    html += @"<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>
                                    <div class='card'>
                                        <div class='tag'>{0}</div>
                                        <div class='title'>{1}</div>
                                    </div>
                                </div>";
                    html = string.Format(html, DbManager.GetText("Lookup", "lblSorry", "Sorry !"), DbManager.GetText("Lookup", "lblSorryNoRecords", "No Records Found !"));
                }

                var serializer = Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer;
            }
            catch (Exception Exp)
            {
                return Json(new { success = false, result = Exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveChannel(Models.ServiceGuideChannel j)
        {
            DbContextTransaction iTransaction = null;
            string mode = "ADDED";
            if (j.RecordId > 0)
                mode = "UPDATED";
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    if (mode == "UPDATED")
                    {
                        var a = iDbContext.ServiceGuideChannels.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId && x.RecordId != j.RecordId).FirstOrDefault();
                        if (a != null)
                            return Json(new { Result = false, Message = DbManager.GetText("ServiceGuide", "lblChannelExistg", "This channel already exists."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        j.RowUpdateDate = DateTime.Now;
                        j.RowUpdatedBy = Security.GetUser().UserName;
                    }
                    else
                    {
                        var a = iDbContext.ServiceGuideChannels.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault();
                        if (a != null)
                            return Json(new { Result = false, Message = DbManager.GetText("ServiceGuide", "lblChannelExistg", "This channel already exists."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        j.RowInsertDate = DateTime.Now;
                        j.RowInsertedBy = Security.GetUser().UserName;
                    }

                    j.IsDeleted = false;
                    j.IpAddress = Security.GetIpAddress();
                    iDbContext.ServiceGuideChannels.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.CategoryId = null;
                        aut.UniqueId = j.RecordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " " + mode + " SERVICE GUIDE CHANNEL  = " + j.RecordId + "-" + j.TitleEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblChannelSave", "Service Guide Channel Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetChannel(int recordId)
        {
            try
            {
                var iData = iDbContext.ServiceGuideChannels.Where(x => x.RecordId == recordId).FirstOrDefault();

                return Json(iData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//

        [HttpPost]
        public JsonResult DeleteChannels(int recordId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceGuideChannels.SingleOrDefault(x => x.RecordId == recordId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideChannels.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.UniqueId = recordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE GUIDE CHANNEL = " + recordId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblChannelDelete", "Service Guide Channel Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region -->  Service Guide Support

        [UrlDecode]
        public ActionResult Support()
        {
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");

                var lookup = Common.DbManager.GetLkLookupIdList("LkServiceGuideSupportTypes");
                if (Helper.CurrentLanguage() == (int)Language.English)
                    ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionEng");
                else
                    ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionAlt");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetSupportList(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetServiceGuideSupport(iSearch, out totalRows);

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.TitleEng
                        ,c.TitleAlt
                        ,EditIcon((int)Guide.Support, c)
                        ,DeleteIcon((int)Guide.Support, c)
                    }
                ).ToList();
                return Json(new
                {
                    draw = iSearch.draw,
                    recordsTotal = totalRows,
                    recordsFiltered = totalRows,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetSupportList", E.ToString());
                return Json(new { result = false, Error = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSupportCards(Models.BaseRequest baseRequest)
        {
            string html = "";
            string channelName = "";
            try
            {
                var result = iDbContext.ServiceGuideSupports.Where(x => x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    var lks = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookUpName.ToUpper() == "LkServiceGuideSupportTypes".ToUpper()).FirstOrDefault();

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.RecordId + "' id='SortOrder" + i.RecordId + "' onchange=\"ChangeSortOrder(" + (int)Guide.Support + "," + i.RecordId + ",'SortOrder" + i.RecordId + "');\">";
                        for (int k = minSort; k <= maxSort; k++)
                        {
                            if (i.SortOrder == k)
                                sortOrderControl += "<option value='" + k + "' selected>" + k + "</option>";
                            else
                                sortOrderControl += "<option value='" + k + "'>" + k + "</option>";
                        }
                        sortOrderControl += "</select>";

                        var lkop = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == lks.LookupId && x.Code == i.TypeId.ToString()).FirstOrDefault();
                        if (lkop != null)
                            channelName = lkop.DescriptionEng;

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";

                        //if (string.IsNullOrEmpty(channelName) == false)
                        //    html += "<div class='circle'>" + channelName.Substring(0, 1) + "</div>";

                        html += "<div class='circle'>" + i.SortOrder + "</div>";
                        html += "<div class='tag'>{3}</div>";
                        //html += "<div class='title nameEn'>" + sortOrderControl + ". {1}</div>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_SUPPORT") == true)
                        {
                            html += "<div class='title nameEn'>" + sortOrderControl + " {1}</div>";
                        }
                        else
                        {
                            html += "<div class='title nameEn'>{1}</div>";
                        }
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_SUPPORT") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_DELETE_SUPPORT") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','"+ name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.RecordId, i.DescriptionEng, i.DescriptionAlt, channelName);
                    }
                }
                else
                {
                    html += @"<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>
                                    <div class='card'>
                                        <div class='tag'>{0}</div>
                                        <div class='title'>{1}</div>
                                    </div>
                                </div>";
                    html = string.Format(html, DbManager.GetText("Lookup", "lblSorry", "Sorry !"), DbManager.GetText("Lookup", "lblSorryNoRecords", "No Records Found !"));
                }

                var serializer = Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer;
            }
            catch (Exception Exp)
            {
                return Json(new { success = false, result = Exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSupport(Models.ServiceGuideSupport j)
        {
            DbContextTransaction iTransaction = null;
            string mode = "ADDED";
            if (j.RecordId > 0)
                mode = "UPDATED";
            try
            {
                if (mode == "UPDATED")
                {
                    var a = iDbContext.ServiceGuideSupports.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId && x.RecordId != j.RecordId).FirstOrDefault();
                    if (a != null)
                        return Json(new { Result = false, Message = DbManager.GetText("ServiceGuide", "lblSupportExists", "This support type already exists."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    j.RowUpdateDate = DateTime.Now;
                    j.RowUpdatedBy = Security.GetUser().UserName;
                }
                else
                {
                    var a = iDbContext.ServiceGuideSupports.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault();
                    if (a != null)
                        return Json(new { Result = false, Message = DbManager.GetText("ServiceGuide", "lblSupportExists", "This support type already exists."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    j.RowInsertDate = DateTime.Now;
                    j.RowInsertedBy = Security.GetUser().UserName;
                }

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    j.IsDeleted = false;
                    j.IpAddress = Security.GetIpAddress();
                    iDbContext.ServiceGuideSupports.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.CategoryId = null;
                        aut.UniqueId = j.RecordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " " + mode + " SERVICE GUIDE Support  = " + j.RecordId + "-" + j.TitleEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblSupportSave", "Service Guide Support Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetSupport(int recordId)
        {
            try
            {
                var iData = iDbContext.ServiceGuideSupports.Where(x => x.RecordId == recordId).FirstOrDefault();

                return Json(iData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//

        [HttpPost]
        public JsonResult DeleteSupport(int recordId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceGuideSupports.SingleOrDefault(x => x.RecordId == recordId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideSupports.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.UniqueId = recordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE GUIDE Support = " + recordId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblSupportDelete", "Service Guide Support Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region -->  Service Guide Videos

        [UrlDecode]
        public ActionResult Videos()
        {
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");

                var lookup = Common.DbManager.GetLkLookupIdList("LkSreviceGuideVideoTypes");
                if (Helper.CurrentLanguage() == (int)Language.English)
                    ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionEng");
                else

                    ViewBag.Types = new SelectList(lookup.ToArray(), "Code", "DescriptionAlt");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetVideosList(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetServiceGuideVideos(iSearch, out totalRows);

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.TitleEng
                        ,c.TitleAlt
                        ,EditIcon((int)Guide.Videos, c)
                        ,DeleteIcon((int)Guide.Videos, c)
                    }
                ).ToList();
                return Json(new
                {
                    draw = iSearch.draw,
                    recordsTotal = totalRows,
                    recordsFiltered = totalRows,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetVideosList", E.ToString());
                return Json(new { result = false, Error = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetVideoCards(Models.BaseRequest baseRequest)
        {
            string html = "";
            string channelName = "";
            try
            {
                var result = iDbContext.ServiceGuideVideos.Where(x => x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    var lks = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookUpName.ToUpper() == "LkSreviceGuideVideoTypes".ToUpper()).FirstOrDefault();

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.TitleEng : i.TitleAlt;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.VideoId + "' id='SortOrder" + i.VideoId + "' onchange=\"ChangeSortOrder(" + (int)Guide.Videos + "," + i.VideoId + ",'SortOrder" + i.VideoId + "');\">";
                        for (int k = minSort; k <= maxSort; k++)
                        {
                            if (i.SortOrder == k)
                                sortOrderControl += "<option value='" + k + "' selected>" + k + "</option>";
                            else
                                sortOrderControl += "<option value='" + k + "'>" + k + "</option>";
                        }
                        sortOrderControl += "</select>";

                        var lkop = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == lks.LookupId && x.Code == i.TypeId.ToString()).FirstOrDefault();
                        if (lkop != null)
                            channelName = lkop.DescriptionEng;

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";

                        //if (string.IsNullOrEmpty(channelName) == false)
                        //    html += "<div class='circle'>" + channelName.Substring(0, 1) + "</div>";

                        html += "<div class='circle'>" + i.SortOrder + "</div>";
                        //html += "<div class='tag'>{3}</div>";
                        //html += "<div class='title nameEn'>" + sortOrderControl + ". {1}</div>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_VIDEO") == true)
                        {
                            html += "<div class='title nameEn'>" + sortOrderControl + " {1}</div>";
                        }
                        else
                        {
                            html += "<div class='title nameEn'>{1}</div>";
                        }
                        html += "<div class='title nameAr'>{2}</div>";

                        html += "<div class='links'>";

                        html += "   <a title='{1}' class='link' href='" + i.UrlEng + "' target='_blank'><svg xmlns='http://www.w3.org/2000/svg' width='25' height='25' fill='currentColor' class='bi bi-youtube' viewBox='0 0 16 16'><path d='M8.051 1.999h.089c.822.003 4.987.033 6.11.335a2.01 2.01 0 0 1 1.415 1.42c.101.38.172.883.22 1.402l.01.104.022.26.008.104c.065.914.073 1.77.074 1.957v.075c-.001.194-.01 1.108-.082 2.06l-.008.105-.009.104c-.05.572-.124 1.14-.235 1.558a2.007 2.007 0 0 1-1.415 1.42c-1.16.312-5.569.334-6.18.335h-.142c-.309 0-1.587-.006-2.927-.052l-.17-.006-.087-.004-.171-.007-.171-.007c-1.11-.049-2.167-.128-2.654-.26a2.007 2.007 0 0 1-1.415-1.419c-.111-.417-.185-.986-.235-1.558L.09 9.82l-.008-.104A31.4 31.4 0 0 1 0 7.68v-.123c.002-.215.01-.958.064-1.778l.007-.103.003-.052.008-.104.022-.26.01-.104c.048-.519.119-1.023.22-1.402a2.007 2.007 0 0 1 1.415-1.42c.487-.13 1.544-.21 2.654-.26l.17-.007.172-.006.086-.003.171-.007A99.788 99.788 0 0 1 7.858 2h.193zM6.4 5.209v4.818l4.157-2.408L6.4 5.209z' /></svg></a>";
                        html += "   <a title='{2}' class='link' href='" + i.UrlAlt + "' target='_blank'><svg xmlns='http://www.w3.org/2000/svg' width='25' height='25' fill='currentColor' class='bi bi-youtube' viewBox='0 0 16 16'><path d='M8.051 1.999h.089c.822.003 4.987.033 6.11.335a2.01 2.01 0 0 1 1.415 1.42c.101.38.172.883.22 1.402l.01.104.022.26.008.104c.065.914.073 1.77.074 1.957v.075c-.001.194-.01 1.108-.082 2.06l-.008.105-.009.104c-.05.572-.124 1.14-.235 1.558a2.007 2.007 0 0 1-1.415 1.42c-1.16.312-5.569.334-6.18.335h-.142c-.309 0-1.587-.006-2.927-.052l-.17-.006-.087-.004-.171-.007-.171-.007c-1.11-.049-2.167-.128-2.654-.26a2.007 2.007 0 0 1-1.415-1.419c-.111-.417-.185-.986-.235-1.558L.09 9.82l-.008-.104A31.4 31.4 0 0 1 0 7.68v-.123c.002-.215.01-.958.064-1.778l.007-.103.003-.052.008-.104.022-.26.01-.104c.048-.519.119-1.023.22-1.402a2.007 2.007 0 0 1 1.415-1.42c.487-.13 1.544-.21 2.654-.26l.17-.007.172-.006.086-.003.171-.007A99.788 99.788 0 0 1 7.858 2h.193zM6.4 5.209v4.818l4.157-2.408L6.4 5.209z' /></svg></a>";

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_VIDEO") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_DELETE_VIDEO") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','"+ name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.VideoId, i.TitleEng, i.TitleAlt, channelName);
                    }
                }
                else
                {
                    html += @"<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>
                                    <div class='card'>
                                        <div class='tag'>{0}</div>
                                        <div class='title'>{1}</div>
                                    </div>
                                </div>";
                    html = string.Format(html, DbManager.GetText("Lookup", "lblSorry", "Sorry !"), DbManager.GetText("Lookup", "lblSorryNoRecords", "No Records Found !"));
                }

                var serializer = Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer;
            }
            catch (Exception Exp)
            {
                return Json(new { success = false, result = Exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveVideos(Models.ServiceGuideVideo j)
        {
            DbContextTransaction iTransaction = null;
            string mode = "ADDED";
            if (j.VideoId > 0)
                mode = "UPDATED";
            try
            {
                if (mode == "UPDATED")
                {
                    if (j.TypeId != 4)
                    {
                        var a = iDbContext.ServiceGuideVideos.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId && x.VideoId != j.VideoId).FirstOrDefault();
                        if (a != null)
                            return Json(new { Result = false, Message = DbManager.GetText("ServiceGuide", "lblVideoExists", "This video type already exists."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                    }
                    j.RowUpdateDate = DateTime.Now;
                    j.RowUpdatedBy = Security.GetUser().UserName;
                }
                else
                {
                    if (j.TypeId != 4)
                    {
                        var a = iDbContext.ServiceGuideVideos.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault();
                        if (a != null)
                            return Json(new { Result = false, Message = DbManager.GetText("ServiceGuide", "lblVideoExists", "This video type already exists."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                    }
                    j.RowInsertDate = DateTime.Now;
                    j.RowInsertedBy = Security.GetUser().UserName;
                }

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    j.IsDeleted = false;
                    j.IpAddress = Security.GetIpAddress();
                    iDbContext.ServiceGuideVideos.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.CategoryId = null;
                        aut.UniqueId = j.VideoId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " " + mode + " SERVICE GUIDE Videos  = " + j.VideoId + "-" + j.TitleEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblVidoSave", "Service Guide Videos Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetVideos(int recordId)
        {
            try
            {
                var iData = iDbContext.ServiceGuideVideos.Where(x => x.VideoId == recordId).FirstOrDefault();

                return Json(iData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//

        [HttpPost]
        public JsonResult DeleteVideos(int recordId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceGuideVideos.SingleOrDefault(x => x.VideoId == recordId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideVideos.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.UniqueId = recordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE GUIDE Videos = " + recordId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblVideoDelete", "Service Guide Video Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region -->  Service Guide Faq

        [UrlDecode]
        public ActionResult FAQ()
        {
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServiceGuideClientAlerts");
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View();
        }

        [HttpPost]
        public JsonResult GetFaqList(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetServiceGuideFaqs(iSearch, out totalRows);

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.QuestionEng
                        ,c.QuestionAlt
                        ,EditIcon((int)Guide.Faq, c)
                        ,DeleteIcon((int)Guide.Faq, c)
                    }
                ).ToList();
                return Json(new
                {
                    draw = iSearch.draw,
                    recordsTotal = totalRows,
                    recordsFiltered = totalRows,
                    data = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetFaqList", E.ToString());
                return Json(new { result = false, Error = E.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFaqCards(Models.BaseRequest baseRequest)
        {
            string html = "";
            try
            {
                var result = iDbContext.ServiceGuideFaqs.Where(x => x.IsDeleted == false).OrderBy(x => x.SortOrder).ToList();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.QuestionEng : i.QuestionAlt;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.ID + "' id='SortOrder" + i.ID + "' onchange=\"ChangeSortOrder(" + (int)Guide.Faq + "," + i.ID + ",'SortOrder" + i.ID + "');\">";
                        for (int k = minSort; k <= maxSort; k++)
                        {
                            if (i.SortOrder == k)
                                sortOrderControl += "<option value='" + k + "' selected>" + k + "</option>";
                            else
                                sortOrderControl += "<option value='" + k + "'>" + k + "</option>";
                        }
                        sortOrderControl += "</select>";

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>{3}</div>";
                        //html += "<div class='title nameEn'>" + sortOrderControl + ". {1}</div>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_FAQ") == true)
                        {
                            html += "<div class='title nameEn'>" + sortOrderControl + " {1}</div>";
                        }
                        else
                        {
                            html += "<div class='title nameEn'>{1}</div>";
                        }
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_UPDATE_FAQ") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_GUIDE_DELETE_FAQ") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','"+ name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.ID, i.QuestionEng, i.QuestionAlt, i.SortOrder);
                    }
                }
                else
                {
                    html += @"<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>
                                    <div class='card'>
                                        <div class='tag'>{0}</div>
                                        <div class='title'>{1}</div>
                                    </div>
                                </div>";
                    html = string.Format(html, DbManager.GetText("Lookup", "lblSorry", "Sorry !"), DbManager.GetText("Lookup", "lblSorryNoRecords", "No Records Found !"));
                }

                var serializer = Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
                serializer.MaxJsonLength = Int32.MaxValue;
                return serializer;
            }
            catch (Exception Exp)
            {
                return Json(new { success = false, result = Exp.ToString() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, result = html }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveFaq(Models.ServiceGuideFaq j)
        {
            DbContextTransaction iTransaction = null;
            string mode = "ADDED";
            if (j.ID > 0)
                mode = "UPDATED";
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    if (mode == "UPDATED")
                    {
                        j.RowUpdateDate = DateTime.Now;
                        j.RowUpdatedBy = Security.GetUser().UserName;
                    }
                    else
                    {
                        j.RowInsertDate = DateTime.Now;
                        j.RowInsertedBy = Security.GetUser().UserName;
                    }
                    j.IsDeleted = false;
                    j.IpAddress = Security.GetIpAddress();
                    iDbContext.ServiceGuideFaqs.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.CategoryId = null;
                        aut.UniqueId = j.ID.ToString();
                        aut.Remarks = Security.GetUser().UserName + " " + mode + " SERVICE GUIDE Faq  = " + j.ID + "-" + j.QuestionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblFaqSave", "Service Guide Faq Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetFaq(int recordId)
        {
            try
            {
                var iData = iDbContext.ServiceGuideFaqs.Where(x => x.ID == recordId).FirstOrDefault();

                return Json(iData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//

        [HttpPost]
        public JsonResult DeleteFaq(int recordId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceGuideFaqs.SingleOrDefault(x => x.ID == recordId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideFaqs.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 8;
                        aut.ServiceId = null;
                        aut.UniqueId = recordId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE GUIDE Faq = " + recordId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblFaqDelete", "Service Guide Faq Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> Publish Service Guide Content

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PublishContent()
        {
            string access_token = string.Empty;
            string dataAccessURL = string.Empty;
            RestClient restClient = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                if (WebConfig.GetStringValue("Use_TokenType") == "ACCESS_TOKEN")
                {
                    access_token = Security.GetBalTokenNew().access_token;
                }
                else
                {
                    access_token = Security.GetBalToken().access_token;
                }

                dataAccessURL = WebConfig.GetStringValue("BalApiUrl");
                restClient = new RestClient(dataAccessURL);
                var Request1 = new RestRequest("api/ServiceGuide/ComputingGuide", Method.GET);
                if (WebConfig.GetStringValue("Use_TokenType") == "ACCESS_TOKEN")
                {
                    Request1.AddHeader("access_token", access_token);
                }
                else
                {
                    Request1.AddHeader("Authorization", "Bearer " + access_token);
                }
                var Response1 = restClient.Execute(Request1);

                var Content = Response1.Content;

                System.Web.HttpContext.Current.Trace.Warn("Computing Response = ", Content);

                if (!Content.Contains("Success"))
                {
                    return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblPublishGuideFail", "Service Guide Content Published Failed.! ") + Content, Icon = "error" }, JsonRequestBehavior.AllowGet);
                }

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 8;
                    aut.ServiceId = null;
                    aut.CategoryId = null;
                    aut.UniqueId = "PublishContent";
                    aut.Remarks = Security.GetUser().UserName + " Published Service Guide Content.";
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
                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblPulishGuide", "Service Guide Content Published Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }//

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PublishServiceCardsContent()
        {
            string access_token = string.Empty;
            string dataAccessURL = string.Empty;
            RestClient restClient = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                if (WebConfig.GetStringValue("Use_TokenType") == "ACCESS_TOKEN")
                {
                    access_token = Security.GetBalTokenNew().access_token;
                }
                else
                {
                    access_token = Security.GetBalToken().access_token;
                }

                dataAccessURL = WebConfig.GetStringValue("BalApiUrl");
                restClient = new RestClient(dataAccessURL);
                var Request1 = new RestRequest("api/ServiceGuide/ComputingAris", Method.GET);
                if (WebConfig.GetStringValue("Use_TokenType") == "ACCESS_TOKEN")
                {
                    Request1.AddHeader("access_token", access_token);
                }
                else
                {
                    Request1.AddHeader("Authorization", "Bearer " + access_token);
                }
                var Response1 = restClient.Execute(Request1);

                var Content = Response1.Content;

                System.Web.HttpContext.Current.Trace.Warn("Computing Response = ", Content);

                if (!Content.Contains("Success"))
                {
                    return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblPublishCardFail", "Service Guide Content Published Failed.!) ") + Content, Icon = "error" }, JsonRequestBehavior.AllowGet);
                }

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 8;
                    aut.ServiceId = null;
                    aut.CategoryId = null;
                    aut.UniqueId = "PublishServiceCardsContent";
                    aut.Remarks = Security.GetUser().UserName + " Clear & Cache Service Cards Content.";
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
                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblPublishCard", "Service Guide Content Published Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }//

        #endregion

        private string EditIcon(int categoryId, object iObject)
        {
            string html = "";

            switch (categoryId)
            {
                case (int)Guide.Procedures:

                    var a = (Models.ServiceGuideProcedure)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Edit' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"EditProcdure('" + a.RecordId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                    }

                    break;

                case (int)Guide.Channels:

                    var b = (Models.ServiceGuideChannel)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Edit' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"EditChannel('" + b.RecordId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                    }

                    break;

                case (int)Guide.Faq:

                    var c = (Models.ServiceGuideFaq)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Edit' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"EditFaq('" + c.ID + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                    }

                    break;

                case (int)Guide.Support:

                    var d = (Models.ServiceGuideSupport)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Edit' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"EditSupport('" + d.RecordId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                    }

                    break;

                case (int)Guide.Videos:

                    var e = (Models.ServiceGuideVideo)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Edit' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"EditVideo('" + e.VideoId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                    }

                    break;
            }
            return html;
        }
        private string DeleteIcon(int categoryId, object iObject)
        {
            string html = "";

            switch (categoryId)
            {
                case (int)Guide.Procedures:

                    var a = (Models.ServiceGuideProcedure)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Delete' src=\"" + Url.Content("~/Content/images/delete_icon.gif") + "\" style='cursor:pointer;' onclick=\"DeleteProcdure('" + a.RecordId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                    }

                    break;

                case (int)Guide.Channels:

                    var b = (Models.ServiceGuideChannel)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Delete' src=\"" + Url.Content("~/Content/images/delete_icon.gif") + "\" style='cursor:pointer;' onclick=\"DeleteChannel('" + b.RecordId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                    }

                    break;

                case (int)Guide.Faq:

                    var c = (Models.ServiceGuideFaq)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Delete' src=\"" + Url.Content("~/Content/images/delete_icon.gif") + "\" style='cursor:pointer;' onclick=\"DeleteFaq('" + c.ID + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                    }

                    break;

                case (int)Guide.Support:

                    var d = (Models.ServiceGuideSupport)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Delete' src=\"" + Url.Content("~/Content/images/delete_icon.gif") + "\" style='cursor:pointer;' onclick=\"DeleteSupport('" + d.RecordId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                    }

                    break;

                case (int)Guide.Videos:

                    var e = (Models.ServiceGuideVideo)iObject;

                    if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
                    {
                        html += "<img title='Click to Delete' src=\"" + Url.Content("~/Content/images/delete_icon.gif") + "\" style='cursor:pointer;' onclick=\"DeleteVideo('" + e.VideoId + "')\" />";
                    }
                    else
                    {
                        html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                    }

                    break;
            }
            return html;
        }

        [HttpPost]
        public JsonResult ChangeSortOrder(int categoryId, int recordId, int sortOrder)
        {
            DbContextTransaction iTransaction = null;
            string categoryName = "";
            try
            {
                categoryName = Enum.GetName(typeof(Guide), categoryId);

                switch (categoryId)
                {
                    case (int)Guide.Procedures:

                        var a = iDbContext.ServiceGuideProcedures.SingleOrDefault(x => x.RecordId == recordId);
                        a.SortOrder = sortOrder;
                        a.RowUpdateDate = DateTime.Now;
                        a.IpAddress = Common.Security.GetIpAddress();
                        a.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideProcedures.AddOrUpdate(a);
                        iDbContext.SaveChanges();

                        break;

                    case (int)Guide.Channels:

                        var b = iDbContext.ServiceGuideChannels.SingleOrDefault(x => x.RecordId == recordId);
                        b.SortOrder = sortOrder;
                        b.RowUpdateDate = DateTime.Now;
                        b.IpAddress = Common.Security.GetIpAddress();
                        b.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideChannels.AddOrUpdate(b);
                        iDbContext.SaveChanges();

                        break;

                    case (int)Guide.Faq:

                        var c = iDbContext.ServiceGuideFaqs.SingleOrDefault(x => x.ID == recordId);
                        c.SortOrder = sortOrder;
                        c.RowUpdateDate = DateTime.Now;
                        c.IpAddress = Common.Security.GetIpAddress();
                        c.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideFaqs.AddOrUpdate(c);
                        iDbContext.SaveChanges();

                        break;

                    case (int)Guide.Support:

                        var d = iDbContext.ServiceGuideSupports.SingleOrDefault(x => x.RecordId == recordId);
                        d.SortOrder = sortOrder;
                        d.RowUpdateDate = DateTime.Now;
                        d.IpAddress = Common.Security.GetIpAddress();
                        d.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideSupports.AddOrUpdate(d);
                        iDbContext.SaveChanges();

                        break;

                    case (int)Guide.Videos:

                        var e = iDbContext.ServiceGuideVideos.SingleOrDefault(x => x.VideoId == recordId);
                        e.SortOrder = sortOrder;
                        e.RowUpdateDate = DateTime.Now;
                        e.IpAddress = Common.Security.GetIpAddress();
                        e.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceGuideVideos.AddOrUpdate(e);
                        iDbContext.SaveChanges();

                        break;

                    case (int)Guide.Beneficiaries:

                        var f = iDbContext.ServiceEntities.SingleOrDefault(x => x.EntityId == recordId);
                        f.SortOrder = sortOrder;
                        f.RowUpdateDate = DateTime.Now;
                        f.IpAddress = Common.Security.GetIpAddress();
                        f.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceEntities.AddOrUpdate(f);
                        iDbContext.SaveChanges();

                        break;

                    case (int)Guide.Types:

                        var g = iDbContext.ServiceTypes.SingleOrDefault(x => x.TypeId == recordId);
                        g.SortOrder = sortOrder;
                        g.RowUpdateDate = DateTime.Now;
                        g.IpAddress = Common.Security.GetIpAddress();
                        g.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceTypes.AddOrUpdate(g);
                        iDbContext.SaveChanges();

                        break;
                }

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 8;
                    aut.ServiceId = null;
                    aut.CategoryId = null;
                    aut.UniqueId = recordId.ToString();
                    aut.Remarks = Security.GetUser().UserName + " CHANGED THE SORT ORDER OF SERVICE GUIDE " + categoryName + "-" + recordId;
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
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("ServiceGuide", "lblChangeSort", "SortOrder Updated Successfully.!"), Icon = "success", ServiceId = "" }, JsonRequestBehavior.AllowGet);
        }

        [UrlDecode]
        public ActionResult Beneficiaries()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts,ServiceGuideClientAlerts");
            return View();
        }
        [UrlDecode]
        public ActionResult Types()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts,ServiceGuideClientAlerts");
            return View();
        }
    }
}