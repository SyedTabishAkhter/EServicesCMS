using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;
using EServicesCms.Common;


namespace EServicesCms.Controllers
{
    [SessionTimeout]
    public class LookupController : BaseController
    {
        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,LookupClientAlerts");
            return View();
        }

        [UrlDecode]
        [HttpPost]
        public JsonResult AjaxGetLkLookup()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetLkLookup();
                if (result != null && result.Count > 0)
                {
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>{0}</div>";
                        html += "<div class='title'>{1}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_LOOKUP") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}');\" class='link'>"+ DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_LOOKUP") == true)
                        {
                            html += "<a href='#' onclick=\"Delete('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_VIEW_LOOKUPLIST") == true)
                        {
                            string url = Common.Helper.URL_Encode(Url.Action("List", "Lookup", null, Request.Url.Scheme, null) + "?id=" + i.LookupId);
                            html += "<a href='"+ url + "'  class='link'>" + DbManager.GetText("Lookup", "lblViewList", "View List") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblViewList", "View List") + "</a>";
                        }
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.LookupId, i.LookUpName);
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
        public JsonResult AjaxSaveLookup(Models.LkLookup iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.LkLookups.SingleOrDefault(x => x.LookupId == iObject.LookupId);
                if (iRecord != null)
                {
                    var b = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookupId != iRecord.LookupId && x.LookUpName.ToUpper() == iObject.LookUpName.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.LookUpName + " "+ DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.LookupId = iObject.LookupId;
                    iRecord.LookUpName = iObject.LookUpName;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.LkLookups.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " updated lookup record = " + iRecord.LookUpName;
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
                    var b = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookUpName.ToUpper() == iObject.LookUpName.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.LookUpName + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.LkLookup();

                    var i = iDbContext.LkLookups.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        Code = i[i.Count - 1].LookupId + 1;

                    iRecord.LookupId = Code;
                    iRecord.LookUpName = iObject.LookUpName;
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Common.Security.GetUser().UserName;
                    iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iDbContext.LkLookups.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " added lookup record = " + iRecord.LookUpName;
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
                return Json(new { Result = false, Message = E.Message, Icon="error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblSaveSuccess", "Lookup Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteLookup(Models.LkLookup iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.LkLookups.SingleOrDefault(x => x.LookupId == iObject.LookupId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iDbContext.LkLookups.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.ExternalLookupId == iRecord.LookUpName).ToList();
                    if (i != null && i.Count > 0)
                    {
                        foreach (var j in i)
                        {
                            j.ExternalLookupId = null;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Common.Security.GetUser().UserName;
                            iDbContext.ServiceFormsConfigs.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " deleted lookup record = " + iRecord.LookUpName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblDeleteSuccess", "Lookup Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [UrlDecode]
        public ActionResult UserAttributes()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,LookupClientAlerts");
            return View();
        }
        [HttpPost]
        public JsonResult AjaxGetLkUserAttributesSSO()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetLkUserAttributesSSO();
                if (result != null && result.Count > 0)
                {
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>{0}</div>";
                        html += "<div class='title'>{1}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_ATTRIBUTE") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_ATTRIBUTE") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteAttribute('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.AttributeId, i.DescriptionEng);
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
        public JsonResult AjaxSaveLkUserAttributesSSO(Models.LkUserAttributesSSO iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();
                var iRecord = iDbContext.LkUserAttributesSSOes.SingleOrDefault(x => x.AttributeId == iObject.AttributeId);
                if (iRecord != null)
                {
                    var b = iDbContext.LkUserAttributesSSOes.Where(x => x.IsDeleted == false && x.AttributeId != iRecord.AttributeId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.AttributeId = iObject.AttributeId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.LkUserAttributesSSOes.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " updated sso attribute record = " + iRecord.DescriptionEng;
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
                    var b = iDbContext.LkUserAttributesSSOes.Where(x => x.IsDeleted == false && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.LkUserAttributesSSO();

                    var i = iDbContext.LkUserAttributesSSOes.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        Code = i[i.Count - 1].AttributeId + 1;

                    iRecord.AttributeId = Code;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.IsDeleted = false;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iDbContext.LkUserAttributesSSOes.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " added sso attribute record = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblAttrSave", "Attribute Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteLkUserAttributesSSO(Models.LkUserAttributesSSO iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.LkUserAttributesSSOes.SingleOrDefault(x => x.AttributeId == iObject.AttributeId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iDbContext.LkUserAttributesSSOes.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.UserField == iRecord.DescriptionEng).ToList();
                    if (i != null && i.Count > 0)
                    {
                        foreach (var j in i)
                        {
                            j.UserField = null;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Common.Security.GetUser().UserName;
                            iDbContext.ServiceFormsConfigs.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " deleted lookup record = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblAttrDelete", "Attribute Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [UrlDecode]
        public ActionResult List(int id)
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,LookupClientAlerts");
            var i = Common.DbManager.GetLkLookup().Where(x => x.LookupId == id).FirstOrDefault().LookUpName;
            ViewBag.LookupId = id;
            ViewBag.LookupName = i;
            return View();
        }

        [UrlDecode]
        [HttpPost]
        public JsonResult AjaxGetLkLookupIdList(int id)
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetLkLookupIdList(id);
                if (result != null && result.Count > 0)
                {
                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle' title='VAL-{1}'>{1}</div>";
                        html += "<div class='title'>{2}</div>";
                        html += "<div class='title'>{3}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_LOOKUPLIST") == true)
                        {
                            html += "<a href='#' onclick=\"ModifyLookupList('{0}','{1}','{2}','{3}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_LOOKUPLIST") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteLookupList('{0}','{1}','"+ name + "','" + name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}');\" class='link diabled'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.OptionId, i.Code, i.DescriptionAlt, i.DescriptionEng);
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
        public JsonResult AjaxSaveLookupItem(Models.LookupOption iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.LookupOptions.SingleOrDefault(x => x.OptionId == iObject.OptionId);
                if (iRecord != null)
                {
                    //var b = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId != iRecord.LookupId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault();
                    //if (b != null)
                    //    return Json(new { Result = false, Message = iObject.DescriptionEng + " Already Exists.", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    //iRecord.OptionId = iObject.OptionId;
                    //iRecord.Code = iObject.Code;
                    //iRecord.LookupId = iObject.LookupId;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.IsDeleted = false;
                    //iRecord.IpAddress = Common.Security.GetIpAddress();
                    //iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.LookupOptions.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " updated lookup item record = " + iRecord.LookupId +"=" + iRecord.DescriptionEng;
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
                    var b = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == iObject.LookupId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.LookupOption();

                    var i = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == iObject.LookupId).OrderByDescending(x=>x.LookupId).ToList();
                    if (i != null && i.Count > 0)
                        Code = Convert.ToInt32(i[i.Count - 1].Code) + 1;

                    iRecord.LookupId = iObject.LookupId;
                    iRecord.Code = Code.ToString();
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    //iRecord.IsActive = true;
                    iRecord.RowInsertDate = DateTime.Now;
                    //iRecord.IpAddress = Common.Security.GetIpAddress();
                    //iRecord.RowInsertedBy = Common.Security.GetUser().UserName;
                    if (i != null && i.Count > 0 && i[i.Count - 1].SortOrder.HasValue)
                        iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    else
                        iRecord.SortOrder =1;
                    iDbContext.LookupOptions.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " added lookup item record = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblItemSaveSucess", "Lookup Item Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteLookupItem(Models.LookupOption iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.LookupOptions.SingleOrDefault(x => x.OptionId == iObject.OptionId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    //iRecord.IpAddress = Common.Security.GetIpAddress();
                    //iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iDbContext.LookupOptions.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " deleted lookup item record = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblItemDelSucess", "Lookup Item Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [UrlDecode]
        public ActionResult Sources()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,LookupClientAlerts");
            return View();
        }

        [UrlDecode]
        [HttpPost]
        public JsonResult AjaxGetApiSources()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetApiSources();
                if (result != null && result.Count > 0)
                {
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='tag'>ID - {0}</div>";
                        html += "<div class='title'>{1}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_API_UPDATE") == true)
                        {
                            html += "<a href='#' onclick=\"ModifyApiSource('{0}','{1}','{2}','{3}','{4}','{5}');\" class='link'>Edit</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link diabled'>Edit</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_API_DELETE") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteApiSource('{0}','{1}');\" class='link'>Delete</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}');\" class='link diabled'>Delete</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.ApiSourceId, i.ApiSourceName, i.ApiAccessURL, i.ApiKey, i.ApiUsername, i.ApiPassword);
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
        public JsonResult AjaxSaveApiSources(Models.API_Sources iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.API_Sources.SingleOrDefault(x => x.ApiSourceId == iObject.ApiSourceId);
                if (iRecord != null)
                {
                    var b = iDbContext.API_Sources.Where(x => x.IsDeleted == false && x.ApiSourceId != iRecord.ApiSourceId && x.ApiSourceName.ToUpper() == iObject.ApiSourceName.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.ApiSourceName + " Already Exists.", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.ApiSourceId = iObject.ApiSourceId;
                    iRecord.ApiAccessURL = iObject.ApiAccessURL;
                    iRecord.ApiKey = iObject.ApiKey;
                    iRecord.ApiPassword = iObject.ApiPassword;
                    iRecord.ApiSourceName = iObject.ApiSourceName;
                    iRecord.ApiUsername = iObject.ApiUsername;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.API_Sources.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " updated API_Sources record = " + iRecord.ApiSourceName;
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
                    var b = iDbContext.API_Sources.Where(x => x.IsDeleted == false && x.ApiSourceName.ToUpper() == iObject.ApiSourceName.ToUpper()).FirstOrDefault();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.ApiSourceName + " Already Exists.", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.API_Sources();

                    iRecord.ApiAccessURL = iObject.ApiAccessURL;
                    iRecord.ApiKey = iObject.ApiKey;
                    iRecord.ApiPassword = iObject.ApiPassword;
                    iRecord.ApiSourceName = iObject.ApiSourceName;
                    iRecord.ApiUsername = iObject.ApiUsername;
                    iRecord.IsDeleted = false;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Common.Security.GetUser().UserName;
                    iDbContext.API_Sources.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " added API_Sources record = " + iRecord.ApiSourceName;
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
            return Json(new { Result = true, Message = "Api Source Saved Successfully.!", Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteApiSource(Models.API_Sources iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.API_Sources.SingleOrDefault(x => x.ApiSourceId == iObject.ApiSourceId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iDbContext.API_Sources.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.Services.Where(x => x.IsDeleted == false && x.ApiSourceId == iRecord.ApiSourceId).ToList();
                    if (i != null && i.Count > 0)
                    {
                        foreach (var j in i)
                        {
                            j.ApiSourceId = null;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Common.Security.GetUser().UserName;
                            iDbContext.Services.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Common.Security.GetUser().UserName + " deleted API_Sources record = " + iRecord.ApiSourceName;
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
            return Json(new { Result = true, Message = "Api Source Deleted Successfully.!", Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [UrlDecode]
        public ActionResult Labels()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,LookupClientAlerts");
            
            var viewIds = iDbContext.LkScreenLabels.Select(x => x.ViewId).Distinct();
            ViewBag.Views = new SelectList(viewIds.ToArray());

            return View();
        }
        [HttpPost]
        public JsonResult AjaxGetLabels(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = iDbContext.ES_GetLabels(pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.ViewId).ToList();
                if (iData != null && iData.Count > 0)
                    totalRows = iData[0].TotalRows.Value;

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.ViewId
                        //,c.LabelId
                        ,c.DescriptionEng
                        ,c.DescriptionAlt
                        ,GetLabelEditIcon(c)
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
                System.Web.HttpContext.Current.Trace.Warn("GetUsers", E.ToString());
                throw E;
            }
            return Json(new { result = false, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        private string GetLabelEditIcon(Models.ES_GetLabels_Result c)
        {
            string html = "";
                
            html += "<img title='Click to Edit Label' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"ModifyLabel('" + c.AbbrId + "','" + c.ViewId + "','" + c.LabelId + "','" + c.DescriptionEng + "','" + c.DescriptionAlt + "')\" />";
            
            return html;
        }

        [HttpPost]
        public JsonResult AjaxSaveLabel(Models.LkScreenLabel iObject)
        {
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.LkScreenLabels.SingleOrDefault(x => x.AbbrId == iObject.AbbrId);
                if (iRecord != null)
                {
                    iRecord.AbbrId = iObject.AbbrId;
                    iRecord.ViewId = iObject.ViewId;
                    iRecord.LabelId = iObject.LabelId;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.LkScreenLabels.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();
                    System.Web.HttpContext.Current.Session["AppCache"] = new EServicesCms.Common.CacheManager();
                    //System.Web.HttpContext.Current.Application["AppCache"] = new EServicesCms.Common.CacheManager();
                }
            }
            catch (Exception E)
            {
                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Lookup", "lblLabelSaveSuccess", " Label Updated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }
    }
}