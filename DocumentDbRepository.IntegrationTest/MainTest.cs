namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MainTest
    {
        [TestMethod]
        public async Task Main()
        {
            using (var documentClient = DocumentClientFactory.Create())
            {
                try
                {
                    await new BasicDatabaseProviderTest(documentClient).RunOrderedTest();

                    await new GenericCollectionProviderTest(documentClient).RunOrderedTest();

                    await new RepositoryTest(documentClient).RunOrderedTest();
                }
                finally
                {
                    documentClient.DeleteDatabaseAsync(
                        UriFactory.CreateDatabaseUri(Config.DocDbDatabase))
                        .Wait();
                }
            }
        }
    }
}
