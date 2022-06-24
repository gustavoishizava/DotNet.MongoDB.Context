using MeuBolsoDigital.MongoDB.Context.Configuration;
using MeuBolsoDigital.MongoDB.Context.Context;
using Microsoft.Extensions.DependencyInjection;

namespace MeuBolsoDigital.MongoDB.Context.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services, Action<MongoDbContextOptions> options) where TContext : DbContext
        {
            var mongoDbContextOptions = new MongoDbContextOptions();
            options(mongoDbContextOptions);

            services.AddSingleton(mongoDbContextOptions)
                    .AddScoped<TContext>();

            return services;
        }
    }
}