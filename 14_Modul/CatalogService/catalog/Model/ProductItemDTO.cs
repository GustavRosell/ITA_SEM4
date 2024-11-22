using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Model;

public enum ProductCategory
{
    None = 0,
    Vegetables = 1,
    Fruit = 2,
    Dairy = 3,
    Meat = 4,
    Poultry = 5,
    Bakery = 6,
    Desserts = 7,
    Beverages = 8
}

[BsonDiscriminator("productitem")]
public class ProductItemDTO
{
    [BsonId]
    public Guid? ProductId { get; set; }
    public ProductCategory ProductCategory { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<Uri> Images { get; set; } = new List<Uri>();
}