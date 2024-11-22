using Microsoft.Extensions.FileProviders;
using Catalog.Services;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Create globally availabel HttpClient for accesing the gateway.
var gatewayUrl = builder.Configuration["GatewayUrl"] ?? "http://localhost:4000";
builder.Services.AddHttpClient("gateway", client =>
{
    client.BaseAddress = new Uri(gatewayUrl);
    client.DefaultRequestHeaders.Add(
    HeaderNames.Accept, "application/json");
});

// Add internal services to the application.
builder.Services.AddSingleton<MongoDBContext>();
builder.Services.AddScoped<ICatalogService, CatalogMongoDBService>();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Setup application to handle upload of static content (images)
// to a predetermined folder
var imagePath = builder.Configuration["CatalogImagePath"] ?? "/tmp/imagebucket";
Console.WriteLine("Image Path: " + imagePath);
var fileProvider = new PhysicalFileProvider(Path.GetFullPath(imagePath));
var requestPath = new PathString("/catalog/images");
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});

Console.WriteLine("File Provider Root: " + fileProvider.Root);

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "The Food Catalog Service");
app.MapRazorPages();

app.Run();
