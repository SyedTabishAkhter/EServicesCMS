using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EServicesCms.Common;

namespace EServicesCms.Controllers
{
    [SessionTimeout]
    public class AuditTrialController : BaseController
    {
        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");

            if (Security.isUserSystemAdministrator() == false)
            {
                int? userDepId = Security.GetUser().DepartmentId;
                var UsersList = iDbContext.Users.Where(x => x.IsDeleted == false && x.DepartmentId == userDepId).ToList();
                //ViewBag.UsersList = new SelectList(UsersList.ToArray(), "UserName", "FullName");
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    ViewBag.UsersList = new SelectList(UsersList.ToArray(), "UserName", "FullName");
                }
                else
                {
                    ViewBag.UsersList = new SelectList(UsersList.ToArray(), "UserName", "FullNameAlt");
                }
            }
            else
            {
                var UsersList = iDbContext.Users.Where(x => x.IsDeleted == false).ToList();
                //ViewBag.UsersList = new SelectList(UsersList.ToArray(), "UserName", "FullName");
                if (Helper.CurrentLanguage() == (int)Language.English)
                {
                    ViewBag.UsersList = new SelectList(UsersList.ToArray(), "UserName", "FullName");
                }
                else
                {
                    ViewBag.UsersList = new SelectList(UsersList.ToArray(), "UserName", "FullNameAlt");
                }
            }

            //if (Security.isUserSystemAdministrator() == false)
            //{
            //    IDictionary<int, string> yesNo = new Dictionary<int, string>();
            //    yesNo.Add(7, "All");
            //    yesNo.Add(8, "Service Guide");
            //    ViewBag.ActionsList = new SelectList(yesNo.OrderBy(x => x.Value), "Key", "Value", 8);
            //}
            //else
            //{
            //    IDictionary<int, string> yesNo = new Dictionary<int, string>();
            //    yesNo.Add(7, "Others");
            //    yesNo.Add(8, "Service Guide");
            //    ViewBag.ActionsList = new SelectList(yesNo.OrderBy(x => x.Value), "Key", "Value", 7);
            //}

            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetAuditTrial(Models.SearchParams iSearch)
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

                var iData = Common.DbManager.GetUserScreenActions(iSearch);
                if (iData != null && iData.Count > 0)
                    totalRows = iData[0].TotalRows;

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                    //c.ActionName
                    c.UniqueId
                    ,c.Remarks
                    ,c.RowInsertedBy
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
    }
}