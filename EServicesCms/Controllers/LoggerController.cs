using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EServicesCms.Common;

namespace EServicesCms.Controllers
{
    [SessionTimeout]
    public class LoggerController : BaseController
    {
        [UrlDecode]
        public ActionResult Index()
        {
            ViewBag.PageAlertCodes = Helper.GetPageMultipleAlertCodes("UserClientAlerts");
            return View();
        }

        [HttpPost]
        public JsonResult AjaxGetLogger(Models.SearchParams iSearch)
        {
            try
            {
                iSearch = iSearch.TrimObject();

                int totalRows = 0;
                int pageSize = iSearch.length != 0 ? Convert.ToInt32(iSearch.length) : 0;
                int pageNumber = iSearch.start / iSearch.length + 1;

                var iData = Common.DbManager.GetLogger(iSearch);
                if (iData != null && iData.Count > 0)
                    totalRows = iData[0].TotalRows;

                IEnumerable<Object> result;
                result = iData.Select(
                    c => new object[]
                    {
                    c.VersionNo
                    ,c.Message
                    ,c.Source
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