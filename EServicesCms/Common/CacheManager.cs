using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class CacheManager
    {
        public List<Models.LkScreenLabel> LkScreenLabels;
        public Dictionary<string, string> loadConfig;
        MOFPortalEntities iDbContext = null;
        public CacheManager()
        {
            iDbContext = new MOFPortalEntities();

            this.LkScreenLabels = iDbContext.LkScreenLabels.Where(x => x.IsDeleted == false).ToList();

            iDbContext.Dispose();

        }
        public LkScreenLabel GetLabel(string viewId, string labelId)
        {
            LkScreenLabel iList = null;
            iList = (from data in LkScreenLabels
                     select data).Where(x => x.ViewId.ToUpper() == viewId.ToUpper() && x.LabelId.ToUpper() == labelId.ToUpper()).FirstOrDefault();
            return iList;
        }
        public NameValueCollection GetLabelCollectionsEn(string viewId)
        {
            NameValueCollection coll = new NameValueCollection();

            List<LkScreenLabel> list = (from abbr in LkScreenLabels
                                        where abbr.ViewId == viewId// && abbr.LabelId == labelId
                                        select abbr).ToList();

            foreach (LkScreenLabel abbr in list)
            {
                coll.Add(abbr.LabelId, abbr.DescriptionEng);
            }
            return coll;
        }
        public NameValueCollection GetLabelCollectionsAr(string viewId)
        {
            NameValueCollection coll = new NameValueCollection();

            List<LkScreenLabel> list = (from abbr in LkScreenLabels
                                        where abbr.ViewId == viewId// && abbr.LabelId == labelId
                                        select abbr).ToList();

            foreach (LkScreenLabel abbr in list)
            {
                coll.Add(abbr.LabelId, abbr.DescriptionAlt);
            }
            return coll;
        }
        public static void ResetApplicationCache()
        {
            //HttpContext.Current.Application["AppCache"] = null;
            //HttpContext.Current.Application["AppCache"] = new CacheManager();

            HttpContext.Current.Session["AppCache"] = null;
            HttpContext.Current.Session["AppCache"] = new CacheManager();
        }
        public static CacheManager GetApplicationCache()
        {
            if (HttpContext.Current.Session["AppCache"] != null)
                return HttpContext.Current.Session["AppCache"] as CacheManager;
            else
                return null;

            //if (HttpContext.Current.Application["AppCache"] != null)
            //    return HttpContext.Current.Application["AppCache"] as CacheManager;
            //else
            //    return null;
        }
    }
}