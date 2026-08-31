using Docker.TesterWebApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


namespace Docker.TesterWebApi.Controllers
{
    [ApiController]
    [Route("api/certificate")]
    public class CertificateController : ControllerBase
    {
        private readonly IOptionsSnapshot<AppSettings> _options;

        public CertificateController(IOptionsSnapshot<AppSettings> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _options = options;
        }


        [HttpGet]
        public IActionResult Get()
        {
            var items = new List<dynamic>();
            var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var certificates = store.Certificates;
            foreach (var certificate in certificates)
            {
                items.Add(CertificateDto.Map(store, certificate));
            }

            if (items.Any())
            {
                return Ok(items);
            }

            return Ok(new
            {
                StoreName = store.Name,
                StoreLocation = store.Location,
            });
        }

        [HttpGet]
        [Route("createPfx")]
        public IActionResult CreatePfx(string id)
        {
            var ba = Utils.ReadResource("X509Sample.pfx.txt");
            var pwd = "Pass@word1";

            using (var certificate = new X509Certificate2(ba, pwd, X509KeyStorageFlags.DefaultKeySet | X509KeyStorageFlags.PersistKeySet))
            using (var store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                store.Close();
            }

            return Ok("Success");
        }

        [HttpGet]
        [Route("install")]
        public IActionResult Install(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = "X509Sample.pfx";
            }
            var pwd = "Pass@word1";
            var path = Path.Combine(_options.Value.DataPath, id);

            using (var certificate = new X509Certificate2(path, pwd, X509KeyStorageFlags.DefaultKeySet))
            using (var store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(certificate);
                store.Close();
            }

            return Ok("Success");
        }

        [HttpGet]
        [Route("findCertificate")]
        public IActionResult FindCertificate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = "test";
            }

            using (var store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2 cert = store.Certificates.OfType<X509Certificate2>().AsEnumerable().FirstOrDefault(c => c.Subject.Contains(id, StringComparison.OrdinalIgnoreCase));

                if (cert != null)
                {
                    var dto = CertificateDto.Map(store, cert);
                    return Ok(dto);
                }
            }

            return Ok($"Not found: {id}");
        }
    }
}
