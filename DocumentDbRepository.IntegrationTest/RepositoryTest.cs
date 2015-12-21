namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Azure.Documents.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class RepositoryTest
    {
        private const int TestDocumentCount = 7;

        private readonly Repository<TestDocument> repository;

        public RepositoryTest(DocumentClient documentClient)
        {
            this.repository = new Repository<TestDocument>(documentClient, Config.DocDbDatabase);
        }

        public async Task RunOrderedTest()
        {
            await this.GetAllEmpty();

            await this.Create();

            await this.GetAll();
        }

        private async Task GetAllEmpty()
        {
            Assert.AreEqual(
                0,
                (await this.repository.GetAll()).AsEnumerable().Count(), 
                "Collection is not empty");
        }

        private async Task GetAll()
        {
            List<TestDocument> testDocuments = (await this.repository.GetAll()).ToList();

            Assert.AreEqual(
                TestDocumentCount,
                testDocuments.Count,
                $"Collection does not contain {TestDocumentCount} documents");
        }

        private async Task Create()
        {
            for (int i = 0; i < TestDocumentCount; i++)
            {
                var testDocument = new TestDocument
                                   {
                                       Name = $"Test Doc {i}",
                                       Created = DateTime.Now.AddDays(-i % 3),
                                       MainDocument = new TestSubDocument
                                                      {
                                                          Name = $"Main Subdoc {i}",
                                                          Count = i,
                                                          Width = i * i + 0.5
                                                      },
                                       OptionalDocuments = new List<TestSubDocument>()
                                   };
                
                for (int j = 0; j < i % 4; j++)
                {
                    testDocument.OptionalDocuments.Add(new TestSubDocument
                                                       {
                                                           Name = $"Subdoc {i}.{j}",
                                                           Count = j,
                                                           Width = j + 5.75
                                                       });
                }

                await this.repository.Create(testDocument);
            }
        }
    }
}
