using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services, Action<MongoDbContextOptions> options) where TContext : DbContext
        {
            var mongoDbContextOptions = new MongoDbContextOptions();
            options(mongoDbContextOptions);

            var client = new MongoClient(mongoDbContextOptions.ConnectionString);

            services.AddSingleton<IMongoClient>(client)
                    .AddSingleton<IMongoDatabase>(client.GetDatabase(mongoDbContextOptions.DatabaseName));

            services.AddSingleton(mongoDbContextOptions)
                    .AddScoped<TContext>();

            return services;
        }
    }
}