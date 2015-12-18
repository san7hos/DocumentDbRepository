namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class GenericCollectionProviderTest
    {
        private readonly DocumentClient documentClient;

        private readonly string databaseId = Config.DocDbDatabase;

        private readonly IDatabaseProvider databaseProvider;

        public GenericCollectionProviderTest(DocumentClient documentClient)
        {
            this.documentClient = documentClient;
            this.databaseProvider = new BasicDatabaseProvider(
                this.documentClient,
                this.databaseId);
        }

        public async Task CreateOrGetCollection()
        {
            await this.CreateCollection();

            await this.GetCollection();
        }

        private async Task CreateCollection()
        {
            Assert.IsNull(
                this.documentClient.CreateDocumentCollectionQuery(
                    UriFactory.CreateDatabaseUri(this.databaseId))
                .Where(c => c.Id == typeof(TestDocument).Name)
                .AsEnumerable()
                .FirstOrDefault(),
                $"Collection {typeof(TestDocument).Name} should not be contained in the database with id {this.databaseId}");

            Assert.IsNotNull(
                await new GenericCollectionProvider<TestDocument>(this.documentClient, this.databaseProvider)
                .CreateOrGetCollection(),
                "collection != null");

            Assert.IsNotNull(
                this.documentClient.CreateDocumentCollectionQuery(
                    UriFactory.CreateDatabaseUri(this.databaseId))
                .Where(c => c.Id == typeof(TestDocument).Name)
                .AsEnumerable()
                .FirstOrDefault(),
                $"Collection {typeof(TestDocument).Name} has not been created in the database with id {this.databaseId}");
        }

        private async Task GetCollection()
        {
            Assert.IsNotNull(
                this.documentClient.CreateDocumentCollectionQuery(
                    UriFactory.CreateDatabaseUri(this.databaseId))
                .Where(c => c.Id == typeof(TestDocument).Name)
                .AsEnumerable()
                .FirstOrDefault(),
                $"Collection {typeof(TestDocument).Name} has not been created in the database with id {this.databaseId}, cannot test get.");

            Assert.IsNotNull(
                await new GenericCollectionProvider<TestDocument>(this.documentClient, this.databaseProvider)
                .CreateOrGetCollection(),
                "collection != null");
        }
    }
}
