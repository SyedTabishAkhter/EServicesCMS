using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EServicesCms.Models;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Net;
using System.Net.Http;
using System.Data;

namespace EServicesCms.Common
{
    public class DbManager
    {
        public static bool IsUserDuplicateUser(int userId, string emailAddress, string userName, string mobileNo)
        {
            bool isExists = false;
            MOFPortalEntities iDbContext = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                if (userId <= 0)
                {
                    var iUser = iDbContext.Users.Where(x => x.UserName == userName
                                                        || x.Email == emailAddress
                                                        || x.Mobile == mobileNo
                                                        ).FirstOrDefault();
                    if (iUser != null)
                        isExists = true;
                }
                else
                {
                    var iUser = iDbContext.Users.Where(x => x.UserId != userId && ( x.UserName == userName
                                                        || x.Email == emailAddress
                                                        || x.Mobile == mobileNo)
                                                        ).FirstOrDefault();
                    if (iUser != null)
                        isExists = true;
                }
                
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return isExists;
        }
        public static string GetText(string viewId, string labelId, string defaultText = "")
        {
            string abbreviationId = "";
            MOFPortalEntities iDbContext = null;
            try
            {
                EServicesCms.Models.LkScreenLabel iDE = CacheManager.GetApplicationCache().GetLabel(viewId, labelId);
                if (iDE != null)
                {
                    switch (Helper.CurrentLanguage())
                    {
                        case (int)Language.Arabic:
                            abbreviationId = iDE.DescriptionAlt;
                            break;
                        default:
                            abbreviationId = iDE.DescriptionEng;
                            break;
                    }
                    if (string.IsNullOrEmpty(abbreviationId))
                        abbreviationId = defaultText;
                }
                else
                {
                    if (!string.IsNullOrEmpty(viewId) && !string.IsNullOrEmpty(labelId) && !string.IsNullOrEmpty(defaultText))
                    {
                        iDbContext = new MOFPortalEntities();

                        iDE = new Models.LkScreenLabel();
                        iDE.ViewId = viewId;
                        iDE.LabelId = labelId;
                        iDE.DescriptionAlt = defaultText + " AR";
                        iDE.DescriptionEng = defaultText;
                        iDE.IsDeleted = false;
                        iDE.IpAddress = Common.Security.GetIpAddress();
                        if (Security.GetUser() != null)
                            iDE.RowInsertedBy = Security.GetUser().UserName;
                        else
                            iDE.RowInsertedBy = "anonymous";
                        iDE.RowInsertDate = DateTime.Now;
                        iDbContext.LkScreenLabels.AddOrUpdate(iDE);
                        iDbContext.SaveChanges();
                        //HttpContext.Current.Application["AppCache"] = new EServicesCms.Common.CacheManager();
                        HttpContext.Current.Session["AppCache"] = new EServicesCms.Common.CacheManager();
                    }
                    abbreviationId = defaultText;
                }
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return abbreviationId;
        }
        public static string GetPlaceHolderText(string viewId, string labelId, string defaultText = "")
        {
            string abbreviationId = "";
            MOFPortalEntities iDbContext = null;
            bool isExists = false;
            try
            {
                EServicesCms.Models.LkScreenLabel iDE = CacheManager.GetApplicationCache().GetLabel(viewId, labelId);
                if (iDE != null)
                {
                    switch (Helper.CurrentLanguage())
                    {
                        case (int)Language.Arabic:
                            abbreviationId = iDE.PhDescriptionEng;
                            break;
                        default:
                            abbreviationId = iDE.PhDescriptionAlt;
                            break;
                    }
                    if (!string.IsNullOrEmpty(abbreviationId))
                    {
                        isExists = true;
                    }
                }

                if (isExists == false)
                {
                    if (!string.IsNullOrEmpty(viewId) && !string.IsNullOrEmpty(labelId) && !string.IsNullOrEmpty(defaultText))
                    {
                        iDbContext = new MOFPortalEntities();

                        iDE = iDbContext.LkScreenLabels.Where(x => x.ViewId == viewId && x.LabelId == labelId).FirstOrDefault();
                        if (iDE != null)
                        {
                            //iDE.ViewId = viewId;
                            //iDE.LabelId = labelId;
                            //iDE.PhDescriptionAlt = defaultText + " عربي";
                            //iDE.PhDescriptionEng = defaultText;
                            //iDE.IsDeleted = false;
                            //iDE.IpAddress = Common.Security.GetIpAddress();
                            //if (Security.GetUser() != null)
                            //    iDE.RowInsertedBy = Security.GetUser().UserName;
                            //else
                            //    iDE.RowInsertedBy = "anonymous";
                            //iDE.RowInsertDate = DateTime.Now;
                            //iDbContext.LkScreenLabels.AddOrUpdate(iDE);
                            //iDbContext.SaveChanges();
                            //HttpContext.Current.Application["AppCache"] = new EServicesCms.Common.CacheManager();
                        }
                        abbreviationId = defaultText;
                    }
                }
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return defaultText;
        }
        public static List<Models.KeyValue> GetDashboard()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var activeServices = iDbContext.Services.Where(x => x.IsDeleted == false && x.IsActive == true && x.ParentServiceId == 0).ToList();
                if (activeServices != null && activeServices.Count > 0)
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard","lblActiveServices", "Active Services");// "Active Services";
                    iKv.Value = activeServices.Count;
                    iKv.SortOrder = 1;
                    iList.Add(iKv);
                }
                else
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblActiveServices", "Active Services");// "Active Services";
                    iKv.Value = 0;
                    iKv.SortOrder = 1;
                    iList.Add(iKv);
                }

                var inActiveServices = iDbContext.Services.Where(x => x.IsDeleted == false && x.IsActive == false && x.ParentServiceId == 0).ToList();
                if (inActiveServices != null && inActiveServices.Count > 0)
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblInActiveServices", "InActive Services");// "InActive Services";
                    iKv.Value = inActiveServices.Count;
                    iKv.SortOrder = 2;
                    iList.Add(iKv);
                }
                else
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblInActiveServices", "InActive Services");// "InActive Services";
                    iKv.Value = 0;
                    iKv.SortOrder = 2;
                    iList.Add(iKv);
                }

                var iServiceRequests = iDbContext.RequestsAndEnquiries.Where(x => x.IsDeleted == false).ToList();
                if (iServiceRequests != null && iServiceRequests.Count > 0)
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblServiceRequests", "Service Requests");// "Service Requests";
                    iKv.Value = iServiceRequests.Count;
                    iKv.SortOrder = 3;
                    iList.Add(iKv);
                }
                else
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblServiceRequests", "Service Requests");// "Service Requests";
                    iKv.Value = 0;
                    iKv.SortOrder = 3;
                    iList.Add(iKv);
                }

                var iLogger = iDbContext.Loggers.Where(x => x.IsDeleted == false).ToList();
                if (iLogger != null && iLogger.Count > 0)
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblErrorLogs", "Error Logs");//  "Error Logs";
                    iKv.Value = iLogger.Count;
                    iKv.SortOrder = 4;
                    iList.Add(iKv);
                }
                else
                {
                    var iKv = new Models.KeyValue();
                    iKv.Key = Common.DbManager.GetText("Dashboard", "lblErrorLogs", "Error Logs");//  "Error Logs";
                    iKv.Value = 0;
                    iKv.SortOrder = 4;
                    iList.Add(iKv);
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.KeyValue> GetDashboardByDepartment()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                List<Models.Department> deptList = null;

                if (Security.isUserSystemAdministrator() == false)
                {
                    int? userDepId = Security.GetUser().DepartmentId;
                    deptList = iDbContext.Departments.Where(x => x.IsDeleted == false && x.DepartmentId == userDepId).ToList();
                }
                else
                {
                    deptList = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
                }

                foreach(var iDpartment in deptList)
                {
                    var activeServices = iDbContext.Services.Where(x => x.IsDeleted == false && x.IsActive == true && x.ParentServiceId == 0 && x.DepartmentId == iDpartment.DepartmentId).ToList();
                    if (activeServices != null && activeServices.Count > 0)
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == 1 ? iDpartment.DepartmentName : iDpartment.DepartmentNameAlt;
                        iKv.Value = activeServices.Count;
                        iKv.SortOrder = 1;
                        iList.Add(iKv);
                    }
                    else
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == 1 ? iDpartment.DepartmentName : iDpartment.DepartmentNameAlt;
                        iKv.Value = 0;
                        iKv.SortOrder = 1;
                        iList.Add(iKv);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.KeyValue> GetDashboardByServiceTypes()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                List<Models.ServiceType> deptList = null;
                deptList = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false).ToList();

                foreach (var iDpartment in deptList)
                {
                    var activeServices = iDbContext.Services.Where(x => x.IsDeleted == false && x.IsActive == true && x.ParentServiceId == 0 && x.TypeId == iDpartment.TypeId).ToList();
                    if (activeServices != null && activeServices.Count > 0)
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? iDpartment.DescriptionEng : iDpartment.DescriptionAlt;
                        iKv.Value = activeServices.Count;
                        iKv.SortOrder = 1;
                        iList.Add(iKv);
                    }
                    else
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? iDpartment.DescriptionEng : iDpartment.DescriptionAlt;
                        iKv.Value = 0;
                        iKv.SortOrder = 1;
                        iList.Add(iKv);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.KeyValue> GetDashboardByServiceEntities()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                List<Models.ServiceEntity> deptList = null;
                deptList = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId > 0).ToList();

                foreach (var iDpartment in deptList)
                {
                    var activeServices = iDbContext.ServiceEntitiesMappings.Where(x => x.IsDeleted == false && x.EntityId == iDpartment.EntityId).ToList();
                    if (activeServices != null && activeServices.Count > 0)
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? iDpartment.DescriptionEng : iDpartment.DescriptionAlt;
                        iKv.Value = activeServices.Count;
                        iKv.SortOrder = 1;
                        iList.Add(iKv);
                    }
                    else
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? iDpartment.DescriptionEng : iDpartment.DescriptionAlt;
                        iKv.Value = 0;
                        iKv.SortOrder = 1;
                        iList.Add(iKv);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.User GetUser(int userId)
        {
            MOFPortalEntities iDbContext = null;
            Models.User i = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                i = iDbContext.Users.Where(x => x.UserId == userId).FirstOrDefault();
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return i;
        }
        public static string GetUserDepartment()
        {
            MOFPortalEntities iDbContext = null;
            string i = "";
            try
            {
                iDbContext = new MOFPortalEntities();

                int? deptId = Security.GetUser().DepartmentId;

                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    i = iDbContext.Departments.Where(x => x.DepartmentId == deptId).FirstOrDefault().DepartmentName;
                }
                else
                {
                    i = iDbContext.Departments.Where(x => x.DepartmentId == deptId).FirstOrDefault().DepartmentNameAlt;
                }
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return i;
        }
        public static string GetUserRole()
        {
            MOFPortalEntities iDbContext = null;
            string i = "";
            try
            {
                iDbContext = new MOFPortalEntities();

                int? RoleId = Security.GetUser().RoleId;

                
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    i = iDbContext.LkRoles.Where(x => x.RoleId == RoleId).FirstOrDefault().Description;
                }
                else
                {
                    i = iDbContext.LkRoles.Where(x => x.RoleId == RoleId).FirstOrDefault().DescriptionAlt;
                }
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return i;
        }
        public static List<Models.ServiceEntityObject> GetServiceEntites()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceEntityObject> iList = new List<Models.ServiceEntityObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false).ToList().TrimList();
                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.ServiceEntity>(i);

                    foreach (var j in i)
                    {
                        var k = new Models.ServiceEntityObject();
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.RemarksAlt = j.RemarksAlt;
                        k.RemarksEng = j.RemarksEng;
                        k.ClassName = j.ClassName;
                        k.EntityId = j.EntityId;
                        k.IsDeleted = j.IsDeleted;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.SortOrder = j.SortOrder;
                        iList.Add(k);
                    }
                }
                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceTypeObject> GetServiceTypes()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceTypeObject> iList = new List<Models.ServiceTypeObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false).ToList().TrimList();
                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.ServiceType>(i);
                    foreach (var j in i)
                    {
                        var k = new Models.ServiceTypeObject();
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.TypeId = j.TypeId;
                        k.IsDeleted = j.IsDeleted;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.SortOrder = j.SortOrder;
                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceTab> GetServiceTabs(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceTab> iList = new List<Models.ServiceTab>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.ServiceTab>(i);
                    foreach (var j in i)
                    {
                        var k = new Models.ServiceTab();
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.TabId = j.TabId;
                        k.IsDeleted = j.IsDeleted;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.SortOrder = j.SortOrder;
                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceObject> GetServices(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceObject> iList = new List<Models.ServiceObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.Services.Where(x => x.IsDeleted == false && x.ParentServiceId == 0).ToList().TrimList();
                //
                if (iSearch.DepartmentId > 0 )
                    i = i.Where(x => x.DepartmentId == iSearch.DepartmentId).ToList().TrimList();
                //
                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.Service>(i);

                    if (string.IsNullOrEmpty(iSearch.SearchCri) == false)
                    {
                        iSearch.SearchCri = iSearch.SearchCri.ToUpper();
                        i = i.Where(c => Common.Helper.IsNullCompare(c.DescriptionAlt).Contains(iSearch.SearchCri)
                        || Common.Helper.IsNullCompare(c.DescriptionEng).Contains(iSearch.SearchCri)
                        || Common.Helper.IsNullCompare(c.ExternalServiceID).Contains(iSearch.SearchCri)
                        ).ToList();
                    }
                    if (iSearch.EntityId.HasValue)
                    {
                        ////i = i.Where(c => c.EntityId == iSearch.EntityId).ToList();

                        var a = iDbContext.ServiceEntitiesMappings.Where(x => x.IsDeleted == false && x.EntityId == iSearch.EntityId).ToList();
                        if (a != null)
                        {
                            var ii = new List<Service>();
                            foreach (var v in a)
                            {
                                ii.AddRange(i.Where(c => c.ServiceId == v.ServiceId).ToList());
                            }
                            i = ii;
                        }
                    }
                    if (iSearch.TypeId.HasValue)
                    {
                        i = i.Where(c => c.TypeId == iSearch.TypeId).ToList();
                    }
                    if (iSearch.IsActive.HasValue && iSearch.IsActive.Value == true)
                    {
                        i = i.Where(c => c.IsActive == true).ToList();
                    }
                    if (iSearch.IsActive.HasValue && iSearch.IsActive.Value == false)
                    {
                        i = i.Where(c => c.IsActive == false).ToList();
                    }

                    foreach (var j in i)
                    {
                        var k = new Models.ServiceObject();
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.EntityId = j.EntityId;
                        //if (j.EntityId.HasValue)
                        //{
                        //    k.EntityNameAlt = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionAlt;
                        //    k.EntityNameEng = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionEng;
                        //}
                        k.ExternalServiceID = j.ExternalServiceID;
                        k.IsDeleted = j.IsDeleted;
                        k.IsActive = j.IsActive;
                        k.UseParentExternalServiceId = j.UseParentExternalServiceId;
                        k.ParentExternalServiceID = j.ParentExternalServiceID;
                        k.ParentServiceId = j.ParentServiceId;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.ServiceId = j.ServiceId;
                        k.ServiceUrl = j.ServiceUrl;
                        k.SortOrder = j.SortOrder;
                        k.TypeId = j.TypeId;
                        k.ApiSourceId = j.ApiSourceId;
                        k.DepartmentId = j.DepartmentId;
                        if (j.TypeId.HasValue)
                        {
                            k.TypeNameAlt = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionAlt;
                            k.TypeNameEng = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionEng;
                        }

                        var a = iDbContext.ServiceEntitiesMappings.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).ToList();
                        if (a != null)
                        {

                            k.Mapping = a;

                            string entityNameEn = "";
                            string entityNameAr = "";

                            foreach (var v in a)
                            {
                                if (v.EntityId.HasValue)
                                {
                                    entityNameEn += iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == v.EntityId).FirstOrDefault().DescriptionEng + "|";
                                    entityNameAr += iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == v.EntityId).FirstOrDefault().DescriptionAlt + "|";
                                }
                            }
                            if (entityNameAr.EndsWith("|"))
                                entityNameAr = entityNameAr.Remove(entityNameAr.Length - 1, 1);
                            if (entityNameEn.EndsWith("|"))
                                entityNameEn = entityNameEn.Remove(entityNameEn.Length - 1, 1);
                            k.EntityNameAlt = entityNameAr;
                            k.EntityNameEng = entityNameEn;
                        }

                        if (j.IsAnonymous.HasValue)
                            k.IsAnonymous = j.IsAnonymous;
                        else
                            k.IsAnonymous = false;

                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceObject> GetSubServices(int parentServiceId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceObject> iList = new List<Models.ServiceObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.Services.Where(x => x.IsDeleted == false && x.ParentServiceId == parentServiceId).ToList().TrimList();

                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.Service>(i);

                    foreach (var j in i)
                    {
                        var k = new Models.ServiceObject();
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.EntityId = j.EntityId;
                        if (j.EntityId.HasValue)
                        {
                            k.EntityNameAlt = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionAlt;
                            k.EntityNameEng = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionEng;
                        }
                        k.ExternalServiceID = j.ExternalServiceID;
                        k.IsDeleted = j.IsDeleted;
                        k.IsActive = j.IsActive;
                        k.UseParentExternalServiceId = j.UseParentExternalServiceId;
                        k.ParentExternalServiceID = j.ParentExternalServiceID;
                        k.soWidgetCode = j.soWidgetCode;
                        k.ParentServiceId = j.ParentServiceId;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.ServiceId = j.ServiceId;
                        k.ServiceUrl = j.ServiceUrl;
                        k.SortOrder = j.SortOrder;
                        k.TypeId = j.TypeId;
                        k.ApiSourceId = j.ApiSourceId;
                        k.DepartmentId = j.DepartmentId;
                        if (j.PrintPreview.HasValue)
                            k.PrintPreview = j.PrintPreview;
                        else
                            k.PrintPreview = false;
                        k.PrintMessage = j.PrintMessage;
                        k.PrintMessageAr = j.PrintMessageAr;
                        if (j.TypeId.HasValue)
                        {
                            k.TypeNameAlt = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionAlt;
                            k.TypeNameEng = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionEng;
                        }
                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceObject> GetParentAndChildServices(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceObject> iList = new List<Models.ServiceObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.Services.Where(x => x.IsDeleted == false && (x.ServiceId == serviceId || x.ParentServiceId == serviceId)).ToList().TrimList();

                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.Service>(i);

                    foreach (var j in i)
                    {
                        var k = new Models.ServiceObject();
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.EntityId = j.EntityId;
                        if (j.EntityId.HasValue)
                        {
                            k.EntityNameAlt = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionAlt;
                            k.EntityNameEng = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionEng;
                        }
                        k.ExternalServiceID = j.ExternalServiceID;
                        k.IsDeleted = j.IsDeleted;
                        k.IsActive = j.IsActive;
                        k.UseParentExternalServiceId = j.UseParentExternalServiceId;
                        k.ParentExternalServiceID = j.ParentExternalServiceID;
                        k.ParentServiceId = j.ParentServiceId;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.ServiceId = j.ServiceId;
                        k.ServiceUrl = j.ServiceUrl;
                        k.SortOrder = j.SortOrder;
                        k.TypeId = j.TypeId;
                        k.ApiSourceId = j.ApiSourceId;
                        k.DepartmentId = j.DepartmentId;
                        if (j.TypeId.HasValue)
                        {
                            k.TypeNameAlt = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionAlt;
                            k.TypeNameEng = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionEng;
                        }
                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceCategoryObject> GetServiceClassifications(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceCategoryObject> iList = new List<Models.ServiceCategoryObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var pc = GetParentAndChildServices(serviceId);
                if (pc != null)
                {
                    foreach (var c in pc)
                    {
                        var i = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == c.ServiceId).ToList().TrimList();
                        if (i != null && i.Count > 0)
                        {
                            foreach (var j in i)
                            {
                                var k = new Models.ServiceCategoryObject();
                                k.DescriptionAlt = j.DescriptionAlt;
                                k.DescriptionEng = j.DescriptionEng;
                                k.CategoryId = j.CategoryId;
                                k.ParentCategoryId = j.ParentCategoryId;
                                k.ServiceId = j.ServiceId;
                                k.IsDeleted = j.IsDeleted;
                                k.IsActive = j.IsActive;
                                k.RowInsertDate = j.RowInsertDate;
                                k.RowUpdateDate = j.RowUpdateDate;
                                k.SortOrder = j.SortOrder;
                                k.CategoryCode = j.CategoryCode;
                                if (j.PrintPreview.HasValue)
                                    k.PrintPreview = j.PrintPreview;
                                else
                                    k.PrintPreview = false;
                                k.PrintMessage = j.PrintMessage;
                                k.PrintMessageAr = j.PrintMessageAr;
                                if (j.TabularLayout.HasValue)
                                    k.TabularLayout = j.TabularLayout;
                                else
                                    k.TabularLayout = false;
                                k.ExternalServiceID = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault().ExternalServiceID;
                                iList.Add(k);
                            }
                        }
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceFormsConfigObject> GetServiceFormsConfig(int serviceId, int categoryId, int inputTypeId,int tabId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceFormsConfigObject> iList = new List<Models.ServiceFormsConfigObject>();
            List<ServiceFormsConfig> i = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                var pc = GetParentAndChildServices(serviceId);
                if (pc != null)
                {
                    foreach (var c in pc)
                    {
                        if (categoryId > 0)
                            i = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == c.ServiceId && x.CategorId == categoryId).OrderBy(x => x.SortOrder).ToList().TrimList();
                        else
                            i = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == c.ServiceId).OrderBy(x => x.SortOrder).ToList().TrimList();

                        if (inputTypeId > 0)
                            i = i.Where(x => x.InputTypeId == inputTypeId).ToList();

                        if (tabId > 0)
                            i = i.Where(x => x.TabId == tabId).ToList();

                        if (i != null && i.Count > 0)
                        {
                            foreach (var j in i)
                            {
                                var k = new Models.ServiceFormsConfigObject();
                                k.Attributes = j.Attributes;
                                k.AttributesAr = j.AttributesAr;
                                k.CategorId = j.CategorId;
                                k.ExternalLookupId = j.ExternalLookupId;
                                k.ID = j.ID;
                                k.InputId = j.InputId;
                                k.InputTypeId = j.InputTypeId;
                                k.IsDeleted = j.IsDeleted;
                                k.IsExternalLookup = j.IsExternalLookup;
                                k.LabelAr = j.LabelAr;
                                k.Label = j.Label;
                                k.LanguageId = j.LanguageId;
                                k.MaxFileSize = j.MaxFileSize;
                                k.Maximum = j.Maximum;
                                k.Message = j.Message;
                                k.MessageAr = j.MessageAr;
                                k.Minimum = j.Minimum;
                                k.Name = j.Name;
                                k.OptionId = j.OptionId;
                                k.Placeholder = j.Placeholder;
                                k.PlaceholderAr = j.PlaceholderAr;
                                k.Required = j.Required;
                                k.RowInsertDate = j.RowInsertDate;
                                k.RowUpdateDate = j.RowUpdateDate;
                                k.ServiceId = j.ServiceId;
                                k.SortOrder = j.SortOrder;
                                k.UserField = j.UserField;
                                k.IsActive = j.IsActive;
                                k.ArabicInput = j.ArabicInput;
                                k.EnglishInput = j.EnglishInput;
                                k.DynamicInput = j.DynamicInput;
                                k.ValidationMessage = j.ValidationMessage;
                                k.ValidationMessageAr = j.ValidationMessageAr;
                                k.HelpMessage = j.HelpMessage;
                                k.HelpMessageAr = j.HelpMessageAr;
                                k.DownloadAttachment = j.DownloadAttachment;
                                k.TabId = j.TabId;
                                k.IsReadOnly = j.IsReadOnly;
                                k.FilterId = j.FilterId;
                                k.FilterValue = j.FilterValue;
                                k.JsonAttribute = j.JsonAttribute;
                                k.Bookmark = j.Bookmark;
                                k.ApplyWordCount = j.ApplyWordCount;
                                k.SpeechToText = j.SpeechToText;

                                k.ExternalServiceID = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault().ExternalServiceID;
                                var CategoryName = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.CategoryId == j.CategorId).FirstOrDefault();
                                if (CategoryName != null)
                                    k.CategoryName = CategoryName.DescriptionEng;
                                k.InputTypeName = iDbContext.LkInputTypes.Where(x => x.IsDeleted == false && x.TypeId == j.InputTypeId).FirstOrDefault().TypeName;

                                var psid = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault();
                                if (psid != null)
                                {
                                    if (psid.ParentServiceId == 0)
                                        k.ParentServiceId = serviceId;
                                    else
                                        k.ParentServiceId = psid.ParentServiceId;
                                }

                                iList.Add(k);
                            }
                        }
                    }
                }

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LkLookup> GetLkLookup()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LkLookup> iList = new List<Models.LkLookup>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.LkLookups.Where(x => x.IsDeleted == false).ToList();

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceGuideHint> GetServiceGuideHints(int typeId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceGuideHint> iList = new List<Models.ServiceGuideHint>();
            try
            {
                iDbContext = new MOFPortalEntities();

                if (typeId > 0)
                    iList = iDbContext.ServiceGuideHints.Where(x => x.IsDeleted == false && x.TypeId == typeId).ToList();
                else
                    iList = iDbContext.ServiceGuideHints.Where(x => x.IsDeleted == false).ToList();

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LkRequestStatu> GetLkRequestStatus()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LkRequestStatu> iList = new List<Models.LkRequestStatu>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.LkRequestStatus.Where(x => x.IsDeleted == false).ToList();

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LoggerObject> GetLogger(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LoggerObject> iList = new List<Models.LoggerObject>();

            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ES_GetLogger(pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.FromDate, iSearch.ToDate);
                if (i != null)
                {
                    foreach (var j in i)
                    {
                        var k = new Models.LoggerObject();
                        k.VersionNo = j.VersionNo;
                        k.Message = j.Message;
                        k.Source = j.Source;
                        k.RowInsertDate = j.RowInsertDate;
                        k.TotalRows = j.TotalRows.Value;
                        iList.Add(k);
                    }
                }
                //iList = iList.OrderByDescending(x => x.RowInsertDate).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.UserScreenActionsObject> GetUserScreenActions(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.UserScreenActionsObject> iList = new List<Models.UserScreenActionsObject>();

            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ES_GetUserScreenActions(pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.FromDate, iSearch.ToDate, iSearch.UserName, iSearch.DepartmentId, iSearch.TypeId);
                if (i != null)
                {
                    foreach (var j in i)
                    {
                        var k = new Models.UserScreenActionsObject();
                        k.ActionName = j.ActionName;
                        k.UniqueId = j.UniqueId;
                        k.Remarks = j.Remarks;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowInsertedBy = j.RowInsertedBy;
                        k.TotalRows = j.TotalRows.Value;
                        iList.Add(k);
                    }
                }
                //iList = iList.OrderByDescending(x => x.RowInsertDate).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceRequestsObject> GetServiceRequests(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceRequestsObject> iList = new List<Models.ServiceRequestsObject>();

            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ES_GeRequestsAndEnquiriesList(iSearch.StatusId, pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.FromDate, iSearch.ToDate, iSearch.DepartmentId);
                if (i != null)
                {
                    foreach (var j in i)
                    {
                        var k = new Models.ServiceRequestsObject();
                        k.IpAddress = j.IpAddress;
                        k.PreviousRequestId = j.PreviousRequestId;
                        k.Remarks = j.Remarks;
                        k.RequestId = j.RequestId;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowInsertedBy = j.RowInsertedBy;
                        k.Service = j.ExternalServiceID;
                        k.StatusAlt = j.StatusAlt;
                        k.StatusEng = j.StatusEng;
                        k.UserId = j.UserId;
                        k.TotalRows = j.TotalRows.Value;
                        iList.Add(k);
                    }
                }
                //iList = iList.OrderByDescending(x => x.RowInsertDate).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.ServiceObject GetService(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            Models.ServiceObject k = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                var j = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).FirstOrDefault();
                if (j != null)
                {
                    k = new Models.ServiceObject();
                    Common.Utility.TrimObject<Models.Service>(j);
                    {
                        k.DescriptionAlt = j.DescriptionAlt;
                        k.DescriptionEng = j.DescriptionEng;
                        k.EntityId = j.EntityId;
                        if (j.EntityId.HasValue)
                        {
                            k.EntityNameAlt = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionAlt;
                            k.EntityNameEng = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == j.EntityId).FirstOrDefault().DescriptionEng;
                        }
                        k.ExternalServiceID = j.ExternalServiceID;
                        k.IsDeleted = j.IsDeleted;
                        k.IsActive = j.IsActive;
                        if (j.PrintPreview.HasValue)
                            k.PrintPreview = j.PrintPreview;
                        else
                            k.PrintPreview = false;

                        k.PrintMessage = j.PrintMessage;
                        k.PrintMessageAr = j.PrintMessageAr;

                        if (j.TabularLayout.HasValue)
                            k.TabularLayout = j.TabularLayout;
                        else
                            k.TabularLayout = false;

                        if (j.IsAnonymous.HasValue)
                            k.IsAnonymous = j.IsAnonymous;
                        else
                            k.IsAnonymous = false;

                        k.soWidgetCode = j.soWidgetCode;
                        k.UseParentExternalServiceId = j.UseParentExternalServiceId;
                        k.ParentExternalServiceID = j.ParentExternalServiceID;
                        k.ParentServiceId = j.ParentServiceId;
                        k.RowInsertDate = j.RowInsertDate;
                        k.RowUpdateDate = j.RowUpdateDate;
                        k.ServiceId = j.ServiceId;
                        k.ServiceUrl = j.ServiceUrl;
                        k.SortOrder = j.SortOrder;
                        k.TypeId = j.TypeId;
                        k.ApiSourceId = j.ApiSourceId;
                        k.DepartmentId = j.DepartmentId;
                        if (j.TypeId.HasValue)
                        {
                            k.TypeNameAlt = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionAlt;
                            k.TypeNameEng = iDbContext.ServiceTypes.Where(x => x.IsDeleted == false && x.TypeId == j.TypeId).FirstOrDefault().DescriptionEng;
                        }
                        var a = iDbContext.ServiceEntitiesMappings.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                        if (a != null)
                        {
                            
                            k.Mapping = a;

                            string entityNameEn = "";
                            string entityNameAr = "";

                            foreach (var v in a)
                            {
                                if (v.EntityId.HasValue)
                                {
                                    entityNameEn += iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == v.EntityId).FirstOrDefault().DescriptionEng + "|";
                                    entityNameAr += iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId == v.EntityId).FirstOrDefault().DescriptionAlt + "|";
                                }
                            }
                            if (entityNameAr.EndsWith("|"))
                                entityNameAr = entityNameAr.Remove(entityNameAr.Length - 1, 1);
                            if (entityNameEn.EndsWith("|"))
                                entityNameEn = entityNameEn.Remove(entityNameEn.Length - 1, 1);
                            k.EntityNameAlt = entityNameAr;
                            k.EntityNameEng = entityNameEn;
                        }

                        var b = iDbContext.ServiceUserTypes.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                        if (b != null)
                        {
                            k.UserTypes = b;
                            //int userTypeLookupId = EServicesCms.Common.WebConfig.GetIntValue("Lk_UserType_LookupId");
                            //foreach (var v in b)
                            //{
                            //    string userTypeEn = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == userTypeLookupId && x.Code == v.UserTypeId.ToString()).FirstOrDefault().DescriptionEng;
                            //    string userTypeAr = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == userTypeLookupId && x.Code == v.UserTypeId.ToString()).FirstOrDefault().DescriptionAlt;
                            //    k.UserTypes.Add(new ServiceUserTypeObject() { UserTypeId = v.UserTypeId.Value, DescriptionAlt = userTypeAr, DescriptionEng = userTypeEn });
                            //}
                        }
                    }
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return k;
        }
        public static Models.ServiceCategoryObject GetServiceClassification(int serviceId, int categoryId)
        {
            MOFPortalEntities iDbContext = null;
            Models.ServiceCategoryObject iList = new Models.ServiceCategoryObject();
            try
            {
                iDbContext = new MOFPortalEntities();

                var j = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == serviceId && x.CategoryId == categoryId).FirstOrDefault().TrimObject();
                if (j != null)
                {
                    var k = new Models.ServiceCategoryObject();
                    k.DescriptionAlt = j.DescriptionAlt;
                    k.DescriptionEng = j.DescriptionEng;
                    k.CategoryId = j.CategoryId;
                    k.ParentCategoryId = j.ParentCategoryId;
                    k.ServiceId = j.ServiceId;
                    k.IsDeleted = j.IsDeleted;
                    k.RowInsertDate = j.RowInsertDate;
                    k.RowUpdateDate = j.RowUpdateDate;
                    k.SortOrder = j.SortOrder;
                    k.IsActive = j.IsActive;
                    if (j.PrintPreview.HasValue)
                        k.PrintPreview = j.PrintPreview;
                    else
                        k.PrintPreview = false;
                    k.CategoryCode = j.CategoryCode;
                    k.PrintMessage = j.PrintMessage;
                    k.PrintMessageAr = j.PrintMessageAr;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.ServiceFormsConfigObject GetServiceInput(int serviceId, int inputId)
        {
            MOFPortalEntities iDbContext = null;
            Models.ServiceFormsConfigObject k = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                var j = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.InputId == inputId).FirstOrDefault().TrimObject();
                if (j != null)
                {
                    k = new Models.ServiceFormsConfigObject();
                    k.Attributes = j.Attributes;
                    k.AttributesAr = j.AttributesAr;
                    k.CategorId = j.CategorId;
                    k.ExternalLookupId = j.ExternalLookupId;
                    k.ID = j.ID;
                    k.InputId = j.InputId;
                    k.InputTypeId = j.InputTypeId;
                    k.IsDeleted = j.IsDeleted;
                    k.IsExternalLookup = j.IsExternalLookup;
                    k.LabelAr = j.LabelAr;
                    k.Label = j.Label;
                    k.LanguageId = j.LanguageId;
                    k.MaxFileSize = j.MaxFileSize;
                    k.Maximum = j.Maximum;
                    k.Message = j.Message;
                    k.MessageAr = j.MessageAr;
                    k.Minimum = j.Minimum;
                    k.Name = j.Name;
                    k.OptionId = j.OptionId;
                    k.Placeholder = j.Placeholder;
                    k.PlaceholderAr = j.PlaceholderAr;
                    k.Required = j.Required;
                    k.RowInsertDate = j.RowInsertDate;
                    k.RowUpdateDate = j.RowUpdateDate;
                    k.SortOrder = j.SortOrder;
                    k.UserField = j.UserField;
                    k.IsActive = j.IsActive;
                    k.ArabicInput = j.ArabicInput;
                    k.EnglishInput = j.EnglishInput;
                    k.DynamicInput = j.DynamicInput;
                    k.ServiceId = j.ServiceId;
                    k.ValidationMessage = j.ValidationMessage;
                    k.ValidationMessageAr = j.ValidationMessageAr;
                    k.HelpMessage = j.HelpMessage;
                    k.HelpMessageAr = j.HelpMessageAr;
                    k.DownloadAttachment = j.DownloadAttachment;
                    k.TabId = j.TabId;
                    k.IsReadOnly = j.IsReadOnly;
                    k.FilterId = j.FilterId;
                    k.FilterValue = j.FilterValue;
                    k.JsonAttribute = j.JsonAttribute;
                    k.Bookmark = j.Bookmark;
                    k.ApplyWordCount = j.ApplyWordCount;
                    k.SpeechToText = j.SpeechToText;
                    k.ServiceName = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault().DescriptionEng;
                    k.ServiceNameAlt = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault().DescriptionAlt;

                    var iServiceDetails = iDbContext.Services.Where(x => x.IsDeleted == false && x.ServiceId == j.ServiceId).FirstOrDefault();
                    if (iServiceDetails != null)
                    {
                        k.ParentServiceId = iServiceDetails.ParentServiceId;
                        k.ServiceName = iServiceDetails.DescriptionEng;
                        k.ServiceNameAlt = iServiceDetails.DescriptionAlt;
                    }
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return k;
        }
        public static List<Models.LkUserAttributesSSO> GetLkUserAttributesSSO()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LkUserAttributesSSO> iList = new List<Models.LkUserAttributesSSO>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.LkUserAttributesSSOes.Where(x => x.IsDeleted == false).ToList();

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.Department> GetDepartments()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.Department> iList = new List<Models.Department>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.Departments.Where(x => x.IsDeleted == false).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LkGroup> GetLkGroups()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LkGroup> iList = new List<Models.LkGroup>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.LkGroups.Where(x => x.IsDeleted == false).ToList();

                iList = iList.OrderBy(x => x.ScreenId).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LkRole> GetLkRoles()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LkRole> iList = new List<Models.LkRole>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.LkRoles.Where(x => x.IsDeleted == false).ToList();
                
                iList = iList.OrderBy(x => x.Description).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.LkRoleObject GetLkRoleObject(int roleId)
        {
            MOFPortalEntities iDbContext = null;
            Models.LkRoleObject i = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                var iRole = iDbContext.LkRoles.Where(x => x.IsDeleted == false && x.RoleId == roleId).FirstOrDefault();
                if (iRole != null)
                {
                    //var i = new LkRoleObject();
                    i = new Models.LkRoleObject();
                    i.RoleId = iRole.RoleId;
                    i.Description = iRole.Description;

                    var roleGroups = iDbContext.RoleGroups.Where(x => x.IsDeleted == false && x.RoleId == iRole.RoleId).ToList();
                    if (roleGroups != null && roleGroups.Count > 0)
                    {
                        i.RoleGroups = new List<LkGroup>();

                        foreach (var iGroup in roleGroups)
                        {
                            var j = new LkGroup();
                            j.GroupId = iGroup.GroupId.Value;
                            var gd = iDbContext.LkGroups.Where(x => x.IsDeleted == false && x.GroupId == j.GroupId).FirstOrDefault();
                            if (gd != null)
                                j.GroupName = gd.GroupName;
                            i.RoleGroups.Add(j);
                        }
                    }
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return i;
        }
        public static List<Models.UserObject> GetUsers(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.UserObject> iList = new List<Models.UserObject>();

            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ES_GetUsers(pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.RoleId, iSearch.DepartmentId);
                if (i != null)
                {
                    foreach (var j in i)
                    {
                        var k = new Models.UserObject();
                        k.DepartmentId = j.DepartmentId;
                        k.DepartmentName = j.DepartmentName;
                        k.Email = j.Email;
                        k.FullName = j.FullName;
                        k.FullNameAlt = j.FullNameAlt;
                        k.IsActive = j.IsActive;
                        k.IsFirstLogin = j.IsFirstLogin;
                        k.Mobile = j.Mobile;
                        k.Password = j.Password;
                        k.RoleId = j.RoleId;
                        k.RoleName = j.RoleName;
                        k.SecurityAnswer = j.SecurityAnswer;
                        k.SecurityQuestionId = j.SecurityQuestionId;
                        k.UserId = j.UserId;
                        k.UserName = j.UserName;
                        k.TotalRows = j.TotalRows.Value;
                        iList.Add(k);
                    }
                }
                iList = iList.OrderByDescending(x => x.RowInsertDate).ToList();
            }
            catch (Exception E)
            {
                HttpContext.Current.Trace.Warn("GetUsers", E.ToString());
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.UserObject GetUserObject(int userId)
        {
            MOFPortalEntities iDbContext = null;
            Models.UserObject iList = new Models.UserObject();
            try
            {
                iDbContext = new MOFPortalEntities();

                var iUser = iDbContext.Users.Where(x => x.IsDeleted == false && x.UserId == userId).FirstOrDefault();
                if (iUser != null)
                {
                    var i = new UserObject();
                    i.DepartmentId = iUser.DepartmentId;
                    i.Email = iUser.Email;
                    i.FullName = iUser.FullName;
                    i.FullNameAlt = iUser.FullNameAlt;
                    i.IsActive = iUser.IsActive;
                    i.IsFirstLogin = iUser.IsFirstLogin;
                    i.Mobile = iUser.Mobile;
                    i.Password = iUser.Password;
                    i.RoleId = iUser.RoleId;
                    i.SecurityAnswer = iUser.SecurityAnswer;
                    i.SecurityQuestionId = iUser.SecurityQuestionId;
                    i.UserId = iUser.UserId;
                    i.UserName = iUser.UserName;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LookupOption> GetLkLookupIdList(int lookupId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LookupOption> iList = new List<Models.LookupOption>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == lookupId).ToList();

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.LookupOption> GetLkLookupIdList(string lookupId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.LookupOption> iList = new List<Models.LookupOption>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var code = iDbContext.LkLookups.Where(x => x.LookUpName.ToUpper() == lookupId.ToUpper()).FirstOrDefault().LookupId;

                iList = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == code).ToList();

                iList = iList.OrderBy(x => x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceFormsDrilldownObject> GetInputDrilldownOLD(int inputId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceFormsDrilldownObject> iList = new List<Models.ServiceFormsDrilldownObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputId == inputId).ToList().TrimList();
                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.ServiceFormsDrilldownConfig>(i);
                    foreach (var j in i)
                    {
                        var k = new Models.ServiceFormsDrilldownObject();
                        k.AlertMessage = j.AlertMessage;
                        k.AlertMessageAr = j.AlertMessageAr;
                        k.DrilldownId = j.DrilldownId;
                        k.InputId = j.InputId.Value;
                        k.InputControlId = j.InputControlId;
                        k.LogicalOperator = j.LogicalOperator;
                        k.LogicalOperator = j.LogicalOperator;
                        k.ReferralId = j.ReferralId;
                        k.ReferralIdValue = j.ReferralIdValue;
                        k.ReferralName = iDbContext.ServiceFormsConfigs.Where(x => x.ID == j.ReferralId).FirstOrDefault().Label;
                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.DrilldownId).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ServiceFormsDrilldownObject> GetInputDrilldown(string InputControlId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceFormsDrilldownObject> iList = new List<Models.ServiceFormsDrilldownObject>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var i = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputControlId == InputControlId).ToList().TrimList();
                if (i != null && i.Count > 0)
                {
                    Common.Utility.TrimList<Models.ServiceFormsDrilldownConfig>(i);
                    foreach (var j in i)
                    {
                        var k = new Models.ServiceFormsDrilldownObject();
                        k.AlertMessage = j.AlertMessage;
                        k.AlertMessageAr = j.AlertMessageAr;
                        k.DrilldownId = j.DrilldownId;
                        k.InputId = j.InputId.Value;
                        k.InputControlId = j.InputControlId;
                        k.LogicalOperator = j.LogicalOperator;
                        k.LogicalOperator = j.LogicalOperator;
                        k.ReferralId = j.ReferralId;
                        k.ReferralIdValue = j.ReferralIdValue;
                        k.ReferralName = iDbContext.ServiceFormsConfigs.Where(x => x.ID == j.ReferralId).FirstOrDefault().Label;
                        iList.Add(k);
                    }
                }

                iList = iList.OrderBy(x => x.DrilldownId).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.API_Sources> GetApiSources()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.API_Sources> iList = new List<Models.API_Sources>();
            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.API_Sources.Where(x => x.IsDeleted == false).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.LkLkLoopUpActionObject GetInputLookupActionObject(int inputId, int lookupId)
        {
            MOFPortalEntities iDbContext = null;
            Models.LkLkLoopUpActionObject i = null;
            try
            {
                iDbContext = new MOFPortalEntities();

                var iRecord = iDbContext.ServiceInputLookupActions.Where(x => x.IsDeleted == false && x.InputId == inputId && x.LookupId == lookupId).FirstOrDefault();
                if (iRecord != null)
                {
                    i = new Models.LkLkLoopUpActionObject();
                    i.ActionId = iRecord.ActionId.Value;
                    //i.DescriptionAlt = iRecord.DescriptionAlt;
                    //i.DescriptionEng = iRecord.DescriptionEng;
                    i.InputId = iRecord.InputId;
                    i.LookupId = iRecord.LookupId;
                    //i.LookupValues = iRecord.LookupValue;

                    i.LookupValues = new List<LookupOption>();

                    var iList = iDbContext.ServiceInputLookupActions.Where(x => x.IsDeleted == false && x.InputId == inputId && x.LookupId == lookupId).ToList();

                    foreach (var iCode in iList)// i.LookupValue.Split(','))
                    {
                        var j = new LookupOption();
                        j.LookupId = i.LookupId;
                        j.Code = iCode.LookupValue;
                        var gd = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == j.LookupId && x.Code == iCode.LookupValue).FirstOrDefault();
                        if (gd != null)
                        {
                            j.DescriptionEng = gd.DescriptionEng;
                            j.DescriptionAlt = gd.DescriptionAlt;
                        }
                        i.LookupValues.Add(j);
                    }
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return i;
        }
        public static string GenerateLookUpValues(int lookupId, List<Models.ServiceInputLookupAction> lSelectedLookupOption, string externalServieId, string LookUpName, string filterValue, bool? IsExternalLookup)
        {
            string retHtml = "<div class='blocks row page_links'>";
            List<Models.LookupOption> lAllGroups = null;
            bool isRG = false;
            string screenName = "";
            string remarks = "";
            string remarksAr = "";
            if (filterValue == "0")
                filterValue = "";
            string url = "";

            var iDbx = new EServicesCms.Models.MOFPortalEntities();

            try
            {
                //if (WebConfig.GetBoolValue("IsExternalLookupList") == true)
                if (IsExternalLookup == true)
                {
                    if (lookupId == 99)
                    {
                        lAllGroups = iDbx.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == lookupId).OrderBy(x => x.SortOrder).ToList();
                    }
                    else
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    (se, cert, chain, sslerror) =>
                    {
                        return true;
                    };
                        //
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        using (HttpClient client = new HttpClient())
                        {
                            if (string.IsNullOrEmpty(filterValue))
                                url = WebConfig.GetStringValue("MOF_ApiAccessURL") + "/api/PublicHelpDesk/GetLookup?externalServiceId=" + externalServieId + "&lookupId=" + LookUpName;
                            else
                                url = WebConfig.GetStringValue("MOF_ApiAccessURL") + "/api/PublicHelpDesk/GetLookup?externalServiceId=" + externalServieId + "&lookupId=" + LookUpName + "&filterValue=" + filterValue;

                            client.DefaultRequestHeaders.Add("x-api-key", WebConfig.GetStringValue("MOF_ApiKey"));

                            HttpContext.Current.Trace.Write(WebConfig.GetStringValue("MOF_ApiKey"));
                            HttpContext.Current.Trace.Write(url);

                            var response = client.GetAsync(url).Result;

                            HttpContext.Current.Trace.Write("Invoking . API StatusCode = " + response.StatusCode);
                            //HttpContext.Current.Trace.Write("Invoking . API Content = " + response.Content);
                            HttpContext.Current.Trace.Write(response.Content.ReadAsStringAsync().Result);

                            try
                            {
                                var iList = Newtonsoft.Json.JsonConvert.DeserializeObject<GetLookupListResponse>(response.Content.ReadAsStringAsync().Result);
                                if (iList != null)
                                {
                                    lAllGroups = new List<LookupOption>();
                                    foreach (var idata in iList.Data)
                                    {
                                        var ioption = new LookupOption();
                                        ioption.Code = idata.Id.ToString();
                                        ioption.DescriptionAlt = idata.NameAr;
                                        ioption.DescriptionEng = idata.NameEn;
                                        ioption.LookupId = lookupId;
                                        //ioption.OptionId = idata.Id;
                                        lAllGroups.Add(ioption);
                                    }
                                }
                            }
                            catch
                            {
                                retHtml = "<B>" + response.Content.ReadAsStringAsync().Result + "</B>";
                                return retHtml;
                            }
                        }
                    }
                }
                else
                    lAllGroups = iDbx.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == lookupId).OrderBy(x => x.SortOrder).ToList();

                List<int> screenIds = lAllGroups.Select(c => c.LookupId).Distinct().ToList();

                if (lAllGroups != null && lAllGroups.Count > 0)
                {
                    foreach (Int16? screenId in screenIds)
                    {
                        screenName = iDbx.LkLookups.Where(x => x.IsDeleted == false && x.LookupId == screenId).FirstOrDefault().LookUpName;
                        //
                        retHtml += @"<div class='col-12 col-md-12' style='overflow-y:scroll; height:400px; overflow-x: hidden;overflow-y: visible;border:1px solid #b68a35;margin:2px 2px 2px 2px;'>
                                             <div class='tag'>                                                    
								                    <h4 class='title nameEn'>%screenName%</h4>
							                    </div>";
                        retHtml = retHtml.Replace("%screenName%", screenName);
                        retHtml = retHtml.Replace("%screenId%", screenId.ToString());
                        //
                        //retHtml += "<div class='row'>";
                        foreach (Models.LookupOption iAll in lAllGroups)
                        {
                            isRG = false;
                            remarks = "";
                            remarksAr = "";
                            //
                            if (lSelectedLookupOption != null && lSelectedLookupOption.Count > 0)
                            {
                                Models.ServiceInputLookupAction iRG = lSelectedLookupOption.Find(delegate (Models.ServiceInputLookupAction i) { return i.LookupValue == iAll.Code; });
                                if (iRG != null)
                                {
                                    isRG = true;
                                    remarks = iRG.DescriptionEng;
                                    remarksAr = iRG.DescriptionAlt;
                                }
                            }
                            //
                            retHtml += "<div class='row'>";
                            retHtml += "    <div class='col-xs-6'>";
                            retHtml += "        <label>";
                            if (isRG == true)
                                retHtml += "            <input name='switch-field-1" + screenId + "' class='ace ace-switch ace-switch-3' id='" + iAll.Code + "' type='checkbox' value='" + iAll.Code + "' checked />";
                            else
                                retHtml += "            <input name='switch-field-1" + screenId + "' class='ace ace-switch ace-switch-3' id='" + iAll.Code + "' type='checkbox' value='" + iAll.Code + "' />";

                            retHtml += "            <span class='lbl' title='" + iAll.Code + " - " + iAll.DescriptionEng + "'>&nbsp;&nbsp;&nbsp;<span class='checkboxText'>" + iAll.Code + " - " + iAll.DescriptionEng + "</span></span>";

                            retHtml += "        </label>";
                            retHtml += "    </div>";

                            retHtml += "    <div class='col-xs-6'>";
                            retHtml += "        <label>";
                            retHtml += "            <input type='text' placeholder='Enter Action Remarks Arabic' class='form-control' id='DescriptionAlt" + iAll.Code + "' type='checkbox' value='" + remarksAr + "' />";
                            retHtml += "            <input type='text' placeholder='Enter Action Remarks' class='form-control' id='DescriptionEng" + iAll.Code + "' type='checkbox' value='" + remarks + "' />";
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
        public static List<Models.ServiceViewer> GetServiceViewer(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ServiceViewer> iList = new List<Models.ServiceViewer>();

            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.ServiceViewers.Where(x => x.ServiceId == serviceId && x.IsDeleted == false).ToList();

                iList = iList.OrderBy(x => x.EmailAddress).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static Models.ImportExport GetServiceObjectForImportExport(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            Models.ImportExport iObject = new ImportExport();
            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;

                var Service = iDbContext.Services.Where(x => x.ServiceId == serviceId).FirstOrDefault();
                iObject.Service = AutoMapper.ObjectToObjectMapper<Models.Service, Models.IEService>(Service, null);

                var ServiceTabs = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (ServiceTabs != null && ServiceTabs.Count > 0)
                    iObject.ServiceTabs = AutoMapper.ListObjectToListObjectMapper<Models.ServiceTab, Models.IEServiceTab>(ServiceTabs);

                //var SubServices = iDbContext.Services.Where(x => x.IsDeleted == false && x.ParentServiceId == serviceId).ToList();
                //if (SubServices != null && SubServices.Count > 0)
                //    iObject.SubServices = AutoMapper.ListObjectToListObjectMapper<Models.Service, Models.IEService>(SubServices);

                var ServiceVideos = iDbContext.ServiceVideos.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (ServiceVideos != null && ServiceVideos.Count > 0)
                    iObject.ServiceVideos = AutoMapper.ListObjectToListObjectMapper<Models.ServiceVideo, Models.IEServiceVideo>(ServiceVideos);

                var ServiceViewers = iDbContext.ServiceViewers.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (ServiceViewers != null && ServiceViewers.Count > 0)
                    iObject.ServiceViewers = AutoMapper.ListObjectToListObjectMapper<Models.ServiceViewer, Models.IEServiceViewer>(ServiceViewers);

                var ServiceUserTypes = iDbContext.ServiceUserTypes.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (ServiceUserTypes != null && ServiceUserTypes.Count > 0)
                    iObject.ServiceUserTypes = AutoMapper.ListObjectToListObjectMapper<Models.ServiceUserType, Models.IEServiceUserTypes>(ServiceUserTypes);

                var ServiceCategories = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).ToList();
                if (ServiceCategories != null && ServiceCategories.Count > 0)
                    iObject.ServiceCategories = AutoMapper.ListObjectToListObjectMapper<Models.ServiceCategory, Models.IEServiceCategory>(ServiceCategories);

                var ServiceCommentsAttachmentsConfig = iDbContext.ServiceCommentsAttachmentsConfigs.Where(x => x.IsDeleted == false && x.ServiceId == serviceId).FirstOrDefault();
                if (ServiceCommentsAttachmentsConfig != null)
                    iObject.ServiceCommentsAttachmentsConfig = AutoMapper.ObjectToObjectMapper<Models.ServiceCommentsAttachmentsConfig, Models.IEServiceCommentsAttachmentsConfig>(ServiceCommentsAttachmentsConfig, null);

                var ServiceFormsConfig = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == serviceId).ToList();
                if (ServiceFormsConfig != null && ServiceFormsConfig.Count > 0)
                {
                    iObject.ServiceFormsConfig = AutoMapper.ListObjectToListObjectMapper<Models.ServiceFormsConfig, Models.IEServiceFormsConfig>(ServiceFormsConfig);

                    foreach (var iInput in iObject.ServiceFormsConfig)
                    {
                        var ServiceTemplates = iDbContext.ServiceTemplates.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).FirstOrDefault();
                        if (ServiceTemplates != null)
                            iInput.ServiceTemplates = AutoMapper.ObjectToObjectMapper<Models.ServiceTemplate, Models.IEServiceTemplate>(ServiceTemplates, null);

                        var ServiceFormsDrilldownConfig = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).ToList();
                        if (ServiceFormsDrilldownConfig != null && ServiceFormsDrilldownConfig.Count > 0)
                            iInput.ServiceFormsDrilldownConfig = AutoMapper.ListObjectToListObjectMapper<Models.ServiceFormsDrilldownConfig, Models.IEServiceFormsDrilldownConfig>(ServiceFormsDrilldownConfig);

                        var ServiceInputLookupActions = iDbContext.ServiceInputLookupActions.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).ToList();
                        if (ServiceInputLookupActions != null && ServiceInputLookupActions.Count > 0)
                            iInput.ServiceInputLookupActions = AutoMapper.ListObjectToListObjectMapper<Models.ServiceInputLookupAction, Models.IEServiceInputLookupAction>(ServiceInputLookupActions);

                        var ServiceInputTooltips = iDbContext.ServiceInputTooltips.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).ToList();
                        if (ServiceInputTooltips != null && ServiceInputTooltips.Count > 0)
                            iInput.ServiceInputTooltips = AutoMapper.ListObjectToListObjectMapper<Models.ServiceInputTooltip, Models.IEServiceInputTooltip>(ServiceInputTooltips);

                        var InputLookup = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookupId == iInput.OptionId).FirstOrDefault();
                        if (InputLookup != null)
                        {
                            iInput.Lookup = AutoMapper.ObjectToObjectMapper<Models.LkLookup, Models.IELkLookup>(InputLookup, null);

                            var LookupOptions = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == iInput.Lookup.LookupId).ToList();
                            if (LookupOptions != null && LookupOptions.Count > 0)
                                iInput.Lookup.LookupOption = AutoMapper.ListObjectToListObjectMapper<Models.LookupOption, Models.IELookupOption>(LookupOptions);
                        }
                    }                 
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iObject;
        }
        public static Models.ImportExport2 GetServiceObjectForImportExport2(int serviceId)
        {
            MOFPortalEntities iDbContext = null;
            Models.ImportExport2 iObject = new ImportExport2();

            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;
                //
                var Services = iDbContext.Services.Where(x => x.ServiceId == serviceId || x.ParentServiceId == serviceId).ToList();
                //
                iObject.Services = new List<IEService>();
                foreach (var Service in Services)
                {
                    //1.Services
                    var IEService = AutoMapper.ObjectToObjectMapper<Models.Service, Models.IEService>(Service, null);

                    //2.Service Categories
                    var ServiceCategories = iDbContext.ServiceCategories.Where(x => x.IsDeleted == false && x.ServiceId == Service.ServiceId).ToList();
                    if (ServiceCategories != null && ServiceCategories.Count > 0)
                        IEService.ServiceCategories = AutoMapper.ListObjectToListObjectMapper<Models.ServiceCategory, Models.IEServiceCategory>(ServiceCategories);

                    //3.Service Tabs
                    var ServiceTabs = iDbContext.ServiceTabs.Where(x => x.IsDeleted == false && x.ServiceId == Service.ServiceId).ToList();
                    if (ServiceTabs != null && ServiceTabs.Count > 0)
                        IEService.ServiceTabs = AutoMapper.ListObjectToListObjectMapper<Models.ServiceTab, Models.IEServiceTab>(ServiceTabs);

                    //4.Service Viewers
                    var ServiceViewers = iDbContext.ServiceViewers.Where(x => x.IsDeleted == false && x.ServiceId == Service.ServiceId).ToList();
                    if (ServiceViewers != null && ServiceViewers.Count > 0)
                        IEService.ServiceViewers = AutoMapper.ListObjectToListObjectMapper<Models.ServiceViewer, Models.IEServiceViewer>(ServiceViewers);

                    //4.1 Service UserTypes
                    var ServiceUserTypes = iDbContext.ServiceUserTypes.Where(x => x.IsDeleted == false && x.ServiceId == Service.ServiceId).ToList();
                    if (ServiceUserTypes != null && ServiceUserTypes.Count > 0)
                        IEService.ServiceUserTypes = AutoMapper.ListObjectToListObjectMapper<Models.ServiceUserType, Models.IEServiceUserTypes>(ServiceUserTypes);

                    //5.Service Comments Config
                    var ServiceCommentsAttachmentsConfig = iDbContext.ServiceCommentsAttachmentsConfigs.Where(x => x.IsDeleted == false && x.ServiceId == Service.ServiceId).FirstOrDefault();
                    if (ServiceCommentsAttachmentsConfig != null)
                        IEService.ServiceCommentsAttachmentsConfig = AutoMapper.ObjectToObjectMapper<Models.ServiceCommentsAttachmentsConfig, Models.IEServiceCommentsAttachmentsConfig>(ServiceCommentsAttachmentsConfig, null);

                    //6.Service Videos
                    //var ServiceVideos = iDbContext.ServiceVideos.Where(x => x.IsDeleted == false && x.ServiceId == Service.ServiceId).ToList();
                    //if (ServiceVideos != null && ServiceVideos.Count > 0)
                    //    IEService.ServiceVideos = AutoMapper.ListObjectToListObjectMapper<Models.ServiceVideo, Models.IEServiceVideo>(ServiceVideos);

                    //6.Service Forms Config
                    var ServiceFormsConfig = iDbContext.ServiceFormsConfigs.Where(x => x.IsDeleted == false && x.LanguageId == 1 && x.ServiceId == Service.ServiceId).ToList();
                    if (ServiceFormsConfig != null && ServiceFormsConfig.Count > 0)
                    {
                        IEService.ServiceFormsConfig = AutoMapper.ListObjectToListObjectMapper<Models.ServiceFormsConfig, Models.IEServiceFormsConfig>(ServiceFormsConfig);

                        foreach (var iInput in IEService.ServiceFormsConfig)
                        {
                            var ServiceTemplates = iDbContext.ServiceTemplates.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).FirstOrDefault();
                            if (ServiceTemplates != null)
                                iInput.ServiceTemplates = AutoMapper.ObjectToObjectMapper<Models.ServiceTemplate, Models.IEServiceTemplate>(ServiceTemplates, null);

                            var ServiceFormsDrilldownConfig = iDbContext.ServiceFormsDrilldownConfigs.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).ToList();
                            if (ServiceFormsDrilldownConfig != null && ServiceFormsDrilldownConfig.Count > 0)
                                iInput.ServiceFormsDrilldownConfig = AutoMapper.ListObjectToListObjectMapper<Models.ServiceFormsDrilldownConfig, Models.IEServiceFormsDrilldownConfig>(ServiceFormsDrilldownConfig);

                            var ServiceInputLookupActions = iDbContext.ServiceInputLookupActions.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).ToList();
                            if (ServiceInputLookupActions != null && ServiceInputLookupActions.Count > 0)
                                iInput.ServiceInputLookupActions = AutoMapper.ListObjectToListObjectMapper<Models.ServiceInputLookupAction, Models.IEServiceInputLookupAction>(ServiceInputLookupActions);

                            var ServiceInputTooltips = iDbContext.ServiceInputTooltips.Where(x => x.IsDeleted == false && x.InputControlId == iInput.ID).ToList();
                            if (ServiceInputTooltips != null && ServiceInputTooltips.Count > 0)
                                iInput.ServiceInputTooltips = AutoMapper.ListObjectToListObjectMapper<Models.ServiceInputTooltip, Models.IEServiceInputTooltip>(ServiceInputTooltips);

                            //var InputLookup = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookupId == iInput.OptionId).FirstOrDefault();
                            var InputLookup = iDbContext.LkLookups.Where(x => x.IsDeleted == false && x.LookUpName == iInput.ExternalLookupId).FirstOrDefault();
                            if (InputLookup != null)
                            {
                                iInput.Lookup = AutoMapper.ObjectToObjectMapper<Models.LkLookup, Models.IELkLookup>(InputLookup, null);

                                //var LookupOptions = iDbContext.LookupOptions.Where(x => x.IsDeleted == false && x.LookupId == iInput.Lookup.LookupId).ToList();
                                var LookupOptions = iDbContext.vLookupOptions.Where(x => x.LookUpName == iInput.ExternalLookupId).ToList();
                                if (LookupOptions != null && LookupOptions.Count > 0)
                                    iInput.Lookup.LookupOption = AutoMapper.ListObjectToListObjectMapper<Models.vLookupOption, Models.IELookupOption>(LookupOptions);
                            }
                        }
                    }
                    iObject.Services.Add(IEService);
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iObject;
        }

        public static ServiceGuideData GetServiceGuideFullData()
        {
            MOFPortalEntities iDbContext = null;
            ServiceGuideData iData = new ServiceGuideData();
            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;
                
                iData.serviceGuide = iDbContext.ServiceGuides.Where(x => x.IsDeleted == false).FirstOrDefault();
                
                iData.serviceGuideChannels = iDbContext.ServiceGuideChannels.Where(x => x.IsDeleted == false).OrderBy(x=>x.SortOrder).ToList();

                iData.serviceGuideFaqs = iDbContext.ServiceGuideFaqs.Where(x => x.IsDeleted == false).OrderBy(x=>x.SortOrder).ToList();

                iData.serviceGuideProcedures = iDbContext.ServiceGuideProcedures.Where(x => x.IsDeleted == false).OrderBy(x=>x.SortOrder).ToList();

                iData.serviceGuideSupports = iDbContext.ServiceGuideSupports.Where(x => x.IsDeleted == false).OrderBy(x=>x.SortOrder).ToList();

                iData.serviceGuideVideos = iDbContext.ServiceGuideVideos.Where(x => x.IsDeleted == false).OrderBy(x=>x.SortOrder).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static ServiceGuide GetServiceGuideData()
        {
            MOFPortalEntities iDbContext = null;
            ServiceGuide iData = new ServiceGuide();
            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;
                iData = iDbContext.ServiceGuides.Where(x => x.IsDeleted == false).FirstOrDefault();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static List<ServiceGuideChannel> GetServiceGuideChannels(Models.SearchParams iSearch, out int totalRecords)
        {
            MOFPortalEntities iDbContext = null;
            List<ServiceGuideChannel> iData = new List<ServiceGuideChannel>();
            totalRecords = 0;
            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;

                iData = iDbContext.ServiceGuideChannels.Where(x => x.IsDeleted == false).ToList();

                totalRecords = iData.Count;

                iData = iData.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static List<ServiceGuideFaq> GetServiceGuideFaqs(Models.SearchParams iSearch, out int totalRecords)
        {
            MOFPortalEntities iDbContext = null;
            List<ServiceGuideFaq> iData = new List<ServiceGuideFaq>();
            totalRecords = 0;
            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;

                iData = iDbContext.ServiceGuideFaqs.Where(x => x.IsDeleted == false).ToList();

                totalRecords = iData.Count;

                iData = iData.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static List<ServiceGuideProcedure> GetServiceGuideProcedures(Models.SearchParams iSearch, out int totalRecords)
        {
            MOFPortalEntities iDbContext = null;
            List<ServiceGuideProcedure> iData = new List<ServiceGuideProcedure>();
            totalRecords = 0;
            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;

                iData = iDbContext.ServiceGuideProcedures.Where(x => x.IsDeleted == false).ToList();

                totalRecords = iData.Count;

                iData = iData.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static List<ServiceGuideSupport> GetServiceGuideSupport(Models.SearchParams iSearch, out int totalRecords)
        {
            MOFPortalEntities iDbContext = null;
            List<ServiceGuideSupport> iData = new List<ServiceGuideSupport>();
            totalRecords = 0;
            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;

                iData = iDbContext.ServiceGuideSupports.Where(x => x.IsDeleted == false).ToList();

                totalRecords = iData.Count;

                iData = iData.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static List<ServiceGuideVideo> GetServiceGuideVideos(Models.SearchParams iSearch, out int totalRecords)
        {
            MOFPortalEntities iDbContext = null;
            List<ServiceGuideVideo> iData = new List<ServiceGuideVideo>();
            totalRecords = 0;
            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();
                iDbContext.Configuration.LazyLoadingEnabled = false;
                iDbContext.Configuration.ProxyCreationEnabled = false;

                iData = iDbContext.ServiceGuideVideos.Where(x => x.IsDeleted == false).ToList();

                totalRecords = iData.Count;

                iData = iData.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iData;
        }
        public static List<Dashboard> GetServiceGuideRatings(DashboardFilters iFilter)
        {
            List<Dashboard> ListOfA = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {
                //string sql = @"Select 
                //                 AVG(Rate) Value,DATENAME(Month,RowInsertDate) as KPI
                //                From 
                //                 dbo.ServiceGuideRatings A With(NoLock)
                //                Where 1=1
                //                 And A.IsDeleted = 0 And Year(RowInsertDate) = {0}
                //                Group By 
                //                 DATENAME(Month,RowInsertDate)";

                //sql = string.Format(sql, iFilter.yearValue);

                //ListOfA = iDbContext.Database.SqlQuery<Dashboard>(sql).OrderBy(x=>x.KPI).ToList();

                var data = iDbContext.ES_GetKpiAverageRating(iFilter.yearValue, Helper.CurrentLanguage()).ToList();

                ListOfA = AutoMapper.ListObjectToListObjectMapper<Models.ES_GetKpiAverageRating_Result, Models.Dashboard>(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return ListOfA;
        }
        public static Models.ChartModel<double> GetServiceGuideRatings_LINE(DashboardFilters iFilter)
        {
            var chartModel = new Models.ChartModel<double>();           
            DataTable dt = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {

                string sql = @"Select 
	                                AVG(Rate) as Value, DATENAME(Month,RowInsertDate) as KPI
                                From 
	                                dbo.ServiceGuideRatings A With(NoLock)
                                Where 1=1
	                                And A.IsDeleted = 0
                                Group By 
	                                DATENAME(Month,RowInsertDate)";

                sql = @"DECLARE 
                            @columns NVARCHAR(MAX) = '',
	                        @sql     NVARCHAR(MAX) = '';


                        SELECT @columns = COALESCE(@columns+',' ,'') + xx 
                        FROM (
                        SELECT distinct
                           QUOTENAME(DATENAME(Month,RowInsertDate)) as xx
                        FROM 
                            dbo.ServiceGuideRatings With(NoLock) Where 1=1 And IsDeleted = 0

                        ) t

                        SET @columns = Right(@columns, LEN(@columns) - 1);
                        --select @columns

                        SET @sql ='
                        SELECT * FROM   
                        (
                            SELECT 
                                rate, 
                                DATENAME(Month,RowInsertDate) as xx
                            FROM 
                                dbo.ServiceGuideRatings With(NoLock) Where 1=1 And IsDeleted = 0
                        ) t 
                        PIVOT(
                            avg(rate) 
                            FOR xx IN ('+ @columns +')
                        ) AS pivot_table;';

                        EXECUTE sp_executesql @sql;";

                var conn = iDbContext.Database.Connection;

                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, (System.Data.SqlClient.SqlConnection)conn))
                {
                    cmd.CommandType = CommandType.Text;

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt = new DataTable("ChartData");
                        dt.Load(dr);
                        dr.Close();
                    }
                    cmd.Dispose();
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<string> seriesName = new List<string>();
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    seriesName.Add(dr["xx"].ToString());
                    //}

                    chartModel.Title = "Rating by Month for Year 2023";
                    chartModel.SubTitle = "";
                    chartModel.XAxisCategories = seriesName.Distinct().ToArray();
                    chartModel.XAxisTitle = "";
                    chartModel.YAxisTitle = "";
                    chartModel.YAxisTooltipValueSuffix = "";

                    List<Models.SeriesModel<double>> lSeries = new List<Models.SeriesModel<double>>();
                    Models.SeriesModel<double> iSeries = new Models.SeriesModel<double>();
                    ICollection<double> data = new List<double>();

                    //var x = new List<string> { "النسبة المئوية", "مهمة غير مكتملة", "المهمة المكتملة", "المهمة الموكلة" };
                    string[] x = System.Globalization.DateTimeFormatInfo.CurrentInfo.MonthNames;

                    foreach (string dr in x)
                    {
                        iSeries = new Models.SeriesModel<double>();
                        data = new List<double>();

                        iSeries.name = dr;


                        foreach (DataColumn iCdr in dt.Columns)
                        {
                            double yvalue = 0;
                            if (iCdr.ColumnName == dr)
                            {
                                yvalue = Convert.ToDouble(dt.Columns[dr].ToString());
                                data.Add(yvalue);
                            }
                        }

                        iSeries.data = data;
                        lSeries.Add(iSeries);
                    }

                    chartModel.Series = lSeries;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return chartModel;
        }
        public static Models.ChartModel<double> GetServiceGuideByTypes_LINE(DashboardFilters iFilter)
        {
            var chartModel = new Models.ChartModel<double>();
            DataTable dt = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {
                /*string sql = @";With SearchCategories As 
                                (
	                                Select 
		                                Count(1) Total, CategoryId 
	                                From dbo.UserActivityLogging  A With(NoLock) 
	                                Where 1=1
	                                And A.IsDeleted = 0 %Year% %Month%
	                                Group By CategoryId
                                )
                                Select 
	                                A.TypeId
	                                ,A.DescriptionAlt
	                                ,A.DescriptionEng
	                                ,B.Total
                                From dbo.ServiceTypes A With(NoLock)
	                                Join SearchCategories B With(NoLock) On A.TypeId = B.CategoryId
                                Where 1=1
                                And A.IsDeleted = 0";

                if (iFilter.yearValue.HasValue)
                {
                    sql = sql.Replace("%Year%", " And Year(RowInsertDate) = " + iFilter.yearValue.Value);
                }
                else
                {
                    sql = sql.Replace("%Year%", "");
                }
                if (string.IsNullOrEmpty(iFilter.monthName) == false)
                {
                    sql = sql.Replace("%Month%", " And DATENAME(Month,RowInsertDate) = '"+ iFilter.monthName + "'");
                }
                else
                {
                    sql = sql.Replace("%Month%", "");
                }

                var conn = iDbContext.Database.Connection;

                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, (System.Data.SqlClient.SqlConnection)conn))
                {
                    cmd.CommandType = CommandType.Text;

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dt = new DataTable("ChartData");
                        dt.Load(dr);
                        dr.Close();
                    }
                    cmd.Dispose();
                }*/

                var iResult = iDbContext.ES_GetKpiByServiceType(iFilter.yearValue, iFilter.monthName, Helper.CurrentLanguage()).ToList();
                if (iResult != null)
                {
                    dt = Utility.ConvertListToDataTable(iResult);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<string> seriesName = null;

                    if (Helper.CurrentLanguage() == (int)Language.Arabic)
                    {
                        var serviceTypes = from st in iDbContext.ServiceTypes.Where(y => y.IsDeleted == false).ToList()
                                           select st.DescriptionAlt;
                        seriesName = serviceTypes.ToList();
                    }
                    else
                    {
                        var serviceTypes = from st in iDbContext.ServiceTypes.Where(y => y.IsDeleted == false).ToList()
                                           select st.DescriptionEng;
                        seriesName = serviceTypes.ToList();
                    }

                    chartModel.Title = DbManager.GetText("ServiceGuide", "lblKpiServiceTypesTitle", "Statistics by Service Type");
                    chartModel.SubTitle = "";
                    chartModel.XAxisCategories = seriesName.Distinct().ToArray();
                    chartModel.XAxisTitle = "";
                    chartModel.YAxisTitle = "";
                    chartModel.YAxisTooltipValueSuffix = "";

                    List<Models.SeriesModel<double>> lSeries = new List<Models.SeriesModel<double>>();
                    Models.SeriesModel<double> iSeries = new Models.SeriesModel<double>();
                    ICollection<double> data = new List<double>();

                    foreach (string dr in seriesName)
                    {
                        iSeries = new Models.SeriesModel<double>();
                        data = new List<double>();

                        iSeries.name = dr;

                        var dtRow = dt
                            .AsEnumerable()
                            .Where(myRow => myRow.Field<string>("KPI") == dr).FirstOrDefault();

                        double yvalue = 0;
                        if (dtRow != null)
                        {
                            yvalue = Convert.ToDouble(dtRow[1].ToString());
                        }

                        data.Add(yvalue);

                        //foreach (DataRow iCdr in dt.Rows)
                        //{
                        //    double yvalue = 0;
                        //    if (iCdr[dr] != null && iCdr[dr].ToString() != null)
                        //        yvalue = Convert.ToDouble(iCdr[dr].ToString());

                        //    data.Add(yvalue);
                        //}

                        iSeries.data = data;
                        lSeries.Add(iSeries);
                    }

                    chartModel.Series = lSeries;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return chartModel;
        }
        public static List<Dashboard> GetServiceGuideVisitors(DashboardFilters iFilter)
        {
            List<Dashboard> ListOfA = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {
                var data = iDbContext.ES_GeKpitHitCount(iFilter.yearValue, Helper.CurrentLanguage()).ToList();

                ListOfA = AutoMapper.ListObjectToListObjectMapper<Models.ES_GeKpitHitCount_Result, Models.Dashboard>(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return ListOfA;
        }
        public static Models.ChartModel<double> GetServiceGuideByUserTypes(DashboardFilters iFilter)
        {
            var chartModel = new Models.ChartModel<double>();
            DataTable dt = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {

                var iResult = iDbContext.ES_GetKpiByUserType(iFilter.yearValue, iFilter.monthName, Helper.CurrentLanguage()).ToList();
                if (iResult != null)
                {
                    dt = Utility.ConvertListToDataTable(iResult);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<string> seriesName = null;

                    if (Helper.CurrentLanguage() == (int)Language.Arabic)
                    {
                        var serviceTypes = from st in iDbContext.ServiceEntities.Where(y => y.IsDeleted == false).ToList()
                                           select st.DescriptionAlt;
                        seriesName = serviceTypes.ToList();
                    }
                    else
                    {
                        var serviceTypes = from st in iDbContext.ServiceEntities.Where(y => y.IsDeleted == false).ToList()
                                           select st.DescriptionEng;
                        seriesName = serviceTypes.ToList();
                    }

                    chartModel.Title = DbManager.GetText("ServiceGuide","lblKpiUserTypesTitle", "Statistics by Type of Users");
                    chartModel.SubTitle = "";
                    chartModel.XAxisCategories = seriesName.Distinct().ToArray();
                    chartModel.XAxisTitle = "";
                    chartModel.YAxisTitle = "";
                    chartModel.YAxisTooltipValueSuffix = "";

                    List<Models.SeriesModel<double>> lSeries = new List<Models.SeriesModel<double>>();
                    Models.SeriesModel<double> iSeries = new Models.SeriesModel<double>();
                    ICollection<double> data = new List<double>();

                    foreach (string dr in seriesName)
                    {
                        iSeries = new Models.SeriesModel<double>();
                        data = new List<double>();

                        iSeries.name = dr;

                        var dtRow = dt
                            .AsEnumerable()
                            .Where(myRow => myRow.Field<string>("KPI") == dr).FirstOrDefault();

                        double yvalue = 0;
                        if (dtRow != null)
                        {
                            yvalue = Convert.ToDouble(dtRow[1].ToString());
                        }

                        data.Add(yvalue);

                        //foreach (DataRow iCdr in dt.Rows)
                        //{
                        //    double yvalue = 0;
                        //    if (iCdr[dr] != null && iCdr[dr].ToString() != null)
                        //        yvalue = Convert.ToDouble(iCdr[dr].ToString());

                        //    data.Add(yvalue);
                        //}

                        iSeries.data = data;
                        lSeries.Add(iSeries);
                    }

                    chartModel.Series = lSeries;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return chartModel;
        }
        public static List<Dashboard> GetServiceGuideByKeywords(Models.DashboardFilters iFilter)
        {
            List<Dashboard> ListOfA = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {
                var data = iDbContext.ES_GetKpiByKeywords(iFilter.yearValue, iFilter.monthName).ToList();

                ListOfA = AutoMapper.ListObjectToListObjectMapper<Models.ES_GetKpiByKeywords_Result, Models.Dashboard>(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return ListOfA;
        }
        public static Models.ChartModel<double> GetServiceGuideByServices(DashboardFilters iFilter)
        {
            var chartModel = new Models.ChartModel<double>();
            DataTable dt = null;
            MOFPortalEntities iDbContext = new MOFPortalEntities();
            try
            {

                var iResult = iDbContext.ES_GetKpiByServices(iFilter.yearValue, iFilter.monthName, Helper.CurrentLanguage()).ToList();
                if (iResult != null)
                {
                    dt = Utility.ConvertListToDataTable(iResult);
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<string> seriesName = null;

                    if (Helper.CurrentLanguage() == (int)Language.Arabic)
                    {
                        var serviceTypes = from st in iDbContext.Services.Where(y => y.IsDeleted == false && string.IsNullOrEmpty(y.ServiceGuid) == false).ToList()
                                           select st.DescriptionAlt;
                        seriesName = serviceTypes.ToList();
                    }
                    else
                    {
                        var serviceTypes = from st in iDbContext.Services.Where(y => y.IsDeleted == false && string.IsNullOrEmpty(y.ServiceGuid) == false).ToList()
                                           select st.DescriptionEng;
                        seriesName = serviceTypes.ToList();
                    }

                    chartModel.Title = DbManager.GetText("ServiceGuide", "lblKpiServicesTitle", "Most Searched Services");
                    chartModel.SubTitle = "";
                    chartModel.XAxisCategories = seriesName.Distinct().ToArray();
                    chartModel.XAxisTitle = "";
                    chartModel.YAxisTitle = "";
                    chartModel.YAxisTooltipValueSuffix = "";

                    List<Models.SeriesModel<double>> lSeries = new List<Models.SeriesModel<double>>();
                    Models.SeriesModel<double> iSeries = new Models.SeriesModel<double>();
                    ICollection<double> data = new List<double>();

                    foreach (string dr in seriesName)
                    {
                        iSeries = new Models.SeriesModel<double>();
                        data = new List<double>();

                        iSeries.name = dr;

                        var dtRow = dt
                            .AsEnumerable()
                            .Where(myRow => myRow.Field<string>("KPI") == dr).FirstOrDefault();

                        double yvalue = 0;
                        if (dtRow != null)
                        {
                            yvalue = Convert.ToDouble(dtRow[1].ToString());
                        }

                        data.Add(yvalue);

                        //foreach (DataRow iCdr in dt.Rows)
                        //{
                        //    double yvalue = 0;
                        //    if (iCdr[dr] != null && iCdr[dr].ToString() != null)
                        //        yvalue = Convert.ToDouble(iCdr[dr].ToString());

                        //    data.Add(yvalue);
                        //}

                        iSeries.data = data;
                        lSeries.Add(iSeries);
                    }

                    chartModel.Series = lSeries;
                }
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return chartModel;
        }

        public static List<Models.KeyValue> GetNoActionDashboardByTypeOfUsers()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                List<Models.ServiceEntity> X = null;
                X = iDbContext.ServiceEntities.Where(x => x.IsDeleted == false && x.EntityId > 0).ToList();
              
                foreach (var Y in X)
                {
                    var Z = iDbContext.NoActionUsers.Where(x => x.IsDeleted == false && (Y.DescriptionAlt.ToLower().Contains(x.UserType.ToLower()) || Y.DescriptionEng.ToLower().Contains(x.UserType.ToLower()))).ToList();
                    if (Z != null && Z.Count > 0)
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? Y.DescriptionEng : Y.DescriptionAlt;
                        iKv.Value = Z.Count;
                        iKv.SortOrder = Z.Count;
                        iList.Add(iKv);
                    }
                    else
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? Y.DescriptionEng : Y.DescriptionAlt;
                        iKv.Value = 0;
                        iKv.SortOrder = 0;
                        iList.Add(iKv);
                    }
                }

                //Anonomys Users
                /*{
                    var Z = iDbContext.NoActionUsers.Where(x => x.IsDeleted == false && string.IsNullOrEmpty(x.UserType) == true).ToList();
                    if (Z != null && Z.Count > 0)
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = DbManager.GetText("Dashboard", "lblNoActionAnonymous", "Anonymous");
                        iKv.Value = Z.Count;
                        iKv.SortOrder = Z.Count;
                        iList.Add(iKv);
                    }
                    else
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = DbManager.GetText("Dashboard", "lblNoActionAnonymous", "Anonymous");
                        iKv.Value = 0;
                        iKv.SortOrder = 0;
                        iList.Add(iKv);
                    }
                }*/

                iList = iList.OrderByDescending(x => x.SortOrder).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.KeyValue> GetNoActionDashboardByServices()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                List<Models.Service> X = null;
                X = iDbContext.Services.Where(x => x.IsDeleted == false && x.IsActive == true).ToList();

                foreach (var Y in X)
                {
                    var Z = iDbContext.NoActionUsers.Where(x => x.IsDeleted == false && (x.ExternalServiceId.ToLower() == Y.ExternalServiceID.ToLower())).ToList();
                    if (Z != null && Z.Count > 0)
                    {
                        var iKv = new Models.KeyValue();
                        iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? Y.DescriptionEng : Y.DescriptionAlt;
                        iKv.Value = Z.Count;
                        iKv.SortOrder = Z.Count;
                        iList.Add(iKv);
                    }
                    //else
                    //{
                    //    var iKv = new Models.KeyValue();
                    //    iKv.Key = Helper.CurrentLanguage() == (int)Language.English ? Y.DescriptionEng : Y.DescriptionAlt;
                    //    iKv.Value = 0;
                    //    iKv.SortOrder = 0;
                    //    iList.Add(iKv);
                    //}
                }

                iList = iList.OrderByDescending(x => x.SortOrder).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.KeyValue> GetNoActionTopUsers()
        {
            MOFPortalEntities iDbContext = null;
            List<Models.KeyValue> iList = new List<KeyValue>();
            try
            {
                iDbContext = new MOFPortalEntities();

                var m = iDbContext.NoActionUsers.Where(x => x.IsDeleted == false).GroupBy(x => new { x.UserType, x.UserName }, (key, group) => new
                {
                    Key = key.UserName,
                    //Key2 = key.UserType,
                    Result = group.ToList()
                }).ToList();

                foreach(var i in m)
                {
                    if (!string.IsNullOrEmpty(i.Key))
                        iList.Add(new KeyValue() { Key = i.Key, Value = i.Result.Count, SortOrder = i.Result.Count });
                }

                iList = iList.OrderByDescending(x => x.SortOrder).Take(5).ToList();
            }
            catch (DbEntityValidationException e)
            {
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
                throw new ApplicationException(sb.ToString());
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ES_GetNoActionsData_Result> GetNoActionsData(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ES_GetNoActionsData_Result> iList = new List<Models.ES_GetNoActionsData_Result>();

            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.ES_GetNoActionsData(pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.TypeId, iSearch.FromDate, iSearch.ToDate).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }
        public static List<Models.ES_GetNoActionsSummary_Result> GetNoActionsSummary(Models.SearchParams iSearch)
        {
            MOFPortalEntities iDbContext = null;
            List<Models.ES_GetNoActionsSummary_Result> iList = new List<Models.ES_GetNoActionsSummary_Result>();

            int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
            int pageNumber = iSearch.start / iSearch.length + 1;

            try
            {
                iDbContext = new MOFPortalEntities();

                iList = iDbContext.ES_GetNoActionsSummary(pageNumber, pageSize, iSearch.order[0].column, iSearch.order[0].dir, iSearch.SearchCri, iSearch.TypeId, iSearch.FromDate, iSearch.ToDate).ToList();
            }
            catch (Exception E)
            {
                throw E;
            }
            finally
            {
                if (iDbContext != null)
                    iDbContext.Dispose();
            }
            return iList;
        }

    }
}