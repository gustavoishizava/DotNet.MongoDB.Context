# DotNet.MongoDB.Context

## Getting Started

Install the [Nuget](https://www.nuget.org/packages/Ishizava.MongoDB.Context) package.

```
PM> Install-Package Ishizava.MongoDB.Context
```

```
> dotnet add package Ishizava.MongoDB.Context
```


## Creating an entity

```csharp
    public class Customer
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }
```

## Create mapping
```csharp
    public class CustomerMap : BsonClassMapConfiguration
    {
        public CustomerMap() : base("customers")
        {
        }

        public override BsonClassMap GetConfiguration()
        {
            var map = new BsonClassMap<Customer>();

            map.MapIdField(x => x.Id);

            map.MapProperty(x => x.Name)
                .SetElementName("full_name");

            map.MapProperty(x => x.CreatedAt)
                .SetElementName("created_at");

            return map;
        }
    }
```

## Creating an DbContext

```csharp
    public class MyDbContext : DbContext
    {
        public MyDbContext(MongoDbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
    }
```


## Configuring dependecy injection

```csharp
services.AddMongoDbContext<MyDbContext>(options =>
{
    options.ConfigureConnection("your_connection_string", "your_database");

    options.AddSerializer(new GuidSerializer(BsonType.String));
    options.AddSerializer(new DecimalSerializer(BsonType.Decimal128));

    options.AddBsonClassMap(new CustomerMap());
});
```


## Example with dependency injection

```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CustomersController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync()
        {
            _context.StartTransaction();

            var customer = new Customer
            {
                Id = ObjectId.GenerateNewId(),
                Name = "Gustavo Ishizava",
                CreatedAt = DateTime.Now
            };

            await _context.Customers.AddAsync(customer);

            await _context.CommitAsync();

            var find = await _context.Customers.Collection
                                .Find(Builders<Customer>.Filter.Where(x => x.Id == customer.Id))
                                .FirstOrDefaultAsync();

            return Ok(find);
        }
    }
```
