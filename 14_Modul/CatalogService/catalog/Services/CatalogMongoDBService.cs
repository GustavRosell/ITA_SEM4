using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Catalog.Model;

namespace Catalog.Services;

/// <summary>
/// Interface definition for the DB service to access the catalog data.
/// </summary>
public interface ICatalogService
{
    Task<ProductItemDTO?> GetProductItem(Guid productId);
    Task<IEnumerable<ProductItemDTO>?> GetProductItemListByCategory(ProductCategory category);    
    Task<Guid?> AddProductItem(ProductItemDTO item);
    Task<long> AddImageToProductItem(Guid productId, Uri imageURI);
}

/// <summary>
/// MongoDB repository service
/// </summary>
public class CatalogMongoDBService : ICatalogService
{
    private ILogger<CatalogMongoDBService> _logger;
    private IConfiguration _config;
    private IMongoDatabase _database;
    private IMongoCollection<ProductItemDTO> _collection;

    /// <summary>
    /// Creates a new instance of the CatalogMongoDBService.
    /// </summary>
    /// <param name="logger">The commun logger facility instance</param>
    /// <param name="config">Systemm configuration instance</param>
    /// <param name="dbcontext">The database context to be used for accessing data.</param>
    public CatalogMongoDBService(ILogger<CatalogMongoDBService> logger, 
            IConfiguration config, MongoDBContext dbcontext)
    {
        _logger = logger;
        _config = config;
        _database = dbcontext.Database;        
        _collection = dbcontext.Collection;
    }

    /// <summary>
    /// Retrieves a product item based on its unique id.
    /// </summary>
    /// <param name="productId">The products unique id</param>
    /// <returns>The products item requested.</returns>
    public async Task<ProductItemDTO?> GetProductItem(Guid productId)
    {
        ProductItemDTO? product = null;
        var filter = Builders<ProductItemDTO>.Filter.Eq(x => x.ProductId, productId);
        
        try
        {
            product = await _collection.Find(filter).SingleOrDefaultAsync();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return product;
    }

    /// <summary>
    /// List product items in catalog within a specific category.
    /// </summary>
    /// <param name="category">The category identifier.</param>
    /// <returns>A list of products</returns>
    public async Task<IEnumerable<ProductItemDTO>?> GetProductItemListByCategory(ProductCategory category)
    {
        var filter = Builders<ProductItemDTO>.Filter.Eq(x => x.ProductCategory, category);
        IEnumerable<ProductItemDTO>? products = null;

        try
        {
            products = await _collection.Find(filter).ToListAsync<ProductItemDTO>();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return products;
    }

    /// <summary>
    /// Add a new Product Item to the database.
    /// </summary>
    /// <param name="item">Product to add to the catalog/param>
    /// <returns>Product with updated Id</returns>
    public async Task<Guid?> AddProductItem(ProductItemDTO item)
    {
        item.ProductId = Guid.NewGuid();
        await _collection.InsertOneAsync(item);
        return item.ProductId;
    }

    /// <summary>
    /// Append an image URI to the Images list in a ProductItem and persists to database.
    /// </summary>
    /// <param name="productId">The products unique ID</param>
    /// <param name="uri">the absolute URI of the image</param>
    /// <returns>Number of items updated.</returns>
    public async Task<long> AddImageToProductItem(Guid productId, Uri uri)
    {
        var filter = Builders<ProductItemDTO>.Filter.Eq("_id", productId.ToString());
        var update = Builders<ProductItemDTO>.Update.AddToSet("Images", uri);

        var res = await _collection.UpdateOneAsync(filter, update);

        return res.ModifiedCount;
    }

}
