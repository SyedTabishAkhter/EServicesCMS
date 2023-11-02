using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;
using EServicesCms.Common;
using System.Data;
using System.IO;

namespace EServicesCms.Controllers
{
    [SessionTimeout]
    public class ServicesController : BaseController
    {
        #region --> Services

        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

            var Entities = Common.DbManager.GetServiceEntites();
            if (Helper.CurrentLanguage() == (int)Language.English)
                ViewBag.Entities = new SelectList(Entities.ToArray(), "EntityId", "DescriptionEng");
            else
                ViewBag.Entities = new SelectList(Entities.ToArray(), "EntityId", "DescriptionAlt");

            var Types = Common.DbManager.GetServiceTypes();
            if (Helper.CurrentLanguage() == (int)Language.English)
                ViewBag.Types = new SelectList(Types.ToArray(), "TypeId", "DescriptionEng");
            else
                ViewBag.Types = new SelectList(Types.ToArray(), "TypeId", "DescriptionAlt");

            if (Security.isUserSystemAdministrator() == false)
            {
                int? userDepId = Security.GetUser().DepartmentId;
                var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false && x.DepartmentId == userDepId).ToList();
                if (Helper.CurrentLanguage() == (int)Language.English)
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                else
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
            }
            else
            {
                var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
                if (Helper.CurrentLanguage() == (int)Language.English)
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                else
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
            }

            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetServices(Models.SearchParams iSearch)
        {
            string html = "";
            string icon = "";
            var ix = new System.Web.Mvc.UrlHelper(Request.RequestContext);
            try
            {
                iSearch = iSearch.TrimObject();
                //
                if (Security.isUserSystemAdministrator() == false)
                {
                    iSearch.DepartmentId = Common.Security.GetUser().DepartmentId;
                }
                //
                var result = Common.DbManager.GetServices(iSearch);
                if (result != null && result.Count > 0)
                {
                    int nctr = 0;
                    foreach (var i in result)
                    {
                        string servName = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;

                        icon = "";
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        if (i.IsAnonymous == true)
                            icon = "<img border='0' width='20' height='15' src='" + ix.Content("~/Content/images/IsAnonymous.png") + "' align='absmiddle' alt='Anonymous' title='Anonymous' />";

                        html += "<div class='card'>";
                        html += "<div class='circle'>" + (nctr + 1) + "</div>";
                        html += "<div class='tag'>" + icon + "  {1}</div>";
                        //html += "<div class='ArText'>{0}</div>";
                        html += "<div class='title nameEn'>{2}</div>";
                        html += "<div class='title nameAr'>{5}</div>";
                        html += "<div class='links'>";
                        html += "<a href='{3}' class='link'>" + DbManager.GetText("Lookup", "lblDetails", "Details") + "</a>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_SERVICE") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteService('" + i.ServiceId + "','" + servName + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{4}','" + servName + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ACTDACT_SERVICE") == true)
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' onclick=\"ActivateService('"+ i.ServiceId + "','" + servName + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' onclick=\"DeActivateService('" + i.ServiceId + "','" + servName + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "DeActivate") + "</a>";
                        }
                        else
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('" + i.ServiceId + "','" + servName + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('" + i.ServiceId + "','" + servName + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "DeActivate") + "</a>";
                        }
                        html += "</div>";
                        html += "<div class='ArText' style='color:maroon;'>{7} {6}</div>";
                        html += "<div class='ArText'>{0}</div>";
                        html += "</div>";
                        html += "</div>";

                        string url = Common.Helper.URL_Encode(Url.Action("ServiceDetails", "Services", null, Request.Url.Scheme, null) + "?serviceId=" + i.ServiceId);

                        string deptName = "";
                        if (i.DepartmentId > 0)
                        {
                            if (EServicesCms.Common.Helper.CurrentLanguage() == (int)Language.English)
                            {
                                deptName = "( " + iDbContext.Departments.Where(x => x.DepartmentId == i.DepartmentId).FirstOrDefault().DepartmentName + " )";
                            }
                            else
                            {
                                deptName = "( " + iDbContext.Departments.Where(x => x.DepartmentId == i.DepartmentId).FirstOrDefault().DepartmentNameAlt + " )";
                            }
                        }

                        string type = "";
                        if (i.TypeId > 0)
                        {
                            if (EServicesCms.Common.Helper.CurrentLanguage() == (int)Language.English)
                            {
                                type = iDbContext.ServiceTypes.Where(x => x.TypeId == i.TypeId).FirstOrDefault().DescriptionEng;
                            }
                            else
                            {
                                type = iDbContext.ServiceTypes.Where(x => x.TypeId == i.TypeId).FirstOrDefault().DescriptionAlt;
                            }
                        }
                        if (EServicesCms.Common.Helper.CurrentLanguage() == (int)Language.English)
                        {
                            html = string.Format(html, i.EntityNameEng.Replace("|", " | "), i.ExternalServiceID, i.DescriptionEng, url, i.ServiceId, i.DescriptionAlt, deptName, type);
                        }
                        else
                        {
                            html = string.Format(html, i.EntityNameAlt.Replace("|", " | "), i.ExternalServiceID, i.DescriptionEng, url, i.ServiceId, i.DescriptionAlt, deptName, type);
                        }
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

        [HttpPost]
        public JsonResult AjaxDeleteService(int serviceId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceCategories.SingleOrDefault(x => x.ServiceId == serviceId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceCategories.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }
                    
                    var i = iDbContext.ServiceFormsConfigs.Where(x => x.ServiceId == serviceId).ToList();
                    if (i != null)
                    {
                        foreach (var j in i)
                        {
                            j.IsDeleted = true;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Security.GetUser().UserName;
                            iDbContext.ServiceFormsConfigs.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }

                    var k = iDbContext.ServiceEntitiesMappings.Where(x => x.ServiceId == serviceId).ToList();
                    if (k != null)
                    {
                        foreach (var j in k)
                        {
                            j.IsDeleted = true;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Security.GetUser().UserName;
                            iDbContext.ServiceEntitiesMappings.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }

                    var ja = iDbContext.Services.SingleOrDefault(x => x.ServiceId == serviceId);
                    ja.IsDeleted = true;
                    ja.RowUpdateDate = DateTime.Now;
                    ja.IpAddress = Common.Security.GetIpAddress();
                    ja.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Services.AddOrUpdate(ja);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = serviceId;
                        aut.UniqueId = ja.ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE RECORD = " + ja.ExternalServiceID;
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

                return Json(new { Result = false, Message = E.Message, Icon= "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelService", "Service Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxActivateService(int serviceId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Services.SingleOrDefault(x => x.ServiceId == serviceId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Services.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = serviceId;
                        aut.UniqueId = iRecord.ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ACTIVATED SERVICE RECORD = " + iRecord.ExternalServiceID;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblActivateService", "Service Activated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeActivateService(int serviceId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Services.SingleOrDefault(x => x.ServiceId == serviceId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Services.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = serviceId;
                        aut.UniqueId = iRecord.ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DE-ACTIVATED SERVICE RECORD = " + iRecord.ExternalServiceID;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDeActivateService", "Service DeActivated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ServiceDetails

        [UrlDecode]
        public ActionResult ServiceDetails(int serviceId)
        {
            Models.ServiceObject iObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                if (serviceId == 0)
                    ViewBag.FormMode = "New";
                else
                    ViewBag.FormMode = "Edit";

                iObject = Common.DbManager.GetService(serviceId);
                if (iObject == null)
                {
                    iObject = new Models.ServiceObject();
                    var i = iDbContext.Services.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        iObject.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iObject.CommentAttachments = new Models.scaConfig();
                    iObject.CommentAttachments.Minimum = 1;
                    iObject.CommentAttachments.Maximum = 3;
                    iObject.CommentAttachments.MaxSize = 2;
                }
                else
                {
                    iObject.CommentAttachments = new Models.scaConfig();
                    var X = iDbContext.ServiceCommentsAttachmentsConfigs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).FirstOrDefault();
                    if (X != null)
                    {
                        iObject.CommentAttachments.Minimum = X.Minimum;
                        iObject.CommentAttachments.Maximum = X.Maximum;
                        iObject.CommentAttachments.MaxSize = X.MaxSize;
                    }
                }

                ViewBag.HasSubServices = false;
                var iSubService = iDbContext.Services.Where(x => x.IsDeleted == false && x.ParentServiceId == serviceId).ToList();
                if (iSubService != null && iSubService.Count > 0)
                    ViewBag.HasSubServices = true;

                ViewBag.HasClassfication = false;
                var iServiceClassifcation = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (iServiceClassifcation != null && iServiceClassifcation.Count > 0)
                    ViewBag.HasClassfication = true;

                ViewBag.HasQuestions = false;
                var iServiceQuestions = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (iServiceClassifcation != null && iServiceClassifcation.Count > 0)
                    ViewBag.HasQuestions = true;

                ViewBag.ShowSubServicesMenu = true;
                ViewBag.ShowClassificationMenu = true;
                ViewBag.ShowQuestionsMenu = true;
                ViewBag.ShowFormBuilderMenu = true;

                if (iServiceQuestions != null && iServiceQuestions.Count > 0)
                {
                    if (iServiceClassifcation != null && iServiceClassifcation.Count > 0)
                        ViewBag.ShowClassificationMenu = true;
                    else
                        ViewBag.ShowClassificationMenu = false;
                
                    if (iSubService != null && iSubService.Count > 0)
                        ViewBag.ShowSubServicesMenu = true;
                    else
                        ViewBag.ShowSubServicesMenu = false;
                }
                else
                {
                    ViewBag.ShowQuestionsMenu = false;
                    ViewBag.ShowFormBuilderMenu = false;
                    if (iServiceClassifcation != null && iServiceClassifcation.Count > 0)
                    {
                        ViewBag.ShowClassificationMenu = true;
                        ViewBag.ShowQuestionsMenu = true;
                        ViewBag.ShowFormBuilderMenu = true;

                        if (iSubService != null && iSubService.Count > 0)
                            ViewBag.ShowSubServicesMenu = true;
                        else
                            ViewBag.ShowSubServicesMenu = false;
                    }
                    else
                    {
                        ViewBag.ShowClassificationMenu = true;
                        ViewBag.ShowSubServicesMenu = true;
                        ViewBag.ShowQuestionsMenu = true;
                        ViewBag.ShowFormBuilderMenu = true;
                    }
                }

                var Entities = Common.DbManager.GetServiceEntites();
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    if (iObject != null)
                        ViewBag.Entities = new SelectList(Entities.ToArray(), "EntityId", "DescriptionEng", iObject.EntityId);
                    else
                        ViewBag.Entities = new SelectList(Entities.ToArray(), "EntityId", "DescriptionEng");
                }
                else
                {
                    if (iObject != null)
                        ViewBag.Entities = new SelectList(Entities.ToArray(), "EntityId", "DescriptionAlt", iObject.EntityId);
                    else
                        ViewBag.Entities = new SelectList(Entities.ToArray(), "EntityId", "DescriptionAlt");
                }

                var Types = Common.DbManager.GetServiceTypes();
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    if (iObject != null)
                        ViewBag.Types = new SelectList(Types.ToArray(), "TypeId", "DescriptionEng", iObject.TypeId);
                    else
                        ViewBag.Types = new SelectList(Types.ToArray(), "TypeId", "DescriptionEng");
                }
                else
                {
                    if (iObject != null)
                        ViewBag.Types = new SelectList(Types.ToArray(), "TypeId", "DescriptionAlt", iObject.TypeId);
                    else
                        ViewBag.Types = new SelectList(Types.ToArray(), "TypeId", "DescriptionAlt");
                }

                var ParentServices = iDbContext.Services.Where(x => x.IsDeleted == false && x.ParentServiceId == 0).ToList();
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    if (iObject != null)
                        ViewBag.ParentServices = new SelectList(ParentServices.ToArray(), "ServiceId", "DescriptionEng", iObject.ParentServiceId);
                    else
                        ViewBag.ParentServices = new SelectList(ParentServices.ToArray(), "ServiceId", "DescriptionEng");
                }
                else
                {
                    if (iObject != null)
                        ViewBag.ParentServices = new SelectList(ParentServices.ToArray(), "ServiceId", "DescriptionAlt", iObject.ParentServiceId);
                    else
                        ViewBag.ParentServices = new SelectList(ParentServices.ToArray(), "ServiceId", "DescriptionAlt");
                }

                var ApiSources = iDbContext.API_Sources.Where(x => x.IsDeleted == false).ToList();
                if (iObject != null)
                    ViewBag.ApiSources = new SelectList(ApiSources.ToArray(), "ApiSourceId", "ApiSourceName", iObject.ApiSourceId);
                else
                    ViewBag.ApiSources = new SelectList(ApiSources.ToArray(), "ApiSourceId", "ApiSourceName");

                //var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
                //if (iObject != null)
                //    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName", iObject.DepartmentId);
                //else
                //    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");

                if (Security.isUserSystemAdministrator() == false)
                {
                    int? userDepId = Security.GetUser().DepartmentId;
                    var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false && x.DepartmentId == userDepId).ToList();
                    //ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
                    }
                }
                else
                {
                    var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
                    //ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
                    }
                }

                ViewBag.HasUserTypes = false;
                var HasUserTypes = iDbContext.ServiceUserTypes.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (HasUserTypes != null && HasUserTypes.Count > 0)
                    ViewBag.HasUserTypes = true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [UrlDecode]
        public ActionResult SubServices(int serviceId)
        {
            Models.ServiceObject iObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                iObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasClassfication = false;
                var iServiceClassifcation = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).FirstOrDefault();
                if (iServiceClassifcation != null)
                    ViewBag.HasClassfication = true;

                ViewBag.HasSubServices = false;
                var iSubService = iDbContext.Services.Where(x => x.IsDeleted == false && x.ParentServiceId == serviceId).ToList();
                if (iSubService != null && iSubService.Count > 0)
                    ViewBag.HasSubServices = true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [HttpPost]
        public JsonResult SaveServiceDetails(Models.ServiceObject iObject)
        {
            DbContextTransaction iTransaction = null;
            var aut = new Models.UserScreenAction();
            try
            {
                iObject = iObject.TrimObject();

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Services.SingleOrDefault(x => x.ServiceId == iObject.ServiceId);
                    if (iRecord == null)
                    {
                        iRecord = new Models.Service();

                        var a = iDbContext.Services.Where(x => x.IsDeleted == false && x.ExternalServiceID.ToUpper() == iObject.ExternalServiceID.ToUpper()).FirstOrDefault().TrimObject();
                        if (a != null)
                            return Json(new { Result = false, Message = iObject.ExternalServiceID + " " + DbManager.GetText("Lookup", "lblExistts", "External Service Id Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var b = iDbContext.Services.Where(x => x.IsDeleted == false && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var c = iDbContext.Services.Where(x => x.IsDeleted == false && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                        if (c != null)
                            return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var i = iDbContext.Services.Where(x => x.IsDeleted == false).ToList();
                        if (i != null && i.Count > 0)
                            iRecord.ServiceId = i[i.Count - 1].ServiceId + 1;

                        iRecord.ParentServiceId = 0;
                        iRecord.PrintPreview = iObject.PrintPreview;
                        iRecord.TabularLayout = iObject.TabularLayout;
                        iRecord.IsAnonymous = iObject.IsAnonymous;
                        iRecord.EntityId = iObject.EntityId;
                        iRecord.TypeId = iObject.TypeId;
                        iRecord.ExternalServiceID = iObject.ExternalServiceID;
                        iRecord.soWidgetCode = iObject.soWidgetCode;
                        iRecord.DescriptionEng = iObject.DescriptionEng;
                        iRecord.DescriptionAlt = iObject.DescriptionAlt;
                        iRecord.ServiceUrl = iObject.ServiceUrl;
                        iRecord.SortOrder = iObject.SortOrder;
                        iRecord.ApiSourceId = iObject.ApiSourceId;
                        iRecord.DepartmentId = iObject.DepartmentId;
                        iRecord.PrintMessage = iObject.PrintMessage;
                        iRecord.PrintMessageAr = iObject.PrintMessageAr;
                        iRecord.RowInsertDate = DateTime.Now;
                        iRecord.RowInsertedBy = Security.GetUser().UserName;
                        iRecord.IsActive = false;
                        aut.Remarks = Security.GetUser().UserName + " ADDED NEW SERVICE RECORD = " + iRecord.ExternalServiceID;
                    }
                    else
                    {
                        var a = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId != iObject.ServiceId && x.ExternalServiceID.ToUpper() == iObject.ExternalServiceID.ToUpper()).FirstOrDefault().TrimObject();
                        if (a != null)
                            return Json(new { Result = false, Message = iObject.ExternalServiceID + " " + DbManager.GetText("Lookup", "lblExistts", "External Service ID Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var b = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId != iObject.ServiceId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var c = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId != iObject.ServiceId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                        if (c != null)
                            return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        iRecord.PrintPreview = iObject.PrintPreview;
                        iRecord.TabularLayout = iObject.TabularLayout;
                        iRecord.IsAnonymous = iObject.IsAnonymous;
                        iRecord.ParentServiceId = 0;
                        iRecord.EntityId = iObject.EntityId;
                        iRecord.TypeId = iObject.TypeId;
                        iRecord.ExternalServiceID = iObject.ExternalServiceID;
                        iRecord.soWidgetCode = iObject.soWidgetCode;
                        iRecord.DescriptionEng = iObject.DescriptionEng;
                        iRecord.DescriptionAlt = iObject.DescriptionAlt;
                        iRecord.ServiceUrl = iObject.ServiceUrl;
                        iRecord.SortOrder = iObject.SortOrder;
                        iRecord.ApiSourceId = iObject.ApiSourceId;
                        iRecord.DepartmentId = iObject.DepartmentId;
                        iRecord.PrintMessage = iObject.PrintMessage;
                        iRecord.PrintMessageAr = iObject.PrintMessageAr;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;

                        aut.Remarks = Security.GetUser().UserName + " UPDATED SERVICE RECORD = " + iRecord.ExternalServiceID;
                    }
                    iRecord.IsDeleted = false;                    
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    Common.Utility.TrimObject<Models.Service>(iRecord);
                    iDbContext.Services.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    iObject.ServiceId = iRecord.ServiceId;

                    var childRecords = iDbContext.Services.Where(x => x.ParentServiceId == iObject.ServiceId).ToList();
                    if (childRecords != null && childRecords.Count > 0)
                    {
                        foreach (var x in childRecords)
                        {
                            x.ParentExternalServiceID = iObject.ExternalServiceID;
                            x.ApiSourceId = iObject.ApiSourceId;
                            x.RowUpdateDate = DateTime.Now;
                            x.RowUpdatedBy = Security.GetUser().UserName;
                            x.IpAddress = Common.Security.GetIpAddress();
                            iDbContext.Services.AddOrUpdate(x);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (iObject.Mapping != null && iObject.Mapping.Count > 0)
                    {
                        var a = iDbContext.ServiceEntitiesMappings.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId).ToList();
                        if (a != null)
                        {
                            iDbContext.ServiceEntitiesMappings.RemoveRange(a);
                            iDbContext.SaveChanges();
                        }

                        foreach (var iEntity in iObject.Mapping)
                        {
                            var x = new Models.ServiceEntitiesMapping();
                            x.ServiceId = iObject.ServiceId;
                            x.EntityId = iEntity.EntityId;
                            x.RowInsertDate = DateTime.Now;
                            x.RowInsertedBy = Security.GetUser().UserName;
                            x.IsDeleted = false;
                            x.IpAddress = Common.Security.GetIpAddress();
                            iDbContext.ServiceEntitiesMappings.AddOrUpdate(x);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (iObject.UserTypes != null && iObject.UserTypes.Count > 0)
                    {
                        var a = iDbContext.ServiceUserTypes.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId).ToList();
                        if (a != null)
                        {
                            iDbContext.ServiceUserTypes.RemoveRange(a);
                            iDbContext.SaveChanges();
                        }

                        foreach (var iType in iObject.UserTypes)
                        {
                            var x = new Models.ServiceUserType();
                            x.ServiceId = iObject.ServiceId;
                            x.UserTypeId = iType.UserTypeId;
                            x.RowInsertDate = DateTime.Now;
                            x.IsDeleted = false;
                            iDbContext.ServiceUserTypes.AddOrUpdate(x);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (iObject.CommentAttachments != null)
                    {
                        var a = iDbContext.ServiceCommentsAttachmentsConfigs.Where(y => y.IsDeleted == false && y.ServiceId == iObject.ServiceId).ToList();
                        if (a != null)
                        {
                            iDbContext.ServiceCommentsAttachmentsConfigs.RemoveRange(a);
                            iDbContext.SaveChanges();
                        }

                        var x = new Models.ServiceCommentsAttachmentsConfig();
                        x.ServiceId = iObject.ServiceId;
                        x.Minimum = iObject.CommentAttachments.Minimum;
                        x.MaxSize = iObject.CommentAttachments.MaxSize;
                        x.Maximum = iObject.CommentAttachments.Maximum;
                        x.RowInsertDate = DateTime.Now;
                        x.RowInsertedBy = Security.GetUser().UserName;
                        x.IsDeleted = false;
                        x.IpAddress = Common.Security.GetIpAddress();
                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(x);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        aut.ActionId = 7;
                        aut.ServiceId = iObject.ServiceId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iObject.ServiceId).FirstOrDefault().ExternalServiceID;
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
                System.Web.HttpContext.Current.Trace.Write(E.ToString());
                if (iTransaction != null)
                    iTransaction.Rollback();
                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblServiceSave", "Service Saved Successfully.!"), Icon = "success", Redirect = Common.Helper.URL_Encode(Url.Action("ServiceDetails", "Services", null, Request.Url.Scheme, null) + "?serviceId=" + iObject.ServiceId) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxCreateSubService(Models.ServiceObject iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var parentRecord = iDbContext.Services.SingleOrDefault(x => x.ServiceId == iObject.ParentServiceId);

                    var a = iDbContext.Services.Where(x => x.IsDeleted == false && x.ExternalServiceID.ToUpper() == iObject.ExternalServiceID.ToUpper()).FirstOrDefault().TrimObject();
                    if (a != null)
                        return Json(new { Result = false, Message = iObject.ExternalServiceID + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var b = iDbContext.Services.Where(x => x.IsDeleted == false && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.Services.Where(x => x.IsDeleted == false && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var iRecord = new Models.Service();

                    var i = iDbContext.Services.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        iRecord.ServiceId = i[i.Count - 1].ServiceId + 1;

                    iRecord.PrintPreview = iObject.PrintPreview;
                    iRecord.ParentServiceId = parentRecord.ServiceId;
                    iRecord.ParentExternalServiceID = parentRecord.ExternalServiceID;
                    iRecord.soWidgetCode = iObject.soWidgetCode;
                    iRecord.EntityId = parentRecord.EntityId;
                    iRecord.TypeId = parentRecord.TypeId;
                    iRecord.ExternalServiceID = iObject.ExternalServiceID;
                    iRecord.UseParentExternalServiceId = iObject.UseParentExternalServiceId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.ServiceUrl = iObject.ServiceUrl;
                    iRecord.ApiSourceId = parentRecord.ApiSourceId;
                    iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.RowInsertedBy = Security.GetUser().UserName;
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.PrintMessage = iObject.PrintMessage;
                    iRecord.PrintMessageAr = iObject.PrintMessageAr;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    Common.Utility.TrimObject<Models.Service>(iRecord);
                    iDbContext.Services.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    iObject.ServiceId = iRecord.ServiceId;

                    if (iObject.CommentAttachments != null)
                    {
                        var aa = iDbContext.ServiceCommentsAttachmentsConfigs.Where(y => y.IsDeleted == false && y.ServiceId == iObject.ServiceId).ToList();
                        if (aa != null)
                        {
                            iDbContext.ServiceCommentsAttachmentsConfigs.RemoveRange(aa);
                            iDbContext.SaveChanges();
                        }

                        var x = new Models.ServiceCommentsAttachmentsConfig();
                        x.ServiceId = iObject.ServiceId;
                        x.Minimum = iObject.CommentAttachments.Minimum;
                        x.MaxSize = iObject.CommentAttachments.MaxSize;
                        x.Maximum = iObject.CommentAttachments.Maximum;
                        x.RowInsertDate = DateTime.Now;
                        x.RowInsertedBy = Security.GetUser().UserName;
                        x.IsDeleted = false;
                        x.IpAddress = Common.Security.GetIpAddress();
                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(x);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.Remarks = Security.GetUser().UserName + " ADDED NEW SUB SERVICE RECORD = " + iRecord.ExternalServiceID;
                        aut.ActionId = 7;
                        aut.ServiceId = iObject.ServiceId;
                        aut.UniqueId = iRecord.ExternalServiceID;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblServiceSave", "Service Saved Successfully.!"), Icon = "success", Redirect = Common.Helper.URL_Encode(Url.Action("ServiceDetails", "Services", null, Request.Url.Scheme, null) + "?serviceId=" + iObject.ServiceId) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxUpdateSubService(Models.ServiceObject iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var a = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId != iObject.ServiceId && x.ExternalServiceID.ToUpper() == iObject.ExternalServiceID.ToUpper()).FirstOrDefault().TrimObject();
                    if (a != null)
                        return Json(new { Result = true, Message = iObject.ExternalServiceID + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var b = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId != iObject.ServiceId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = true, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId != iObject.ServiceId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = true, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var parentRecord = iDbContext.Services.SingleOrDefault(x => x.ServiceId == iObject.ParentServiceId);

                    var iRecord = iDbContext.Services.SingleOrDefault(x => x.ServiceId == iObject.ServiceId);
                    iRecord.ExternalServiceID = iObject.ExternalServiceID;
                    iRecord.UseParentExternalServiceId = iObject.UseParentExternalServiceId;
                    iRecord.ParentExternalServiceID = parentRecord.ExternalServiceID;
                    iRecord.soWidgetCode = iObject.soWidgetCode;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.PrintPreview = iObject.PrintPreview;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.PrintMessage = iObject.PrintMessage;
                    iRecord.PrintMessageAr = iObject.PrintMessageAr;
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    Common.Utility.TrimObject<Models.Service>(iRecord);
                    iDbContext.Services.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    iObject.ServiceId = iRecord.ServiceId;

                    if (iObject.CommentAttachments != null)
                    {
                        var aa = iDbContext.ServiceCommentsAttachmentsConfigs.Where(y => y.IsDeleted == false && y.ServiceId == iObject.ServiceId).ToList();
                        if (aa != null)
                        {
                            iDbContext.ServiceCommentsAttachmentsConfigs.RemoveRange(aa);
                            iDbContext.SaveChanges();
                        }

                        var x = new Models.ServiceCommentsAttachmentsConfig();
                        x.ServiceId = iObject.ServiceId;
                        x.Minimum = iObject.CommentAttachments.Minimum;
                        x.MaxSize = iObject.CommentAttachments.MaxSize;
                        x.Maximum = iObject.CommentAttachments.Maximum;
                        x.RowInsertDate = DateTime.Now;
                        x.RowInsertedBy = Security.GetUser().UserName;
                        x.IsDeleted = false;
                        x.IpAddress = Common.Security.GetIpAddress();
                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(x);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.Remarks = Security.GetUser().UserName + " UPDATES SUB SERVICE RECORD = " + iRecord.ExternalServiceID;
                        aut.ActionId = 7;
                        aut.ServiceId = iObject.ServiceId;
                        aut.UniqueId = iRecord.ExternalServiceID;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblServiceSave", "Service Saved Successfully.!"), Icon = "success", Redirect = Common.Helper.URL_Encode(Url.Action("ServiceDetails", "Services", null, Request.Url.Scheme, null) + "?serviceId=" + iObject.ServiceId) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AjaxGetSubServices(int serviceId)
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetSubServices(serviceId);
                if (result != null && result.Count > 0)
                {
                    int nctr = 1;
                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>" + nctr + "</div>";
                        html += "<div class='tag'>{3}</div>";
                        html += "<div class='title nameEn'>{1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";

                        int Minimum = 1;
                        int Maximum = 3;
                        int MaxSize = 2;

                        var aa = iDbContext.ServiceCommentsAttachmentsConfigs.Where(y => y.IsDeleted == false && y.ServiceId == i.ServiceId).FirstOrDefault();
                        if (aa != null)
                        {
                            if (aa.Minimum.HasValue)
                                Minimum = aa.Minimum.Value;
                            if (aa.Maximum.HasValue)
                                Maximum = aa.Maximum.Value;
                            if (aa.MaxSize.HasValue)
                                MaxSize = aa.MaxSize.Value;
                        }

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_SUB_SERVICE") == true)
                        {
                            html += "<a href='#' onclick=\"ModifySubService('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','{3}','{4}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_SUB_SERVICE") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteSubService('{0}','{5}','"+ name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{5}','{1}')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ACTDACT_SUB_SERVICE") == true)
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' onclick=\"ActivateSubService('{0}','{5}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' onclick=\"DeActivateSubService('{0}','{5}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDeActivate", "DeActivate") + "</a>";
                        }
                        else
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{5}','{1}')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{5}','{1}')\" class='link'>" + DbManager.GetText("Lookup", "lblDeActivate", "DeActivate") + "</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";

                        html = string.Format(html, i.ServiceId, i.DescriptionEng, i.DescriptionAlt, i.ExternalServiceID, i.UseParentExternalServiceId, serviceId, Minimum,Maximum,MaxSize,i.PrintPreview,i.soWidgetCode,i.PrintMessage,i.PrintMessageAr);
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

        #endregion

        #region --> ServiceClassifications

        [UrlDecode]
        public ActionResult ServiceClassifications(int serviceId)
        {
            Models.ServiceObject iObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                ViewBag.FormMode = "Edit";
                iObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasSubServices = false;

                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                {
                    ViewBag.HasSubServices = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.HasSubServices = false;
                    SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [HttpPost]
        public JsonResult AjaxGetServiceClassifications(int serviceId)
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetServiceClassifications(serviceId);
                if (result != null && result.Count > 0)
                {
                    int nctr = 1;
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle' title='{0}'>" + nctr + "</div>";
                        html += "<div class='tag'>{4} <span class='mBrown'>(" + i.CategoryCode + ")</span></div>";
                        html += "<div class='title nameEn'>{1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";

                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_CLASSIFICATION") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','{3}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_CLASSFICIATION") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteServiceClassification('{3}','{0}','"+ name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{3}','{0}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ACTDACT_CLASSIFICATION") == true)
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' onclick=\"ActivateServiceClassification('{3}','{0}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' onclick=\"DeActivateServiceClassification('{3}','{0}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDeActivate", "DeActivate") + "</a>";
                        }
                        else
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('{3}','{0}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('{3}','{0}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDeActivate", "DeActivate") + "</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.CategoryId, i.DescriptionEng, i.DescriptionAlt,i.ServiceId, i.ExternalServiceID,i.PrintPreview,i.PrintMessage,i.PrintMessageAr,i.TabularLayout);
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

        [HttpPost]
        public JsonResult AjaxSaveServiceClassifications(Models.ServiceCategoryObject iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.ServiceCategories.SingleOrDefault(x => x.CategoryId == iObject.CategoryId);
                if (iRecord != null)
                {
                    var b = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.CategoryId != iRecord.CategoryId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.CategoryId != iRecord.CategoryId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.TabularLayout = iObject.TabularLayout;
                    iRecord.PrintPreview = iObject.PrintPreview;
                    iRecord.PrintMessage = iObject.PrintMessage;
                    iRecord.PrintMessageAr = iObject.PrintMessageAr;
                    iRecord.CategoryId = iObject.CategoryId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;

                    if (string.IsNullOrEmpty(iRecord.CategoryGuid))
                        iRecord.CategoryGuid = Guid.NewGuid().ToString();

                    iDbContext.ServiceCategories.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " UPDATES SERVICE CLASSIFICATION = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    var b = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.ServiceCategory();

                    var i = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        Code = i[i.Count - 1].CategoryId + 1;

                    iRecord.CategoryGuid = Guid.NewGuid().ToString();

                    iRecord.CategoryCode = "SC-" + Code;
                    iRecord.TabularLayout = iObject.TabularLayout;
                    iRecord.PrintPreview = iObject.PrintPreview;
                    iRecord.PrintMessage = iObject.PrintMessage;
                    iRecord.PrintMessageAr = iObject.PrintMessageAr;
                    iRecord.ServiceId = iObject.ServiceId;
                    iRecord.CategoryId = Code;
                    iRecord.ParentCategoryId = 0;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Security.GetUser().UserName;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iDbContext.ServiceCategories.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.Remarks = Security.GetUser().UserName + " ADDED SERVICE CLASSIFICATION = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblClassSave", "Service Classification Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteServiceClassifications(int serviceId, int categoryId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceCategories.SingleOrDefault(x => x.CategoryId == categoryId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceCategories.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.ServiceFormsConfigs.Where(x => x.ServiceId == serviceId && x.CategorId == categoryId).ToList();
                    if (i != null)
                    {
                        foreach (var j in i)
                        {
                            j.IsDeleted = true;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Security.GetUser().UserName;
                            iDbContext.ServiceFormsConfigs.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE CLASSIFICATION = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblClassDel", "Service Classification Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxActivateClassifications(int serviceId, int categoryId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceCategories.SingleOrDefault(x => x.ServiceId == serviceId && x.CategoryId == categoryId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceCategories.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ACTIVATED SERVICE CLASSIFICATION = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblClassACt", "Service Classification Activated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeActivateClassifications(int serviceId, int categoryId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceCategories.SingleOrDefault(x => x.ServiceId == serviceId && x.CategoryId == categoryId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceCategories.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DE-ACTIVATED SERVICE CLASSIFICATION = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblClassDeAct", "Service Classification DeActivated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ServiceEntities

        [UrlDecode]
        public ActionResult ServiceEntities()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");
            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetServiceEntities()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetServiceEntites();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;
                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.EntityId + "' id='SortOrder" + i.EntityId + "' onchange=\"ChangeSortOrder2(" + (int)Guide.Beneficiaries + "," + i.EntityId + ",'SortOrder" + i.EntityId + "');\">";
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
                        html += "<div class='circle'>{0}</div>";
                        html += "<div class='title nameEn'>" + sortOrderControl + " {1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        {
                            html += "<a href='#' onclick=\"ModifyEntities('{0}','{1}','{2}','{3}','{4}','{5}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteServiceEntities('{0}','"+ name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','" + name + "');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";
                        html = string.Format(html, i.EntityId, i.DescriptionEng, i.DescriptionAlt, i.RemarksEng, i.RemarksAlt, i.ClassName);
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
        public JsonResult AjaxSaveServiceEntities(Models.ServiceEntityObject iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.ServiceEntities.SingleOrDefault(x => x.EntityId == iObject.EntityId);
                if (iRecord != null)
                {
                    var b = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId != iRecord.EntityId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId != iRecord.EntityId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.EntityId = iObject.EntityId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.RemarksAlt = iObject.RemarksAlt;
                    iRecord.RemarksEng = iObject.RemarksEng;
                    iRecord.ClassName = iObject.ClassName;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceEntities.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " updated service entity = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    var b = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.ServiceEntity();

                    var i = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        Code = i[i.Count - 1].EntityId + 1;

                    iRecord.EntityId = Code;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.RemarksAlt = iObject.RemarksAlt;
                    iRecord.RemarksEng = iObject.RemarksEng;
                    iRecord.ClassName = iObject.ClassName;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Security.GetUser().UserName;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iDbContext.ServiceEntities.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " added service entity = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveEntity", "Service Entity Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteServiceEntities(int entityId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceEntities.SingleOrDefault(x => x.EntityId == entityId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceEntities.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.Services.Where(x => x.EntityId == entityId).ToList();
                    if (i != null)
                    {
                        foreach (var j in i)
                        {
                            j.IsDeleted = true;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Security.GetUser().UserName;
                            iDbContext.Services.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " deleted service entity and services of entity = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelEntity", "Service Entity Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ServiceInputs

        [UrlDecode]
        public ActionResult ServiceInputs(int serviceId)
        {
            Models.ServiceObject iObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");
                ViewBag.FormMode = "Edit";
                iObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasClassfication = false;
                ViewBag.Classifications = null;
                var Classifications = Common.DbManager.GetServiceClassifications(serviceId);
                if (Classifications != null && Classifications.Count > 0)
                {
                    ViewBag.HasClassfication = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionAlt");
                    }
                }

                ViewBag.HasSubServices = false;

                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                {
                    ViewBag.HasSubServices = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.HasSubServices = false;
                    SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }

                var InputTypes = iDbContext.LkInputTypes.Where(x => x.IsDeleted == false).ToList();
                if (InputTypes != null && InputTypes.Count > 0)
                {
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.InputTypes = new SelectList(InputTypes.ToArray(), "TypeId", "ShortDescriptionEng");
                    }
                    else
                    {
                        ViewBag.InputTypes = new SelectList(InputTypes.ToArray(), "TypeId", "ShortDescriptoinAlt");
                    }
                }

                ViewBag.ServiceTabs = null;
                if (iObject.TabularLayout.HasValue && iObject.TabularLayout == true)
                {
                    var ServiceTabs = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.ServiceTabs = new SelectList(ServiceTabs.ToArray(), "TabId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.ServiceTabs = new SelectList(ServiceTabs.ToArray(), "TabId", "DescriptionAlt");
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [HttpPost]
        public JsonResult AjaxGetServiceInputs(int serviceId, int categoryId, int inputTypeId, int tabId)
        {
            string html = "";
            string title = "";
            try
            {
                var result = Common.DbManager.GetServiceFormsConfig(serviceId, categoryId, inputTypeId, tabId);
                if (result != null && result.Count > 0)
                {
                    result = result.OrderBy(x => x.SortOrder).ToList();
                    int nctr = 1;

                    //int minSort = result[0].SortOrder.Value;
                    //int maxSort = result[result.Count - 1].SortOrder.Value;

                    int minSort = 1;
                    int maxSort = 100;

                    if (result[0].SortOrder != null && result[0].SortOrder.HasValue)
                    {
                        minSort = result[0].SortOrder.Value;
                        maxSort = result[result.Count - 1].SortOrder.Value + 25;
                    }

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.Label : i.LabelAr;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.InputId+"' id='SortOrder"+i.InputId+"' onchange=\"ChangeSortOrder("+i.InputId+ ",'SortOrder" + i.InputId + "');\">";
                        
                        for (int k = minSort; k <= maxSort; k++)
                        {
                            if (i.SortOrder == k)
                                sortOrderControl += "<option value='" + k + "' selected>" + k + "</option>";
                            else
                                sortOrderControl += "<option value='" + k + "'>" + k + "</option>";
                        }

                        sortOrderControl += "</select>";

                        title = "";
                        if (string.IsNullOrEmpty(i.InputTypeName) == false)
                            title += i.InputTypeName + " |";

                        if (i.Required.HasValue && i.Required.Value == true)
                            title += " Required=Yes |";
                        else
                            title += " Required=No |";

                        if (i.SortOrder.HasValue)
                            title += " Order="+ i.SortOrder + " |";

                        if (i.DownloadAttachment.HasValue && i.DownloadAttachment == true)
                            title += " Print=Yes |";

                        if (title.EndsWith("|"))
                            title = title.Remove(title.Length - 1, 1);

                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle' title='{0}'>" + nctr + "</div>";
                        html += "<div class='tag'>"+ FileHelper.GetInputTypeIcon(i.InputTypeName) + " - {4}</div>";
                        html += "<div class='title catEn'>{5}</div>";

                        if (i.Required.HasValue && i.Required.Value == true)
                            html += "<div class='title nameEn' title='" + title + "'>" + sortOrderControl + ". {1} - <span class='IsRequired'>*</span></div>";
                        else
                            html += "<div class='title nameEn' title='" + title + "'>" + sortOrderControl + ". {1}</div>";


                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_SERVICE_FIELD") == true)
                        {
                            html += "<a href='{0}' class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='{0}' class='link'>" + DbManager.GetText("Lookup", "lblViewIcon", "View") + "</a>";
                        }

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_SERVICE_FIELD") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteServiceInput('{3}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{3}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ACTDACT_SERVICE_FIELD") == true)
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' onclick=\"ActivateServiceInput('{3}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' onclick=\"DeActivateServiceInput('{3}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lbldeActivate", "DeActivate") + "</a>";
                        }
                        else
                        {
                            if (i.IsActive == false)
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('{3}','" + name + "')\" class='link'>" + DbManager.GetText("Lookup", "lblActivate", "Activate") + "</a>";
                            else
                                html += "<a href='#' style='display:none;' onclick=\"NoPermission('{3}','" + name + "}')\" class='link'>" + DbManager.GetText("Lookup", "lbldeActivate", "DeActivate") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";

                        string url = Common.Helper.URL_Encode(Url.Action("FormBuilder", "Services", null, Request.Url.Scheme, null) + "?serviceId="+ i.ParentServiceId + "&inputId=" + i.InputId);

                        html = string.Format(html, url, i.Label, i.LabelAr,i.InputId, i.ExternalServiceID, i.CategoryName);
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

        [HttpPost]
        public JsonResult AjaxSaveServiceInputs(Models.ServiceFormsConfigObject iObject)
        {
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == iObject.InputId);
                if (iRecord != null)
                {
                    iRecord.CategorId = iObject.CategorId;
                    iRecord.InputTypeId = iObject.InputTypeId;
                    iRecord.ID = iObject.ID;
                    iRecord.Name = iObject.Name;
                    iRecord.Label = iObject.Label;
                    iRecord.Placeholder = iObject.Placeholder;
                    iRecord.Required = iObject.Required;
                    iRecord.ArabicInput = iObject.ArabicInput;
                    iRecord.EnglishInput = iObject.EnglishInput;
                    iRecord.DynamicInput = iObject.DynamicInput;
                    iRecord.OptionId = iObject.OptionId;
                    iRecord.Attributes = iObject.Attributes;
                    iRecord.Message = iObject.Message;
                    iRecord.Maximum = iObject.Maximum;
                    iRecord.Minimum = iObject.Minimum;
                    iRecord.SortOrder = iObject.SortOrder;
                    iRecord.LabelAr = iObject.LabelAr;
                    iRecord.PlaceholderAr = iObject.PlaceholderAr;
                    iRecord.AttributesAr = iObject.AttributesAr;
                    iRecord.MessageAr = iObject.MessageAr;
                    iRecord.UserField = iObject.UserField;
                    iRecord.IsExternalLookup = iObject.IsExternalLookup;
                    iRecord.ExternalLookupId = iObject.ExternalLookupId;
                    iRecord.MaxFileSize = iObject.MaxFileSize;
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " UPDATED SERVICE FORMS INPUT RECORD = " + iRecord.InputId + "-" + iRecord.Label;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    iRecord = new Models.ServiceFormsConfig();

                    iRecord.ServiceId = iObject.ServiceId;
                    iRecord.CategorId = iObject.CategorId;
                    iRecord.LanguageId = 1;
                    iRecord.InputTypeId = iObject.InputTypeId;
                    iRecord.ID = iObject.ID;
                    iRecord.Name = iObject.Name;
                    iRecord.Label = iObject.Label;
                    iRecord.Placeholder = iObject.Placeholder;
                    iRecord.Required = iObject.Required;
                    iRecord.ArabicInput = iObject.ArabicInput;
                    iRecord.EnglishInput = iObject.EnglishInput;
                    iRecord.DynamicInput = iObject.DynamicInput;
                    iRecord.OptionId = iObject.OptionId;
                    iRecord.Attributes = iObject.Attributes;
                    iRecord.Message = iObject.Message;
                    iRecord.Maximum = iObject.Maximum;
                    iRecord.Minimum = iObject.Minimum;
                    iRecord.SortOrder = iObject.SortOrder;
                    iRecord.LabelAr = iObject.LabelAr;
                    iRecord.PlaceholderAr = iObject.PlaceholderAr;
                    iRecord.AttributesAr = iObject.AttributesAr;
                    iRecord.MessageAr = iObject.MessageAr;
                    iRecord.UserField = iObject.UserField;
                    iRecord.IsExternalLookup = iObject.IsExternalLookup;
                    iRecord.ExternalLookupId = iObject.ExternalLookupId;
                    iRecord.MaxFileSize = iObject.MaxFileSize;
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Security.GetUser().UserName;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.SortOrder = iObject.SortOrder;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ADDED SERVICE FORMS INPUT RECORD = " + iRecord.InputId + "-" + iRecord.Label;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblInputSave", "Service Input Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteServiceInputs(int inputId)
        {
            DbContextTransaction iTransaction = null;
            int serviceId = 0;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    serviceId = iRecord.ServiceId.Value;

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE FORM INPUT FIELD = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblInputDel", "Service Input Deleted Successfully.!"), Icon = "success", ServiceId = serviceId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxActivateServiceInputs(int inputId)
        {
            DbContextTransaction iTransaction = null;
            int serviceId = 0;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    serviceId = iRecord.ServiceId.Value;

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ACTIVATED SERVICE FORM INPUT FIELD = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblInputAct", "Service Input Activated Successfully.!"), Icon = "success", ServiceId = serviceId }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeActivateServiceInputs(int inputId)
        {
            DbContextTransaction iTransaction = null;
            int serviceId = 0;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    serviceId = iRecord.ServiceId.Value;

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DE-ACTIVATED SERVICE FORM INPUT FIELD = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblInputDeAct", "Service Input DeActivated Successfully.!"), Icon = "success", ServiceId = serviceId }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadServiceClassifications(int serviceId)
        {
            try
            {
                var Classifications = Common.DbManager.GetServiceClassifications(serviceId);
                if (Classifications != null && Classifications.Count > 0)
                {
                    ViewBag.HasClassfication = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionAlt");
                    }
                }
                return Json(ViewBag.Classifications, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//

        [HttpPost]
        public JsonResult ChangeSortOrder(int inputId, int sortOrder)
        {
            DbContextTransaction iTransaction = null;
            int serviceId = 0;
            try
            {
                var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                iRecord.SortOrder = sortOrder;
                iRecord.RowUpdateDate = DateTime.Now;
                iRecord.IpAddress = Common.Security.GetIpAddress();
                iRecord.RowUpdatedBy = Security.GetUser().UserName;
                iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                iDbContext.SaveChanges();

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 7;
                    aut.ServiceId = iRecord.ServiceId;
                    aut.CategoryId = iRecord.CategorId;
                    aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                    aut.Remarks = Security.GetUser().UserName + " CHANGED THE SORT ORDER OF SERVICE FORM INPUT FIELD = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSortChange", "SortOrder Updated Successfully.!"), Icon = "success", ServiceId = serviceId }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ServiceTypes
        [UrlDecode]
        public ActionResult ServiceTypes()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");
            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetServiceTypes()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetServiceTypes();
                if (result != null && result.Count > 0)
                {
                    int minSort = 1;
                    int maxSort = result.Count + 5;

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;

                        string sortOrderControl = "<select class='sortselect' name='SortOrder" + i.TypeId + "' id='SortOrder" + i.TypeId + "' onchange=\"ChangeSortOrder2(" + (int)Guide.Types + "," + i.TypeId + ",'SortOrder" + i.TypeId + "');\">";
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
                        html += "<div class='circle'>{0}</div>";
                        html += "<div class='title nameEn'>" + sortOrderControl + "{1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        {
                            html += "<a href='#' onclick=\"Modify('{0}','{1}','{2}','');\"  class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','');\"  class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteServiceType('{0}','"+ name + "');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','" + name + "');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";

                        html = string.Format(html, i.TypeId, i.DescriptionEng, i.DescriptionAlt);
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
        public JsonResult AjaxSaveServiceTypes(Models.ServiceTypeObject iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.ServiceTypes.SingleOrDefault(x => x.TypeId == iObject.TypeId);
                if (iRecord != null)
                {
                    var b = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId != iRecord.TypeId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId != iRecord.TypeId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.TypeId = iObject.TypeId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceTypes.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " updated service type = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    var b = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.ServiceType();

                    var i = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        Code = i[i.Count - 1].TypeId + 1;

                    iRecord.TypeId = Code;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Security.GetUser().UserName;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.SortOrder = i[i.Count - 1].SortOrder + 1;
                    iDbContext.ServiceTypes.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " added service type = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblTypeSave", "Service Type Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteServiceTypes(int typeId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceTypes.SingleOrDefault(x => x.TypeId == typeId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceTypes.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.Services.Where(x => x.TypeId == typeId).ToList();
                    if (i != null)
                    {
                        foreach (var j in i)
                        {
                            j.IsDeleted = true;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Security.GetUser().UserName;
                            iDbContext.Services.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " delete service type = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblTypeDel", "Service Type Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ServiceRequests

        [UrlDecode]
        public ActionResult ServiceRequests()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

            var Status = Common.DbManager.GetLkRequestStatus();
            //ViewBag.Status = new SelectList(Status.ToArray(), "StatusId", "DescriptionEng");
            if (Helper.CurrentLanguage() == (int)Language.English)
            {
                ViewBag.Status = new SelectList(Status.ToArray(), "StatusId", "DescriptionEng");
            }
            else
            {
                ViewBag.Status = new SelectList(Status.ToArray(), "StatusId", "DescriptionAlt");
            }

            if (Security.isUserSystemAdministrator() == false)
            {
                int? userDepId = Security.GetUser().DepartmentId;
                var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false && x.DepartmentId == userDepId).ToList();
                //ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                }
                else
                {
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
                }
            }
            else
            {
                var Departments = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
                //ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
                }
                else
                {
                    ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
                }
            }

            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetServiceRequests(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                if (Security.isUserSystemAdministrator() == false)
                {
                    iSearch.DepartmentId = Security.GetUser().DepartmentId;
                }

                var iData = Common.DbManager.GetServiceRequests(iSearch);
                if (iData != null && iData.Count > 0)
                    totalRows = iData[0].TotalRows;

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                    c.RequestId
                    ,c.Service
                    ,c.StatusEng
                    ,c.UserId
                    ,Common.Helper.IsNullGet<DateTime>(c.RowInsertDate)
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
                throw E;
            }
            return Json(new { result = false, Error = "" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> FormBuilder

        [UrlDecode]
        public ActionResult FormBuilder(int serviceId, int inputId)
        {
            Models.ServiceFormsConfigObject iObject = new Models.ServiceFormsConfigObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                if (inputId == 0)
                    ViewBag.FormMode = "New";
                else
                    ViewBag.FormMode = "Edit";

                var iServiceObject = Common.DbManager.GetService(serviceId);

                iObject = Common.DbManager.GetServiceInput(serviceId, inputId);
                if (iObject == null)
                {
                    iObject = new Models.ServiceFormsConfigObject();
                    iObject.ServiceId = serviceId;
                    iObject.ServiceName = iServiceObject.DescriptionEng;
                    iObject.ServiceNameAlt = iServiceObject.DescriptionAlt;
                }
                ViewBag.HasSubServices = false;
                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                {
                    ViewBag.HasSubServices = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                        if (iObject != null)
                            ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng", iObject.ServiceId);
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                        if (iObject != null)
                            ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt", iObject.ServiceId);
                    }
                }
                else
                {
                    ViewBag.HasSubServices = false;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                        if (iObject != null)
                            ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng", iObject.ServiceId);
                    }
                    else
                    {
                        SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                        if (iObject != null)
                            ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt", iObject.ServiceId);
                    }
                }

                ViewBag.HasClassfication = false;
                ViewBag.Classifications = null;
                var Classifications = Common.DbManager.GetServiceClassifications(serviceId);
                if (Classifications != null && Classifications.Count > 0)
                {
                    ViewBag.HasClassfication = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionEng");
                        if (iObject != null)
                            ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionEng", iObject.CategorId);
                    }
                    else
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionAlt");
                        if (iObject != null)
                            ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionAlt", iObject.CategorId);
                    }
                }
                else
                {
                    ViewBag.Classifications = null;
                }

                var InputTypes = iDbContext.LkInputTypes.Where(x => x.IsDeleted == false).ToList();
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    ViewBag.InputTypes = new SelectList(InputTypes.ToArray(), "TypeId", "ShortDescriptionEng");
                    if (iObject != null)
                        ViewBag.InputTypes = new SelectList(InputTypes.ToArray(), "TypeId", "ShortDescriptionEng", iObject.InputTypeId);
                }
                else
                {
                    ViewBag.InputTypes = new SelectList(InputTypes.ToArray(), "TypeId", "ShortDescriptoinAlt");
                    if (iObject != null)
                        ViewBag.InputTypes = new SelectList(InputTypes.ToArray(), "TypeId", "ShortDescriptoinAlt", iObject.InputTypeId);
                }
                

                var LogicalOperators = iDbContext.LkLogicalOperators.Where(x => x.IsDeleted == false).ToList();
                ViewBag.LogicalOperators = new SelectList(LogicalOperators.ToArray(), "LogicalOperator", "LogicalOperator");

                var Lookups = iDbContext.LkLookups.Where(x => x.IsDeleted == false).ToList();
                if (iObject != null)
                    ViewBag.Lookups = new SelectList(Lookups.ToArray(), "LookupId", "LookUpName", iObject.OptionId);
                else
                    ViewBag.Lookups = new SelectList(Lookups.ToArray(), "LookupId", "LookUpName");

                var UserAttributes = iDbContext.LkUserAttributesSSOes.Where(x => x.IsDeleted == false).ToList();
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    if (iObject != null)
                        ViewBag.UserAttributes = new SelectList(UserAttributes.ToArray(), "DescriptionEng", "DescriptionEng", iObject.UserField);
                    else
                        ViewBag.UserAttributes = new SelectList(UserAttributes.ToArray(), "DescriptionEng", "DescriptionEng");
                }
                else
                {
                    if (iObject != null)
                        ViewBag.UserAttributes = new SelectList(UserAttributes.ToArray(), "DescriptionEng", "DescriptionAlt", iObject.UserField);
                    else
                        ViewBag.UserAttributes = new SelectList(UserAttributes.ToArray(), "DescriptionEng", "DescriptionAlt");
                }
                

                var languageInput = iDbContext.LkInputLanguages.Where(x => x.IsDeleted == false).ToList();
                if (iObject != null)
                    ViewBag.EnglishInputTypes = new SelectList(languageInput.ToArray(), "InputId", "Language", iObject.EnglishInput);
                else
                    ViewBag.EnglishInputTypes = new SelectList(languageInput.ToArray(), "InputId", "Language", 2);

                //var  arabicInput = iDbContext.LkInputLanguages.Where(x => x.IsDeleted == false).ToList();
                if (iObject != null)
                    ViewBag.ArabicInputTypes = new SelectList(languageInput.ToArray(), "InputId", "Language", iObject.ArabicInput);
                else
                    ViewBag.ArabicInputTypes = new SelectList(languageInput.ToArray(), "InputId", "Language", 3);

                var lookupTypes = iDbContext.ServiceFormsConfigs.Where(x => (x.InputTypeId == (int)FormInputTypes.Dropdownlist || x.InputTypeId == (int)FormInputTypes.RadioButton || x.InputTypeId == (int)FormInputTypes.Checkbox) && x.ServiceId == serviceId).ToList();
                ViewBag.lookupTypes = new SelectList(lookupTypes.ToArray(), "ID", "Label");

                IDictionary<int, string> yesNo = new Dictionary<int, string>();
                yesNo.Add(0, "No");
                yesNo.Add(1, "Yes");

                if (iObject != null)
                    ViewBag.DynamicInput = new SelectList(yesNo.OrderBy(x => x.Value), "Key", "Value", iObject.DynamicInput.ToString());
                else
                    ViewBag.DynamicInput = new SelectList(yesNo.OrderBy(x => x.Value), "Key", "Value", 0);

                ViewBag.ServiceTabs = null;
                if (iServiceObject.TabularLayout.HasValue && iServiceObject.TabularLayout == true)
                {
                    var ServiceTabs = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        if (iObject != null)
                            ViewBag.ServiceTabs = new SelectList(ServiceTabs.ToArray(), "TabId", "DescriptionEng", iObject.TabId);
                        else
                            ViewBag.ServiceTabs = new SelectList(ServiceTabs.ToArray(), "TabId", "DescriptionEng");
                    }
                    else
                    {
                        if (iObject != null)
                            ViewBag.ServiceTabs = new SelectList(ServiceTabs.ToArray(), "TabId", "DescriptionAlt", iObject.TabId);
                        else
                            ViewBag.ServiceTabs = new SelectList(ServiceTabs.ToArray(), "TabId", "DescriptionAlt");
                    }
                }

                IDictionary<int, string> LkActions = new Dictionary<int, string>();
                //LkActions.Add(0, "None");
                LkActions.Add(1, "Stop Submission");
                ViewBag.LkActions = new SelectList(LkActions.OrderBy(x => x.Value), "Key", "Value");

                ViewBag.IsCustomService = false;
                if (WebConfig.GetStringValue("MOF_CustomServiceIds").ToUpper().Contains(iServiceObject.ExternalServiceID.ToUpper()))
                    ViewBag.IsCustomService = true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [HttpPost]
        public JsonResult SaveForm(Models.ServiceFormsConfigObject j)
        {
            int serviceId = 0;
            int inputId = 0;
            bool isNew = false;
            try
            {
                j = j.TrimObject();

                if (j.InputTypeId == 14)
                    j.IsReadOnly = false;

                var iServiceDetails = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault();
                if (iServiceDetails != null && iServiceDetails.ParentServiceId.HasValue && iServiceDetails.ParentServiceId.Value > 0)
                {
                    serviceId = iServiceDetails.ParentServiceId.Value;
                }
                else
                {
                    serviceId = j.ServiceId.Value;
                }

                var k = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == j.InputId);
                if (k == null)//NEW INPUT
                {
                    if (j.CategorId.HasValue == false)
                    {
                        var b = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.Label.ToUpper() == j.Label.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = j.Label + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var c = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.LabelAr.ToUpper() == j.LabelAr.ToUpper()).FirstOrDefault().TrimObject();
                        if (c != null)
                            return Json(new { Result = false, Message = j.LabelAr + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var b = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.CategorId == j.CategorId && x.Label.ToUpper() == j.Label.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = j.Label + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var c = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.CategorId == j.CategorId && x.LabelAr.ToUpper() == j.LabelAr.ToUpper()).FirstOrDefault().TrimObject();
                        if (c != null)
                            return Json(new { Result = false, Message = j.LabelAr + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                    }

                    k = new Models.ServiceFormsConfig();

                    k.ServiceId = j.ServiceId;
                    k.Attributes = j.Attributes;
                    k.AttributesAr = j.AttributesAr;
                    k.InputTypeId = j.InputTypeId;
                    k.LabelAr = j.LabelAr;
                    k.Label = j.Label;
                    k.LanguageId = 1;
                    k.MaxFileSize = j.MaxFileSize;
                    k.Maximum = j.Maximum;
                    k.Message = j.Message;
                    k.MessageAr = j.MessageAr;
                    k.Minimum = j.Minimum;

                    if (j.CategorId.HasValue && j.CategorId > 0)
                        k.CategorId = j.CategorId;
                    else
                        k.CategorId = null;

                    if (string.IsNullOrEmpty(j.ExternalLookupId) == false)
                    {
                        int lookupId = Convert.ToInt32(j.ExternalLookupId);
                        k.ExternalLookupId = iDbContext.LkLookups.Where(x => x.LookupId == lookupId).FirstOrDefault().LookUpName;
                        //k.IsExternalLookup = true;
                        k.IsExternalLookup = j.IsExternalLookup;
                        k.OptionId = lookupId;

                        //var lkpOp = iDbContext.LookupOptions.Where(x => x.LookupId == lookupId && x.IsDeleted == false).ToList();
                        //if (lkpOp != null && lkpOp.Count > 0)
                        //    k.IsExternalLookup = false;
                    }
                    else
                    {
                        k.IsExternalLookup = false;
                    }

                    //DownloadAttachment Validation
                    if (j.DownloadAttachment == true)
                    {
                        if (j.CategorId.HasValue == true)
                        {
                            var da = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.CategorId == j.CategorId && x.DownloadAttachment == true).FirstOrDefault().TrimObject();
                            //if (da != null)
                            //    return Json(new { Result = false, Message = "More than one attachments are not allowed for download", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                            //else
                                k.DownloadAttachment = j.DownloadAttachment;
                        }
                        else
                        {
                            var da = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.DownloadAttachment == true).FirstOrDefault().TrimObject();
                            //if (da != null)
                            //    return Json(new { Result = false, Message = "More than one attachments are not allowed for download", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                            //else
                                k.DownloadAttachment = j.DownloadAttachment;
                        }
                    }
                    else
                    {
                        k.DownloadAttachment = j.DownloadAttachment;
                    }

                    k.ID = Helper.GetRandomCode();
                    k.Name = Helper.GetRandomCode();
                    //k.OptionId = null;
                    k.IsDeleted = false;
                    k.IsActive = true;

                    k.Placeholder = j.Placeholder;
                    k.PlaceholderAr = j.PlaceholderAr;
                    k.Required = j.Required;
                    k.ArabicInput = j.ArabicInput;
                    k.EnglishInput = j.EnglishInput;
                    k.DynamicInput = j.DynamicInput;
                    k.SortOrder = j.SortOrder;
                    k.UserField = j.UserField;
                    k.IsReadOnly = j.IsReadOnly;
                    k.ApplyWordCount = j.ApplyWordCount;
                    k.SpeechToText = j.SpeechToText;
                    k.TabId = j.TabId;

                    //k.ReferralId = j.ReferralId;
                    //k.LogicalOperator = j.LogicalOperator;
                    //k.ReferralIdValue = j.ReferralIdValue;
                    k.ValidationMessage = j.ValidationMessage;
                    k.ValidationMessageAr = j.ValidationMessageAr;
                    k.HelpMessage = j.HelpMessage;
                    k.HelpMessageAr = j.HelpMessageAr;
                    k.FilterId = j.FilterId;
                    k.FilterValue = j.FilterValue;
                    k.JsonAttribute = j.JsonAttribute;
                    k.Bookmark = j.Bookmark;

                    //if (!string.IsNullOrEmpty(j.ReferralId) && !string.IsNullOrEmpty(j.LogicalOperator) && !string.IsNullOrEmpty(j.ReferralIdValue))
                    //{
                    //    var referralRecord = iDbContext.ServiceFormsConfigs.Where(x => x.ID == j.ReferralId).FirstOrDefault();
                    //    if (referralRecord != null)
                    //    {
                    //        referralRecord.HasDrilldown = true;
                    //        iDbContext.ServiceFormsConfigs.AddOrUpdate(referralRecord);
                    //        iDbContext.SaveChanges();
                    //    }
                    //}

                    k.RowInsertDate = DateTime.Now;
                    k.RowInsertedBy = Security.GetUser().UserName;
                    isNew = true;
                    //if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    //{
                    //    var aut = new Models.UserScreenAction();
                    //    aut.ActionId = 7;
                    //    aut.ServiceId = serviceId;
                    //    aut.CategoryId = k.CategorId;
                    //    aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == serviceId).FirstOrDefault().ExternalServiceID;
                    //    aut.Remarks = Security.GetUser().UserName + " added service input field = " + k.InputId + "-" + k.Label;
                    //    aut.RowInsertDate = DateTime.Now;
                    //    aut.IpAddress = Common.Security.GetIpAddress();
                    //    aut.RowInsertedBy = Security.GetUser().UserName;
                    //    aut.IsDeleted = false;
                    //    iDbContext.UserScreenActions.AddOrUpdate(aut);
                    //    iDbContext.SaveChanges();
                    //}
                }
                else
                {
                    if (j.CategorId.HasValue == false)
                    {
                        var b = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.InputId != j.InputId && x.ServiceId == j.ServiceId && x.LanguageId == 1 && x.Label.ToUpper() == j.Label.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = j.Label + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var c = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.InputId != j.InputId && x.ServiceId == j.ServiceId && x.LanguageId == 1 && x.LabelAr.ToUpper() == j.LabelAr.ToUpper()).FirstOrDefault().TrimObject();
                        if (c != null)
                            return Json(new { Result = false, Message = j.LabelAr + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var b = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.InputId != j.InputId && x.ServiceId == j.ServiceId && x.LanguageId == 1 && x.CategorId == j.CategorId && x.Label.ToUpper() == j.Label.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = j.Label + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        var c = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.InputId != j.InputId && x.ServiceId == j.ServiceId && x.LanguageId == 1 && x.CategorId == j.CategorId && x.LabelAr.ToUpper() == j.LabelAr.ToUpper()).FirstOrDefault().TrimObject();
                        if (c != null)
                            return Json(new { Result = false, Message = j.LabelAr + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                    }

                    k.InputTypeId = j.InputTypeId;
                    k.LabelAr = j.LabelAr;
                    k.Label = j.Label;
                    k.MaxFileSize = j.MaxFileSize;
                    k.Maximum = j.Maximum;
                    k.Message = j.Message;
                    k.MessageAr = j.MessageAr;
                    k.Minimum = j.Minimum;

                    if (j.CategorId.HasValue && j.CategorId > 0)
                        k.CategorId = j.CategorId;
                    else
                        k.CategorId = null;

                    if (string.IsNullOrEmpty(j.ExternalLookupId) == false)
                    {
                        int lookupId = Convert.ToInt32(j.ExternalLookupId);
                        k.ExternalLookupId = iDbContext.LkLookups.Where(x => x.LookupId == lookupId).FirstOrDefault().LookUpName;
                        //k.IsExternalLookup = true;
                        k.IsExternalLookup = j.IsExternalLookup;
                        k.OptionId = lookupId;

                        //var lkpOp = iDbContext.LookupOptions.Where(x => x.LookupId == lookupId && x.IsDeleted == false).ToList();
                        //if (lkpOp != null && lkpOp.Count > 0)
                        //    k.IsExternalLookup = false;
                    }
                    else
                    {
                        k.IsExternalLookup = false;
                    }

                    //DownloadAttachment Validation
                    
                    if (j.DownloadAttachment == true)
                    {
                        if (j.CategorId.HasValue == true)
                        {
                            var da = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.InputId != k.InputId && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.CategorId == j.CategorId && x.DownloadAttachment == true).FirstOrDefault().TrimObject();
                            //if (da != null)
                            //    return Json(new { Result = false, Message = "More than one attachments are not allowed for download", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                            //else
                                k.DownloadAttachment = j.DownloadAttachment;
                        }
                        else
                        {
                            var da = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.InputId != k.InputId && x.LanguageId == 1 && x.ServiceId == j.ServiceId && x.DownloadAttachment == true).FirstOrDefault().TrimObject();
                            //if (da != null)
                            //    return Json(new { Result = false, Message = "More than one attachments are not allowed for download", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                            //else
                                k.DownloadAttachment = j.DownloadAttachment;
                        }
                    }
                    else
                    {
                        k.DownloadAttachment = j.DownloadAttachment;
                    }

                    //k.OptionId = null;
                    k.IsDeleted = false;
                    k.Placeholder = j.Placeholder;
                    k.PlaceholderAr = j.PlaceholderAr;
                    k.Required = j.Required;
                    k.ArabicInput = j.ArabicInput;
                    k.EnglishInput = j.EnglishInput;
                    k.DynamicInput = j.DynamicInput;
                    k.SortOrder = j.SortOrder;
                    k.UserField = j.UserField;
                    k.RowUpdateDate = DateTime.Now;
                    k.RowUpdatedBy = Security.GetUser().UserName;
                    k.IsReadOnly = j.IsReadOnly;
                    k.ApplyWordCount = j.ApplyWordCount;
                    k.SpeechToText = j.SpeechToText;
                    k.TabId = j.TabId;

                    //if (!string.IsNullOrEmpty(k.ReferralId))
                    //{
                    //    var kRr = iDbContext.ServiceFormsConfigs.Where(x => x.ID == j.ReferralId).FirstOrDefault();
                    //    if (kRr != null)
                    //    {
                    //        kRr.HasDrilldown = false;
                    //        iDbContext.ServiceFormsConfigs.AddOrUpdate(kRr);
                    //        iDbContext.SaveChanges();
                    //    }
                    //}

                    //k.ReferralId = j.ReferralId;
                    //k.LogicalOperator = j.LogicalOperator;
                    //k.ReferralIdValue = j.ReferralIdValue;
                    k.ValidationMessage = j.ValidationMessage;
                    k.ValidationMessageAr = j.ValidationMessageAr;
                    k.HelpMessage = j.HelpMessage;
                    k.HelpMessageAr = j.HelpMessageAr;
                    k.FilterId = j.FilterId;
                    k.FilterValue = j.FilterValue;
                    k.JsonAttribute = j.JsonAttribute;
                    k.Bookmark = j.Bookmark;

                    //if (!string.IsNullOrEmpty(j.ReferralId) && !string.IsNullOrEmpty(j.LogicalOperator) && !string.IsNullOrEmpty(j.ReferralIdValue))
                    //{
                    //    var referralRecord = iDbContext.ServiceFormsConfigs.Where(x => x.ID == j.ReferralId).FirstOrDefault();
                    //    if (referralRecord != null)
                    //    {
                    //        referralRecord.HasDrilldown = true;
                    //        iDbContext.ServiceFormsConfigs.AddOrUpdate(referralRecord);
                    //        iDbContext.SaveChanges();
                    //    }
                    //}

                    //if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    //{
                    //    var aut = new Models.UserScreenAction();
                    //    aut.ActionId = 7;
                    //    aut.ServiceId = serviceId;
                    //    aut.CategoryId = k.CategorId;
                    //    aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == serviceId).FirstOrDefault().ExternalServiceID;
                    //    aut.Remarks = Security.GetUser().UserName + " updated service input field = " + k.Label;
                    //    aut.RowInsertDate = DateTime.Now;
                    //    aut.IpAddress = Common.Security.GetIpAddress();
                    //    aut.RowInsertedBy = Security.GetUser().UserName;
                    //    aut.IsDeleted = false;
                    //    iDbContext.UserScreenActions.AddOrUpdate(aut);
                    //    iDbContext.SaveChanges();
                    //}
                }
                k.IpAddress = Common.Security.GetIpAddress();
                Common.Utility.TrimObject<Models.ServiceFormsConfig>(k);
                iDbContext.ServiceFormsConfigs.AddOrUpdate(k);
                iDbContext.SaveChanges();

                inputId = k.InputId;
                var updateInputID = iDbContext.ServiceFormsConfigs.Where(x => x.InputId == inputId).FirstOrDefault();
                if (updateInputID != null)
                {
                    if (isNew)
                    {
                        TimeSpan span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                        string ReferenceNo = Convert.ToString((double)span.TotalSeconds).Replace(".", "");
                        updateInputID.ID = ReferenceNo;
                    }

                    //if (updateInputID.ID.Length <= 8)
                    //    updateInputID.ID = inputId + "-" + updateInputID.ID;
                    //if (updateInputID.Name.Length <= 8)
                    //    updateInputID.Name = inputId + "-" + updateInputID.Name;


                    iDbContext.ServiceFormsConfigs.AddOrUpdate(updateInputID);
                    iDbContext.SaveChanges();
                }

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 7;
                    aut.ServiceId = serviceId;
                    aut.CategoryId = k.CategorId;
                    aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == serviceId).FirstOrDefault().ExternalServiceID;
                    if (isNew)
                        aut.Remarks = Security.GetUser().UserName + " - ADDED NEW SERVICE INPUT FORM FIELD RECORD = " + k.InputId + "-" + k.Label;
                    else
                        aut.Remarks = Security.GetUser().UserName + " - UPDATED SERVICE INPUT FORM FIELD RECORD = " + k.InputId + "-" + k.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveForm", "Service Input Form Successfully.!"), Icon = "success", Redirect = Common.Helper.URL_Encode(Url.Action("ServiceInputs", "Services", null, Request.Url.Scheme, null) + "?serviceId=" + serviceId) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteFormBuilder(int inputId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " - DELETED SERVICE INPUT FIELD RECORD  = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDeleteForm", "Service Form Builder Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxActivateFormBuilder(int inputId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + "- ACTIVATED SERVICE INPUT FIELD RECORD = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblActForm", "Service Input Activated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeActivateFormBuilder(int inputId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == inputId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceFormsConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();
                    
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + "- DE-ACTIVATED SERVICE INPUT FIELD RECORD = " + iRecord.InputId + "-" + iRecord.Label;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDeActForm", "Service Input DeActivated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadServiceSelectTypes(int serviceId, int categoryId, long inputId)
        {
            try
            {
                var lookupTypes = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && (x.InputTypeId == (int)FormInputTypes.Dropdownlist || x.InputTypeId == (int)FormInputTypes.RadioButton || x.InputTypeId == (int)FormInputTypes.Checkbox || x.InputTypeId == (int)FormInputTypes.SingleCheckbox) && x.ServiceId == serviceId).ToList();
                if (categoryId > 0)
                {
                    lookupTypes = lookupTypes.Where(x => x.CategorId == categoryId).ToList();
                }
                if (inputId > 0)
                {
                    lookupTypes = lookupTypes.Where(x => x.InputId != inputId).ToList();
                }

                if (lookupTypes != null && lookupTypes.Count > 0)
                {
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.lookupTypes = new SelectList(lookupTypes.ToArray(), "ID", "Label");
                    }
                    else
                    {
                        ViewBag.lookupTypes = new SelectList(lookupTypes.ToArray(), "ID", "LabelAr");
                    }
                }
                return Json(ViewBag.lookupTypes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        [HttpPost]
        public JsonResult AjaxGetInputDrilldownOLD(int inputId)
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetInputDrilldownOLD(inputId);
                if (result != null && result.Count > 0)
                {
                    html += @"<div class='xtable' id='results'>
                              <div class='theader'>
                                <div class='xtable_header'>Referral</div>
                                <div class='xtable_header'>Operator</div>
                                <div class='xtable_header'>Value</div>
                                <div class='xtable_header'>&nbsp;</div>
                              </div>";

                    foreach (var i in result)
                    {
                        html += "<div class='xtable_row'>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>Referral</div>";
                        html += "       <div class='xtable_cell'>" + i.ReferralName + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>Operator</div>";
                        html += "       <div class='xtable_cell'>" + i.LogicalOperator + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>Value</div>";
                        html += "       <div class='xtable_cell'>" + i.ReferralIdValue + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>&nbsp;</div>";
                        html += "       <div class='xtable_cell'>";

                        //if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        //{
                        //    html += "<a href='#' onclick=\"ModifyDrilldown('{0}','{1}','{2}','{3}','{4}');\"  class='link'>Edit</a>";
                        //}
                        //else
                        //{
                        //    html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','');\"  class='link'>Edit</a>";
                        //}
                        //html += "&nbsp;|&nbsp;";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteInputDrilldown('{0}','{1}');\"  class='link'>Delete</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\"  class='link'>Delete</a>";
                        }

                        html += "       </div>";
                        html += "   </div>";

                        html += "</div>";

                        html = string.Format(html, i.DrilldownId, i.ReferralName, i.ReferralId, i.LogicalOperator, i.ReferralIdValue);
                    }

                    html += "</div>";
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
        public JsonResult AjaxGetInputDrilldown(string InputControlId)
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetInputDrilldown(InputControlId);
                if (result != null && result.Count > 0)
                {
                    html += @"<div class='xtable' id='results'>
                              <div class='theader'>
                                <div class='xtable_header'>Referral</div>
                                <div class='xtable_header'>Operator</div>
                                <div class='xtable_header'>Value</div>
                                <div class='xtable_header'>&nbsp;</div>
                              </div>";

                    foreach (var i in result)
                    {
                        html += "<div class='xtable_row'>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblReferral", "Referral") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.ReferralName + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblOperator", "Operator") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.LogicalOperator + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblValue", "Value") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.ReferralIdValue + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>&nbsp;</div>";
                        html += "       <div class='xtable_cell'>";

                        //if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ADMIN") == true)
                        //{
                        //    html += "<a href='#' onclick=\"ModifyDrilldown('{0}','{1}','{2}','{3}','{4}');\"  class='link'>Edit</a>";
                        //}
                        //else
                        //{
                        //    html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','');\"  class='link'>Edit</a>";
                        //}
                        //html += "&nbsp;|&nbsp;";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_SERVICE_FIELD") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteInputDrilldown('{0}','{1}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "       </div>";
                        html += "   </div>";

                        html += "</div>";

                        html = string.Format(html, i.DrilldownId, i.ReferralName, i.ReferralId, i.LogicalOperator, i.ReferralIdValue);
                    }

                    html += "</div>";
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
        public JsonResult AjaxSaveInputDrilldown(Models.ServiceFormsDrilldownConfig iObject)
        {
            try
            {
                iObject = iObject.TrimObject();

                //var iInputConfigRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.InputId == iObject.InputId);
                var iInputConfigRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.ID == iObject.InputControlId);

                var iRecord = iDbContext.ServiceFormsDrilldownConfigs.SingleOrDefault(x => x.DrilldownId == iObject.DrilldownId);
                if (iRecord != null)
                {
                    //var b = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputId == iRecord.InputId && x.ReferralId == iObject.ReferralId && x.LogicalOperator == iObject.LogicalOperator && x.ReferralIdValue == iObject.ReferralIdValue).FirstOrDefault().TrimObject();
                    //if (b != null)
                    //    return Json(new { Result = false, Message = " Duplicate filter.", Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.InputControlId = iObject.InputControlId;
                    iRecord.ReferralId = iObject.ReferralId;
                    iRecord.ReferralIdValue = iObject.ReferralIdValue;
                    iRecord.LogicalOperator = iObject.LogicalOperator;
                    iRecord.IsDeleted = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.RowUpdatedBy = Common.Security.GetUser().UserName;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var referralRecord = iDbContext.ServiceFormsConfigs.Where(x => x.ServiceId == iInputConfigRecord.ServiceId && x.InputTypeId == (int)FormInputTypes.Dropdownlist && x.ID == iObject.ReferralId).FirstOrDefault();
                    if (referralRecord != null)
                    {
                        referralRecord.HasDrilldown = true;
                        iDbContext.ServiceFormsConfigs.AddOrUpdate(referralRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " UPDATED SERVICE INPUT DRILLDOWN = " + iRecord.InputId + "-" + iRecord.ReferralId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    //var b = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputId == iObject.InputId && x.ReferralId == iObject.ReferralId && x.LogicalOperator == iObject.LogicalOperator && x.ReferralIdValue == iObject.ReferralIdValue).FirstOrDefault().TrimObject();
                    var b = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputControlId == iObject.InputControlId && x.ReferralId == iObject.ReferralId && x.LogicalOperator == iObject.LogicalOperator && x.ReferralIdValue == iObject.ReferralIdValue).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = DbManager.GetText("Services", "lblDuplicateFilter", "Duplicate filter."), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.ServiceFormsDrilldownConfig();

                    iRecord.InputId = iObject.InputId;
                    iRecord.InputControlId = iObject.InputControlId;
                    iRecord.ReferralId = iObject.ReferralId;
                    iRecord.ReferralIdValue = iObject.ReferralIdValue;
                    iRecord.LogicalOperator = iObject.LogicalOperator;
                    iRecord.IsDeleted = false;
                    iRecord.RowInsertedBy = Common.Security.GetUser().UserName;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertDate = DateTime.Now;
                    iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var referralRecord = iDbContext.ServiceFormsConfigs.Where(x => x.ServiceId == iInputConfigRecord.ServiceId && x.InputTypeId == (int)FormInputTypes.Dropdownlist && x.ID == iObject.ReferralId).FirstOrDefault();
                    if (referralRecord != null)
                    {
                        referralRecord.HasDrilldown = true;
                        iDbContext.ServiceFormsConfigs.AddOrUpdate(referralRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " ADDED SERVICE INPUT DRILLDOWN = " + iRecord.InputId + "-" + iRecord.ReferralId;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveFilter", "Input drilldown Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteInputDrilldown(int did)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsDrilldownConfigs.SingleOrDefault(x => x.DrilldownId == did);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var referralRecord = iDbContext.ServiceFormsConfigs.Where(x => x.ID == iRecord.ReferralId).FirstOrDefault();
                    if (referralRecord != null)
                    {
                        referralRecord.HasDrilldown = false;
                        iDbContext.ServiceFormsConfigs.AddOrUpdate(referralRecord);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = referralRecord.ServiceId;
                        aut.CategoryId = referralRecord.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == referralRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED INPUT DRILLDOWN = " + iRecord.ReferralId;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDeleteFilter", "Service Tab Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ServiceTabs
        [UrlDecode]
        public ActionResult ServiceTabs(int serviceId)
        {
            Models.ServiceObject iObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                ViewBag.FormMode = "Edit";
                iObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasSubServices = false;

                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                    ViewBag.HasSubServices = true;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iObject);
        }

        [HttpPost]
        public JsonResult AjaxGetServiceTabs(int serviceId)
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetServiceTabs(serviceId);
                if (result != null && result.Count > 0)
                {
                    int nctr = 1;
                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle' title='{0}'>"+ nctr + "</div>";
                        html += "<div class='title nameEn'>{1}</div>";
                        html += "<div class='title nameAr'>{2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_TAB") == true)
                        {
                            html += "<a href='#' onclick=\"ModifyTab('{0}','{1}','{2}','');\"  class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}','');\"  class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_TAB") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteServiceTab('{0}','"+ name + "');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','" + name + "');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";

                        html = string.Format(html, i.TabId, i.DescriptionEng, i.DescriptionAlt);
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

        [HttpPost]
        public JsonResult AjaxSaveServiceTabs(Models.ServiceTab iObject)
        {
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.ServiceTabs.SingleOrDefault(x => x.TabId == iObject.TabId);
                if (iRecord != null)
                {
                    var b = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.TabId != iRecord.TabId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.TabId != iRecord.TabId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.TabId = iObject.TabId;
                    iRecord.ServiceId = iObject.ServiceId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceTabs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = null;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " UPDATED SERVICE TAB = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }
                }
                else
                {
                    var b = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.DescriptionAlt.ToUpper() == iObject.DescriptionAlt.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.DescriptionAlt + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == iObject.ServiceId && x.DescriptionEng.ToUpper() == iObject.DescriptionEng.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.DescriptionEng + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.ServiceTab();

                    var i = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false).ToList();
                    if (i != null && i.Count > 0)
                        Code = i[i.Count - 1].TabId + 1;

                    iRecord.TabId = Code;
                    iRecord.ServiceId = iObject.ServiceId;
                    iRecord.DescriptionEng = iObject.DescriptionEng;
                    iRecord.DescriptionAlt = iObject.DescriptionAlt;
                    iRecord.IsDeleted = false;
                    iRecord.RowInsertDate = DateTime.Now;
                    iRecord.SortOrder = Code;
                    iDbContext.ServiceTabs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = null;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ADDED SERVICE TAB = " + iRecord.DescriptionEng;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveTab", "Service Tab Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteServiceTabs(int tabId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceTabs.SingleOrDefault(x => x.TabId == tabId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceTabs.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var i = iDbContext.ServiceFormsConfigs.Where(x => x.TabId == tabId).ToList();
                    if (i != null)
                    {
                        foreach (var j in i)
                        {
                            j.TabId = null;
                            j.RowUpdateDate = DateTime.Now;
                            j.IpAddress = Common.Security.GetIpAddress();
                            j.RowUpdatedBy = Security.GetUser().UserName;
                            iDbContext.ServiceFormsConfigs.AddOrUpdate(j);
                            iDbContext.SaveChanges();
                        }
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = null;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE TAB = " + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelTab", "Service Tab Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> Lookup Action

        [HttpPost]
        public JsonResult GetInputLookupActions(string inputId, int lookupId, int? actionId, string filterValue)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            Models.ServiceInputLookupAction iRecord = null;
            string externalServieId = "";
            try
            {
                //var iActions = DbManager.GetInputLookupActionObject(inputId, lookupId);
                //if (iActions != null && iActions.Options != null && iActions.Options.Count > 0)

                var iInputDetails = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.ID == inputId).FirstOrDefault();

                var iLookupDetails = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookupId == lookupId).FirstOrDefault();

                var iServiceDetails = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == iInputDetails.ServiceId).FirstOrDefault();
                if (iServiceDetails.UseParentExternalServiceId == false)
                {
                    if (string.IsNullOrEmpty(iServiceDetails.ParentExternalServiceID) == false)
                        externalServieId = iServiceDetails.ParentExternalServiceID;
                    else
                        externalServieId = iServiceDetails.ExternalServiceID;
                }
                else
                {
                    externalServieId = iServiceDetails.ExternalServiceID;
                }                

                var iActions = iDbContext.ServiceInputLookupActions.Where(x => x.IsDeleted == false && x.InputControlId == inputId && x.LookupId == lookupId).ToList();
                if (actionId.HasValue)
                    iActions = iActions.Where(x => x.ActionId == actionId).ToList();
                if (iActions != null && iActions.Count > 0)
                {
                    iRecord = iActions[0];
                    html = DbManager.GenerateLookUpValues(lookupId, iActions, externalServieId, iLookupDetails.LookUpName, filterValue, iInputDetails.IsExternalLookup);
                }
                else
                {
                    html = DbManager.GenerateLookUpValues(lookupId, null, externalServieId, iLookupDetails.LookUpName, filterValue, iInputDetails.IsExternalLookup);
                }

                iResult = new JsonResult();
                if (iRecord != null)
                    iResult.Data = new { result = true, html = html, actionId = iRecord.ActionId, remarks = iRecord.DescriptionEng, remarksAr = iRecord.DescriptionAlt };
                else
                    iResult.Data = new { result = true, html = html, actionId = "", remarks = "", remarksAr = "" };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, html = E.Message, actionId = "", remarks = "", remarksAr = "", Icon = "error" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, html = "", url = "", actionId = "", remarks = "", remarksAr = "", Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxSaveLookupAction(Models.LkLkLoopUpActionObject iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    //var iRecords = iDbContext.ServiceInputLookupActions.Where(x => x.InputId == iObject.InputId && x.LookupId == iObject.LookupId && x.ActionId == iObject.ActionId).ToList();
                    var iRecords = iDbContext.ServiceInputLookupActions.Where(x => x.InputControlId == iObject.InputControlId && x.LookupId == iObject.LookupId && x.ActionId == iObject.ActionId).ToList();
                    if (iRecords != null && iRecords.Count > 0)
                    {
                        iDbContext.ServiceInputLookupActions.RemoveRange(iRecords);
                        iDbContext.SaveChanges();
                    }

                    foreach (var iRecord in iObject.LookupValues)
                    {
                        var i = new Models.ServiceInputLookupAction();
                        i.ActionId = iObject.ActionId;
                        i.InputControlId = iObject.InputControlId;
                        i.DescriptionAlt = iRecord.DescriptionAlt;
                        i.DescriptionEng = iRecord.DescriptionEng;
                        i.InputId = iObject.InputId;
                        i.LookupId = iObject.LookupId;
                        i.LookupValue = iRecord.Code;
                        i.IsDeleted = false;
                        i.RowInsertDate = DateTime.Now;
                        iDbContext.ServiceInputLookupActions.AddOrUpdate(i);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = iObject.LookupId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " ADDED LOOKUP ACTION RECORD = " + iObject.LookupId;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveAction", " Action Record Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeletLookupAction(string inputId, int lookupId, int? actionId)
        {
            DbContextTransaction iTransaction = null;
            bool isValid = false;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecords = iDbContext.ServiceInputLookupActions.Where(x => x.InputControlId == inputId && x.LookupId == lookupId).ToList();
                    if (actionId.HasValue )
                        iRecords = iRecords.Where(x => x.ActionId == actionId).ToList();

                    if (iRecords != null && iRecords.Count > 0)
                    {
                        isValid = true;
                        iDbContext.ServiceInputLookupActions.RemoveRange(iRecords);
                        iDbContext.SaveChanges();

                        if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                        {
                            var aut = new Models.UserScreenAction();
                            aut.ActionId = 7;
                            aut.UniqueId = lookupId.ToString();
                            aut.Remarks = Security.GetUser().UserName + " DELETED LOOKUP ACTION RECORD = " + lookupId;
                            aut.RowInsertDate = DateTime.Now;
                            aut.IpAddress = Common.Security.GetIpAddress();
                            aut.RowInsertedBy = Security.GetUser().UserName;
                            aut.IsDeleted = false;
                            iDbContext.UserScreenActions.AddOrUpdate(aut);
                            iDbContext.SaveChanges();
                        }
                    }
                    iTransaction.Commit();

                    if (!isValid)
                        return Json(new { Result = true, Message = DbManager.GetText("Services", "lblNoAction", "No Actions to Delete !"), Icon = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelAction", "Action Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> Service Viewers
        [HttpPost]
        public JsonResult AjaxGetViewers(int serviceId)
        {
            string html = "";
            int nCtr = 0;
            try
            {
                var result = Common.DbManager.GetServiceViewer(serviceId);
                if (result != null && result.Count > 0)
                {
                    html += @"<div class='xtable' id='results'>
                              <div class='theader'>
                                <div class='xtable_header'>S.No.</div>
                                <div class='xtable_header'>Viewer</div>
                                <div class='xtable_header'>&nbsp;</div>
                              </div>";

                    foreach (var i in result)
                    {
                        html += "<div class='xtable_row'>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblSno", "S.No.") + "</div>";
                        html += "       <div class='xtable_cell'>" + (nCtr + 1) + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblViewer", "Viewer") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.EmailAddress + "</div>";
                        html += "   </div>";


                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>&nbsp;</div>";
                        html += "       <div class='xtable_cell'>";

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_SERVICE") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteViewer('{0}','{1}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "       </div>";
                        html += "   </div>";

                        html += "</div>";

                        html = string.Format(html, i.RecordId, i.EmailAddress);
                    }

                    html += "</div>";
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
        public JsonResult AjaxAddViewers(int serviceId, string viewerId)
        {
            try
            {
                var isExists = iDbContext.ServiceViewers.SingleOrDefault(x => x.EmailAddress == viewerId && x.IsDeleted == false && x.ServiceId == serviceId);
                if (isExists != null)
                {
                    return Json(new { Result = false, Message = DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);
                }
                var iRecord = new Models.ServiceViewer();
                iRecord.EmailAddress = Helper.RefineString(viewerId);
                iRecord.ServiceId = serviceId;
                iRecord.IsDeleted = false;
                iRecord.RowInsertDate = DateTime.Now;
                iRecord.TrimObject();
                iDbContext.ServiceViewers.AddOrUpdate(iRecord);
                iDbContext.SaveChanges();


                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 7;
                    aut.ServiceId = iRecord.ServiceId;
                    aut.CategoryId = null;
                    aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                    aut.Remarks = Security.GetUser().UserName + " ADDED SERVICE VIEWER = " + viewerId;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveViewer", "Viewer Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteViewer(int did)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceViewers.SingleOrDefault(x => x.RecordId == did);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceViewers.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = null;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE VIEWER = " + iRecord.EmailAddress;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelViewer", "Service Viewer Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteAllViewers(int serviceId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceViewers.Where(x => x.ServiceId == serviceId && x.IsDeleted == false).ToList();
                    if (iRecord != null && iRecord.Count > 0)
                    {
                        foreach (var i in iRecord)
                        {
                            i.IsDeleted = true;
                            i.RowUpdateDate = DateTime.Now;
                            iDbContext.ServiceViewers.AddOrUpdate(i);
                            iDbContext.SaveChanges();
                        }

                        if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                        {
                            var aut = new Models.UserScreenAction();
                            aut.ActionId = 7;
                            aut.ServiceId = serviceId;
                            aut.CategoryId = null;
                            aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == serviceId).FirstOrDefault().ExternalServiceID;
                            aut.Remarks = Security.GetUser().UserName + " DELETED ALL VIEWERS FOR SERVICE = " + aut.UniqueId;
                            aut.RowInsertDate = DateTime.Now;
                            aut.IpAddress = Common.Security.GetIpAddress();
                            aut.RowInsertedBy = Security.GetUser().UserName;
                            aut.IsDeleted = false;
                            iDbContext.UserScreenActions.AddOrUpdate(aut);
                            iDbContext.SaveChanges();
                        }
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelViewer", "Service Viewer Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region --> Templates

        [UrlDecode]
        public ActionResult Templates(int serviceId)
        {
            Models.ServiceObject iServiceObject  = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                iServiceObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasSubServices = false;
                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                {
                    ViewBag.HasSubServices = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.HasSubServices = false;
                    SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }

                ViewBag.HasClassfication = false;
                ViewBag.Classifications = null;
                var Classifications = Common.DbManager.GetServiceClassifications(serviceId);
                if (Classifications != null && Classifications.Count > 0)
                {
                    ViewBag.HasClassfication = true;
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.Classifications = null;
                }

                //var InputsList = Common.DbManager.GetServiceFormsConfig(serviceId, 0, (int)FormInputTypes.Attachment, 0);
                //if (InputsList != null && InputsList.Count > 0)
                //{
                //    if (Helper.CurrentLanguage() == (int)Language.English)
                //    {
                //        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "Label");
                //    }
                //    else
                //    {
                //        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "LabelAr");
                //    }
                //}
                //else
                {
                    var yesNo = new List<Models.ServiceFormsConfigObject>();
                    //yesNo.Add(1, "Yes");
                    ViewBag.InputsList = new SelectList(yesNo.ToArray());
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iServiceObject);
        }

        [HttpPost]
        public JsonResult SaveTemplate(Models.ServiceTemplateRequest j)
        {
            DbContextTransaction iTransaction = null;
            string fileName = "";
            string fileNameAlt = "";
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    //var iRecord = iDbContext.ServiceTemplates.SingleOrDefault(x => x.IsDeleted == false && x.ServiceId == j.ServiceId && x.CategoryId == j.CategoryId && x.InputId == j.InputId);
                    var iRecord = iDbContext.ServiceTemplates.SingleOrDefault(x => x.IsDeleted == false && x.ServiceId == j.ServiceId && x.CategoryId == j.CategoryId && x.InputControlId == j.InputControlId);
                    if (iRecord != null)
                    {
                        iRecord.IsDeleted = true;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iDbContext.ServiceTemplates.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();
                    }

                    //string serverPath = System.Web.HttpContext.Current.Server.MapPath("/Templates/");
                    string serverPath = WebConfig.GetStringValue("TemplateUploadFilePath");

                    int storageMode = WebConfig.GetIntValue("ServiceTemplates_Source");

                    Stream fs = null;
                    BinaryReader br = null;
                    byte[] enByteArray = null;
                    byte[] arByteArray = null;

                    bool bHasFiles = false;

                    //Reading English Template
                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null)
                    {
                        System.IO.FileInfo f = new System.IO.FileInfo(file.FileName);

                        fs = file.InputStream;
                        br = new BinaryReader(fs);
                        enByteArray = br.ReadBytes((Int32)fs.Length);

                        //if (j.CategoryId <=0)
                        //    fileName = iDbContext.Services.SingleOrDefault(x=>x.ServiceId == j.ServiceId).ExternalServiceID + f.Extension;
                        //else
                        //    fileName = iDbContext.Services.SingleOrDefault(x => x.ServiceId == j.ServiceId).ExternalServiceID + "_" + j.CategoryId + f.Extension;

                        fileName = iDbContext.Services.SingleOrDefault(x => x.ServiceId == j.ServiceId).ExternalServiceID;
                        if (j.CategoryId > 0)
                            fileName += "-SC-"+ j.CategoryId;
                        if (string.IsNullOrEmpty(j.InputControlId) == false)
                            fileName += "-IN-" + j.InputControlId;

                        fileName = fileName + "_EN" + f.Extension;

                        switch(storageMode)
                        {
                            case (int)TemplatesSources.Folder:

                                if (System.IO.File.Exists(serverPath + fileName))
                                {
                                    System.IO.File.Delete(serverPath + fileName);
                                }
                                System.IO.Stream fileContent = file.InputStream;
                                file.SaveAs(serverPath + fileName);

                                break;
                        }
                    }
                    //Reading ARabic Template
                    file = Request.Files[1];
                    if (file != null)
                    {
                        System.IO.FileInfo f = new System.IO.FileInfo(file.FileName);

                        fs = file.InputStream;
                        br = new BinaryReader(fs);
                        arByteArray = br.ReadBytes((Int32)fs.Length);

                        fileNameAlt = iDbContext.Services.SingleOrDefault(x => x.ServiceId == j.ServiceId).ExternalServiceID;
                        if (j.CategoryId > 0)
                            fileNameAlt += "-SC-" + j.CategoryId;
                        if (string.IsNullOrEmpty(j.InputControlId) == false)
                            fileNameAlt += "-IN-" + j.InputControlId;

                        fileNameAlt = fileNameAlt + "_AR" + f.Extension;

                        switch (storageMode)
                        {
                            case (int)TemplatesSources.Folder:

                                if (System.IO.File.Exists(serverPath + fileNameAlt))
                                {
                                    System.IO.File.Delete(serverPath + fileNameAlt);
                                }
                                System.IO.Stream fileContent = file.InputStream;
                                file.SaveAs(serverPath + fileNameAlt);

                                break;
                        }
                        bHasFiles = true;
                    }

                    if (bHasFiles)
                    {
                        iRecord = new Models.ServiceTemplate();
                        iRecord.ServiceId = j.ServiceId;
                        iRecord.CategoryId = j.CategoryId;
                        iRecord.InputId = j.InputId;
                        iRecord.InputControlId = j.InputControlId;
                        iRecord.Name = j.Name;

                        //iRecord.FileUrl = WebConfig.GetStringValue("TemplateDownloadFilePath") + fileName;
                        //iRecord.FileUrlAlt = WebConfig.GetStringValue("TemplateDownloadFilePath") + fileNameAlt;

                        iRecord.FileUrl = fileName;
                        iRecord.FileUrlAlt = fileNameAlt;
                        iRecord.IsDeleted = false;
                        iRecord.RowInsertDate = DateTime.Now;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowInsertedBy = Security.GetUser().UserName;
                        iDbContext.ServiceTemplates.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();

                        switch (storageMode)
                        {
                            case (int)TemplatesSources.Database:

                                var std = new Models.ServiceTemplatesData();
                                std.TemplateId = iRecord.TemplateId;
                                std.Language = "en";
                                std.FileName = fileName;
                                std.FileType = FileHelper.GetContentType(fileName);
                                std.FileSize = FileHelper.CalculateFileSize(enByteArray.Length);
                                std.FileData = Convert.ToBase64String(enByteArray);
                                std.IsDeleted = false;
                                std.RowInsertDate = DateTime.Now;
                                std.IpAddress = Common.Security.GetIpAddress();
                                std.RowInsertedBy = Security.GetUser().UserName;
                                iDbContext.ServiceTemplatesDatas.AddOrUpdate(std);
                                iDbContext.SaveChanges();

                                std = new Models.ServiceTemplatesData();
                                std.TemplateId = iRecord.TemplateId;
                                std.Language = "ar";
                                std.FileName = fileNameAlt;
                                std.FileType = FileHelper.GetContentType(fileNameAlt);
                                std.FileSize = FileHelper.CalculateFileSize(arByteArray.Length);
                                std.FileData = Convert.ToBase64String(arByteArray);
                                std.IsDeleted = false;
                                std.RowInsertDate = DateTime.Now;
                                std.IpAddress = Common.Security.GetIpAddress();
                                std.RowInsertedBy = Security.GetUser().UserName;
                                iDbContext.ServiceTemplatesDatas.AddOrUpdate(std);
                                iDbContext.SaveChanges();

                                break;
                        }
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = j.ServiceId;
                        aut.CategoryId = j.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ADDED NEW SERVICE TEMPLATE  = " + iRecord.Name + "-" + fileName;
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
            catch (DbEntityValidationException e)
            {
                if (iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
                sb.AppendLine();
                return Json(new { Result = false, Message = sb.ToString(), Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Dispose();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveTemplate", "Template Uploaded Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxGetTemplates(int serviceId, int categoryId, string inputId)
        {
            string html = "";
            int nCtr = 0;
            try
            {
                var result = iDbContext.ServiceTemplates.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (result != null && result.Count > 0)
                {
                    if (categoryId > 0)
                        result = result.Where(x => x.IsDeleted == false && x.ServiceId == serviceId && x.CategoryId == categoryId).ToList();

                    if (string.IsNullOrEmpty(inputId) == false)
                        result = result.Where(x => x.IsDeleted == false && x.ServiceId == serviceId && x.CategoryId == categoryId && x.InputControlId == inputId).ToList();
                    else
                    {
                        var x = Json(new { success = true, result = string.Empty }, JsonRequestBehavior.AllowGet);
                        x.MaxJsonLength = Int32.MaxValue;
                        return x;
                    }

                    //if (inputId > 0)
                    //    result = result.Where(x => x.IsDeleted == false && x.ServiceId == serviceId && x.CategoryId == categoryId && x.InputId == inputId).ToList();

                    html += @"<div class='xtable' id='results'>
                              <div class='theader'>
                                <div class='xtable_header'>Name</div>
                                <div class='xtable_header'>&nbsp;</div>
                              </div>";

                    foreach (var i in result)
                    {
                        html += "<div class='xtable_row'>";

                        //html += "   <div class='xtable_small'>";
                        //html += "       <div class='xtable_cell'>S.No.</div>";
                        //html += "       <div class='xtable_cell'>" + i.TemplateId + "</div>";
                        //html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>Name</div>";
                        html += "       <div class='xtable_cell'>" + i.Name + "</div>";
                        html += "   </div>";

                        //string downloadUrlEn = Helper.URL_Encode(Server.MapPath("~/Common/Download.ashx?templateId="+i.TemplateId+"&language=en"));
                        //string downloadUrlAr = Helper.URL_Encode(Server.MapPath("~/Common/Download.ashx?templateId="+i.TemplateId+"&language=ar"));

                        string downloadUrlEn = Helper.URL_Encode(Url.Content("~/Common/DownloadFile.ashx?templateId=" + i.TemplateId + "&language=en"));
                        string downloadUrlAr = Helper.URL_Encode(Url.Content("~/Common/DownloadFile.ashx?templateId=" + i.TemplateId + "&language=ar"));

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>&nbsp;</div>";
                        html += "       <div class='xtable_cell'>";
                        //html += "           <a href='#' onclick=\"DownloadTemplate('{2}');\"  class='link'>" + DbManager.GetText("Services", "lblDownloadEn", "Download (EN)") + "</a>";
                        html += "           <a href='"+ downloadUrlEn + "' target='_blank' class='link'>" + DbManager.GetText("Services", "lblDownloadEn", "Download (EN)") + "</a>";
                        html += "&nbsp;|&nbsp;";
                        html += "           <a href='" + downloadUrlAr + "' target='_blank'  class='link'>" + DbManager.GetText("Services", "lblDownloadAr", "Download (AR)") + "</a>";
                        //html += "           <a href='#' onclick=\"DownloadTemplate('{3}');\"  class='link'>" + DbManager.GetText("Services", "lblDownloadAr", "Download (AR)") + "</a>";

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_TEMPLATE") == true)
                        {
                            html += "&nbsp;|&nbsp;";
                            html += "<a href='#' onclick=\"DeleteTemplate('{0}','{1}','{2}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{2}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "       </div>";
                        html += "   </div>";

                        html += "</div>";

                        html = string.Format(html, i.TemplateId, i.Name, i.FileUrl,i.FileUrlAlt);
                    }

                    html += "</div>";
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
        public JsonResult AjaxDeleteTemplate(int templateId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceTemplates.SingleOrDefault(x => x.TemplateId == templateId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceTemplates.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var std = iDbContext.ServiceTemplatesDatas.Where(x => x.TemplateId == templateId).ToList();
                    if (std != null && std.Count > 0)
                    {
                        iDbContext.ServiceTemplatesDatas.RemoveRange(std);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE TEMPLATE = " + iRecord.Name;
                        aut.RowInsertDate = DateTime.Now;
                        aut.IpAddress = Common.Security.GetIpAddress();
                        aut.RowInsertedBy = Security.GetUser().UserName;
                        aut.IsDeleted = false;
                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                        iDbContext.SaveChanges();
                    }

                    try
                    {
                        var fileUrl = new Uri(iRecord.FileUrl);
                        string serverPath = WebConfig.GetStringValue("TemplateUploadFilePath");
                        string fileName = System.IO.Path.GetFileName(fileUrl.LocalPath);

                        if (System.IO.File.Exists(serverPath + fileName))
                        {
                            System.IO.File.Delete(serverPath + fileName);
                        }
                    }
                    catch { }

                    iTransaction.Commit();
                }
            }
            catch (Exception E)
            {
                if (iTransaction != null)
                    iTransaction.Rollback();

                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelTemplate", "Service Template Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadServiceClassificationsAttachmentInputs(int serviceId, int categoryId)
        {
            try
            {
                var InputsList = Common.DbManager.GetServiceFormsConfig(serviceId, categoryId, (int)FormInputTypes.Attachment, 0);
                if (InputsList != null && InputsList.Count > 0)
                {
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "Label");
                    }
                    else
                    {
                        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "LabelAr");
                    }
                }
                return Json(ViewBag.InputsList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//


        #endregion

        #region --> Tooltips

        [UrlDecode]
        public ActionResult Tooltip(int serviceId)
        {
            Models.ServiceObject iServiceObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                iServiceObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasSubServices = false;
                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                {
                    ViewBag.HasSubServices = true;
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.HasSubServices = false;
                    SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }

                ViewBag.HasClassfication = false;
                ViewBag.Classifications = null;
                var Classifications = Common.DbManager.GetServiceClassifications(serviceId);
                if (Classifications != null && Classifications.Count > 0)
                {
                    ViewBag.HasClassfication = true;
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.Classifications = new SelectList(Classifications.ToArray(), "CategoryId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.Classifications = null;
                }

                var InputsList = Common.DbManager.GetServiceFormsConfig(-1, 0, 0, 0);
                if (InputsList != null && InputsList.Count > 0)
                {
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "Label");
                    }
                    else
                    {
                        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "LabelAr");
                    }
                }
                else
                {
                    IDictionary<int, string> yesNo = new Dictionary<int, string>();
                    yesNo.Add(0, "-");

                    ViewBag.InputsList = new SelectList(yesNo.OrderBy(x => x.Value), "Key", "Value", 0);
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iServiceObject);
        }

        [HttpPost]
        public JsonResult SaveTooltip(Models.ServiceInputTooltip j)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.IsDeleted == false && x.ID == j.InputControlId);
                    var iService = iDbContext.Services.SingleOrDefault(x => x.IsDeleted == false && x.ServiceId == iRecord.ServiceId);
                    var iCategory = iDbContext.ServiceCategories.SingleOrDefault(x => x.IsDeleted == false && x.ServiceId == iRecord.ServiceId && x.CategoryId == iRecord.CategorId);

                    var iRecords = iDbContext.ServiceInputTooltips.Where(x => x.InputControlId == j.InputControlId).FirstOrDefault();
                    if (iRecords != null)
                    {
                        iDbContext.ServiceInputTooltips.Remove(iRecords);
                        iDbContext.SaveChanges();
                    }

                    j.TypeId = 1;
                    j.IsDeleted = false;
                    j.RowInsertDate = DateTime.Now;
                    iDbContext.ServiceInputTooltips.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategorId;
                        aut.UniqueId = iService.ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ADDED NEW SERVICE TOOLTIP  = " + j.InputId + "-" + j.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveTooltip", "Tooltip Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxGetToolTips(int serviceId, int categoryId, string inputId)
        {
            string html = "";
            try
            {
                var result = iDbContext.ServiceInputTooltips.Where(x => x.IsDeleted == false && x.InputControlId == inputId).ToList();
                if (result != null && result.Count > 0)
                {
                    html += @"<div class='xtable' style='width:100%;' id='results'>
                              <div class='theader'>
                                <div class='xtable_header' style='width:35%;'>Tooltip English</div>
                                <div class='xtable_header' style='width:35%;'>Tooltip Arabic</div>
                                <div class='xtable_header' style='width:10%;'>Guide</div>
                                <div class='xtable_header' style='width:10%;'>Card</div>
                                <div class='xtable_header' style='width:10%;'>&nbsp;</div>
                              </div>";

                    foreach (var i in result)
                    {
                        string name = Helper.CurrentLanguage() == 1 ? i.DescriptionEng : i.DescriptionAlt;
                        html += "<div class='xtable_row'>";

                        //html += "   <div class='xtable_small'>";
                        //html += "       <div class='xtable_cell'>S.No.</div>";
                        //html += "       <div class='xtable_cell'>" + i.TemplateId + "</div>";
                        //html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblToolTipDesc", "Description") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.DescriptionEng + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblToolTipDescAr", "Description Ar") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.DescriptionAlt + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblToolTipDGuide", "Guide") + "</div>";
                        html += "       <div class='xtable_cell'>" + (i.EnableGuide == true ? "Yes":"No") + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblToolTipDCard", "Card") + "</div>";
                        html += "       <div class='xtable_cell'>" + (i.EnableCard == true ? "Yes" : "No") + "</div>";
                        html += "   </div>";


                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>&nbsp;</div>";
                        html += "       <div class='xtable_cell'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_TOOLTIP") == true)
                        {
                            //html += "&nbsp;|&nbsp;";
                            html += "<a href='#' onclick=\"DeleteTooltip('{0}','"+name+"');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','" + name + "');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "       </div>";
                        html += "   </div>";

                        html += "</div>";

                        html = string.Format(html, i.TooltipId, name);
                    }

                    html += "</div>";
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
        public JsonResult AjaxDeleteTooltip(int tooltipId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceInputTooltips.SingleOrDefault(x => x.TooltipId == tooltipId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.ServiceInputTooltips.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var iInput = iDbContext.ServiceFormsConfigs.SingleOrDefault(x => x.IsDeleted == false && x.ID == iRecord.InputControlId);

                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iInput.ServiceId;
                        aut.CategoryId = iInput.CategorId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iInput.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE TOOLTIP = " + iRecord.InputId + "-" + iRecord.DescriptionEng;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelTooltip", "Service Template Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadServiceClassificationsInputs(int serviceId, int categoryId)
        {
            try
            {
                var InputsList = Common.DbManager.GetServiceFormsConfig(serviceId, categoryId, 0, 0);
                if (InputsList != null && InputsList.Count > 0)
                {
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "Label");
                    }
                    else
                    {
                        ViewBag.InputsList = new SelectList(InputsList.ToArray(), "ID", "LabelAr");
                    }
                }
                return Json(ViewBag.InputsList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                throw E;
            }
        }//

        #endregion

        #region --> Videos

        [UrlDecode]
        public ActionResult Videos(int serviceId)
        {
            Models.ServiceObject iServiceObject = new Models.ServiceObject();
            try
            {
                ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts,ServicesClientAlerts");

                iServiceObject = Common.DbManager.GetService(serviceId);

                ViewBag.HasSubServices = false;
                var SubServices = Common.DbManager.GetSubServices(serviceId);
                if (SubServices != null && SubServices.Count > 0)
                {
                    ViewBag.HasSubServices = true;
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
                else
                {
                    ViewBag.HasSubServices = false;
                    SubServices = Common.DbManager.GetParentAndChildServices(serviceId);
                    
                    if (Helper.CurrentLanguage() == (int)Language.English)
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionEng");
                    }
                    else
                    {
                        ViewBag.SubServices = new SelectList(SubServices.ToArray(), "ServiceId", "DescriptionAlt");
                    }
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return View(iServiceObject);
        }

        [HttpPost]
        public JsonResult SaveVideo(Models.ServiceVideo j)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    j.CategoryId = 0;
                    j.IsDeleted = false;
                    j.RowInsertDate = DateTime.Now;
                    j.RowInsertedBy = Security.GetUser().UserName;
                    iDbContext.ServiceVideos.AddOrUpdate(j);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = j.ServiceId;
                        aut.CategoryId = j.CategoryId;
                        aut.UniqueId = iDbContext.Services.SingleOrDefault(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " ADDED NEW SERVICE VIDEO  = " + j.VideoId + "-" + j.Name;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblSaveVideo", "Video Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxGetVideos(int serviceId)
        {
            string html = "";
            try
            {
                var result = iDbContext.ServiceVideos.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (result != null && result.Count > 0)
                {
                    html += @"<div class='xtable' id='results'>
                              <div class='theader'>
                                <div class='xtable_header'>Name English</div>
                                <div class='xtable_header'>Name Arabic</div>
                                <div class='xtable_header'>&nbsp;</div>
                                <div class='xtable_header'>&nbsp;</div>
                                <div class='xtable_header'>&nbsp;</div>
                              </div>";

                    foreach (var i in result)
                    {
                        html += "<div class='xtable_row'>";

                        //html += "   <div class='xtable_small'>";
                        //html += "       <div class='xtable_cell'>S.No.</div>";
                        //html += "       <div class='xtable_cell'>" + i.TemplateId + "</div>";
                        //html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>"+ DbManager.GetText("Services", "lblVideoNameEn", "Name English") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.Name + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>" + DbManager.GetText("Services", "lblVideoNameAr", "Name Arabic") + "</div>";
                        html += "       <div class='xtable_cell'>" + i.NameAlt + "</div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'></div>";
                        html += "       <div class='xtable_cell'>";
                        html += "       <a class='xbutton' href='"+i.VideoUrl+"' target='_blank'><svg xmlns='http://www.w3.org/2000/svg' width='25' height='25' fill='currentColor' class='bi bi-youtube' viewBox='0 0 16 16'><path d='M8.051 1.999h.089c.822.003 4.987.033 6.11.335a2.01 2.01 0 0 1 1.415 1.42c.101.38.172.883.22 1.402l.01.104.022.26.008.104c.065.914.073 1.77.074 1.957v.075c-.001.194-.01 1.108-.082 2.06l-.008.105-.009.104c-.05.572-.124 1.14-.235 1.558a2.007 2.007 0 0 1-1.415 1.42c-1.16.312-5.569.334-6.18.335h-.142c-.309 0-1.587-.006-2.927-.052l-.17-.006-.087-.004-.171-.007-.171-.007c-1.11-.049-2.167-.128-2.654-.26a2.007 2.007 0 0 1-1.415-1.419c-.111-.417-.185-.986-.235-1.558L.09 9.82l-.008-.104A31.4 31.4 0 0 1 0 7.68v-.123c.002-.215.01-.958.064-1.778l.007-.103.003-.052.008-.104.022-.26.01-.104c.048-.519.119-1.023.22-1.402a2.007 2.007 0 0 1 1.415-1.42c.487-.13 1.544-.21 2.654-.26l.17-.007.172-.006.086-.003.171-.007A99.788 99.788 0 0 1 7.858 2h.193zM6.4 5.209v4.818l4.157-2.408L6.4 5.209z' /></svg></a>";
                        html += "       </div>";
                        html += "   </div>";

                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'></div>";
                        html += "       <div class='xtable_cell'>";
                        html += "       <a class='xbutton' href='" + i.VideoAltUrl + "' target='_blank'><svg xmlns='http://www.w3.org/2000/svg' width='25' height='25' fill='currentColor' class='bi bi-youtube' viewBox='0 0 16 16'><path d='M8.051 1.999h.089c.822.003 4.987.033 6.11.335a2.01 2.01 0 0 1 1.415 1.42c.101.38.172.883.22 1.402l.01.104.022.26.008.104c.065.914.073 1.77.074 1.957v.075c-.001.194-.01 1.108-.082 2.06l-.008.105-.009.104c-.05.572-.124 1.14-.235 1.558a2.007 2.007 0 0 1-1.415 1.42c-1.16.312-5.569.334-6.18.335h-.142c-.309 0-1.587-.006-2.927-.052l-.17-.006-.087-.004-.171-.007-.171-.007c-1.11-.049-2.167-.128-2.654-.26a2.007 2.007 0 0 1-1.415-1.419c-.111-.417-.185-.986-.235-1.558L.09 9.82l-.008-.104A31.4 31.4 0 0 1 0 7.68v-.123c.002-.215.01-.958.064-1.778l.007-.103.003-.052.008-.104.022-.26.01-.104c.048-.519.119-1.023.22-1.402a2.007 2.007 0 0 1 1.415-1.42c.487-.13 1.544-.21 2.654-.26l.17-.007.172-.006.086-.003.171-.007A99.788 99.788 0 0 1 7.858 2h.193zM6.4 5.209v4.818l4.157-2.408L6.4 5.209z' /></svg></a>";
                        html += "       </div>";
                        html += "   </div>";


                        html += "   <div class='xtable_small'>";
                        html += "       <div class='xtable_cell'>&nbsp;</div>";
                        html += "       <div class='xtable_cell'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_VIDEO") == true)
                        {
                            //html += "&nbsp;|&nbsp;";
                            html += "<a href='#' onclick=\"DeleteVideo('{0}','{1}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\"  class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "       </div>";
                        html += "   </div>";

                        html += "</div>";

                        html = string.Format(html, i.VideoId, i.Name);
                    }

                    html += "</div>";
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
        public JsonResult AjaxDeleteVideo(int videoId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.ServiceVideos.SingleOrDefault(x => x.VideoId == videoId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.ServiceVideos.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();
                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.ServiceId = iRecord.ServiceId;
                        aut.CategoryId = iRecord.CategoryId;
                        aut.UniqueId = iDbContext.Services.Where(x => x.ServiceId == iRecord.ServiceId).FirstOrDefault().ExternalServiceID;
                        aut.Remarks = Security.GetUser().UserName + " DELETED SERVICE VIDEO = " + iRecord.VideoId + "-" + iRecord.Name;
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
            return Json(new { Result = true, Message = DbManager.GetText("Services", "lblDelVideo", "Service Video Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> Export
        [UrlDecode]
        public ActionResult ExportService(int serviceId)
        {
            MemoryStream stream = null;
            string contentType = string.Empty;
            string fileDownloadName = string.Empty;
            try
            {
                var iService = iDbContext.Services.Where(x => x.ServiceId == serviceId).FirstOrDefault();

                var iObject = DbManager.GetServiceObjectForImportExport2(serviceId);

                Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
                settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                settings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(iObject, settings);

                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(jsonString);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                {
                    var aut = new Models.UserScreenAction();
                    aut.ActionId = 7;
                    aut.ServiceId = serviceId;
                    aut.CategoryId = 0;
                    aut.UniqueId = iService.ExternalServiceID;
                    aut.Remarks = Security.GetUser().UserName + " EXPORTED SERVICE DETAILS  = " + iService.DescriptionEng + "-" + iService.ExternalServiceID + ".json";
                    aut.RowInsertDate = DateTime.Now;
                    aut.IpAddress = Common.Security.GetIpAddress();
                    aut.RowInsertedBy = Security.GetUser().UserName;
                    aut.IsDeleted = false;
                    iDbContext.UserScreenActions.AddOrUpdate(aut);
                    iDbContext.SaveChanges();
                }

                stream = new MemoryStream(byteArray);
                return new FileContentResult(stream.ToArray(), "application/json")
                {
                    FileDownloadName = iService.ExternalServiceID + ".json"
                };
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return null;
        }

        #endregion

        #region --> Import

        [HttpPost]
        public JsonResult ImportService()
        {
            DbContextTransaction iTransaction = null;
            Models.ImportExport existingRecord = null;
            string serviceId = "";
            string fileName = "";
            bool hasTransaction = false;
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
                    fileName = file.FileName;
                    System.IO.FileInfo f = new System.IO.FileInfo(fileName);

                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes(file.ContentLength);

                    string result = System.Text.Encoding.UTF8.GetString(binData);

                    var importedObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.ImportExport2>(result);
                    if (importedObject != null)
                    {
                        //Step 1 : Check if External Service Id exists on the Target Database...
                        string temp = importedObject.Services[0].ExternalServiceID;
                        var existingObject = iDbContext.Services.Where(x => x.ExternalServiceID == temp).FirstOrDefault();
                        if (existingObject != null)
                        {
                            #region --> UPDATE EXISTING SERVICE IN DB

                            foreach (var Service in importedObject.Services)
                            {
                                using (iTransaction = iDbContext.Database.BeginTransaction())
                                {
                                    hasTransaction = true;

                                    //Step 1.1 : Start Importing the Service and Update the details...
                                    existingRecord = DbManager.GetServiceObjectForImportExport(Service.ServiceId);
                                    //
                                    if (Service.ServiceId == existingRecord.Service.ServiceId)
                                    {
                                        var xService = AutoMapper.ObjectToObjectMapper<Models.IEService, Models.Service>(Service, null);
                                        if (xService != null)
                                        {
                                            xService.IsDeleted = false;
                                            xService.RowUpdateDate = DateTime.Now;
                                            xService.IpAddress = Common.Security.GetIpAddress();
                                            xService.RowUpdatedBy = Security.GetUser().UserName;

                                            //Log Existing Record
                                            var Services_Log = AutoMapper.ObjectToObjectMapper<Models.IEService, Models.Services_Log>(existingRecord.Service, null);
                                            iDbContext.Services_Log.Add(Services_Log);
                                            //

                                            iDbContext.Services.AddOrUpdate(xService);
                                            iDbContext.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        return Json(new { Result = false, Message = "Service " + Service.ServiceId +"&"+ Service.ExternalServiceID + " does not exists.", Icon = "error" }, JsonRequestBehavior.AllowGet);
                                    }
                                    //
                                    var ServiceTabs = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceTab, Models.ServiceTab>(Service.ServiceTabs);
                                    if (ServiceTabs != null && ServiceTabs.Count > 0)
                                    {
                                        if (ServiceTabs.Count == existingRecord.ServiceTabs.Count)
                                        {
                                            //Log Existing Record
                                            var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceTab, Models.ServiceTabs_Log>(existingRecord.ServiceTabs);
                                            iDbContext.ServiceTabs_Log.AddRange(LogRecord);
                                            //

                                            foreach (var ServiceTab in ServiceTabs)
                                            {
                                                var isIdExists = existingRecord.ServiceTabs.Where(x => x.ServiceId == Service.ServiceId && x.TabId == ServiceTab.TabId).FirstOrDefault();
                                                if (isIdExists != null)
                                                {
                                                    ServiceTab.RowUpdateDate = DateTime.Now;
                                                    iDbContext.ServiceTabs.AddOrUpdate(ServiceTab);
                                                    iDbContext.SaveChanges();
                                                }
                                                else
                                                {
                                                    return Json(new { Result = false, Message = "ServiceTab " + Service.ServiceId + "&" + ServiceTab.TabId + " does not exists.", Icon = "error" }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                        }
                                        else if (ServiceTabs.Count > existingRecord.ServiceTabs.Count)
                                        {
                                            //Log Existing Record
                                            var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceTab, Models.ServiceTabs_Log>(existingRecord.ServiceTabs);
                                            iDbContext.ServiceTabs_Log.AddRange(LogRecord);
                                            //

                                            foreach (var ServiceTab in ServiceTabs)
                                            {
                                                var isIdExists = existingRecord.ServiceTabs.Where(x => x.ServiceId == Service.ServiceId && x.TabId == ServiceTab.TabId).FirstOrDefault();
                                                if (isIdExists == null)
                                                {
                                                    ServiceTab.RowInsertDate = DateTime.Now;
                                                    iDbContext.ServiceTabs.AddOrUpdate(ServiceTab);
                                                    iDbContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                    //
                                    //var ServiceVideos = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceVideo, Models.ServiceVideo>(Service.ServiceVideos);
                                    //if (ServiceVideos != null && ServiceVideos.Count > 0)
                                    //{
                                    //    //Log Existing Record
                                    //    var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceVideo, Models.ServiceVideos_Log>(existingRecord.ServiceVideos);
                                    //    iDbContext.ServiceVideos_Log.AddRange(LogRecord);
                                    //    //

                                    //    var existingVideos = iDbContext.ServiceVideos.Where(x => x.ServiceId == Service.ServiceId).ToList();
                                    //    if (existingVideos != null)
                                    //    {
                                    //        iDbContext.ServiceVideos.RemoveRange(existingVideos);
                                    //        iDbContext.SaveChanges();
                                    //    }

                                    //    foreach (var ServiceVideo in ServiceVideos)
                                    //    {
                                    //        ServiceVideo.IsDeleted = false;
                                    //        ServiceVideo.RowInsertDate = DateTime.Now;
                                    //        ServiceVideo.IpAddress = Common.Security.GetIpAddress();
                                    //        ServiceVideo.RowInsertedBy = Security.GetUser().UserName;
                                    //        iDbContext.ServiceVideos.AddOrUpdate(ServiceVideo);
                                    //        iDbContext.SaveChanges();
                                    //    }
                                    //}
                                    //
                                    var ServiceViewers = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceViewer, Models.ServiceViewer>(Service.ServiceViewers);
                                    if (ServiceViewers != null && ServiceViewers.Count > 0)
                                    {
                                        //Log Existing Record
                                        var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceViewer, Models.ServiceViewers_Log>(existingRecord.ServiceViewers);
                                        iDbContext.ServiceViewers_Log.AddRange(LogRecord);
                                        //

                                        var existingViewers = iDbContext.ServiceViewers.Where(x => x.ServiceId == Service.ServiceId).ToList();
                                        if (existingViewers != null)
                                        {
                                            iDbContext.ServiceViewers.RemoveRange(existingViewers);
                                            iDbContext.SaveChanges();
                                        }

                                        foreach (var ServiceViewer in ServiceViewers)
                                        {
                                            ServiceViewer.IsDeleted = false;
                                            ServiceViewer.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceViewers.AddOrUpdate(ServiceViewer);
                                            iDbContext.SaveChanges();
                                        }
                                    }
                                    //
                                    var ServiceUserTypes = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceUserTypes, Models.ServiceUserType>(Service.ServiceUserTypes);
                                    if (ServiceUserTypes != null && ServiceUserTypes.Count > 0)
                                    {
                                        //Log Existing Record
                                        //var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceUserTypes, Models.ServiceUserType>(existingRecord.ServiceUserTypes);
                                        //iDbContext.ServiceViewers_Log.AddRange(LogRecord);
                                        //

                                        var existingViewers = iDbContext.ServiceUserTypes.Where(x => x.ServiceId == Service.ServiceId).ToList();
                                        if (existingViewers != null)
                                        {
                                            iDbContext.ServiceUserTypes.RemoveRange(existingViewers);
                                            iDbContext.SaveChanges();
                                        }

                                        foreach (var ServiceUserType in ServiceUserTypes)
                                        {
                                            ServiceUserType.IsDeleted = false;
                                            ServiceUserType.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceUserTypes.AddOrUpdate(ServiceUserType);
                                            iDbContext.SaveChanges();
                                        }
                                    }
                                    //
                                    var ServiceCommentsAttachmentsConfig = AutoMapper.ObjectToObjectMapper<Models.IEServiceCommentsAttachmentsConfig, Models.ServiceCommentsAttachmentsConfig>(Service.ServiceCommentsAttachmentsConfig, null);
                                    if (ServiceCommentsAttachmentsConfig != null)
                                    {
                                        //Log Existing Record
                                        var LogRecord = AutoMapper.ObjectToObjectMapper<Models.IEServiceCommentsAttachmentsConfig, Models.ServiceCommentsAttachmentsConfig_Log>(existingRecord.ServiceCommentsAttachmentsConfig, null);
                                        iDbContext.ServiceCommentsAttachmentsConfig_Log.Add(LogRecord);
                                        //

                                        var existingCAC = iDbContext.ServiceCommentsAttachmentsConfigs.Where(x => x.ServiceId == Service.ServiceId).ToList();
                                        if (existingCAC != null)
                                        {
                                            iDbContext.ServiceCommentsAttachmentsConfigs.RemoveRange(existingCAC);
                                            iDbContext.SaveChanges();
                                        }
                                        ServiceCommentsAttachmentsConfig.IsDeleted = false;
                                        ServiceCommentsAttachmentsConfig.RowInsertDate = DateTime.Now;
                                        ServiceCommentsAttachmentsConfig.IpAddress = Common.Security.GetIpAddress();
                                        ServiceCommentsAttachmentsConfig.RowInsertedBy = Security.GetUser().UserName;
                                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(ServiceCommentsAttachmentsConfig);
                                        iDbContext.SaveChanges();
                                    }
                                    //
                                    var ServiceCategories = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceCategory, Models.ServiceCategory>(Service.ServiceCategories);
                                    if (ServiceCategories != null && ServiceCategories.Count > 0)
                                    {
                                        if (ServiceCategories.Count == existingRecord.ServiceCategories.Count)
                                        {
                                            //Log Existing Record
                                            var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceCategory, Models.ServiceCategories_Log>(existingRecord.ServiceCategories);
                                            iDbContext.ServiceCategories_Log.AddRange(LogRecord);
                                            //

                                            foreach (var ServiceCategory in ServiceCategories)
                                            {
                                                var isIdExists = existingRecord.ServiceCategories.Where(x => x.ServiceId == Service.ServiceId && x.CategoryId == ServiceCategory.CategoryId).FirstOrDefault();
                                                if (isIdExists != null)
                                                {
                                                    ServiceCategory.IpAddress = Common.Security.GetIpAddress();
                                                    ServiceCategory.RowUpdatedBy = Common.Security.GetUser().UserName;
                                                    ServiceCategory.RowUpdateDate = DateTime.Now;
                                                    iDbContext.ServiceCategories.AddOrUpdate(ServiceCategory);
                                                    iDbContext.SaveChanges();
                                                }
                                                else
                                                {
                                                    return Json(new { Result = false, Message = "ServiceCategories " + Service.ServiceId + "&" + ServiceCategory.CategoryId + " does not exists.", Icon = "error" }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                        }
                                        else if (ServiceCategories.Count > existingRecord.ServiceCategories.Count)
                                        {
                                            //Log Existing Record
                                            var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceCategory, Models.ServiceCategories_Log>(existingRecord.ServiceCategories);
                                            iDbContext.ServiceCategories_Log.AddRange(LogRecord);
                                            //

                                            foreach (var ServiceCategory in ServiceCategories)
                                            {
                                                var isIdExists = existingRecord.ServiceCategories.Where(x => x.ServiceId == Service.ServiceId && x.CategoryId == ServiceCategory.CategoryId).FirstOrDefault();
                                                if (isIdExists == null)
                                                {
                                                    ServiceCategory.IpAddress = Common.Security.GetIpAddress();
                                                    ServiceCategory.RowInsertedBy = Common.Security.GetUser().UserName;
                                                    ServiceCategory.RowInsertDate = DateTime.Now;
                                                    iDbContext.ServiceCategories.AddOrUpdate(ServiceCategory);
                                                    iDbContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                    //
                                    var ServiceFormsConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsConfig, Models.ServiceFormsConfig>(Service.ServiceFormsConfig);
                                    if (ServiceFormsConfig != null && ServiceFormsConfig.Count > 0)
                                    {
                                        //foreach (var sfc in ServiceFormsConfig)
                                        //{
                                        //    sfc.IsDeleted = false;
                                        //    sfc.RowInsertDate = DateTime.Now;
                                        //    sfc.IpAddress = Common.Security.GetIpAddress();
                                        //    sfc.RowInsertedBy = Security.GetUser().UserName;
                                        //    iDbContext.ServiceFormsConfigs.AddOrUpdate(sfc);
                                        //    iDbContext.SaveChanges();
                                        //}
                                        if (ServiceFormsConfig.Count == existingRecord.ServiceFormsConfig.Count)
                                        {
                                            //Log Existing Record
                                            var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsConfig, Models.ServiceFormsConfig_Log>(existingRecord.ServiceFormsConfig);
                                            iDbContext.ServiceFormsConfig_Log.AddRange(LogRecord);
                                            //

                                            foreach (var sfc in ServiceFormsConfig)
                                            {
                                                var isIdExists = existingRecord.ServiceFormsConfig.Where(x => x.ServiceId == Service.ServiceId && x.ID == sfc.ID).FirstOrDefault();
                                                if (isIdExists != null)
                                                {
                                                    sfc.IpAddress = Common.Security.GetIpAddress();
                                                    sfc.RowUpdatedBy = Common.Security.GetUser().UserName;
                                                    sfc.RowUpdateDate = DateTime.Now;
                                                    iDbContext.ServiceFormsConfigs.AddOrUpdate(sfc);
                                                    iDbContext.SaveChanges();
                                                }
                                                else
                                                {
                                                    return Json(new { Result = false, Message = "ServiceFormsConfig " + Service.ServiceId + "&" + sfc.ID + " does not exists.", Icon = "error" }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                        }
                                        else if (ServiceFormsConfig.Count > existingRecord.ServiceFormsConfig.Count)
                                        {
                                            //Log Existing Record
                                            var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsConfig, Models.ServiceFormsConfig_Log>(existingRecord.ServiceFormsConfig);
                                            iDbContext.ServiceFormsConfig_Log.AddRange(LogRecord);
                                            //

                                            foreach (var sfc in ServiceFormsConfig)
                                            {
                                                var isIdExists = existingRecord.ServiceFormsConfig.Where(x => x.ServiceId == Service.ServiceId && x.ID == sfc.ID).FirstOrDefault();
                                                if (isIdExists == null)
                                                {
                                                    sfc.IpAddress = Common.Security.GetIpAddress();
                                                    sfc.RowInsertedBy = Common.Security.GetUser().UserName;
                                                    sfc.RowInsertDate = DateTime.Now;
                                                    iDbContext.ServiceFormsConfigs.AddOrUpdate(sfc);
                                                    iDbContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }

                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        foreach (var iInput in Service.ServiceFormsConfig)
                                        {
                                            if (iInput.ServiceTemplates != null)
                                            {
                                                //Log Existing Record
                                                var LogRecord = AutoMapper.ObjectToObjectMapper<Models.IEServiceTemplate, Models.ServiceTemplates_Log>(iInput.ServiceTemplates, null);
                                                iDbContext.ServiceTemplates_Log.Add(LogRecord);
                                                //

                                                var isExists = iDbContext.ServiceTemplates.Where(x => x.InputControlId == iInput.ID).ToList();
                                                if (isExists != null)
                                                {
                                                    iDbContext.ServiceTemplates.RemoveRange(isExists);
                                                    iDbContext.SaveChanges();
                                                }

                                                var ServiceTemplates = AutoMapper.ObjectToObjectMapper<Models.IEServiceTemplate, Models.ServiceTemplate>(iInput.ServiceTemplates, null);
                                                if (ServiceTemplates != null)
                                                {
                                                    ServiceTemplates.IsDeleted = false;
                                                    ServiceTemplates.RowInsertDate = DateTime.Now;
                                                    ServiceTemplates.IpAddress = Common.Security.GetIpAddress();
                                                    ServiceTemplates.RowInsertedBy = Security.GetUser().UserName;

                                                    iDbContext.ServiceTemplates.AddOrUpdate(ServiceTemplates);
                                                    iDbContext.SaveChanges();
                                                }
                                            }

                                            if (iInput.ServiceFormsDrilldownConfig != null)
                                            {
                                                //Log Existing Record
                                                var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsDrilldownConfig, Models.ServiceFormsDrilldownConfig_Log>(iInput.ServiceFormsDrilldownConfig);
                                                iDbContext.ServiceFormsDrilldownConfig_Log.AddRange(LogRecord);
                                                //

                                                var isExists = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.InputControlId == iInput.ID).ToList();
                                                if (isExists != null)
                                                {
                                                    iDbContext.ServiceFormsDrilldownConfigs.RemoveRange(isExists);
                                                    iDbContext.SaveChanges();
                                                }

                                                var ServiceFormsDrilldownConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsDrilldownConfig, Models.ServiceFormsDrilldownConfig>(iInput.ServiceFormsDrilldownConfig);
                                                if (ServiceFormsDrilldownConfig != null && ServiceFormsDrilldownConfig.Count > 0)
                                                {
                                                    foreach (var sfc in ServiceFormsDrilldownConfig)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        sfc.IpAddress = Common.Security.GetIpAddress();
                                                        sfc.RowInsertedBy = Security.GetUser().UserName;
                                                        iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputLookupActions != null)
                                            {
                                                //Log Existing Record
                                                var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputLookupAction, Models.ServiceInputLookupActions_Log>(iInput.ServiceInputLookupActions);
                                                iDbContext.ServiceInputLookupActions_Log.AddRange(LogRecord);
                                                //

                                                var isExists = iDbContext.ServiceInputLookupActions.Where(x => x.InputControlId == iInput.ID).ToList();
                                                if (isExists != null)
                                                {
                                                    iDbContext.ServiceInputLookupActions.RemoveRange(isExists);
                                                    iDbContext.SaveChanges();
                                                }

                                                var ServiceInputLookupActions = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputLookupAction, Models.ServiceInputLookupAction>(iInput.ServiceInputLookupActions);
                                                if (ServiceInputLookupActions != null && ServiceInputLookupActions.Count > 0)
                                                {
                                                    foreach (var sfc in ServiceInputLookupActions)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputLookupActions.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputTooltips != null)
                                            {
                                                //Log Existing Record
                                                var LogRecord = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputTooltip, Models.ServiceInputTooltips_Log>(iInput.ServiceInputTooltips);
                                                iDbContext.ServiceInputTooltips_Log.AddRange(LogRecord);
                                                //

                                                var isExists = iDbContext.ServiceInputTooltips.Where(x => x.InputControlId == iInput.ID).ToList();
                                                if (isExists != null)
                                                {
                                                    iDbContext.ServiceInputTooltips.RemoveRange(isExists);
                                                    iDbContext.SaveChanges();
                                                }

                                                var ServiceInputTooltips = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputTooltip, Models.ServiceInputTooltip>(iInput.ServiceInputTooltips);
                                                if (ServiceInputTooltips != null && ServiceInputTooltips.Count > 0)
                                                {
                                                    foreach (var sfc in ServiceInputTooltips)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputTooltips.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.Lookup != null)
                                            {
                                                var Lookup = AutoMapper.ObjectToObjectMapper<Models.IELkLookup, Models.LkLookup>(iInput.Lookup, null);
                                                if (Lookup != null)
                                                {
                                                    Lookup.IsDeleted = false;
                                                    Lookup.RowInsertDate = DateTime.Now;
                                                    Lookup.IpAddress = Common.Security.GetIpAddress();
                                                    Lookup.RowInsertedBy = Security.GetUser().UserName;

                                                    //Log Existing Record
                                                    var LogRecord = AutoMapper.ObjectToObjectMapper<Models.IELkLookup, Models.LkLookups_Log>(iInput.Lookup, null);
                                                    iDbContext.LkLookups_Log.Add(LogRecord);
                                                    //

                                                    iDbContext.LkLookups.AddOrUpdate(Lookup);
                                                    iDbContext.SaveChanges();

                                                    if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                    {
                                                        //Log Existing Record
                                                        var LogRecord2 = AutoMapper.ListObjectToListObjectMapper<Models.IELookupOption, Models.LookupOptions_Log>(iInput.Lookup.LookupOption);
                                                        iDbContext.LookupOptions_Log.AddRange(LogRecord2);
                                                        //

                                                        var LookupOptions = AutoMapper.ListObjectToListObjectMapper<Models.IELookupOption, Models.LookupOption>(iInput.Lookup.LookupOption);

                                                        foreach (var iLkOptions in LookupOptions)
                                                        {
                                                            var isExists = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == iLkOptions.LookupId && x.Code == iLkOptions.Code).FirstOrDefault();
                                                            if (isExists != null)
                                                            {
                                                                isExists.IsDeleted = false;
                                                                isExists.RowInsertDate = DateTime.Now;
                                                                iDbContext.LookupOptions.AddOrUpdate(isExists);
                                                                iDbContext.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                iLkOptions.Code = iLkOptions.Code.ToString();
                                                                iLkOptions.IsDeleted = false;
                                                                iLkOptions.RowInsertDate = DateTime.Now;
                                                                iDbContext.LookupOptions.AddOrUpdate(iLkOptions);
                                                                iDbContext.SaveChanges();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                                    {
                                        var aut = new Models.UserScreenAction();
                                        aut.ActionId = 7;
                                        aut.ServiceId = Service.ServiceId;
                                        aut.CategoryId = 0;
                                        aut.UniqueId = Service.ExternalServiceID;
                                        aut.Remarks = Security.GetUser().UserName + " IMPORTED SERVICE  = " + Service.DescriptionEng + "-" + fileName;
                                        aut.RowInsertDate = DateTime.Now;
                                        aut.IpAddress = Common.Security.GetIpAddress();
                                        aut.RowInsertedBy = Security.GetUser().UserName;
                                        aut.IsDeleted = false;
                                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                                        iDbContext.SaveChanges();
                                    }
                                    iTransaction.Commit();
                                    hasTransaction = false;
                                }
                            }

                            #endregion                            
                        }
                        else
                        {
                            //Step 2 : Start Importing the Service and Create New Record...

                            #region --> ADD NEW SERVICE FROM FILE...

                            foreach(var Service in importedObject.Services)
                            {
                                int newServiceId = iDbContext.Services.Max(x => x.ServiceId) + 1;
                                int newCategoryId = iDbContext.ServiceCategories.Max(x => x.CategoryId);
                                int newLkLookupId = iDbContext.LkLookups.Max(x => x.LookupId);

                                Service.ServiceId = newServiceId;

                                //ServiceCategories...
                                if (Service.ServiceCategories != null && Service.ServiceCategories.Count > 0)
                                {
                                    foreach (var serviceCategory in Service.ServiceCategories)
                                    {
                                        newCategoryId = newCategoryId + 1;

                                        //ServiceFormsConfig...
                                        if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                        {
                                            var serviceFormConfig = Service.ServiceFormsConfig.Where(x => x.CategorId == serviceCategory.CategoryId).ToList();
                                            foreach (var iInput in serviceFormConfig)
                                            {
                                                iInput.ServiceId = newServiceId;
                                                iInput.CategorId = newCategoryId;

                                                //TimeSpan span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                                                //iInput.ID = Convert.ToString((double)span.TotalSeconds).Replace(".", "");

                                                //ServiceTemplates...
                                                if (iInput.ServiceTemplates != null)
                                                {
                                                    iInput.ServiceTemplates.ServiceId = newServiceId;
                                                    iInput.ServiceTemplates.CategoryId = newCategoryId;
                                                }
                                                //Lookups...
                                                if (iInput.OptionId.HasValue && iInput.OptionId > 0)
                                                {
                                                    if (iInput.Lookup != null)
                                                    {
                                                        var isLookupExists = iDbContext.LkLookups.Where(x => x.LookUpName == iInput.Lookup.LookUpName).FirstOrDefault();
                                                        if (isLookupExists != null)
                                                        {
                                                            iInput.OptionId = isLookupExists.LookupId;
                                                            if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                            {
                                                                foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                                {
                                                                    iLkOptions.LookupId = isLookupExists.LookupId;
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            newLkLookupId = newLkLookupId + 1;
                                                            iInput.OptionId = newLkLookupId;
                                                            if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                            {
                                                                foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                                {
                                                                    iLkOptions.LookupId = newLkLookupId;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                //ServiceFormsDrilldownConfig
                                                //if (iInput.ServiceFormsDrilldownConfig != null && iInput.ServiceFormsDrilldownConfig.Count > 0)
                                                //{
                                                //    foreach (var ddlConfig in iInput.ServiceFormsDrilldownConfig)
                                                //    {
                                                //        ddlConfig.InputControlId = iInput.ID;
                                                //    }
                                                //}
                                                //ServiceInputLookupActions
                                                //if (iInput.ServiceInputLookupActions != null && iInput.ServiceInputLookupActions.Count > 0)
                                                //{
                                                //    foreach (var lkAction in iInput.ServiceInputLookupActions)
                                                //    {
                                                //        lkAction.InputControlId = iInput.ID;
                                                //    }
                                                //}
                                                //ServiceInputTooltips
                                                //if (iInput.ServiceInputTooltips != null && iInput.ServiceInputTooltips.Count > 0)
                                                //{
                                                //    foreach (var toolTip in iInput.ServiceInputTooltips)
                                                //    {
                                                //        toolTip.InputControlId = iInput.ID;
                                                //    }
                                                //}
                                            }
                                        }
                                        serviceCategory.ServiceId = newServiceId;
                                        serviceCategory.CategoryId = newCategoryId;
                                        serviceCategory.CategoryCode = "SC-" + newCategoryId;
                                    }
                                }
                                else
                                {
                                    //ServiceFormsConfig...
                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        var serviceFormConfig = Service.ServiceFormsConfig.ToList();
                                        foreach (var iInput in serviceFormConfig)
                                        {
                                            iInput.ServiceId = newServiceId;
                                            iInput.CategorId = null;

                                            //TimeSpan span = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
                                            //iInput.ID = Convert.ToString((double)span.TotalSeconds).Replace(".", "");

                                            //ServiceTemplates...
                                            if (iInput.ServiceTemplates != null)
                                            {
                                                iInput.ServiceTemplates.ServiceId = newServiceId;
                                                iInput.ServiceTemplates.CategoryId = null;
                                            }
                                            //Lookups...
                                            if (iInput.OptionId.HasValue && iInput.OptionId > 0)
                                            {
                                                if (iInput.Lookup != null)
                                                {
                                                    var isLookupExists = iDbContext.LkLookups.Where(x => x.LookUpName == iInput.Lookup.LookUpName).FirstOrDefault();
                                                    if (isLookupExists != null)
                                                    {
                                                        iInput.OptionId = isLookupExists.LookupId;
                                                        if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                        {
                                                            foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                            {
                                                                iLkOptions.LookupId = isLookupExists.LookupId;
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        newLkLookupId = newLkLookupId + 1;
                                                        iInput.OptionId = newLkLookupId;
                                                        if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                        {
                                                            foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                            {
                                                                iLkOptions.LookupId = newLkLookupId;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //ServiceCommentsAttachmentsConfig...
                                if (Service.ServiceCommentsAttachmentsConfig != null)
                                {
                                    Service.ServiceCommentsAttachmentsConfig.ServiceId = newServiceId;
                                }
                                //ServiceTabs...
                                if (Service.ServiceTabs != null && Service.ServiceTabs.Count > 0)
                                {
                                    foreach (var serviceTab in Service.ServiceTabs)
                                    {
                                        serviceTab.ServiceId = newServiceId;
                                    }
                                }
                                //ServiceVideos...
                                //if (Service.ServiceVideos != null && Service.ServiceVideos.Count > 0)
                                //{
                                //    foreach (var servicevideo in Service.ServiceVideos)
                                //    {
                                //        servicevideo.ServiceId = newServiceId;
                                //    }
                                //}
                                //ServiceViewers...
                                if (Service.ServiceViewers != null && Service.ServiceViewers.Count > 0)
                                {
                                    foreach (var serviceViewer in Service.ServiceViewers)
                                    {
                                        serviceViewer.ServiceId = newServiceId;
                                    }
                                }

                                if (Service.ServiceUserTypes != null && Service.ServiceUserTypes.Count > 0)
                                {
                                    foreach (var serviceUserType in Service.ServiceUserTypes)
                                    {
                                        serviceUserType.ServiceId = newServiceId;
                                    }
                                }

                                //Bind IE Model to EF Model Here...AND SAVE...
                                using (iTransaction = iDbContext.Database.BeginTransaction())
                                {
                                    hasTransaction = true;

                                    var xService = AutoMapper.ObjectToObjectMapper<Models.IEService, Models.Service>(Service, null);
                                    if (xService != null)
                                    {
                                        xService.IsDeleted = false;
                                        xService.RowInsertDate = DateTime.Now;
                                        xService.IpAddress = Common.Security.GetIpAddress();
                                        xService.RowInsertedBy = Security.GetUser().UserName;
                                        iDbContext.Services.AddOrUpdate(xService);
                                        iDbContext.SaveChanges();
                                    }

                                    var ServiceTabs = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceTab, Models.ServiceTab>(Service.ServiceTabs);
                                    if (ServiceTabs != null && ServiceTabs.Count > 0)
                                    {
                                        foreach (var ServiceTab in ServiceTabs)
                                        {
                                            ServiceTab.IsDeleted = false;
                                            ServiceTab.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceTabs.AddOrUpdate(ServiceTab);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    //var ServiceVideos = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceVideo, Models.ServiceVideo>(Service.ServiceVideos);
                                    //if (ServiceVideos != null && ServiceVideos.Count > 0)
                                    //{
                                    //    iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceVideos] OFF");

                                    //    foreach (var ServiceVideo in ServiceVideos)
                                    //    {
                                    //        ServiceVideo.IsDeleted = false;
                                    //        ServiceVideo.RowInsertDate = DateTime.Now;
                                    //        ServiceVideo.IpAddress = Common.Security.GetIpAddress();
                                    //        ServiceVideo.RowInsertedBy = Security.GetUser().UserName;
                                    //        iDbContext.ServiceVideos.AddOrUpdate(ServiceVideo);
                                    //        iDbContext.SaveChanges();
                                    //    }
                                    //}

                                    var ServiceViewers = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceViewer, Models.ServiceViewer>(Service.ServiceViewers);
                                    if (ServiceViewers != null && ServiceViewers.Count > 0)
                                    {
                                        iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceViewers] OFF");

                                        foreach (var ServiceViewer in ServiceViewers)
                                        {
                                            ServiceViewer.IsDeleted = false;
                                            ServiceViewer.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceViewers.AddOrUpdate(ServiceViewer);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceCategories = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceCategory, Models.ServiceCategory>(Service.ServiceCategories);
                                    if (ServiceCategories != null && ServiceCategories.Count > 0)
                                    {
                                        foreach (var ServiceCategory in ServiceCategories)
                                        {
                                            ServiceCategory.IsDeleted = false;
                                            ServiceCategory.RowInsertDate = DateTime.Now;
                                            ServiceCategory.IpAddress = Common.Security.GetIpAddress();
                                            ServiceCategory.RowInsertedBy = Security.GetUser().UserName;
                                            iDbContext.ServiceCategories.AddOrUpdate(ServiceCategory);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceCommentsAttachmentsConfig = AutoMapper.ObjectToObjectMapper<Models.IEServiceCommentsAttachmentsConfig, Models.ServiceCommentsAttachmentsConfig>(Service.ServiceCommentsAttachmentsConfig, null);
                                    if (ServiceCommentsAttachmentsConfig != null)
                                    {
                                        ServiceCommentsAttachmentsConfig.IsDeleted = false;
                                        ServiceCommentsAttachmentsConfig.RowInsertDate = DateTime.Now;
                                        ServiceCommentsAttachmentsConfig.IpAddress = Common.Security.GetIpAddress();
                                        ServiceCommentsAttachmentsConfig.RowInsertedBy = Security.GetUser().UserName;

                                        iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceCommentsAttachmentsConfig] OFF");
                                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(ServiceCommentsAttachmentsConfig);
                                        iDbContext.SaveChanges();
                                    }

                                    var ServiceFormsConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsConfig, Models.ServiceFormsConfig>(Service.ServiceFormsConfig);
                                    if (ServiceFormsConfig != null && ServiceFormsConfig.Count > 0)
                                    {
                                        iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceFormsConfig] OFF");
                                        //foreach (var sfc in ServiceFormsConfig)
                                        //{
                                        //    sfc.InputId = 0;
                                        //}
                                        foreach (var sfc in ServiceFormsConfig)
                                        {
                                            sfc.IsDeleted = false;
                                            sfc.RowInsertDate = DateTime.Now;
                                            sfc.IpAddress = Common.Security.GetIpAddress();
                                            sfc.RowInsertedBy = Security.GetUser().UserName;
                                            iDbContext.ServiceFormsConfigs.AddOrUpdate(sfc);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        foreach (var iInput in Service.ServiceFormsConfig)
                                        {
                                            if (iInput.ServiceTemplates != null)
                                            {
                                                var ServiceTemplates = AutoMapper.ObjectToObjectMapper<Models.IEServiceTemplate, Models.ServiceTemplate>(iInput.ServiceTemplates, null);
                                                if (ServiceTemplates != null)
                                                {
                                                    ServiceTemplates.IsDeleted = false;
                                                    ServiceTemplates.RowInsertDate = DateTime.Now;
                                                    ServiceTemplates.IpAddress = Common.Security.GetIpAddress();
                                                    ServiceTemplates.RowInsertedBy = Security.GetUser().UserName;

                                                    iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceTemplates] OFF");
                                                    iDbContext.ServiceTemplates.AddOrUpdate(ServiceTemplates);
                                                    iDbContext.SaveChanges();
                                                }
                                            }

                                            if (iInput.ServiceFormsDrilldownConfig != null)
                                            {
                                                var ServiceFormsDrilldownConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsDrilldownConfig, Models.ServiceFormsDrilldownConfig>(iInput.ServiceFormsDrilldownConfig);
                                                if (ServiceFormsDrilldownConfig != null && ServiceFormsDrilldownConfig.Count > 0)
                                                {
                                                    iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceFormsDrilldownConfig] OFF");
                                                    foreach (var sfc in ServiceFormsDrilldownConfig)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        sfc.IpAddress = Common.Security.GetIpAddress();
                                                        sfc.RowInsertedBy = Security.GetUser().UserName;
                                                        iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputLookupActions != null)
                                            {
                                                var ServiceInputLookupActions = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputLookupAction, Models.ServiceInputLookupAction>(iInput.ServiceInputLookupActions);
                                                if (ServiceInputLookupActions != null && ServiceInputLookupActions.Count > 0)
                                                {
                                                    iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceInputLookupActions] OFF");
                                                    foreach (var sfc in ServiceInputLookupActions)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputLookupActions.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputTooltips != null)
                                            {
                                                var ServiceInputTooltips = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputTooltip, Models.ServiceInputTooltip>(iInput.ServiceInputTooltips);
                                                if (ServiceInputTooltips != null && ServiceInputTooltips.Count > 0)
                                                {
                                                    iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceInputTooltips] OFF");
                                                    foreach (var sfc in ServiceInputTooltips)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputTooltips.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.Lookup != null)
                                            {
                                                var Lookup = AutoMapper.ObjectToObjectMapper<Models.IELkLookup, Models.LkLookup>(iInput.Lookup, null);
                                                if (Lookup != null)
                                                {
                                                    Lookup.IsDeleted = false;
                                                    Lookup.RowInsertDate = DateTime.Now;
                                                    Lookup.IpAddress = Common.Security.GetIpAddress();
                                                    Lookup.RowInsertedBy = Security.GetUser().UserName;
                                                    iDbContext.LkLookups.AddOrUpdate(Lookup);
                                                    iDbContext.SaveChanges();

                                                    if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                    {
                                                        var LookupOptions = AutoMapper.ListObjectToListObjectMapper<Models.IELookupOption, Models.LookupOption>(iInput.Lookup.LookupOption);

                                                        iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[LookupOptions] OFF");
                                                        foreach (var iLkOptions in LookupOptions)
                                                        {
                                                            var isExists = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == iLkOptions.LookupId && x.Code == iLkOptions.Code).FirstOrDefault();
                                                            if (isExists != null)
                                                            {
                                                                isExists.IsDeleted = false;
                                                                isExists.RowInsertDate = DateTime.Now;
                                                                iDbContext.LookupOptions.AddOrUpdate(isExists);
                                                                iDbContext.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                iLkOptions.IsDeleted = false;
                                                                iLkOptions.RowInsertDate = DateTime.Now;
                                                                iDbContext.LookupOptions.AddOrUpdate(iLkOptions);
                                                                iDbContext.SaveChanges();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                                    {
                                        var aut = new Models.UserScreenAction();
                                        aut.ActionId = 7;
                                        aut.ServiceId = newServiceId;
                                        aut.CategoryId = 0;
                                        aut.UniqueId = xService.ExternalServiceID;
                                        aut.Remarks = Security.GetUser().UserName + " IMPORTED SERVICE  = " + xService.DescriptionEng + "-" + fileName;
                                        aut.RowInsertDate = DateTime.Now;
                                        aut.IpAddress = Common.Security.GetIpAddress();
                                        aut.RowInsertedBy = Security.GetUser().UserName;
                                        aut.IsDeleted = false;
                                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                                        iDbContext.SaveChanges();
                                    }
                                    iTransaction.Commit();
                                    hasTransaction = false;
                                }

                            }                           

                            #endregion                            
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                if (hasTransaction && iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
                sb.AppendLine();
                return Json(new { Result = false, Message = sb.ToString(), Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                if (hasTransaction && iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }
                return Json(new { Result = false, Message = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = serviceId +"-"+ DbManager.GetText("Services", "lblImportSuccess", " Service Imported Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportServiceOverride()
        {
            DbContextTransaction iTransaction = null;
            //Models.ImportExport existingRecord = null;
            string serviceId = "";
            string fileName = "";
            bool hasTransaction = false;
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
                    fileName = file.FileName;
                    System.IO.FileInfo f = new System.IO.FileInfo(fileName);

                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes(file.ContentLength);

                    string result = System.Text.Encoding.UTF8.GetString(binData);

                    var importedObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.ImportExport2>(result);
                    if (importedObject != null)
                    {
                        serviceId = importedObject.Services[0].ExternalServiceID;
                        using (iTransaction = iDbContext.Database.BeginTransaction())
                        {
                            hasTransaction = true;
                            
                            foreach (var Service in importedObject.Services)
                            {
                                //Step 1 : Check if Service Exists then Log Record -> Delete Records -> Import
                                string temp = Service.ExternalServiceID;
                                var existingObject = iDbContext.Services.Where(x => x.ExternalServiceID == temp).FirstOrDefault();
                                if (existingObject != null)
                                {
                                    string sql = "Exec dbo.ES_LogRecordsBeforeImport @ExternalServiceID = '" + temp + "'";
                                    iDbContext.Database.ExecuteSqlCommand(sql);
                                }

                                //Step 2 : Start Importing the Service
                                int newLkLookupId = iDbContext.LkLookups.Max(x => x.LookupId);

                                //ServiceCategories...
                                if (Service.ServiceCategories != null && Service.ServiceCategories.Count > 0)
                                {
                                    foreach (var serviceCategory in Service.ServiceCategories)
                                    {
                                        //ServiceFormsConfig...
                                        if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                        {
                                            var serviceFormConfig = Service.ServiceFormsConfig.Where(x => x.CategorId == serviceCategory.CategoryId).ToList();
                                            foreach (var iInput in serviceFormConfig)
                                            {
                                                //Lookups...
                                                if (iInput.OptionId.HasValue && iInput.OptionId > 0)
                                                {
                                                    if (iInput.Lookup != null)
                                                    {
                                                        var isLookupExists = iDbContext.LkLookups.Where(x => x.LookUpName == iInput.Lookup.LookUpName).FirstOrDefault();
                                                        if (isLookupExists != null)
                                                        {
                                                            iInput.OptionId = isLookupExists.LookupId;
                                                            if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                            {
                                                                foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                                {
                                                                    iLkOptions.LookupId = isLookupExists.LookupId;
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            newLkLookupId = newLkLookupId + 1;
                                                            iInput.OptionId = newLkLookupId;
                                                            if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                            {
                                                                foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                                {
                                                                    iLkOptions.LookupId = newLkLookupId;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        serviceCategory.ServiceId = serviceCategory.ServiceId;
                                        serviceCategory.CategoryId = serviceCategory.CategoryId;
                                        serviceCategory.CategoryCode = "SC-" + serviceCategory.CategoryId;
                                    }
                                }
                                else
                                {
                                    //ServiceFormsConfig...
                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        var serviceFormConfig = Service.ServiceFormsConfig.ToList();
                                        foreach (var iInput in serviceFormConfig)
                                        {
                                            //Lookups...
                                            if (iInput.OptionId.HasValue && iInput.OptionId > 0)
                                            {
                                                if (iInput.Lookup != null)
                                                {
                                                    var isLookupExists = iDbContext.LkLookups.Where(x => x.LookUpName == iInput.Lookup.LookUpName).FirstOrDefault();
                                                    if (isLookupExists != null)
                                                    {
                                                        iInput.OptionId = isLookupExists.LookupId;
                                                        if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                        {
                                                            foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                            {
                                                                iLkOptions.LookupId = isLookupExists.LookupId;
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        newLkLookupId = newLkLookupId + 1;
                                                        iInput.OptionId = newLkLookupId;
                                                        if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                        {
                                                            foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                            {
                                                                iLkOptions.LookupId = newLkLookupId;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //Bind IE Model to EF Model Here...AND SAVE...
                                {
                                    var xService = AutoMapper.ObjectToObjectMapper<Models.IEService, Models.Service>(Service, null);
                                    if (xService != null)
                                    {
                                        xService.IsDeleted = false;
                                        xService.RowInsertDate = DateTime.Now;
                                        xService.IpAddress = Common.Security.GetIpAddress();
                                        xService.RowInsertedBy = Security.GetUser().UserName;
                                        iDbContext.Services.AddOrUpdate(xService);
                                        iDbContext.SaveChanges();
                                    }

                                    var ServiceTabs = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceTab, Models.ServiceTab>(Service.ServiceTabs);
                                    if (ServiceTabs != null && ServiceTabs.Count > 0)
                                    {
                                        foreach (var ServiceTab in ServiceTabs)
                                        {
                                            ServiceTab.IsDeleted = false;
                                            ServiceTab.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceTabs.AddOrUpdate(ServiceTab);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceViewers = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceViewer, Models.ServiceViewer>(Service.ServiceViewers);
                                    if (ServiceViewers != null && ServiceViewers.Count > 0)
                                    {
                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceViewers] OFF");

                                        foreach (var ServiceViewer in ServiceViewers)
                                        {
                                            ServiceViewer.IsDeleted = false;
                                            ServiceViewer.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceViewers.AddOrUpdate(ServiceViewer);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceUserTypes = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceUserTypes, Models.ServiceUserType>(Service.ServiceUserTypes);
                                    if (ServiceUserTypes != null && ServiceUserTypes.Count > 0)
                                    {
                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceViewers] OFF");

                                        foreach (var ServiceUserType in ServiceUserTypes)
                                        {
                                            ServiceUserType.IsDeleted = false;
                                            ServiceUserType.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceUserTypes.AddOrUpdate(ServiceUserType);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceCategories = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceCategory, Models.ServiceCategory>(Service.ServiceCategories);
                                    if (ServiceCategories != null && ServiceCategories.Count > 0)
                                    {
                                        foreach (var ServiceCategory in ServiceCategories)
                                        {
                                            ServiceCategory.IsDeleted = false;
                                            ServiceCategory.RowInsertDate = DateTime.Now;
                                            ServiceCategory.IpAddress = Common.Security.GetIpAddress();
                                            ServiceCategory.RowInsertedBy = Security.GetUser().UserName;
                                            iDbContext.ServiceCategories.AddOrUpdate(ServiceCategory);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceCommentsAttachmentsConfig = AutoMapper.ObjectToObjectMapper<Models.IEServiceCommentsAttachmentsConfig, Models.ServiceCommentsAttachmentsConfig>(Service.ServiceCommentsAttachmentsConfig, null);
                                    if (ServiceCommentsAttachmentsConfig != null)
                                    {
                                        ServiceCommentsAttachmentsConfig.IsDeleted = false;
                                        ServiceCommentsAttachmentsConfig.RowInsertDate = DateTime.Now;
                                        ServiceCommentsAttachmentsConfig.IpAddress = Common.Security.GetIpAddress();
                                        ServiceCommentsAttachmentsConfig.RowInsertedBy = Security.GetUser().UserName;

                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceCommentsAttachmentsConfig] OFF");
                                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(ServiceCommentsAttachmentsConfig);
                                        iDbContext.SaveChanges();
                                    }

                                    var ServiceFormsConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsConfig, Models.ServiceFormsConfig>(Service.ServiceFormsConfig);
                                    if (ServiceFormsConfig != null && ServiceFormsConfig.Count > 0)
                                    {
                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceFormsConfig] OFF");
                                        foreach (var sfc in ServiceFormsConfig)
                                        {
                                            sfc.IsDeleted = false;
                                            sfc.RowInsertDate = DateTime.Now;
                                            sfc.IpAddress = Common.Security.GetIpAddress();
                                            sfc.RowInsertedBy = Security.GetUser().UserName;
                                            iDbContext.ServiceFormsConfigs.AddOrUpdate(sfc);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        foreach (var iInput in Service.ServiceFormsConfig)
                                        {
                                            if (iInput.ServiceTemplates != null)
                                            {
                                                var ServiceTemplates = AutoMapper.ObjectToObjectMapper<Models.IEServiceTemplate, Models.ServiceTemplate>(iInput.ServiceTemplates, null);
                                                if (ServiceTemplates != null)
                                                {
                                                    ServiceTemplates.IsDeleted = false;
                                                    ServiceTemplates.RowInsertDate = DateTime.Now;
                                                    ServiceTemplates.IpAddress = Common.Security.GetIpAddress();
                                                    ServiceTemplates.RowInsertedBy = Security.GetUser().UserName;

                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceTemplates] OFF");
                                                    iDbContext.ServiceTemplates.AddOrUpdate(ServiceTemplates);
                                                    iDbContext.SaveChanges();
                                                }
                                            }

                                            if (iInput.ServiceFormsDrilldownConfig != null)
                                            {
                                                var ServiceFormsDrilldownConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsDrilldownConfig, Models.ServiceFormsDrilldownConfig>(iInput.ServiceFormsDrilldownConfig);
                                                if (ServiceFormsDrilldownConfig != null && ServiceFormsDrilldownConfig.Count > 0)
                                                {
                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceFormsDrilldownConfig] OFF");
                                                    foreach (var sfc in ServiceFormsDrilldownConfig)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        sfc.IpAddress = Common.Security.GetIpAddress();
                                                        sfc.RowInsertedBy = Security.GetUser().UserName;
                                                        iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputLookupActions != null)
                                            {
                                                var ServiceInputLookupActions = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputLookupAction, Models.ServiceInputLookupAction>(iInput.ServiceInputLookupActions);
                                                if (ServiceInputLookupActions != null && ServiceInputLookupActions.Count > 0)
                                                {
                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceInputLookupActions] OFF");
                                                    foreach (var sfc in ServiceInputLookupActions)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputLookupActions.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputTooltips != null)
                                            {
                                                var ServiceInputTooltips = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputTooltip, Models.ServiceInputTooltip>(iInput.ServiceInputTooltips);
                                                if (ServiceInputTooltips != null && ServiceInputTooltips.Count > 0)
                                                {
                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceInputTooltips] OFF");
                                                    foreach (var sfc in ServiceInputTooltips)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputTooltips.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.Lookup != null)
                                            {
                                                var Lookup = AutoMapper.ObjectToObjectMapper<Models.IELkLookup, Models.LkLookup>(iInput.Lookup, null);
                                                if (Lookup != null)
                                                {
                                                    Lookup.IsDeleted = false;
                                                    Lookup.RowInsertDate = DateTime.Now;
                                                    Lookup.IpAddress = Common.Security.GetIpAddress();
                                                    Lookup.RowInsertedBy = Security.GetUser().UserName;
                                                    iDbContext.LkLookups.AddOrUpdate(Lookup);
                                                    iDbContext.SaveChanges();

                                                    if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                    {
                                                        var LookupOptions = AutoMapper.ListObjectToListObjectMapper<Models.IELookupOption, Models.LookupOption>(iInput.Lookup.LookupOption);

                                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[LookupOptions] OFF");
                                                        foreach (var iLkOptions in LookupOptions)
                                                        {
                                                            iLkOptions.IsDeleted = false;
                                                            iLkOptions.RowInsertDate = DateTime.Now;
                                                            iDbContext.LookupOptions.AddOrUpdate(iLkOptions);
                                                            iDbContext.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                                    {
                                        var aut = new Models.UserScreenAction();
                                        aut.ActionId = 7;
                                        aut.ServiceId = Service.ServiceId;
                                        aut.CategoryId = 0;
                                        aut.UniqueId = xService.ExternalServiceID;
                                        aut.Remarks = Security.GetUser().UserName + " IMPORTED SERVICE  = " + xService.DescriptionEng + "-" + xService.ExternalServiceID + "-" + fileName;
                                        aut.RowInsertDate = DateTime.Now;
                                        aut.IpAddress = Common.Security.GetIpAddress();
                                        aut.RowInsertedBy = Security.GetUser().UserName;
                                        aut.IsDeleted = false;
                                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                                        iDbContext.SaveChanges();
                                    }
                                }
                            }
                            iTransaction.Commit();
                            hasTransaction = false;
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                if (hasTransaction && iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
                sb.AppendLine();
                return Json(new { Result = false, Message = serviceId + "-" + sb.ToString(), Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                if (hasTransaction && iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }
                return Json(new { Result = false, Message = serviceId + "-" + E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = serviceId + "-" + DbManager.GetText("Services", "lblImportSuccess", " Service Imported Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImportService2()
        {
            DbContextTransaction iTransaction = null;
            //Models.ImportExport existingRecord = null;
            string serviceId = "";
            string fileName = "";
            bool hasTransaction = false;
            try
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
                    fileName = file.FileName;
                    System.IO.FileInfo f = new System.IO.FileInfo(fileName);

                    BinaryReader b = new BinaryReader(file.InputStream);
                    byte[] binData = b.ReadBytes(file.ContentLength);

                    string result = System.Text.Encoding.UTF8.GetString(binData);

                    var importedObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.ImportExport2>(result);
                    if (importedObject != null)
                    {
                        serviceId = importedObject.Services[0].ExternalServiceID;
                        using (iTransaction = iDbContext.Database.BeginTransaction())
                        {
                            hasTransaction = true;

                            foreach (var Service in importedObject.Services)
                            {
                                //Step 1 : Check if Service Exists then Log Record -> Delete Records -> Import
                                string temp = Service.ExternalServiceID;
                                var existingObject = iDbContext.Services.Where(x => x.ExternalServiceID == temp).FirstOrDefault();
                                if (existingObject != null)
                                {
                                    string sql = "Exec dbo.ES_LogRecordsBeforeImport @ExternalServiceID = '" + temp + "'";
                                    iDbContext.Database.ExecuteSqlCommand(sql);
                                }

                                //Step 2 : Start Importing the Service
                                int newLkLookupId = iDbContext.LkLookups.Max(x => x.LookupId);

                                //ServiceCategories...
                                if (Service.ServiceCategories != null && Service.ServiceCategories.Count > 0)
                                {
                                    foreach (var serviceCategory in Service.ServiceCategories)
                                    {
                                        //ServiceFormsConfig...
                                        if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                        {
                                            var serviceFormConfig = Service.ServiceFormsConfig.Where(x => x.CategorId == serviceCategory.CategoryId).ToList();
                                            foreach (var iInput in serviceFormConfig)
                                            {
                                                //Lookups...
                                                if (iInput.OptionId.HasValue && iInput.OptionId > 0)
                                                {
                                                    if (iInput.Lookup != null)
                                                    {
                                                        var isLookupExists = iDbContext.LkLookups.Where(x => x.LookUpName == iInput.Lookup.LookUpName).FirstOrDefault();
                                                        if (isLookupExists != null)
                                                        {
                                                            iInput.OptionId = isLookupExists.LookupId;
                                                            if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                            {
                                                                foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                                {
                                                                    iLkOptions.LookupId = isLookupExists.LookupId;
                                                                }
                                                            }

                                                        }
                                                        else
                                                        {
                                                            newLkLookupId = newLkLookupId + 1;
                                                            iInput.OptionId = newLkLookupId;
                                                            if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                            {
                                                                foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                                {
                                                                    iLkOptions.LookupId = newLkLookupId;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        serviceCategory.ServiceId = serviceCategory.ServiceId;
                                        serviceCategory.CategoryId = serviceCategory.CategoryId;
                                        serviceCategory.CategoryCode = "SC-" + serviceCategory.CategoryId;
                                    }
                                }
                                else
                                {
                                    //ServiceFormsConfig...
                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        var serviceFormConfig = Service.ServiceFormsConfig.ToList();
                                        foreach (var iInput in serviceFormConfig)
                                        {
                                            //Lookups...
                                            if (iInput.OptionId.HasValue && iInput.OptionId > 0)
                                            {
                                                if (iInput.Lookup != null)
                                                {
                                                    var isLookupExists = iDbContext.LkLookups.Where(x => x.LookUpName == iInput.Lookup.LookUpName).FirstOrDefault();
                                                    if (isLookupExists != null)
                                                    {
                                                        iInput.OptionId = isLookupExists.LookupId;
                                                        if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                        {
                                                            foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                            {
                                                                iLkOptions.LookupId = isLookupExists.LookupId;
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        newLkLookupId = newLkLookupId + 1;
                                                        iInput.OptionId = newLkLookupId;
                                                        if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                        {
                                                            foreach (var iLkOptions in iInput.Lookup.LookupOption)
                                                            {
                                                                iLkOptions.LookupId = newLkLookupId;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                //Bind IE Model to EF Model Here...AND SAVE...
                                {
                                    var dt = Utility.ConvertObjectToDataTable(Service, "Services");

                                    string sql = "insert into Services (";

                                    foreach (System.Data.DataColumn dc in dt.Columns)
                                    {
                                        sql += dc.ColumnName + ", ";
                                    }
                                    sql = sql.Remove(sql.Length - 1, 1);
                                    sql += ") Values (";


                                    for (int i = 0; i < dt.Rows.Count; i++)
                                    {
                                        foreach (System.Data.DataColumn dc in dt.Columns)
                                        {
                                            sql += "'" + dt.Rows[i][dc.ColumnName].ToString().Trim() + "', ";
                                        }
                                        
                                    }
                                    sql = sql.Remove(sql.Length - 1, 1);
                                    sql += ")";

                                    var xService = AutoMapper.ObjectToObjectMapper<Models.IEService, Models.Service>(Service, null);
                                    if (xService != null)
                                    {
                                        xService.IsDeleted = false;
                                        xService.RowInsertDate = DateTime.Now;
                                        xService.IpAddress = Common.Security.GetIpAddress();
                                        xService.RowInsertedBy = Security.GetUser().UserName;
                                        iDbContext.Services.AddOrUpdate(xService);
                                        iDbContext.SaveChanges();
                                    }

                                    var ServiceTabs = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceTab, Models.ServiceTab>(Service.ServiceTabs);
                                    if (ServiceTabs != null && ServiceTabs.Count > 0)
                                    {
                                        foreach (var ServiceTab in ServiceTabs)
                                        {
                                            ServiceTab.IsDeleted = false;
                                            ServiceTab.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceTabs.AddOrUpdate(ServiceTab);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceViewers = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceViewer, Models.ServiceViewer>(Service.ServiceViewers);
                                    if (ServiceViewers != null && ServiceViewers.Count > 0)
                                    {
                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceViewers] OFF");

                                        foreach (var ServiceViewer in ServiceViewers)
                                        {
                                            ServiceViewer.IsDeleted = false;
                                            ServiceViewer.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceViewers.AddOrUpdate(ServiceViewer);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceUserTypes = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceUserTypes, Models.ServiceUserType>(Service.ServiceUserTypes);
                                    if (ServiceUserTypes != null && ServiceUserTypes.Count > 0)
                                    {
                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceViewers] OFF");

                                        foreach (var ServiceUserType in ServiceUserTypes)
                                        {
                                            ServiceUserType.IsDeleted = false;
                                            ServiceUserType.RowInsertDate = DateTime.Now;
                                            iDbContext.ServiceUserTypes.AddOrUpdate(ServiceUserType);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceCategories = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceCategory, Models.ServiceCategory>(Service.ServiceCategories);
                                    if (ServiceCategories != null && ServiceCategories.Count > 0)
                                    {
                                        foreach (var ServiceCategory in ServiceCategories)
                                        {
                                            ServiceCategory.IsDeleted = false;
                                            ServiceCategory.RowInsertDate = DateTime.Now;
                                            ServiceCategory.IpAddress = Common.Security.GetIpAddress();
                                            ServiceCategory.RowInsertedBy = Security.GetUser().UserName;
                                            iDbContext.ServiceCategories.AddOrUpdate(ServiceCategory);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    var ServiceCommentsAttachmentsConfig = AutoMapper.ObjectToObjectMapper<Models.IEServiceCommentsAttachmentsConfig, Models.ServiceCommentsAttachmentsConfig>(Service.ServiceCommentsAttachmentsConfig, null);
                                    if (ServiceCommentsAttachmentsConfig != null)
                                    {
                                        ServiceCommentsAttachmentsConfig.IsDeleted = false;
                                        ServiceCommentsAttachmentsConfig.RowInsertDate = DateTime.Now;
                                        ServiceCommentsAttachmentsConfig.IpAddress = Common.Security.GetIpAddress();
                                        ServiceCommentsAttachmentsConfig.RowInsertedBy = Security.GetUser().UserName;

                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceCommentsAttachmentsConfig] OFF");
                                        iDbContext.ServiceCommentsAttachmentsConfigs.AddOrUpdate(ServiceCommentsAttachmentsConfig);
                                        iDbContext.SaveChanges();
                                    }

                                    var ServiceFormsConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsConfig, Models.ServiceFormsConfig>(Service.ServiceFormsConfig);
                                    if (ServiceFormsConfig != null && ServiceFormsConfig.Count > 0)
                                    {
                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceFormsConfig] OFF");
                                        foreach (var sfc in ServiceFormsConfig)
                                        {
                                            sfc.IsDeleted = false;
                                            sfc.RowInsertDate = DateTime.Now;
                                            sfc.IpAddress = Common.Security.GetIpAddress();
                                            sfc.RowInsertedBy = Security.GetUser().UserName;
                                            iDbContext.ServiceFormsConfigs.AddOrUpdate(sfc);
                                            iDbContext.SaveChanges();
                                        }
                                    }

                                    if (Service.ServiceFormsConfig != null && Service.ServiceFormsConfig.Count > 0)
                                    {
                                        foreach (var iInput in Service.ServiceFormsConfig)
                                        {
                                            if (iInput.ServiceTemplates != null)
                                            {
                                                var ServiceTemplates = AutoMapper.ObjectToObjectMapper<Models.IEServiceTemplate, Models.ServiceTemplate>(iInput.ServiceTemplates, null);
                                                if (ServiceTemplates != null)
                                                {
                                                    ServiceTemplates.IsDeleted = false;
                                                    ServiceTemplates.RowInsertDate = DateTime.Now;
                                                    ServiceTemplates.IpAddress = Common.Security.GetIpAddress();
                                                    ServiceTemplates.RowInsertedBy = Security.GetUser().UserName;

                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceTemplates] OFF");
                                                    iDbContext.ServiceTemplates.AddOrUpdate(ServiceTemplates);
                                                    iDbContext.SaveChanges();
                                                }
                                            }

                                            if (iInput.ServiceFormsDrilldownConfig != null)
                                            {
                                                var ServiceFormsDrilldownConfig = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceFormsDrilldownConfig, Models.ServiceFormsDrilldownConfig>(iInput.ServiceFormsDrilldownConfig);
                                                if (ServiceFormsDrilldownConfig != null && ServiceFormsDrilldownConfig.Count > 0)
                                                {
                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceFormsDrilldownConfig] OFF");
                                                    foreach (var sfc in ServiceFormsDrilldownConfig)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        sfc.IpAddress = Common.Security.GetIpAddress();
                                                        sfc.RowInsertedBy = Security.GetUser().UserName;
                                                        iDbContext.ServiceFormsDrilldownConfigs.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputLookupActions != null)
                                            {
                                                var ServiceInputLookupActions = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputLookupAction, Models.ServiceInputLookupAction>(iInput.ServiceInputLookupActions);
                                                if (ServiceInputLookupActions != null && ServiceInputLookupActions.Count > 0)
                                                {
                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceInputLookupActions] OFF");
                                                    foreach (var sfc in ServiceInputLookupActions)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputLookupActions.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.ServiceInputTooltips != null)
                                            {
                                                var ServiceInputTooltips = AutoMapper.ListObjectToListObjectMapper<Models.IEServiceInputTooltip, Models.ServiceInputTooltip>(iInput.ServiceInputTooltips);
                                                if (ServiceInputTooltips != null && ServiceInputTooltips.Count > 0)
                                                {
                                                    //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[ServiceInputTooltips] OFF");
                                                    foreach (var sfc in ServiceInputTooltips)
                                                    {
                                                        sfc.IsDeleted = false;
                                                        sfc.RowInsertDate = DateTime.Now;
                                                        iDbContext.ServiceInputTooltips.AddOrUpdate(sfc);
                                                        iDbContext.SaveChanges();
                                                    }
                                                }
                                            }

                                            if (iInput.Lookup != null)
                                            {
                                                var Lookup = AutoMapper.ObjectToObjectMapper<Models.IELkLookup, Models.LkLookup>(iInput.Lookup, null);
                                                if (Lookup != null)
                                                {
                                                    Lookup.IsDeleted = false;
                                                    Lookup.RowInsertDate = DateTime.Now;
                                                    Lookup.IpAddress = Common.Security.GetIpAddress();
                                                    Lookup.RowInsertedBy = Security.GetUser().UserName;
                                                    iDbContext.LkLookups.AddOrUpdate(Lookup);
                                                    iDbContext.SaveChanges();

                                                    if (iInput.Lookup.LookupOption != null && iInput.Lookup.LookupOption.Count > 0)
                                                    {
                                                        var LookupOptions = AutoMapper.ListObjectToListObjectMapper<Models.IELookupOption, Models.LookupOption>(iInput.Lookup.LookupOption);

                                                        //iDbContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[LookupOptions] OFF");
                                                        foreach (var iLkOptions in LookupOptions)
                                                        {
                                                            iLkOptions.IsDeleted = false;
                                                            iLkOptions.RowInsertDate = DateTime.Now;
                                                            iDbContext.LookupOptions.AddOrUpdate(iLkOptions);
                                                            iDbContext.SaveChanges();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                                    {
                                        var aut = new Models.UserScreenAction();
                                        aut.ActionId = 7;
                                        aut.ServiceId = Service.ServiceId;
                                        aut.CategoryId = 0;
                                        aut.UniqueId = xService.ExternalServiceID;
                                        aut.Remarks = Security.GetUser().UserName + " IMPORTED SERVICE  = " + xService.DescriptionEng + "-" + xService.ExternalServiceID + "-" + fileName;
                                        aut.RowInsertDate = DateTime.Now;
                                        aut.IpAddress = Common.Security.GetIpAddress();
                                        aut.RowInsertedBy = Security.GetUser().UserName;
                                        aut.IsDeleted = false;
                                        iDbContext.UserScreenActions.AddOrUpdate(aut);
                                        iDbContext.SaveChanges();
                                    }
                                }
                            }
                            iTransaction.Commit();
                            hasTransaction = false;
                        }
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                if (hasTransaction && iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("- Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().FullName, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("-- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage));
                    }
                }
                sb.AppendLine();
                return Json(new { Result = false, Message = serviceId + "-" + sb.ToString(), Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception E)
            {
                if (hasTransaction && iTransaction != null)
                {
                    iTransaction.Dispose();
                    iTransaction = null;
                }
                return Json(new { Result = false, Message = serviceId + "-" + E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = true, Message = serviceId + "-" + DbManager.GetText("Services", "lblImportSuccess", " Service Imported Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [UrlDecode]
        public ActionResult NoAction()
        {
            return View();
        }
        [UrlDecode]
        public ActionResult NoActionsData(string id)
        {
            var iData = iDbContext.ES_GetNoActionsUserData(id, string.Empty).ToList();

            ViewBag.UserName = id;

            return View(iData);
        }

        [UrlDecode]
        public ActionResult NoActionsSummary()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");

            List<Models.KeyValue> iList = new List<Models.KeyValue>();
            var X = iDbContext.Services.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
            foreach (var Y in X)
            {
                var Z = iDbContext.NoActionUsers.Where(x => x.IsDeleted == false && (x.ExternalServiceId.ToLower() == Y.ExternalServiceID.ToLower())).FirstOrDefault();
                if (Z != null)
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? Y.ExternalServiceID : Y.ExternalServiceID;
                    iKv.Value = Y.ServiceId;
                    iList.Add(iKv);
                }
            }
            if (iList != null && iList.Count > 0)
                ViewBag.ServicesList = new SelectList(iList.ToArray(), "Value", "Key", iList[0].Value.ToString());
            else
                ViewBag.ServicesList = new SelectList(iList.ToArray(), "Value", "Key");

            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetNoActionsSummary(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                if (Security.isUserSystemAdministrator() == false)
                {
                    iSearch.DepartmentId = Security.GetUser().DepartmentId;
                }

                if (iSearch.FromDate.HasValue && iSearch.ToDate.HasValue)
                {
                    string fromDate = iSearch.FromDate.Value.ToString("MM/dd/yyyy") + " 00:00:01";
                    string todate = iSearch.ToDate.Value.ToString("MM/dd/yyyy") + " 23:59:59";

                    iSearch.FromDate = DateTime.Parse(fromDate);
                    iSearch.ToDate = DateTime.Parse(todate);
                }

                var iData = Common.DbManager.GetNoActionsSummary(iSearch);
                if (iData != null && iData.Count > 0)
                    totalRows = iData[0].TotalRows.Value;

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                      c.RecordId
                    , GetNoActionDataLink(c)
                    , c.Total
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
                throw E;
            }
            return Json(new { result = false, Error = "" }, JsonRequestBehavior.AllowGet);
        }
        private string GetNoActionDataLink(Models.ES_GetNoActionsSummary_Result c)
        {
            string url = Common.Helper.URL_Encode(Url.Action("NoActionsData", "Services", null, Request.Url.Scheme, null) + "?id=" + c.UserName);
            string html = "";
            {
                html += "<a href='#'  onclick=\"ViewNoActionsData('" + url + "');\" class='subtitle'>" + c.UserName + "</a>";
            }
            return html;
        }
    }
}