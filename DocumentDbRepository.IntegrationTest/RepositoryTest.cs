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
        private const int TestDocumentCount = 15;

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

            await this.GetWhere();
        }

        private async Task GetWhere()
        {
            var random = new Random();
            var seed1 = random.Next(TestDocumentCount);
            var seed2 = random.Next(TestDocumentCount);

            List<TestDocument> testDocuments = (await this.repository
                                                          .GetWhere(d => d.Seed == seed1 || d.Seed == seed2))
                .ToList()
                .OrderBy(d => d.Seed)
                .ToList();

            Assert.IsTrue(
                TestDocumentFactory.Create(seed1).Equals(testDocuments.Find(d => d.Seed == seed1)),
                $"Test document with seed {seed1} is invalid");

            Assert.IsTrue(
                TestDocumentFactory.Create(seed2).Equals(testDocuments.Find(d => d.Seed == seed2)),
                $"Test document with seed {seed2} is invalid");
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

            int seed = new Random().Next(TestDocumentCount);
            TestDocument random = testDocuments.OrderBy(d => d.Seed).ElementAt(seed);

            Assert.IsTrue(
                TestDocumentFactory.Create(seed).Equals(random),
                $"Test document on index {seed} is invalid");
        }

        private async Task Create()
        {
            for (int i = 0; i < TestDocumentCount; i++)
            {
                await this.repository.Create(
                    TestDocumentFactory.Create(i));
            }
        }


    }
}
