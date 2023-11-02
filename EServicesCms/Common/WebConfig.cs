using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace EServicesCms.Common
{
    public class WebConfig
    {
        /// <summary>
        /// GetStringValue
        /// </summary>
        /// <returns></returns>
        public static string GetStringValue(string key)
        {
            string strReturn = string.Empty;
            try
            {
                if (ConfigurationManager.AppSettings[key] == null)
                    throw new ApplicationException("Please configure " + key + " in WebConfig.");
                else
                    strReturn = ConfigurationManager.AppSettings[key].ToString();
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return strReturn;
        }
        /// <summary>
        /// Value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntValue(string key)
        {
            int iReturn = 0;
            try
            {
                if (ConfigurationManager.AppSettings[key] == null)
                    throw new ApplicationException("Please configure " + key + " in WebConfig.");
                else
                    iReturn = Convert.ToInt32(ConfigurationManager.AppSettings[key].ToString());
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return iReturn;
        }
        /// <summary>
        /// ValueLong
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long GetLongValue(string key)
        {
            long iReturn = 0;
            try
            {
                if (ConfigurationManager.AppSettings[key] == null)
                    throw new ApplicationException("Please configure " + key + " in WebConfig.");
                else
                    iReturn = Convert.ToInt32(ConfigurationManager.AppSettings[key].ToString());
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return iReturn;
        }
        /// <summary>
        /// GetBoolValue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetBoolValue(string key)
        {
            bool iReturn = false;
            try
            {
                if (ConfigurationManager.AppSettings[key] == null)
                    throw new ApplicationException("Please configure " + key + " in WebConfig.");
                else
                {
                    if (Convert.ToInt32(ConfigurationManager.AppSettings[key].ToString()) == 1)
                        iReturn = true;
                }
            }
            catch (Exception Exp)
            {
                throw Exp;
            }
            return iReturn;
        }
    }
}