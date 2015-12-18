namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Azure.Documents;

    internal class TestDocument : Resource
    {
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public TestSubDocument MainDocument { get; set; }

        public List<TestSubDocument> OptionalDocuments { get; set; }
    }
}
