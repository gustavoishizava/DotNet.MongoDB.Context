using MongoDB.Bson.Serialization;

namespace DotNet.MongoDB.Context.Mapping
{
    public interface IBsonClassMapConfiguration
    {
        string CollectionName { get; }
        BsonClassMap BsonClassMap { get; }
        bool IsEntity { get; }
        BsonClassMap GetConfiguration();
        void Apply();
    }
}