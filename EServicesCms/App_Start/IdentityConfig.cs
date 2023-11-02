using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;
using ITfoxtec;
using System;
using System.Configuration;
using System.IdentityModel.Claims;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Helpers;

namespace EServicesCms
{
    public static class IdentityConfig
    {
        public static Saml2Configuration Saml2Configuration { get; private set; } = new Saml2Configuration();

        public static void RegisterIdentity()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            Saml2Configuration.Issuer = ConfigurationManager.AppSettings["Saml2:Issuer"];
            //Saml2Configuration.SingleSignOnDestination = new Uri(ConfigurationManager.AppSettings["Saml2:SingleSignOnDestination"]);
            //Saml2Configuration.SingleLogoutDestination = new Uri(ConfigurationManager.AppSettings["Saml2:SingleLogoutDestination"]);

            Saml2Configuration.SignatureAlgorithm = ConfigurationManager.AppSettings["Saml2:SignatureAlgorithm"];
            //Saml2Configuration.SignAuthnRequest = true;
            Saml2Configuration.SigningCertificate = CertificateUtil.Load(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Saml2:SigningCertificateFile"]), ConfigurationManager.AppSettings["Saml2:SigningCertificatePassword"]);
            //Saml2Configuration.SignatureValidationCertificates.Add(CertificateUtil.Load(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Saml2:SignatureValidationCertificate"])));

            Saml2Configuration.CertificateValidationMode = (X509CertificateValidationMode)Enum.Parse(typeof(X509CertificateValidationMode), ConfigurationManager.AppSettings["Saml2:CertificateValidationMode"]);
            Saml2Configuration.RevocationMode = (X509RevocationMode)Enum.Parse(typeof(X509RevocationMode), ConfigurationManager.AppSettings["Saml2:RevocationMode"]);

            Saml2Configuration.AllowedAudienceUris.Add(Saml2Configuration.Issuer);

            var entityDescriptor = new EntityDescriptor();
            entityDescriptor.ReadIdPSsoDescriptorFromFile(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["Saml2:IdPMetadata"]));
            if (entityDescriptor.IdPSsoDescriptor != null)
            {
                Saml2Configuration.AllowedIssuer = entityDescriptor.EntityId;
                Saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                Saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                Saml2Configuration.SignAuthnRequest = true;

            }
            else
            {
                throw new Exception("IdPSsoDescriptor not loaded from metadata.");
            }
        }
    }
}