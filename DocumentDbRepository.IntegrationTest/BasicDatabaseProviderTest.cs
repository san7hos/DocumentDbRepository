namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class BasicDatabaseProviderTest
    {
        private readonly string databaseId = Config.DocDbDatabase;

        private readonly DocumentClient documentClient;

        public BasicDatabaseProviderTest(DocumentClient documentClient)
        {
            this.documentClient = documentClient;
        }

        public async Task CreateOrGetDb()
        {
            await this.CreateDb();

            await this.GetDb();
        }

        private async Task CreateDb()
        {
            Assert.IsNull(
                this.documentClient.CreateDatabaseQuery()
                .Where(d => d.Id == this.databaseId)
                .AsEnumerable()
                .FirstOrDefault(),
                $"Database account should not contain database with id {this.databaseId}");

            Assert.IsNotNull(
                await new BasicDatabaseProvider(this.documentClient, this.databaseId)
                .CreateOrGetDb(), 
                "db != null");

            Assert.IsNotNull(
                this.documentClient.CreateDatabaseQuery()
                .Where(d => d.Id == this.databaseId)
                .AsEnumerable()
                .FirstOrDefault(),
                $"Database {this.databaseId} has not been created");
        }

        private async Task GetDb()
        {
            Assert.IsNotNull(
                this.documentClient.CreateDatabaseQuery()
                .Where(d => d.Id == this.databaseId)
                .AsEnumerable()
                .FirstOrDefault(),
                $"Database {this.databaseId} has not been created, cannot test get.");

            Assert.IsNotNull(
                await new BasicDatabaseProvider(this.documentClient, this.databaseId)
                .CreateOrGetDb(),
                "db != null");
        }
    }
}
