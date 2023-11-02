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
    public class UsersController : BaseController
    {
        #region --> USERS 

        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");

            var Roles = Common.DbManager.GetLkRoles();
            //ViewBag.Roles = new SelectList(Roles.ToArray(), "RoleId", "Description");
            if (Helper.CurrentLanguage() == (int)Language.English)
            {
                ViewBag.Roles = new SelectList(Roles.ToArray(), "RoleId", "Description");
            }
            else
            {
                ViewBag.Roles = new SelectList(Roles.ToArray(), "RoleId", "DescriptionAlt");
            }

            var Departments = Common.DbManager.GetDepartments();
            //ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
            if (Helper.CurrentLanguage() == (int)Language.English)
            {
                ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentName");
            }
            else
            {
                ViewBag.Departments = new SelectList(Departments.ToArray(), "DepartmentId", "DepartmentNameAlt");
            }

            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetUsers(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetUsers(iSearch);
                if (iData != null && iData.Count > 0)
                    totalRows = iData[0].TotalRows;

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                        c.FullName
                        ,c.DepartmentName
                        ,c.Email
                        ,c.Mobile
                        ,c.UserName
                        ,c.RoleName
                        ,GetUserEditIcon(c)
                        ,GetUserDeleteIcon(c)
                        ,GetUserActDactIcon(c)
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

        private string GetUserEditIcon(Models.UserObject c)
        {
            string html = "";
            if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_USER") == true)
            {
                string passWord = "";
                //if (string.IsNullOrEmpty(c.Password) == false)
                //{
                //    passWord = Common.Helper.DecodeFromBase64String(c.Password);
                //}
                html += "<img title='' src=\"" + Url.Content("~/Content/images/Ico_Edit.gif") + "\" style='cursor:pointer;' onclick=\"ModifyUser('" + c.UserId + "','" + c.RoleId + "','" + c.DepartmentId + "','" + c.FullName + "','" + c.Email + "','" + c.Mobile + "','" + c.UserName + "','" + passWord + "','" + c.FullNameAlt + "')\" />";
            }
            else
            {
                html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
            }
            return html;
        }
        private string GetUserDeleteIcon(Models.UserObject c)
        {
            string html = "";
            if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_USER") == true)
            {
                html += "<img title='' src=\"" + Url.Content("~/Content/images/delete_icon.gif") + "\" style='cursor:pointer;' onclick=\"DeleteUser('" + c.UserId + "','" + c.FullName + "','" + c.FullNameAlt + "')\" />";
            }
            else
            {
                html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
            }
            return html;
        }
        private string GetUserActDactIcon(Models.UserObject c)
        {
            string html = "";
            if (c.IsActive == true)
            {
                if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ACTDACT_UESR") == true)
                {
                    html += "<img title='Click to DeActivate User' src=\"" + Url.Content("~/Content/images/user.gif") + "\" style='cursor:pointer;' onclick=\"DeActivateUser('" + c.UserId + "','" + c.FullName + "','" + c.FullNameAlt + "')\" />";
                }
                else
                {
                    html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lbluInactive", "Inactive") + "</a>";
                }
            }
            else
            {
                if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_ACTDACT_UESR") == true)
                {
                    html += "<img title='Click to Activate User' src=\"" + Url.Content("~/Content/images/userdisable.gif") + "\" style='cursor:pointer;' onclick=\"ActivateUser('" + c.UserId + "','" + c.FullName + "','" + c.FullNameAlt + "')\" />";
                }
                else
                {
                    html += "<a href='#' style='display:none;' onclick=\"NoPermission();\" class='link'>" + DbManager.GetText("Lookup", "lbluActive", "Active") + "</a>";
                }
            }
            
            return html;
        }

        [HttpPost]
        public JsonResult AjaxSaveUser(Models.UserObject iObject)
        {
            try
            {
                iObject = iObject.TrimObject();

                var iRecord = iDbContext.Users.SingleOrDefault(x => x.UserId == iObject.UserId);
                if (iRecord != null)
                {
                    var b = iDbContext.Users.Where(x => x.IsDeleted == false && x.UserId != iRecord.UserId && x.UserName.ToUpper() == iObject.UserName.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.UserName + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon="info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.Users.Where(x => x.IsDeleted == false && x.UserId != iRecord.UserId && x.Email.ToUpper() == iObject.Email.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.Email + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord.UserId = iObject.UserId;
                    iRecord.RoleId = iObject.RoleId;
                    iRecord.DepartmentId = iObject.DepartmentId;
                    iRecord.FullName = iObject.FullName;
                    iRecord.FullNameAlt = iObject.FullNameAlt;
                    iRecord.Email = iObject.Email;
                    iRecord.UserName = iObject.UserName;
                    iRecord.Mobile = iObject.Mobile;
                    //iRecord.Password = Common.Helper.EncodeToBase64String("12345678");
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iDbContext.Users.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " updated user record = " + iRecord.UserName;
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
                    var b = iDbContext.Users.Where(x => x.IsDeleted == false && x.UserName.ToUpper() == iObject.UserName.ToUpper()).FirstOrDefault().TrimObject();
                    if (b != null)
                        return Json(new { Result = false, Message = iObject.UserName + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    var c = iDbContext.Users.Where(x => x.IsDeleted == false && x.Email.ToUpper() == iObject.Email.ToUpper()).FirstOrDefault().TrimObject();
                    if (c != null)
                        return Json(new { Result = false, Message = iObject.Email + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                    iRecord = new Models.User();

                    if (iObject.RoleId > 0)
                        iRecord.RoleId = iObject.RoleId;
                    iRecord.DepartmentId = iObject.DepartmentId;
                    iRecord.FullName = iObject.FullName;
                    iRecord.FullNameAlt = iObject.FullNameAlt;
                    iRecord.Email = iObject.Email;
                    iRecord.UserName = iObject.UserName;
                    iRecord.Mobile = iObject.Mobile;
                    iRecord.Password = Common.Helper.EncodeToBase64String("12345678");
                    iRecord.IsFirstLogin = false;
                    iRecord.IsActive = true;
                    iRecord.IsDeleted = false;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowInsertedBy = Security.GetUser().UserName;
                    iRecord.RowInsertDate = DateTime.Now;
                    iDbContext.Users.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.Remarks = Security.GetUser().UserName + " added user record = " + iRecord.UserName;
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
            return Json(new { Result = true, Message = iObject.UserName + DbManager.GetText("Users", "lblSaveSuccess", " User Record Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteUser(int userId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Users.SingleOrDefault(x => x.UserId == userId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Users.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = iDbContext.Users.Where(x => x.UserId == userId).FirstOrDefault().UserName;
                        aut.Remarks = Security.GetUser().UserName + " delete user record = " + iRecord.UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblUserDel", "User Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxActivateUser(int userId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Users.SingleOrDefault(x => x.UserId == userId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Users.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = iDbContext.Users.Where(x => x.UserId == userId).FirstOrDefault().UserName;
                        aut.Remarks = Security.GetUser().UserName + " activated user record = " + iRecord.UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblUserAct", " User Activated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeActivateUser(int userId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Users.SingleOrDefault(x => x.UserId == userId);
                    iRecord.IsDeleted = false;
                    iRecord.IsActive = false;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Users.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = iDbContext.Users.Where(x => x.UserId == userId).FirstOrDefault().UserName;
                        aut.Remarks = Security.GetUser().UserName + " deactivated user record = " + iRecord.UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblUserDeact", " User DeActivated Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxChangePassword(Models.ChangePassword iObject)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                iObject = iObject.TrimObject();

                int userId = Security.GetUser().UserId;

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Users.SingleOrDefault(x => x.UserId == userId);
                    iRecord.Password = Common.Helper.EncodeToBase64String(iObject.NewPassword);
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Users.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = iDbContext.Users.Where(x => x.UserId == userId).FirstOrDefault().UserName;
                        aut.Remarks = Security.GetUser().UserName + " changed password for user record = " + iRecord.UserName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblUserChangePassword", "Password Changed Successfully. Please Login with New Password!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region --> ROLES 

        [UrlDecode]
        public ActionResult Roles()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");
            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetRoles()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetLkRoles();
                if (result != null && result.Count > 0)
                {
                    int nctr = 1;
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>"+ nctr + "</div>";
                        html += "<div class='tag'>{1}</div>";
                        html += "<div class='tag'>{3}</div>";
                        html += "<br />";
                        html += "<div class='title nameEn'>" + DbManager.GetText("Lookup", "lblTotalGroups", "Total Groups") + " : {2}</div>";
                        html += "<div class='links'>";
                        
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_ROLE") == true)
                        {
                            html += "<a href='#' onclick=\"ModifyRole('{0}','{1}','{3}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }

                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_ROLE") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteRole('{0}','{1}','{3}')\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }

                        html += "</div>";
                        html += "</div>";
                        html += "</div>";

                        var groupCount = iDbContext.RoleGroups.Where(x => x.RoleId == i.RoleId).ToList();

                        html = string.Format(html, i.RoleId, i.Description, groupCount.Count,i.DescriptionAlt);
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
        public JsonResult AjaxSaveRole(Models.LkRoleObject iObject)
        {
            DbContextTransaction iTransaction = null;
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.LkRoles.SingleOrDefault(x => x.RoleId == iObject.RoleId);
                    if (iRecord != null)
                    {
                        var b = iDbContext.LkRoles.Where(x => x.IsDeleted == false && x.RoleId != iRecord.RoleId && x.Description.ToUpper() == iObject.Description.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = iObject.Description + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        iRecord.RoleId = iObject.RoleId;
                        iRecord.Description = iObject.Description;
                        iRecord.DescriptionAlt = iObject.DescriptionAlt;
                        iRecord.IsDeleted = false;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iDbContext.LkRoles.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();

                        if (iObject.RoleGroups != null && iObject.RoleGroups.Count > 0)
                        {
                            var a = iDbContext.RoleGroups.Where(x => x.IsDeleted == false && x.RoleId == iObject.RoleId).ToList();
                            if (a != null)
                            {
                                iDbContext.RoleGroups.RemoveRange(a);
                                iDbContext.SaveChanges();
                            }

                            foreach (var xx in iObject.RoleGroups)
                            {
                                var x = new Models.RoleGroup();
                                x.RoleId = iObject.RoleId;
                                x.GroupId = xx.GroupId;
                                x.RowInsertDate = DateTime.Now;
                                x.RowInsertedBy = Security.GetUser().UserName;
                                x.IsDeleted = false;
                                x.IpAddress = Common.Security.GetIpAddress();
                                iDbContext.RoleGroups.AddOrUpdate(x);
                                iDbContext.SaveChanges();
                            }
                        }

                        if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                        {
                            var aut = new Models.UserScreenAction();
                            aut.ActionId = 7;
                            aut.UniqueId = iObject.RoleId.ToString();
                            aut.Remarks = Security.GetUser().UserName + " updated role record = " + iRecord.Description;
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
                        var b = iDbContext.LkRoles.Where(x => x.IsDeleted == false && x.Description.ToUpper() == iObject.Description.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = iObject.Description + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        iRecord = new Models.LkRole();

                        var i = iDbContext.LkRoles.Where(x => x.IsDeleted == false).ToList();
                        if (i != null && i.Count > 0)
                            Code = i[i.Count - 1].RoleId + 1;

                        iRecord.RoleId = Convert.ToInt16(Code);
                        iRecord.Description = iObject.Description;
                        iRecord.DescriptionAlt = iObject.DescriptionAlt;
                        iRecord.IsDeleted = false;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowInsertedBy = Security.GetUser().UserName;
                        iRecord.RowInsertDate = DateTime.Now;
                        iDbContext.LkRoles.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();

                        if (iObject.RoleGroups != null && iObject.RoleGroups.Count > 0)
                        {
                            foreach (var xx in iObject.RoleGroups)
                            {
                                var x = new Models.RoleGroup();
                                x.RoleId = iRecord.RoleId;
                                x.GroupId = xx.GroupId;
                                x.RowInsertDate = DateTime.Now;
                                x.RowInsertedBy = Security.GetUser().UserName;
                                x.IsDeleted = false;
                                x.IpAddress = Common.Security.GetIpAddress();
                                iDbContext.RoleGroups.AddOrUpdate(x);
                                iDbContext.SaveChanges();
                            }
                        }

                        if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                        {
                            var aut = new Models.UserScreenAction();
                            aut.ActionId = 7;
                            aut.UniqueId = Code.ToString();
                            aut.Remarks = Security.GetUser().UserName + " added role record = " + iRecord.Description;
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
            return Json(new { Result = true, Message = iObject.Description + DbManager.GetText("Users", "lblRoleSave", " Role Record Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteRole(int roleId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.LkRoles.SingleOrDefault(x => x.RoleId == roleId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.LkRoles.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    var a = iDbContext.RoleGroups.Where(x => x.IsDeleted == false && x.RoleId == roleId).ToList();
                    if (a != null)
                    {
                        iDbContext.RoleGroups.RemoveRange(a);
                        iDbContext.SaveChanges();
                    }

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = roleId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " delete user record = " + iRecord.Description;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblRoleDelete", "Role Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRoleGroups(int roleId)
        {
            string html = string.Empty;
            JsonResult iResult = null;
            try
            {
                var iGroupRoles = DbManager.GetLkRoleObject(roleId);
                if (iGroupRoles != null && iGroupRoles.RoleGroups != null && iGroupRoles.RoleGroups.Count > 0)
                {
                    html = GenerateRoleGroups(iGroupRoles.RoleGroups);
                }
                else
                {
                    html = GenerateRoleGroups(null);
                }

                iResult = new JsonResult();
                iResult.Data = new { result = true, html = html };
                iResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                iResult.MaxJsonLength = int.MaxValue;
                iResult.ContentType = "application/json";
                return iResult;
            }
            catch (Exception E)
            {
                return Json(new { result = false, html = E.Message, Icon = "error" }, JsonRequestBehavior.AllowGet);
                throw E;
            }
            return Json(new { result = false, html ="", url = "", Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        private static string GenerateRoleGroups(List<Models.LkGroup> lRoleGroups)
        {
            string retHtml = "<div class='blocks row page_links'>";
            List<Models.LkGroup> lAllGroups = null;
            bool isRG = false;
            string screenName = "";

            var iDbx = new EServicesCms.Models.MOFPortalEntities();

            try
            {
                lAllGroups = iDbx.LkGroups.Where(x => x.IsDeleted == false).OrderBy(x => x.ScreenId).ToList();

                List<Int16?> screenIds = lAllGroups.Select(c => c.ScreenId).Distinct().ToList();

                if (lAllGroups != null && lAllGroups.Count > 0)
                {
                    foreach (Int16? screenId in screenIds)
                    {
                        if (screenId == 0)
                            screenName = "System";
                        else
                            screenName = iDbx.LkGroups.Where(x => x.IsDeleted == false && x.ScreenId == screenId).FirstOrDefault().Description;
                        //
                        retHtml += @"<div class='col-12 col-md-5' style='overflow-y:scroll; height:200px; overflow-x: hidden;overflow-y: visible;border:1px solid #b68a35;margin:2px 2px 2px 2px;'>
                                             <div class='tag'>                                                    
								                    <h4 class='title nameEn'>%screenName%</h4>
							                    </div>";
                        retHtml = retHtml.Replace("%screenName%", screenName);
                        retHtml = retHtml.Replace("%screenId%", screenId.ToString());
                        //
                        //retHtml += "<div class='row'>";
                        foreach (Models.LkGroup iAll in lAllGroups.Where(x => x.ScreenId == screenId).OrderBy(x=>x.SortOrder).ToList())
                        {
                            isRG = false;
                            //
                            if (lRoleGroups != null && lRoleGroups.Count > 0)
                            {
                                Models.LkGroup iRG = lRoleGroups.Find(delegate (Models.LkGroup i) { return i.GroupId == iAll.GroupId; });
                                if (iRG != null)
                                    isRG = true;
                            }
                            //
                            retHtml += "<div class='row'>";
                            retHtml += "    <div class='col-xs-12'>";
                            retHtml += "        <label>";
                            if (isRG == true)
                                retHtml += "            <input name='switch-field-1" + screenId + "' class='ace ace-switch ace-switch-3' id='" + iAll.GroupId + "' type='checkbox' value='" + iAll.GroupId + "' checked />";
                            else
                                retHtml += "            <input name='switch-field-1" + screenId + "' class='ace ace-switch ace-switch-3' id='" + iAll.GroupId + "' type='checkbox' value='" + iAll.GroupId + "' />";

                            //if (iAll.ParentGroupId == 0)
                            //    retHtml += "            <span class='lbl' title='" + iAll.Description + "'>&nbsp;&nbsp;&nbsp;<strong>" + iAll.Description + "</strong></span>";
                            //else
                                retHtml += "            <span class='lbl' title='" + iAll.Description + "'>&nbsp;&nbsp;&nbsp;<span class='checkboxText'>" + iAll.Description + "</span></span>";

                            retHtml += "        </label>";
                            retHtml += "    </div>";
                            retHtml += "</div>";
                        }
                        //retHtml += "</div>";
                        retHtml += "</div>";
                    }
                }
                else
                    retHtml = "<B>NO RECORDS</B>";
                retHtml += "</div>";
            }
            catch (Exception E)
            {
                throw E;
            }
            return retHtml;
        }

        #endregion

        #region --> DEPARTMENTS 

        [UrlDecode]
        public ActionResult Departments()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");
            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetDepartment()
        {
            string html = "";
            try
            {
                var result = Common.DbManager.GetDepartments();
                if (result != null && result.Count > 0)
                {
                    int nctr = 1;
                    foreach (var i in result)
                    {
                        html += "<div class='col-12 col-md-6 col-lg-4 col-xxl-3' myName='BOND'>";
                        html += "<div class='card'>";
                        html += "<div class='circle'>" + nctr + "</div>";
                        html += "<div class='tag'>{1}</div>";
                        html += "<div class='tag'>{3}</div>";
                        html += "<br />";
                        html += "<div class='title nameEn'>" + DbManager.GetText("Lookup", "lblTotalUsers", "Total Users") + " : {2}</div>";
                        html += "<div class='links'>";
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_UPDATE_DEPARTMENT") == true)
                        {
                            html += "<a href='#' onclick=\"ModifyDepartment('{0}','{1}','{3}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}');\" class='link'>" + DbManager.GetText("Lookup", "lblEditIcon", "Edit") + "</a>";
                        }
                        if (EServicesCms.Common.Security.isUserAuthorized("MOF_ESRV_DELETE_DEPARTMENT") == true)
                        {
                            html += "<a href='#' onclick=\"DeleteDepartment('{0}','{1}','{3}');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        else
                        {
                            html += "<a href='#' style='display:none;' onclick=\"NoPermission('{0}','{1}','{3}');\" class='link'>" + DbManager.GetText("Lookup", "lblDelIcon", "Delete") + "</a>";
                        }
                        html += "</div>";
                        html += "</div>";
                        html += "</div>";

                        var usersCount = iDbContext.Users.Where(x => x.DepartmentId == i.DepartmentId).ToList();

                        html = string.Format(html, i.DepartmentId, i.DepartmentName, usersCount.Count,i.DepartmentNameAlt);
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
        public JsonResult AjaxSaveDepartment(Models.Department iObject)
        {
            DbContextTransaction iTransaction = null;
            int Code = 1;
            try
            {
                iObject = iObject.TrimObject();

                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Departments.SingleOrDefault(x => x.DepartmentId == iObject.DepartmentId);
                    if (iRecord != null)
                    {
                        var b = iDbContext.Departments.Where(x => x.IsDeleted == false && x.DepartmentId != iRecord.DepartmentId && x.DepartmentName.ToUpper() == iObject.DepartmentName.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = iObject.DepartmentName + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        iRecord.DepartmentId = iObject.DepartmentId;
                        iRecord.DepartmentName = iObject.DepartmentName;
                        iRecord.DepartmentNameAlt = iObject.DepartmentNameAlt;
                        iRecord.IsDeleted = false;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowUpdatedBy = Security.GetUser().UserName;
                        iRecord.RowUpdateDate = DateTime.Now;
                        iDbContext.Departments.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();

                        if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                        {
                            var aut = new Models.UserScreenAction();
                            aut.ActionId = 7;
                            aut.UniqueId = iObject.DepartmentId.ToString();
                            aut.Remarks = Security.GetUser().UserName + " updated Departments record = " + iRecord.DepartmentName;
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
                        var b = iDbContext.Departments.Where(x => x.IsDeleted == false && x.DepartmentName.ToUpper() == iObject.DepartmentName.ToUpper()).FirstOrDefault().TrimObject();
                        if (b != null)
                            return Json(new { Result = false, Message = iObject.DepartmentName + " " + DbManager.GetText("Lookup", "lblExistts", "Already Exists"), Icon = "info", Redirect = "" }, JsonRequestBehavior.AllowGet);

                        iRecord = new Models.Department();

                        var i = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
                        if (i != null && i.Count > 0)
                            Code = i[i.Count - 1].DepartmentId + 1;

                        iRecord.DepartmentId = Convert.ToInt16(Code);
                        iRecord.DepartmentName = iObject.DepartmentName;
                        iRecord.DepartmentNameAlt = iObject.DepartmentNameAlt;
                        iRecord.IsDeleted = false;
                        iRecord.IpAddress = Common.Security.GetIpAddress();
                        iRecord.RowInsertedBy = Security.GetUser().UserName;
                        iRecord.RowInsertDate = DateTime.Now;
                        iDbContext.Departments.AddOrUpdate(iRecord);
                        iDbContext.SaveChanges();

                        if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                        {
                            var aut = new Models.UserScreenAction();
                            aut.ActionId = 7;
                            aut.UniqueId = Code.ToString();
                            aut.Remarks = Security.GetUser().UserName + " added Departments record = " + iRecord.DepartmentName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblDeptSave", "Department Record Saved Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AjaxDeleteDepartment(int departmentId)
        {
            DbContextTransaction iTransaction = null;
            try
            {
                using (iTransaction = iDbContext.Database.BeginTransaction())
                {
                    var iRecord = iDbContext.Departments.SingleOrDefault(x => x.DepartmentId == departmentId);
                    iRecord.IsDeleted = true;
                    iRecord.RowUpdateDate = DateTime.Now;
                    iRecord.IpAddress = Common.Security.GetIpAddress();
                    iRecord.RowUpdatedBy = Security.GetUser().UserName;
                    iDbContext.Departments.AddOrUpdate(iRecord);
                    iDbContext.SaveChanges();

                    if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                    {
                        var aut = new Models.UserScreenAction();
                        aut.ActionId = 7;
                        aut.UniqueId = departmentId.ToString();
                        aut.Remarks = Security.GetUser().UserName + " delete Departments record = " + iRecord.DepartmentName;
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
            return Json(new { Result = true, Message = DbManager.GetText("Users", "lblDeptDelete", "Department Deleted Successfully.!"), Icon = "success" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}