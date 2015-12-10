namespace Santhos.DocumentDb.Repository
{
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;

    public interface ICollectionProvider
    {
        Task<DocumentCollection> CreateOrGetCollection();

        Task<string> GetCollectionDocumentsLink();
    }
}
