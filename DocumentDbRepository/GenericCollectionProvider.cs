namespace Santhos.DocumentDb.Repository
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public class GenericCollectionProvider<TDocument> : ICollectionProvider
    {
        private readonly DocumentClient documentClient;

        private readonly IDatabaseProvider databaseProvider;

        public GenericCollectionProvider(
            DocumentClient documentClient,
            IDatabaseProvider databaseProvider)
        {
            this.documentClient = documentClient;
            this.databaseProvider = databaseProvider;
        }

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

        public virtual async Task<string> GetCollectionDocumentsLink()
        {
            return (await this.CreateOrGetCollection()).DocumentsLink;
        }

        public virtual string GetCollectionId()
        {
            return typeof(TDocument).Name;
        }
    }
}
