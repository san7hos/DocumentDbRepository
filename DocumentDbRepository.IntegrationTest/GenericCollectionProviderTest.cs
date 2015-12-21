namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class GenericCollectionProviderTest
    {
        private readonly DocumentClient documentClient;

        private readonly ICollectionProvider collectionProvider;

        public GenericCollectionProviderTest(DocumentClient documentClient)
        {
            this.documentClient = documentClient;

            this.collectionProvider = new GenericCollectionProvider<TestDocument>(
                documentClient,
                new BasicDatabaseProvider(
                    this.documentClient,
                    Config.DocDbDatabase));
        }

        public async Task RunOrderedTest()
        {
            await this.CreateOrGetCollection();

            await this.GetDocumentsLink();
        }

        private async Task GetDocumentsLink()
        {
            var docLink = await this.collectionProvider.GetCollectionDocumentsLink();

            Assert.IsNotNull(docLink, "docLink != null");
            
            Assert.IsNotNull(
                this.documentClient.CreateDocumentCollectionQuery(
                        UriFactory.CreateDatabaseUri(Config.DocDbDatabase))
                    .ToList()
                    .FirstOrDefault(c => c.DocumentsLink == docLink),
                $"Collection with doc link {docLink} not found");
        }

        private async Task CreateOrGetCollection()
        {
            await this.CreateCollection();

            await this.GetCollection();
        }

        private async Task CreateCollection()
        {
            Assert.IsNull(
                this.documentClient.CreateDocumentCollectionQuery(
                        UriFactory.CreateDatabaseUri(Config.DocDbDatabase))
                    .Where(c => c.Id == typeof(TestDocument).Name)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Collection {typeof(TestDocument).Name} should not be contained in the database with id {Config.DocDbDatabase}");

            Assert.IsNotNull(
                await this.collectionProvider.CreateOrGetCollection(),
                "collection != null");

            Assert.IsNotNull(
                this.documentClient.CreateDocumentCollectionQuery(
                        UriFactory.CreateDatabaseUri(Config.DocDbDatabase))
                    .Where(c => c.Id == typeof(TestDocument).Name)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Collection {typeof(TestDocument).Name} has not been created in the database with id {Config.DocDbDatabase}");
        }

        private async Task GetCollection()
        {
            Assert.IsNotNull(
                this.documentClient.CreateDocumentCollectionQuery(
                        UriFactory.CreateDatabaseUri(Config.DocDbDatabase))
                    .Where(c => c.Id == typeof(TestDocument).Name)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Collection {typeof(TestDocument).Name} has not been created in the database with id {Config.DocDbDatabase}, cannot test get.");

            Assert.IsNotNull(
                await this.collectionProvider.CreateOrGetCollection(),
                "collection != null");
        }
    }
}
