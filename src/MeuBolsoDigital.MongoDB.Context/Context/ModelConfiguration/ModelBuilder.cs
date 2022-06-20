using MongoDB.Bson.Serialization;

namespace MeuBolsoDigital.MongoDB.Context.Context.ModelConfiguration
{
    public class ModelBuilder
    {
        private readonly List<ModelMap> _modelMaps;
        public IReadOnlyList<ModelMap> ModelMaps => _modelMaps.AsReadOnly();

        public ModelBuilder()
        {
            _modelMaps = new();
        }

        public void AddModelMap(string collectionName, BsonClassMap bsonClassMap)
        {
            var modelMap = new ModelMap(collectionName, bsonClassMap);
            if (ModelMapExists(modelMap))
                return;

            _modelMaps.Add(modelMap);
        }

        private bool ModelMapExists(ModelMap modelMap)
        {
            return _modelMaps.Any(x => x.CollectionName.Equals(modelMap.CollectionName) || x.BsonClassMap.ClassType == modelMap.BsonClassMap.ClassType);
        }
    }
}