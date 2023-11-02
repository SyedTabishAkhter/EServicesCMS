using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using EServicesCms.Models;
using System.DirectoryServices.AccountManagement;
using System.Web.Security;
using RestSharp;

namespace EServicesCms.Common
{
    public class TokenValidation
    {
        public bool Validated { get { return Errors.Count == 0; } }
        public readonly List<TokenValidationStatus> Errors = new List<TokenValidationStatus>();
    }

    public enum TokenValidationStatus
    {
        Expired,
        WrongUser,
        WrongPurpose,
        WrongGuid,
        WrongUserName,
        WrongPassword,
        WrongDeviceId
    }

    public class Security
    {
        public Security()
        {

        }
        public static Models.User GetUser()
        {
            Models.User strReturn = null;
            try
            {
                if (HttpContext.Current.Session["iUser"] != null)
                    strReturn = HttpContext.Current.Session["iUser"] as Models.User;
            }
            catch (Exception Exp)
            {
                strReturn = null;
            }
            return strReturn;
        }
        public static bool IsUserLoggedIn()
        {
            var isValid = true;
            try
            {
                if (GetUser() == null)
                    isValid = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isValid;
        }
        public static string GetUserName()
        {
            string strReturn = string.Empty;
            try
            {
                if (HttpContext.Current.Session["gszUser"] != null)
                    strReturn = HttpContext.Current.Session["gszUser"].ToString();
            }
            catch (Exception Exp)
            {
                strReturn = string.Empty; ;
            }
            return strReturn;
        }
        public static string GetIpAddress()
        {
            string strReturn = string.Empty;
            try
            {
                if (HttpContext.Current.Session["iClientIpAddress"] != null)
                    strReturn = HttpContext.Current.Session["iClientIpAddress"].ToString();
            }
            catch (Exception Exp)
            {
                strReturn = string.Empty;
            }
            return strReturn;
        }
        public static string GenerateRandomPassword()
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyz";
            Random randNum = new Random();
            char[] chars = new char[8];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < 8; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
        public static string GenerateToken(string userName, string password, string deviceId)
        {
            string _time = Crypto.Encrypt(DateTime.Now.ToString());
            string _IdUserName = Crypto.Encrypt(userName);
            string _keyPassword = Crypto.Encrypt(password);
            string _deviceId = Crypto.Encrypt(deviceId);
            string tokenData = Crypto.Encrypt(_time + "-X-" + _IdUserName + "-X-" + _keyPassword + "-X-" + _deviceId);
            return tokenData;
        }
        public static TokenValidation ValidateToken(int userId, string deviceId, string token)
        {
            Models.User user = DbManager.GetUser(userId);

            var result = new TokenValidation();

            string tokenData = Crypto.Decrypt(token);
            string[] arrRay = tokenData.Split(new string[] { "-X-" }, StringSplitOptions.None);
            string _time = Crypto.Decrypt(arrRay[0].ToString().Replace("\0", "")).Replace("\0", "").Replace(" AM","").Replace(" PM", "");
            string _IdUserName = Crypto.Decrypt(arrRay[1].ToString().Replace("\0", "")).Replace("\0", "");
            string _keyPassword = Crypto.Decrypt(arrRay[2].ToString().Replace("\0", "")).Replace("\0", "");
            string _deviceId = Crypto.Decrypt(arrRay[3].ToString().Replace("\0", "")).Replace("\0", "");

            //DateTime when = DateTime.Parse(_time);
            //if (when < DateTime.UtcNow.AddDays(-8))
            //{
            //    result.Errors.Add(TokenValidationStatus.Expired);
            //}
            if (user.UserName != _IdUserName)
            {
                result.Errors.Add(TokenValidationStatus.WrongUserName);
            }
            if ("VLRA@BIGAN" != _keyPassword)
            {
                result.Errors.Add(TokenValidationStatus.WrongPassword);
            }
            if (deviceId != _deviceId)
            {
                result.Errors.Add(TokenValidationStatus.WrongDeviceId);
            }
            return result;
        }
        public static bool Authenticate(string domainName, string userName, string passWord)
        {
            bool isValid = false;
            try
            {
                //switch (WebConfig.GetIntValue("ACL_Mode"))
                //{
                //    case (int)AclMode.ActiveDirectory:
                //        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName))
                //        {
                //            isValid = pc.ValidateCredentials(userName, passWord);
                //        }
                //        break;

                //    case (int)AclMode.Database:

                //        isValid = Dal.Manager.Users.Authenticate(userName, passWord);

                //        break;
                //}
            }
            catch (Exception ex)
            {
                //Helper.Write2LogFile("Inside Authenticate ex = " + ex.ToString());
                //isValid = false;
                //throw ex;
            }
            return isValid;
        }
        public static bool Authenticate(string userName, string passWord)
        {
            bool isValid = false;
            try
            {
                //switch (WebConfig.GetIntValue("ACL_Mode"))
                //{
                //    case (int)AclMode.ActiveDirectory:

                //        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, WebConfig.GetStringValue("ACL_Domain")))
                //        {
                //            isValid = pc.ValidateCredentials(userName, passWord);
                //        }
                //        break;

                //    case (int)AclMode.Database:

                //        isValid = Dal.Manager.Users.Authenticate(userName, passWord);

                //        break;
                //}
            }
            catch (Exception ex)
            {
                //Helper.Write2LogFile("Inside Authenticate ex = " + ex.ToString());
                isValid = false;
                throw ex;
            }
            return isValid;
        }
        public static bool Authenticate(out string userName)
        {
            bool isValid = false;
            userName = string.Empty;
            try
            {
                //userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();

                //if (string.IsNullOrEmpty(userName) == false)
                //    userName = userName.Split('\\')[1];

                //switch (WebConfig.GetIntValue("ACL_Mode"))
                //{
                //    case (int)AclMode.ActiveDirectory:

                //        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, WebConfig.GetStringValue("ACL_Domain")))
                //        {
                //            UserPrincipal user = UserPrincipal.FindByIdentity(pc, userName);

                //            if (user != null)
                //            {
                //                isValid = true;
                //                //// check if user is member of that group
                //                //if (user.IsMemberOf(group))
                //                //{
                //                //    // do something.....
                //                //}
                //            }
                //            else
                //            {
                //            }

                //        }
                //        break;

                //    case (int)AclMode.Database:

                //        //isValid = Dal.Manager.UsersManager.Authenticate(userName, passWord);
                //        isValid = true;
                //        break;


                //}
            }
            catch (Exception ex)
            {
                //Helper.Write2LogFile("Inside Authenticate ex = " + ex.ToString());
                isValid = false;
                throw ex;
            }
            return isValid;
        }
        public static UserPrincipal userPrincipal(string userId)
        {
            UserPrincipal up = null;
            try
            {
                //Helper.Write2LogFile("Inside userPrincipal userId = " + userId);

                //switch (WebConfig.GetIntValue("ACL_Mode"))
                //{
                //    case (int)AclMode.ActiveDirectory:

                //        Helper.Write2LogFile("Inside userPrincipal ACL_Domain = " + WebConfig.GetStringValue("ACL_Domain"));

                //        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, WebConfig.GetStringValue("ACL_Domain")))
                //        {
                //            Helper.Write2LogFile("b4 FindByIdentity userId = " + userId);

                //            up = UserPrincipal.FindByIdentity(pc, userId);

                //            Helper.Write2LogFile("After FindByIdentity userId = " + up.Name);
                //        }
                //        break;

                //    case (int)AclMode.Database:

                //        up = null;

                //        break;
                //}
            }
            catch (Exception ex)
            {
                //Helper.Write2LogFile("Inside userPrincipal ex = " + ex.ToString());
                up = null;
                throw ex;
            }
            return up;
        }
        public static Models.User GetUserInfoFromAD(string userId)
        {
            Models.User iUser = null;
            UserPrincipal up = null;
            try
            {
                //Helper.Write2LogFile("Inside GetUserInfoFromAD userId = " + userId);

                //switch (WebConfig.GetIntValue("ACL_Mode"))
                //{
                //    case (int)AclMode.ActiveDirectory:

                //        Helper.Write2LogFile("Inside GetUserInfoFromAD ACL_Domain = " + WebConfig.GetStringValue("ACL_Domain"));

                //        using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, WebConfig.GetStringValue("ACL_Domain")))
                //        {
                //            Helper.Write2LogFile("b4 FindByIdentity userId = " + userId);

                //            up = UserPrincipal.FindByIdentity(pc, userId);
                //            if (up != null)
                //            {
                //                iUser = new Model.Users();
                //                iUser.UserName = userId;
                //                iUser.Name = up.Name;
                //                iUser.Email = up.EmailAddress;
                //                iUser.Mobile = up.VoiceTelephoneNumber;
                //                iUser.EmployeeId = up.EmployeeId;
                //                Helper.Write2LogFile("After FindByIdentity userId = " + up.Name);
                //            }
                //            else
                //            {
                //                Helper.Write2LogFile("UserPrincipal.FindByIdentity IS NULL");
                //            }
                //        }
                //        break;

                //    case (int)AclMode.Database:

                //        iUser = null;

                //        break;
                //}
            }
            catch (Exception ex)
            {
                //Helper.Write2LogFile("Inside GetUserInfoFromAD ex = " + ex.ToString());
                iUser = null;
                throw ex;
            }
            return iUser;
        }
        public static bool isUserAuthorized(string groupTagName, bool isAdminCheck = true)
        {
            bool bReturn = false;
            try
            {
                if (isAdminCheck == true)
                    bReturn = isUserSystemAdministrator();

                if (string.IsNullOrEmpty(groupTagName))
                {
                    bReturn = true;
                    return bReturn;
                }
                //
                if (bReturn == false)
                {
                    if (HttpContext.Current.Session["iUserRoleGroups"] != null)
                    {
                        var  lstUserGroups = HttpContext.Current.Session["iUserRoleGroups"] as LkRoleObject;
                        if (lstUserGroups != null)
                        {
                            if (lstUserGroups.RoleGroups != null && lstUserGroups.RoleGroups.Count > 0)
                            {
                                Models.LkGroup iAccess = lstUserGroups.RoleGroups.Find(delegate (Models.LkGroup lsa) { return lsa.GroupName == groupTagName; });
                                if (iAccess != null)
                                {
                                    bReturn = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Exp)
            {
                bReturn = false;
            }
            return bReturn;
        }
        public static bool isUserSystemAdministrator()
        {
            bool bReturn = false;
            try
            {
                if (HttpContext.Current.Session["iUserRoleGroups"] != null)
                {
                    var lstUserGroups = HttpContext.Current.Session["iUserRoleGroups"] as LkRoleObject;
                    if (lstUserGroups != null)
                    {
                        if (lstUserGroups.RoleGroups != null && lstUserGroups.RoleGroups.Count > 0)
                        {
                            Models.LkGroup iAccess = lstUserGroups.RoleGroups.Find(delegate (Models.LkGroup lsa) { return lsa.GroupName == "MOF_ESRV_ADMIN"; });
                            if (iAccess != null)
                            {
                                bReturn = true;
                            }
                        }
                    }
                }
            }
            catch (Exception Exp)
            {
                bReturn = false;
            }
            return bReturn;
        }
        public static Models.BalTokenResponse GetBalToken()
        {
            Models.BalTokenResponse tokenResponse = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                RestClient restClient = new RestClient(WebConfig.GetStringValue("BalApiUrl"));

                var authRequest = new RestRequest("/token", Method.POST) { RequestFormat = DataFormat.Json };


                var i = new Models.BalTokenRequest();
                i.username = Helper.EncodeToBase64String(WebConfig.GetStringValue("CmsApiUsername"));
                i.password = Helper.EncodeToBase64String(WebConfig.GetStringValue("CmsApiPassword"));

                authRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);
                string encodedBody = string.Format("username={0}&password= {1}&grant_type={2}", i.username, i.password, "password");
                authRequest.AddParameter("application/x-www-form-urlencoded", encodedBody, ParameterType.RequestBody);

                System.Web.HttpContext.Current.Trace.Warn("encodedBody", encodedBody);

                System.Web.HttpContext.Current.Trace.Warn("authResponse", "b4 authResponse");

                var authResponse = restClient.Execute(authRequest);

                System.Web.HttpContext.Current.Trace.Warn("authResponse", "after authResponse");

                var Content = authResponse.Content;

                System.Web.HttpContext.Current.Trace.Warn("Content=", Content);
                System.Web.HttpContext.Current.Trace.Warn("StatusCode=", authResponse.StatusCode.ToString());

                if (authResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.BalTokenResponse>(Content);
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetBalToken Error", ex.ToString());
                if (ex.InnerException != null)
                    System.Web.HttpContext.Current.Trace.Warn("GetBalToken InnerException", ex.InnerException.ToString());

                throw ex;
            }
            return tokenResponse;
        }
        public static Models.TokenResponse GetBalTokenNew()
        {
            Models.TokenResponse tokenResponse = null;
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                System.Web.HttpContext.Current.Trace.Warn("BalApiUrl", WebConfig.GetStringValue("BalApiUrl"));

                RestClient restClient = new RestClient(WebConfig.GetStringValue("BalApiUrl"));

                var authRequest = new RestRequest("/api/CmsApi/GetToken", Method.POST) { RequestFormat = DataFormat.Json };

                authRequest.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);

                var TokenClientId = Helper.EncryptStringAES(WebConfig.GetStringValue("TokenClientId"));
                var TokenUserName = Helper.EncryptStringAES(WebConfig.GetStringValue("TokenUserName"));
                var TokenPassword = Helper.EncryptStringAES(WebConfig.GetStringValue("TokenPassword"));

                string encodedBody = string.Format("clientId={0}&username={1}&password={2}", TokenClientId, TokenUserName, TokenPassword);
                authRequest.AddParameter("application/x-www-form-urlencoded", encodedBody, ParameterType.RequestBody);

                System.Web.HttpContext.Current.Trace.Warn("encodedBody", encodedBody);

                System.Web.HttpContext.Current.Trace.Warn("authResponse", "b4 authResponse");

                var authResponse = restClient.Execute(authRequest);

                System.Web.HttpContext.Current.Trace.Warn("authResponse", "after authResponse");

                var Content = authResponse.Content;

                System.Web.HttpContext.Current.Trace.Warn("Content=", Content);
                System.Web.HttpContext.Current.Trace.Warn("StatusCode=", authResponse.StatusCode.ToString());

                if (authResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.TokenResponse>(Content);
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Trace.Warn("GetBalToken Error", ex.ToString());
                if (ex.InnerException != null)
                    System.Web.HttpContext.Current.Trace.Warn("GetBalToken InnerException", ex.InnerException.ToString());

                throw ex;
            }
            return tokenResponse;
        }
    }
}