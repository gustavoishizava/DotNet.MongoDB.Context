using DotNet.MongoDB.Context.Configuration;
using DotNet.MongoDB.Context.Context;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services, Action<MongoDbContextOptions> options) where TContext : DbContext
        {
            var mongoDbContextOptions = new MongoDbContextOptions();
            options(mongoDbContextOptions);

            ApplyBsonMaps(mongoDbContextOptions);
            ApplySerializers(mongoDbContextOptions);
            ApplyConventions(mongoDbContextOptions);

            var client = new MongoClient(mongoDbContextOptions.ConnectionString);

            services.AddSingleton<IMongoClient>(client)
                    .AddSingleton<IMongoDatabase>(client.GetDatabase(mongoDbContextOptions.DatabaseName));

            services.AddSingleton(mongoDbContextOptions)
                    .AddScoped<TContext>();

            return services;
        }

        private static void ApplyBsonMaps(MongoDbContextOptions options)
            => options.BsonClassMapConfigurations.ToList().ForEach(x => x.Apply());

        private static void ApplySerializers(MongoDbContextOptions options)
        {
            foreach (var serializer in options.Serializers)
            {
                var type = serializer.GetType().BaseType.GenericTypeArguments[0];
                BsonSerializer.RegisterSerializer(type, serializer);
            }
        }

        private static void ApplyConventions(MongoDbContextOptions options)
            => ConventionRegistry.Register("Conventions", options.ConventionPack, _ => true);
    }
}