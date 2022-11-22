using MongoDB.Bson.Serialization;

namespace DotNet.MongoDB.Context.Mapping
{
    public interface IBsonClassMapConfiguration<TModel>
    {
        string CollectionName { get; }
        bool IsEntity { get; }
        BsonClassMap<TModel> GetConfiguration();
        void Apply();
    }
}