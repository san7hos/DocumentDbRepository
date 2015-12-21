namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System;
    using System.Collections.Generic;

    internal static class TestDocumentFactory
    {
        public static TestDocument Create(int seed)
        {
            var testDocument = new TestDocument
            {
                Seed = seed,
                Name = $"Test Doc {seed}",
                Created = DateTime.Now.AddDays(-seed % 3),
                MainDocument = new TestSubDocument
                {
                    Name = $"Main Subdoc {seed}",
                    Count = seed,
                    Width = seed * seed + 0.5
                },
                OptionalDocuments = new List<TestSubDocument>()
            };

            for (int j = 0; j < seed % 4; j++)
            {
                testDocument.OptionalDocuments.Add(new TestSubDocument
                {
                    Name = $"Subdoc {seed}.{j}",
                    Count = j,
                    Width = j + 5.75
                });
            }

            return testDocument;
        }
    }
}
