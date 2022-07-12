using MongoDB.Bson.Serialization;

namespace DotNet.MongoDB.Context.Context.ModelConfiguration
{
    public class ModelBuilder
    {
        private readonly List<ModelMap> _modelMaps;
        public IReadOnlyList<ModelMap> ModelMaps => _modelMaps.AsReadOnly();

        public ModelBuilder()
        {
            _modelMaps = new();
        }

        public void AddModelMap<TModel>(string collectionName, Action<BsonClassMap<TModel>> mapConfig = null) where TModel : class
        {
            var bsonClassMap = new BsonClassMap<TModel>();

            if (mapConfig is not null)
                mapConfig(bsonClassMap);

            var modelMap = new ModelMap(collectionName, bsonClassMap);
            if (ModelMapExists(modelMap))
                return;

            _modelMaps.Add(modelMap);
        }

        public string GetCollectionName(Type type)
        {
            return _modelMaps.FirstOrDefault(x => x.BsonClassMap.ClassType == type)?.CollectionName;
        }

        private bool ModelMapExists(ModelMap modelMap)
        {
            return _modelMaps.Any(x => x.CollectionName.Equals(modelMap.CollectionName) || x.BsonClassMap.ClassType == modelMap.BsonClassMap.ClassType);
        }
    }
}