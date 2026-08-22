using Default_AspNetCoreWebApiSdk;
using Microsoft.AspNetCore.Mvc;


namespace Default_AspNetCoreWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public  IActionResult Get()
        {
            var products = ProductService.GetProducts();
            return Ok(products);
        }
    }
}
