namespace Santhos.DocumentDb.Repository
{
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;

    /// <summary>
    /// A provider for DocumentDb database where the collections and documents are stored.
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>Creates or gets the DocumentDb database.</summary>
        /// <returns>DocumentDb database</returns>
        Task<Database> CreateOrGetDb();

        /// <summary>Gets the DocumentDb database self link.</summary>
        /// <returns>DocumentDb database self link</returns>
        Task<string> GetDbSelfLink();
    }
}