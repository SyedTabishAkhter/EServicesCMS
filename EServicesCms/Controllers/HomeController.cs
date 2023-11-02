using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Web.Routing;
using System.Xml.Serialization;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
//using System.IdentityModel.Services;
using System.Security.Authentication;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Mvc;
using EServicesCms.Models;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.Entity;

namespace EServicesCms.Controllers
{
    public class HomeController : Controller
    {
        private const string authorizationServerTokenAddress = "/OAuth/Token";
        private readonly Saml2Configuration config;

        [Authorize]
        public ActionResult Secure()
        {
            // The NameIdentifier
            var nameIdentifier = ClaimsPrincipal.Current.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single();

            return View();
        }
        public HomeController()
        {
            config = IdentityConfig.Saml2Configuration;
        }

        public ActionResult SamlConsume(string SAMLResponse)
        {
            string redirectUrl = string.Empty;
            string username = string.Empty;
            string useremail = string.Empty;
            string userdisplayname = string.Empty;
            string mobile = string.Empty;
            string samltoken = string.Empty;
            string statusCode = string.Empty;
            string responseTo = string.Empty;
            string nameId = string.Empty;
            string loginType = string.Empty;
            string userType = string.Empty;
            Models.BaseResponse iResponse = new Models.BaseResponse();
            Models.MOFPortalEntities iDbContext = new Models.MOFPortalEntities();
            try
            {
                HttpContext.Trace.Warn("SAMLResponse", SAMLResponse);

                if (string.IsNullOrEmpty(SAMLResponse) == false)
                {
                    string responseXml = Common.Helper.DecodeFromBase64String(SAMLResponse);

                    //string responseXml = @"<?xml version='1.0' encoding='UTF-8'?><samlp:Response xmlns:samlp='urn:oasis:names:tc:SAML:2.0:protocol' Destination='https://eservicesportal-uat.mof.gov.ae/EServicesCms/Home/SamlConsume' ID='_b52ca015b87496e22c11bbbdcac49864' IssueInstant='2023-02-21T07:40:07.608Z' Version='2.0'><saml:Issuer xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion'>https://myplace.mof.gov.ae/SAAS/API/1.0/GET/metadata/idp.xml</saml:Issuer><ds:Signature xmlns:ds='http://www.w3.org/2000/09/xmldsig#'><ds:SignedInfo><ds:CanonicalizationMethod Algorithm='http://www.w3.org/2001/10/xml-exc-c14n#'/><ds:SignatureMethod Algorithm='http://www.w3.org/2000/09/xmldsig#rsa-sha1'/><ds:Reference URI='#_b52ca015b87496e22c11bbbdcac49864'><ds:Transforms><ds:Transform Algorithm='http://www.w3.org/2000/09/xmldsig#enveloped-signature'/><ds:Transform Algorithm='http://www.w3.org/2001/10/xml-exc-c14n#'/></ds:Transforms><ds:DigestMethod Algorithm='http://www.w3.org/2000/09/xmldsig#sha1'/><ds:DigestValue>a7Q9rZfdlb8EcZCrOR5xPuBY8lc=</ds:DigestValue></ds:Reference></ds:SignedInfo><ds:SignatureValue>hL0bxiTeMmbwTS+EwASAFmmuvTw37S+N5fVJQ7IhA/G/NdFAjRYx0yZ3Wo11CLYG1v8LPZ6g+Ui8dwI3BYu4tRoFNsfuob9Hx5hNCUfFgRELlLZPRunDkwr1UVUnbgPd3OJNi+xMxLAP4sJM7ml25gZOWl6Y06NzoRayShoqrd1KHTWmBbgEwLdF4ibvrklXhczprBdnNHCob0Ip8tRv4ePftJREdQRm5Ecl8/k/OPr0+zk92H9ET0wOru90llgOtQVQGDDxu96SbsisRvw7nxGl1NT9OF8rKROAvJRAnvzjCq9Ibw7wmNESPoQpPrvdyCP7Nca4kORuepYE7rAdWH9HDdPDeUBoTkeZxWjwLgGvEQ7NTDaDxqswIXdMBV04qSVC896KWhJB12WHW8ScPm2EQQ0N/lWlNeo3ZHL+qp4hbRWl/MQbxBxDKEIu55GxaRvpGGoegdUN5F+8YGjXIPX/Nnd/IQg5sEK5zdeloHtHvq+ce+zoy8zZha6NVVKUzuIHbzDOabhzDkk7p3wiShurRwhVWbL00VyCQ9m6iAJx7pGQ8V7Jn3uQQKM5ePX4q7zlkwQj6cvGCIVpWMEJmk84+PWJomOee77jSPZYd3wVBZMBnC5ATY3bgWsSiz7+unfEgcxMIoYGNnQK9oA1S1VhxQvHa4pMBvcKulr/Neo=</ds:SignatureValue></ds:Signature><samlp:Status><samlp:StatusCode Value='urn:oasis:names:tc:SAML:2.0:status:Success'/></samlp:Status><saml:Assertion xmlns:saml='urn:oasis:names:tc:SAML:2.0:assertion' ID='_afe9d626261656811465b9f669e3f7a6' IssueInstant='2023-02-21T07:40:07.608Z' Version='2.0'><saml:Issuer>https://myplace.mof.gov.ae/SAAS/API/1.0/GET/metadata/idp.xml</saml:Issuer><saml:Subject><saml:NameID Format='urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified' NameQualifier='https://myplace.mof.gov.ae/SAAS/API/1.0/GET/metadata/idp.xml'>aasaid</saml:NameID><saml:SubjectConfirmation Method='urn:oasis:names:tc:SAML:2.0:cm:bearer'><saml:SubjectConfirmationData NotOnOrAfter='2023-02-21T07:43:27.608Z' Recipient='https://eservicesportal-uat.mof.gov.ae/EServicesCms/Home/SamlConsume'/></saml:SubjectConfirmation></saml:Subject><saml:Conditions NotBefore='2023-02-21T07:39:52.608Z' NotOnOrAfter='2023-02-21T07:43:27.608Z'><saml:AudienceRestriction><saml:Audience>https://eservicesportal-uat.mof.gov.ae/EServicesCms/Auth/Login</saml:Audience></saml:AudienceRestriction></saml:Conditions><saml:AuthnStatement AuthnInstant='2023-02-21T07:40:07.608Z' SessionIndex='_461a37e67c97baed5831655b17315bb9'><saml:AuthnContext><saml:AuthnContextClassRef>urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport</saml:AuthnContextClassRef></saml:AuthnContext></saml:AuthnStatement><saml:AttributeStatement><saml:Attribute Name='username' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified'><saml:AttributeValue xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:type='xsd:string'>aasaid</saml:AttributeValue></saml:Attribute><saml:Attribute Name='useremail' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified'><saml:AttributeValue xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:type='xsd:string'>aasaid@mof.gov.ae</saml:AttributeValue></saml:Attribute><saml:Attribute Name='firstname' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified'><saml:AttributeValue xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:type='xsd:string'>Amr</saml:AttributeValue></saml:Attribute><saml:Attribute Name='lastname' NameFormat='urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified'><saml:AttributeValue xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xsi:type='xsd:string'>Abd ElHamid Said</saml:AttributeValue></saml:Attribute></saml:AttributeStatement></saml:Assertion></samlp:Response>";

                    HttpContext.Trace.Warn("responseXml", responseXml);
                    //
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(responseXml);
                    //
                    XmlNamespaceManager xMan = new XmlNamespaceManager(xDoc.NameTable);
                    xMan.AddNamespace("q1", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xMan.AddNamespace("ns1", "urn:oasis:names:tc:SAML:2.0:assertion");
                    //
                    XmlNode xNode = xDoc.SelectSingleNode("q1:Response/q1:Status/q1:StatusCode/@Value", xMan);
                    if (xNode != null)
                    {
                        statusCode = xNode.Value;
                        if (string.IsNullOrEmpty(statusCode) == false && statusCode.ToUpper().Contains("SUCCESS") == false)
                            throw new ApplicationException("Oops ! something went wrong. Saml Response Status is NOT success.");
                    }
                    xNode = xDoc.SelectSingleNode("q1:Response/@InResponseTo", xMan);
                    if (xNode != null)
                    {
                        responseTo = xNode.Value;
                    }
                    //
                    DateTime expirationDate = DateTime.Now.AddHours(5);
                    XmlNode node = xDoc.SelectSingleNode("q1:Response/ns1:Assertion[1]/ns1:Subject/ns1:SubjectConfirmation/ns1:SubjectConfirmationData", xMan);
                    if (node != null && node.Attributes["NotOnOrAfter"] != null)
                    {
                        DateTime.TryParse(node.Attributes["NotOnOrAfter"].Value, out expirationDate);
                    }
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'username']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        username = xNode.InnerText;
                    }
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'loginType']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        loginType = xNode.InnerText;
                    }
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'userType']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        userType = xNode.InnerText;
                    }
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'useremail']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        useremail = xNode.InnerText;
                    }
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'userdisplayname']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        userdisplayname = xNode.InnerText;
                    }
                    string firstName = "";
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'firstname']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        firstName = xNode.InnerText;
                    }
                    string lastname = "";
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'lastname']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        lastname = xNode.InnerText;
                    }
                    userdisplayname = firstName + " " + lastname;
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'mobile']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        mobile = xNode.InnerText;
                    }
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:AttributeStatement/ns1:Attribute[@Name = 'token']/ns1:AttributeValue", xMan);
                    if (xNode != null)
                    {
                        samltoken = xNode.InnerText;//helpdesk api header value AccessToken
                    }
                    //
                    xNode = xDoc.SelectSingleNode("q1:Response/ns1:Assertion/ns1:Subject/ns1:NameID", xMan);
                    if (xNode != null)
                    {
                        nameId = xNode.InnerText;
                    }
                    //
                    HttpContext.Trace.Warn("Before check User in cms db", username);
                    HttpContext.Trace.Warn("Before check User in cms db", useremail);
                    //
                    var isUserInCms = iDbContext.Users.Where(x => x.UserName == username || x.UserName == useremail).FirstOrDefault();
                    if (isUserInCms != null)
                    {
                        //HttpContext.Trace.Warn("User details in cms db", Newtonsoft.Json.JsonConvert.SerializeObject(isUserInCms));

                        if (isUserInCms.IsActive == false)
                        {
                            iResponse.Status = (int)EServicesCms.Common.UserStatus.InActive;
                            //iResponse.Message = "Sorry ! Your account " + userdisplayname + " is In-Active. Please contact administrator.";
                            iResponse.Message = userdisplayname + " " + Common.DbManager.GetText("Login", "lblSSo1", "Sorry! You do not have sufficient privilage to access the system.");
                            return View("About", iResponse);
                        }
                        else if(isUserInCms.IsDeleted == true)
                        {
                            iResponse.Status = (int)EServicesCms.Common.UserStatus.Deleted;
                            //iResponse.Message = "Sorry ! Your account " + userdisplayname + " is deleted. Please contact administrator.";
                            iResponse.Message = userdisplayname + " " + Common.DbManager.GetText("Login", "lblSSo2", "Sorry! Your account is deleted. Please contact administrator.");
                            return View("About", iResponse);
                        }
                        else
                        {
                            iResponse.Status = (int)EServicesCms.Common.UserStatus.Active;
                            //iResponse.Message = "Kudos ! " + userdisplayname + " you have been authenticated successfully. Please wait....";
                            iResponse.Message = userdisplayname + " " + Common.DbManager.GetText("Login", "lblSSo3", "Kudos! you have been authenticated successfully. Please wait.");

                            var iRoleGropus = Common.DbManager.GetLkRoleObject(isUserInCms.RoleId.Value);

                            Session["iUser"] = isUserInCms;
                            Session["iUserRoleGroups"] = iRoleGropus;

                            if (Common.Security.isUserAuthorized("MOF_ESRV_SYSTEM") == false)
                            {
                                iResponse.Status = (int)EServicesCms.Common.UserStatus.UnAuthorized;
                                //iResponse.Message = "Sorry ! " + userdisplayname + " you do not have sufficient privilage to access the system.<br /> Please contact administrator.";
                                iResponse.Message = userdisplayname + " " + Common.DbManager.GetText("Login", "lblSSo4", "Sorry! you do not have sufficient privilage to access the system.<br /> Please contact administrator.");
                                return View("About", iResponse);
                            }

                            Session["iClientIpAddress"] = GetClientIp();
                            Session["LoginAt"] = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");

                            if (Common.WebConfig.GetBoolValue("LogCmsAction2AuditTrial"))
                            {
                                var aut = new Models.UserScreenAction();
                                aut.ActionId = 7;
                                aut.ServiceId = null;
                                aut.CategoryId = null;
                                aut.UniqueId = isUserInCms.UserName;
                                aut.Remarks = isUserInCms.UserName + " LOGGED-IN to MofEServicesCms Portal";
                                aut.RowInsertDate = DateTime.Now;
                                aut.IpAddress = GetClientIp();
                                aut.RowInsertedBy = isUserInCms.UserName;
                                aut.IsDeleted = false;
                                iDbContext.UserScreenActions.AddOrUpdate(aut);
                                iDbContext.SaveChanges();
                            }
                            //redirectUrl = EServicesCms.Common.Helper.URL_Encode(Url.Action("Index", "Dashboard", null, Request.Url.Scheme, null) + "?id=1");
                            //Response.Redirect(redirectUrl, true);
                        }
                    }
                    else
                    {
                        iResponse.Status = (int)EServicesCms.Common.UserStatus.UnAuthorized;
                        //iResponse.Message = "Sorry ! " + userdisplayname + " you do not have sufficient privilage to access the system. <br /> Please contact sytem administrator.";
                        iResponse.Message = userdisplayname + " " + Common.DbManager.GetText("Login", "lblSSo44", "Sorry! you do not have sufficient privilage to access the system.<br /> Please contact administrator.");
                        return View("About", iResponse);
                    }
                }
                else
                {
                    iResponse.Status = (int)EServicesCms.Common.UserStatus.UnAuthorized;
                    iResponse.Message = Common.DbManager.GetText("Login", "lblSSo5", "Sorry ! EmptyOrNull Response Received from SSO. Please contact sytem administrator.");
                    return View("About", iResponse);
                }
            }
            catch (Exception Exp)
            {
                HttpContext.Trace.Warn("ERROR", Exp.ToString());
                throw Exp;
            }
            finally
            {
                iDbContext.Dispose();
            }
            return View("About", iResponse);
        }
        private string GetClientIp()
        {
            string ip = string.Empty;
            try
            {
                ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    string[] ipRange = ip.Split(',');
                    string trueIP = ipRange[0].Trim();
                }
                else
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception exp)
            {
                ip = "127.0.0.1";
            }
            return ip;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}