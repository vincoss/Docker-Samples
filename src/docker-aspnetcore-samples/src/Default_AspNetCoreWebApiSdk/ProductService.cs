namespace Default_AspNetCoreWebApiSdk
{
    public class ProductService
    {
        public static List<string> GetProducts()
        {
            return new List<string> { "Laptop", "Mouse", "Keyboard" };
        }
    }
}
