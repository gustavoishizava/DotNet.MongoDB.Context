using MongoDB.Driver;

namespace DotNet.MongoDB.Context.Context.Operations
{
    public class BulkOperationModel<TDocument> where TDocument : class
    {
        public FilterDefinition<TDocument> Filter { get; private set; }
        public TDocument Document { get; private set; }

        public BulkOperationModel(FilterDefinition<TDocument> filter, TDocument document)
        {
            if (filter is null)
                throw new ArgumentNullException(nameof(filter), "Filter cannot be null.");

            if (document is null)
                throw new ArgumentNullException(nameof(document), "Document cannot be null.");

            Filter = filter;
            Document = document;
        }
    }
}