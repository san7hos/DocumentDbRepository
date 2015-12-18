namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System;

    using Microsoft.Azure.Documents.Client;

    internal static class DocumentClientFactory
    {
        public static DocumentClient Create()
        {
            return new DocumentClient(
                new Uri(Config.DocDbEndpoint),
                Config.DocDbAuth);
        }
    }
}
