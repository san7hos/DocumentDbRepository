namespace Santhos.DocumentDb.Repository
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public class BasicDatabaseProvider : IDatabaseProvider
    {
        private readonly DocumentClient client;

        private readonly string databaseId;

        public BasicDatabaseProvider(DocumentClient client, string databaseId)
        {
            this.client = client;
            this.databaseId = databaseId;
        }

        public virtual async Task<Database> CreateOrGetDb()
        {
            Database db = this.client.CreateDatabaseQuery()
                .Where(d => d.Id == this.databaseId)
                .AsEnumerable()
                .FirstOrDefault();
            
            return db ?? await this.client.CreateDatabaseAsync(new Database { Id = this.databaseId });
        }

        public virtual async Task<string> GetDbSelfLink()
        {
            return (await this.CreateOrGetDb()).SelfLink;
        }
    }
}
