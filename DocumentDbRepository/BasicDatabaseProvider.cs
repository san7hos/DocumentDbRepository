namespace Santhos.DocumentDb.Repository
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    /// <summary>
    /// Provides DocumentDb database based on given database identifier
    /// </summary>
    public class BasicDatabaseProvider : IDatabaseProvider
    {
        private readonly DocumentClient client;

        private readonly string databaseId;

        /// <summary>Initializes a new instance of the <see cref="BasicDatabaseProvider"/> class.</summary>
        /// <param name="client">DocumentDb Document Client</param>
        /// <param name="databaseId">Database identifier</param>
        public BasicDatabaseProvider(DocumentClient client, string databaseId)
        {
            this.client = client;
            this.databaseId = databaseId;
        }

        /// <summary>
        /// Creates or gets the DocumentDb database. Queries the DocumentDb subscription 
        /// using the <see cref="DocumentClient"/> specified in the constructor.
        /// If a database with the specified database id exists, returns the instance.
        /// If the database does not exist, creates a new instance and returns it.
        /// </summary>
        /// <returns>DocumentDb database</returns>
        public virtual async Task<Database> CreateOrGetDb()
        {
            Database db = this.client.CreateDatabaseQuery()
                .Where(d => d.Id == this.databaseId)
                .AsEnumerable()
                .FirstOrDefault();
            
            return db ?? await this.client.CreateDatabaseAsync(new Database { Id = this.databaseId });
        }

        /// <summary>
        /// Gets the DocumentDb database self link. Uses the <see cref="CreateOrGetDb"/> method to
        /// obtain the database instance.
        /// </summary>
        /// <returns>DocumentDb database self link</returns>
        public virtual async Task<string> GetDbSelfLink()
        {
            return (await this.CreateOrGetDb()).SelfLink;
        }
    }
}
