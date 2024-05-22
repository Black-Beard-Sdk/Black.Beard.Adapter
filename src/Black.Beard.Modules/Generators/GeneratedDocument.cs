using System.Text;

namespace Bb.Generators
{


    public class GeneratedDocument
    {

        public GeneratedDocument(TemplateDocument source, ContextGenerator ctx)
        {
            this.Source = source;
            this.Context = ctx;
        }

        public TemplateDocument Source { get; }

        public ContextGenerator Context { get; }

        public string Filename { get; set; }

        public StringBuilder Content { get; set; }

        public uint Crc => Crc32.CalculateCrc32(Content);

        public string RelativePath { get; internal set; }

        internal string FullPath => _fullPath ?? (_fullPath = Context.RootPath.Combine(RelativePath));

        internal string FullFilename => _fullFilename ?? (_fullFilename = Context.RootPath.Combine(FullPath, Filename));


        public void WriteOnDisk()
        {
            var path = this.FullPath;
            path.CreateFolderIfNotExists();
            FullFilename.Save(this.Content.ToString());
        }


        private string _fullPath;
        private string _fullFilename;

    }


}
