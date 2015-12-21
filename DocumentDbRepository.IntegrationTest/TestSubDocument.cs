// ReSharper disable NonReadonlyMemberInGetHashCode
namespace Santhos.DocumentDb.Repository.IntegrationTest
{
    internal class TestSubDocument
    {
        public string Name { get; set; }

        public double Width { get; set; }

        public int Count { get; set; }

        public override bool Equals(object obj)
        {
            var testSubDocument = obj as TestSubDocument;
            if (testSubDocument == null)
            {
                return false;
            }

            return this.Name == testSubDocument.Name &&
                   this.Width.Equals(testSubDocument.Width) &&
                   this.Count == testSubDocument.Count;
        }
        
        public override int GetHashCode()
        {
            return new
                   {
                       this.Name, 
                       this.Width, 
                       this.Count
                   }.GetHashCode();
        }
    }
}
