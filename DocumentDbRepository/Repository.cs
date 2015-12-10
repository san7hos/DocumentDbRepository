namespace Santhos.DocumentDb.Repository
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public class Repository<TDocument> where TDocument : Resource
    {
        private readonly DocumentClient documentClient;

        private readonly ICollectionProvider collectionProvider;

        public Repository(
            DocumentClient documentClient,
            ICollectionProvider collectionProvider)
        {
            this.documentClient = documentClient;
            this.collectionProvider = collectionProvider;
        }

        public async Task<IOrderedQueryable<TDocument>> CreateDocumentQuery()
        {
            return this.documentClient.CreateDocumentQuery<TDocument>(
                await this.collectionProvider.GetCollectionDocumentsLink());
        }

        public async Task<IOrderedQueryable<TDocument>> GetAll()
        {
            return await this.CreateDocumentQuery();
        }

        public async Task<IQueryable<TDocument>> GetWhere(Expression<Func<TDocument, bool>> predicate)
        {
            return (await this.CreateDocumentQuery()).Where(predicate);
        }

        public async Task<TDocument> Get(Expression<Func<TDocument, bool>> predicate)
        {
            return (await this.CreateDocumentQuery())
                .FirstOrDefault(predicate);
        }

        public async Task<TDocument> Get(string id)
        {
            return await this.Get(d => d.Id == id);
        }

        public async Task<TDocument> Create(TDocument document)
        {
            return (TDocument)(dynamic)(await this.documentClient.CreateDocumentAsync(
                await this.collectionProvider.GetCollectionDocumentsLink(), 
                document))
                .Resource;
        }

        public async Task<TDocument> Update(TDocument document)
        {
            string selfLink = string.IsNullOrWhiteSpace(document.SelfLink)
                                  ? (await this.Get(document.Id)).SelfLink
                                  : document.SelfLink;

            return (TDocument)(dynamic)(await this.documentClient.ReplaceDocumentAsync(
                selfLink, 
                document))
                .Resource;
        }

        public async Task<TDocument> Delete(TDocument document)
        {
            TDocument oldDocument = await this.Get(document.Id);

            return (TDocument)(dynamic)(await this.documentClient.DeleteDocumentAsync(oldDocument.SelfLink))
                .Resource;
        }
    }
}
