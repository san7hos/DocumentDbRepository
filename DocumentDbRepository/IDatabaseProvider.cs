namespace Santhos.DocumentDb.Repository
{
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;

    public interface IDatabaseProvider
    {
        Task<Database> CreateOrGetDb();

        Task<string> GetDbSelfLink();
    }
}