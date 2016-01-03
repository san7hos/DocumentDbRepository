namespace Santhos.DocumentDb.Repository
{
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents;

    /// <summary>
    /// A provider for DocumentDb collections created and stored in the database.
    /// The documents are stored in the collections.
    /// </summary>
    public interface ICollectionProvider
    {
        /// <summary>
        /// Creates or gets the document collection.
        /// </summary>
        /// <returns>Document collection where the documents are stored</returns>
        Task<DocumentCollection> CreateOrGetCollection();

        /// <summary>
        /// Gets the collection documents link
        /// </summary>
        /// <returns>
        /// Collection documents link of the collection that is created by
        /// <see cref="CreateOrGetCollection"/>
        /// </returns>
        Task<string> GetCollectionDocumentsLink();
    }
}
