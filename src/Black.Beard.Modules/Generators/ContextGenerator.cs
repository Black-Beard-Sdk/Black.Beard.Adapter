
namespace Bb.Generators
{


    public class ContextGenerator
    {

        public ContextGenerator()
        {
            
        }

        public string RootPath { get; set; }


        public string RootNamespace { get; set; }


        public Bb.Diagrams.Diagnostics Diagnostics { get; set; }


        public void WriteOnDisk(IEnumerable<GeneratedDocument> documents)
        {
            foreach (var item in documents)
                item.WriteOnDisk();
        }

    

    }


}
