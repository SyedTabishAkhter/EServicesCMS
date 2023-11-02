using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Security.Claims;
using EServicesCms.Identity;
//using System.IdentityModel.Services;
using System.Security.Authentication;
using System.Xml;

namespace EServicesCms.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;

        public AuthController()
        {
            config = IdentityConfig.Saml2Configuration;
        }
        public ActionResult Login(string language = "ar")
        {
            //System.Web.HttpCookie cookie  = new System.Web.HttpCookie("language");
            //cookie.Value = language;
            //Request.Cookies.Set(cookie);
            //Response.AppendCookie(cookie);
            //Response.Headers["set-cookie"] = "language=" + language;
            //Response.Headers["set-cookie"] = "language=" + language;
            //
            HttpContext.Trace.Warn("Auth", "Inside Auth Login");

            HttpContext.Trace.Warn("config", Newtonsoft.Json.JsonConvert.SerializeObject(config));

            var binding = new Saml2RedirectBinding();

            HttpContext.Trace.Warn("Auth", "Before Bind");

            return binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();
        }
        public ActionResult AssertionConsumerService()
        {
            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(config);

            binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
            }
            binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            saml2AuthnResponse.CreateSession(claimsAuthenticationManager: new DefaultClaimsAuthenticationManager());

            var relayStateQuery = binding.GetRelayStateQuery();
            var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");
            return Redirect(returnUrl);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var binding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(config, ClaimsPrincipal.Current).DeleteSession();
            return binding.Bind(logoutRequest).ToActionResult();
        }
        public ActionResult LoggedOut()
        {
            var binding = new Saml2PostBinding();
            binding.Unbind(Request.ToGenericHttpRequest(), new Saml2LogoutResponse(config));

            //FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            //FederatedAuthentication.SessionAuthenticationModule.SignOut();

            return Redirect(Url.Content("~/"));
        }
        public ActionResult SingleLogout()
        {
            Saml2StatusCodes status;
            var requestBinding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(config, ClaimsPrincipal.Current);
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), logoutRequest);
                status = Saml2StatusCodes.Success;
                logoutRequest.DeleteSession();
            }
            catch (Exception exc)
            {
                // log exception
                Debug.WriteLine("SingleLogout error: " + exc.ToString());
                status = Saml2StatusCodes.RequestDenied;
            }

            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = requestBinding.RelayState;
            var saml2LogoutResponse = new Saml2LogoutResponse(config)
            {
                InResponseToAsString = logoutRequest.IdAsString,
                Status = status,
            };
            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }
    }
}