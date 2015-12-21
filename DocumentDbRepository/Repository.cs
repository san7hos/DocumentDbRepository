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

        public Repository(DocumentClient documentClient, string databaseId) : this(
            documentClient, 
            new GenericCollectionProvider<TDocument>(
                documentClient, 
                new BasicDatabaseProvider(documentClient, databaseId)))
        {
        } 

        public Repository(
            DocumentClient documentClient,
            ICollectionProvider collectionProvider)
        {
            this.documentClient = documentClient;
            this.collectionProvider = collectionProvider;
        }

        public virtual async Task<IOrderedQueryable<TDocument>> CreateDocumentQuery()
        {
            return this.documentClient.CreateDocumentQuery<TDocument>(
                await this.collectionProvider.GetCollectionDocumentsLink());
        }

        public virtual async Task<IOrderedQueryable<TDocument>> GetAll()
        {
            return await this.CreateDocumentQuery();
        }

        public virtual async Task<IQueryable<TDocument>> GetWhere(Expression<Func<TDocument, bool>> predicate)
        {
            return (await this.CreateDocumentQuery()).Where(predicate);
        }

        public virtual async Task<TDocument> Get(Expression<Func<TDocument, bool>> predicate)
        {
            return (await this.GetWhere(predicate))
                .AsEnumerable()
                .FirstOrDefault();
        }

        public virtual async Task<TDocument> Get(string id)
        {
            return await this.Get(d => d.Id == id);
        }

        public virtual async Task<TDocument> Create(TDocument document)
        {
            return (TDocument)(dynamic)(await this.documentClient.CreateDocumentAsync(
                await this.collectionProvider.GetCollectionDocumentsLink(), 
                document))
                .Resource;
        }

        public virtual async Task<TDocument> Update(TDocument document)
        {
            var selfLink = await this.GetDocumentSelfLink(document);

            if (string.IsNullOrWhiteSpace(selfLink))
            {
                throw new InvalidOperationException("Document does not exist in collection");
            }

            return (TDocument)(dynamic)(await this.documentClient.ReplaceDocumentAsync(
                selfLink, 
                document))
                .Resource;
        }

        public virtual async Task Delete(TDocument document)
        {
            var selfLink = await this.GetDocumentSelfLink(document);

            if (string.IsNullOrWhiteSpace(selfLink))
            {
                throw new InvalidOperationException("Document does not exist in collection");
            }

            await this.documentClient.DeleteDocumentAsync(selfLink);
        }

        private async Task<string> GetDocumentSelfLink(TDocument document)
        {
            return string.IsNullOrWhiteSpace(document.SelfLink)
                                  ? (await this.Get(document.Id))?.SelfLink
                                  : document.SelfLink;
        }
    }
}
