// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Azure.Documents;

    internal class TestDocument : Resource
    {
        private const double CreatedDelta = 5.0;

        public int Seed { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public TestSubDocument MainDocument { get; set; }

        public List<TestSubDocument> OptionalDocuments { get; set; }

        public override bool Equals(object obj)
        {
            var testDocument = obj as TestDocument;
            if (testDocument == null)
            {
                return false;
            }

            return
                this.Seed == testDocument.Seed &&
                this.Name == testDocument.Name &&
                Math.Abs((this.Created - testDocument.Created).TotalSeconds) < CreatedDelta &&
                this.MainDocument.Equals(testDocument.MainDocument) &&
                this.CompareOptionalDocuments(testDocument);
        }

        public override int GetHashCode()
        {
            return new
            {
                this.Seed,
                this.Name,
                this.Created,
                this.MainDocument,
                this.OptionalDocuments
            }.GetHashCode();
        }

        private bool CompareOptionalDocuments(TestDocument testDocument)
        {
            if (this.OptionalDocuments.Count != testDocument.OptionalDocuments.Count)
            {
                return false;
            }
            
            return !this.OptionalDocuments.Where((t, i) => !t.Equals(testDocument.OptionalDocuments[i])).Any();
        }
    }
}
