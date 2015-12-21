namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class BasicDatabaseProviderTest
    {
        private readonly DocumentClient documentClient;

        private readonly IDatabaseProvider basicDatabaseProvider;

        public BasicDatabaseProviderTest(DocumentClient documentClient)
        {
            this.documentClient = documentClient;
            this.basicDatabaseProvider = new BasicDatabaseProvider(documentClient, Config.DocDbDatabase);
        }

        public async Task RunOrderedTest()
        {
            await this.CreateOrGetDb();

            await this.GetDbSelfLink();
        }

        private async Task GetDbSelfLink()
        {
            var dbSelfLink = await this.basicDatabaseProvider.GetDbSelfLink();

            Assert.IsNotNull(dbSelfLink, "dbSelfLink != null");

            Assert.IsNotNull(
                this.documentClient.CreateDatabaseQuery()
                    .Where(d => d.SelfLink == dbSelfLink)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Database with self link {dbSelfLink} could not be found");
        }

        private async Task CreateOrGetDb()
        {
            await this.CreateDb();

            await this.GetDb();
        }

        private async Task CreateDb()
        {
            Assert.IsNull(
                this.documentClient.CreateDatabaseQuery()
                    .Where(d => d.Id == Config.DocDbDatabase)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Database account should not contain database with id {Config.DocDbDatabase}");

            Assert.IsNotNull(
                await this.basicDatabaseProvider.CreateOrGetDb(), 
                "db != null");

            Assert.IsNotNull(
                this.documentClient.CreateDatabaseQuery()
                    .Where(d => d.Id == Config.DocDbDatabase)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Database {Config.DocDbDatabase} has not been created");
        }

        private async Task GetDb()
        {
            Assert.IsNotNull(
                this.documentClient.CreateDatabaseQuery()
                    .Where(d => d.Id == Config.DocDbDatabase)
                    .AsEnumerable()
                    .FirstOrDefault(),
                $"Database {Config.DocDbDatabase} has not been created, cannot test get.");

            Assert.IsNotNull(
                await this.basicDatabaseProvider.CreateOrGetDb(),
                "db != null");
        }
    }
}
