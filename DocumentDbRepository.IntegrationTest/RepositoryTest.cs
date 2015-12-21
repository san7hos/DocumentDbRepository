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

        private readonly Random random = new Random();

        public RepositoryTest(DocumentClient documentClient)
        {
            this.repository = new Repository<TestDocument>(documentClient, Config.DocDbDatabase);
        }

        public async Task RunOrderedTest()
        {
            await this.CreateQuery();

            await this.GetAllEmpty();

            await this.Create();

            await this.GetAll();

            await this.GetWhere();

            await this.GetPredicate();

            await this.GetId();

            await this.Update();

            await this.Delete();

            await this.GetNonExistent();

            await this.UpdateNonExistent();

            await this.DeleteNonExistent();
        }

        private async Task CreateQuery()
        {
            var query = await this.repository.CreateDocumentQuery();

            Assert.IsNotNull(query, "query != null");
        }

        private async Task DeleteNonExistent()
        {
            try
            {
                await this.repository.Delete(new TestDocument
                                             {
                                                 Seed = TestDocumentCount + 1
                                             });
            }
            catch (InvalidOperationException)
            {
                return;
            }

            Assert.Fail();
        }

        private async Task UpdateNonExistent()
        {
            try
            {
                await this.repository.Update(new TestDocument
                                             {
                                                 Seed = TestDocumentCount + 1
                                             });
            }
            catch (InvalidOperationException)
            {
                return;
            }

            Assert.Fail();
        }

        private async Task GetNonExistent()
        {
            var nonexistent = await this.repository.Get(d => d.Seed == TestDocumentCount + 1);

            Assert.IsNull(nonexistent, "nonexistent != null");
        }

        private async Task Delete()
        {
            var seed = this.GetRandomSeed();
            var randomDocument = await this.repository.Get(d => d.Seed == seed);

            await this.repository.Delete(randomDocument);

            var deletedDocument = await this.repository.Get(d => d.Seed == seed);

            Assert.IsNull(deletedDocument, "deletedDocument != null");
        }

        private async Task Update()
        {
            var seed = this.GetRandomSeed();
            var randomDocument = await this.repository.Get(d => d.Seed == seed);

            string updatedName = "Different name";
            randomDocument.Name = updatedName;
            var returnedDocument = await this.repository.Update(randomDocument);

            Assert.AreEqual(
                updatedName,
                returnedDocument.Name,
                "Document name has not been updated");

            var updatedDocument = await this.repository.Get(d => d.Seed == seed);

            Assert.IsTrue(
                updatedDocument.Equals(returnedDocument) &&
                updatedDocument.Equals(randomDocument),
                "Updated document does not equal to chosen manually updated document");

            Assert.IsFalse(
                TestDocumentFactory.Create(seed).Equals(updatedDocument),
                "Document has not been updated");
        }

        private async Task GetId()
        {
            var seed = this.GetRandomSeed();
            var randomDocument = await this.repository.Get(d => d.Seed == seed);

            var randomDocById = await this.repository.Get(randomDocument.Id);

            Assert.IsTrue(
                randomDocument.Equals(randomDocById),
                "Document obtained by predicate is different from document obtained by id");
        }

        private async Task GetPredicate()
        {
            var seed = this.GetRandomSeed();
            var randomDocument = await this.repository.Get(d => d.Seed == seed);

            Assert.IsTrue(
                TestDocumentFactory.Create(seed).Equals(randomDocument),
                $"Test document with seed {seed} is invalid");
        }

        private async Task GetWhere()
        {
            var seed1 = this.GetRandomSeed();
            var seed2 = this.GetRandomSeed();

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

            int seed = this.GetRandomSeed();
            TestDocument randomDocument = testDocuments.OrderBy(d => d.Seed).ElementAt(seed);

            Assert.IsTrue(
                TestDocumentFactory.Create(seed).Equals(randomDocument),
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

        private int GetRandomSeed()
        {
            return this.random.Next(TestDocumentCount);
        }
    }
}
