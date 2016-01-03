namespace Santhos.DocumentDb.Repository
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    /// <summary>
    /// Provides collections based on the generic type name
    /// </summary>
    /// <typeparam name="TDocument">Type of the document.</typeparam>
    public class GenericCollectionProvider<TDocument> : ICollectionProvider
    {
        private readonly DocumentClient documentClient;

        private readonly IDatabaseProvider databaseProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCollectionProvider{TDocument}"/> class
        /// </summary>
        /// <param name="documentClient">DocumentDb Document Client</param>
        /// <param name="databaseProvider">
        /// Database provider to obtain the database 
        /// on which the collections are managed
        /// </param>
        public GenericCollectionProvider(
            DocumentClient documentClient,
            IDatabaseProvider databaseProvider)
        {
            this.documentClient = documentClient;
            this.databaseProvider = databaseProvider;
        }

        /// <summary>
        /// Creates or gets the document collection. Queries the database obtained from the database
        /// provider using the <see cref="DocumentClient"/>
        /// If a collection with the collection id from <see cref="GetCollectionId"/> exists, 
        /// returns the instance. If the collection does not exist, creates a new instance and returns it.
        /// </summary>
        /// <returns>Document collection where the documents are stored</returns>
        public async Task<DocumentCollection> CreateOrGetCollection()
        {
            var collection = 
                this.documentClient.CreateDocumentCollectionQuery(await this.databaseProvider.GetDbSelfLink())
                .Where(c => c.Id == this.GetCollectionId())
                .AsEnumerable()
                .FirstOrDefault();

            return collection ??
                 await this.documentClient.CreateDocumentCollectionAsync(
                    await this.databaseProvider.GetDbSelfLink(),
                    new DocumentCollection { Id = this.GetCollectionId() });
        }

        /// <summary>
        /// Gets the collection documents link. Calls the <see cref="CreateOrGetCollection"/> method
        /// to obtain the collection.
        /// </summary>
        /// <returns>Collection documents link</returns>
        public virtual async Task<string> GetCollectionDocumentsLink()
        {
            return (await this.CreateOrGetCollection()).DocumentsLink;
        }

        /// <summary>
        /// Gets the document collection identifier as the specified generic type name
        /// </summary>
        /// <returns>Name of the specified generic type</returns>
        public virtual string GetCollectionId()
        {
            return typeof(TDocument).Name;
        }
    }
}
