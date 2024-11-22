using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Catalog.Model;

namespace Catalog.Services;

/// <summary>
/// MongoDB database context class.
/// </summary>
public class MongoDBContext
{
    public IMongoDatabase Database { get; set; }
    public IMongoCollection<ProductItemDTO> Collection { get; set; }

    /// <summary>
    /// Create an instance of the context class.
    /// </summary>
    /// <param name="logger">Global logging facility.</param>
    /// <param name="config">System configuration instance.</param>
    public MongoDBContext(ILogger<CatalogMongoDBService> logger, IConfiguration config)
    {        
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        var client = new MongoClient(config["MongoConnectionString"]);
        Database = client.GetDatabase(config["CatalogDatabase"]);
        Collection = Database.GetCollection<ProductItemDTO>(config["CatalogCollection"]);

        logger.LogInformation($"Connected to database {config["CatalogDatabase"]}");
        logger.LogInformation($"Using collection {config["CatalogCollection"]}");
    }

}
