using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Catalog.Model;
using Catalog.Services;

namespace Catalog.Controllers;

/// <summary>
/// The CatalogController implements the HTTP interface for accessing
/// the product items catalog from a food business.
/// </summary>
[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ILogger<CatalogController> _logger;
    private string _imagePath = string.Empty;
    private readonly ICatalogService _dbService;

    /// <summary>
    /// Create an instance of the Catalog controller.
    /// </summary>
    /// <param name="logger">Global logging instance</param>
    /// <param name="configuration">Global configuration instance</param>
    /// <param name="dbservice">Database respository</param>
    public CatalogController(ILogger<CatalogController> logger, 
        IConfiguration configuration, ICatalogService dbservice)
    {
        _logger = logger;
        _imagePath = configuration["CatalogImagePath"]!;
        _dbService = dbservice;
    }

    /// <summary>
    /// Service version endpoint. 
    /// Fetches metadata information, through reflection from the service assembly.
    /// </summary>
    /// <returns>All metadata attributes from assembly in text string</returns>
    [HttpGet("version")]
    public Dictionary<string,string> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;

        properties.Add("service", "Catalog");
        var ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).ProductVersion ?? "Undefined";
        properties.Add("version", ver);

        var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
        var localIPAddr = feature?.LocalIpAddress?.ToString() ?? "N/A";
        properties.Add("local-host-address", localIPAddr);

        return properties;
    }

    [HttpGet("GetProduct")]
    public async Task<ProductItemDTO?> GetProduct(Guid productId)
    {
        _logger.LogInformation($"Request for product with guid: {productId}");

        ProductItemDTO? result = await _dbService.GetProductItem(productId);

        return result;
    }

    [HttpGet("GetProductsByCategory")]
    public async Task<IEnumerable<ProductItemDTO>?> GetProductsByCategory(ProductCategory category)
    {
        _logger.LogInformation($"Request for products in category: {category}");

        return await _dbService.GetProductItemListByCategory(category);

    }

    [HttpPost("CreateProduct")]
    public Task<Guid?> CreateProduct(ProductItemDTO dto)
    {
        return _dbService.AddProductItem(dto);
    }

    [HttpPost("AddImage"), DisableRequestSizeLimit]
    public async Task<IActionResult> UploadImage()
    {
        List<Uri> images = new List<Uri>();

        try
        {
            var formId = Request.Form["guid"];

            if (String.IsNullOrEmpty(formId))
            {
                return BadRequest("The product id could not be identified.");
            }

            Guid productId = new Guid(formId!);
            ProductItemDTO? result = await _dbService.GetProductItem(productId);

            if(result != null)
            {
                foreach (var formFile in Request.Form.Files) 
                {
                    if (formFile.Length > 0)
                    {
                        var fileName = "image-" + Guid.NewGuid().ToString() + ".jpg";
                        var fullPath = _imagePath + Path.DirectorySeparatorChar + fileName;
                        _logger.LogInformation($"Saving file {fullPath}");

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            formFile.CopyTo(stream);
                        }

                        var imageURI = new Uri(fileName, UriKind.RelativeOrAbsolute);

                        if (await _dbService.AddImageToProductItem(productId, imageURI) > 0)
                        {
                            images.Add(imageURI);
                        }
                    } 
                    else 
                    {
                        return BadRequest("Empty file submited.");
                    }
                }
            }
            else
            {
                return StatusCode(404, $"Product not found in catalog");
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("Upload image faillure {}", ex);
            return StatusCode(500, $"Internal server error.");
        }

        return Ok(images);
    }

}
