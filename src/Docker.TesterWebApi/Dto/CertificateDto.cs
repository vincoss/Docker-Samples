using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Docker.TesterWebApi.Dto
{
    public class CertificateDto
    {
        public string StoreName { get; set; }
        public string StoreLocation { get; set; }
        public string Subject { get; set; }
        public string Issuer { get; set; }
        public string IssuerName { get; set; }
        public string FriendlyName { get; set; }
        public bool HasPrivateKey { get; set; }

        public static CertificateDto Map(X509Store store, X509Certificate2 cert)
        {
            return new CertificateDto
            {
                StoreName = store.Name,
                StoreLocation = store.Location.ToString(),
                Subject = cert.Subject,
                Issuer =  cert.Issuer,
                IssuerName = cert.IssuerName.ToString(),
                FriendlyName = cert.FriendlyName,
                HasPrivateKey = cert.HasPrivateKey
            };
        }
    }
}
