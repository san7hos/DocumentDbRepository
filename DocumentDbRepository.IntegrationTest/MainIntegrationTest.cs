namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MainIntegrationTest
    {
        private DocumentClient documentClient;

        [TestInitialize]
        public void Init()
        {
            this.documentClient = DocumentClientFactory.Create();
        }

        [TestMethod]
        public async Task Main()
        {
            
            await new BasicDatabaseProviderTest(this.documentClient)
                .CreateOrGetDb();

            await new GenericCollectionProviderTest(this.documentClient)
                .CreateOrGetCollection();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.documentClient.DeleteDatabaseAsync(
                UriFactory.CreateDatabaseUri(Config.DocDbDatabase)).Wait();

            this.documentClient.Dispose();
        }
    }
}
