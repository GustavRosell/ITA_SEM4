using Microsoft.AspNetCore.Mvc.RazorPages;
using Catalog.Model;
namespace MyApp.Namespace
{
    public class CatalogListModel : PageModel
    {
        private readonly IHttpClientFactory? _clientFactory = null;
        public List<ProductItemDTO>? Products { get; set; }
        public CatalogListModel(IHttpClientFactory clientFactory)
        => _clientFactory = clientFactory;
        public void OnGet()
        {
            using HttpClient? client = _clientFactory?.CreateClient("gateway");
            try
            {
                Products = client?.GetFromJsonAsync<List<ProductItemDTO>>(
                "catalog/GetProductsByCategory?category=1").Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}