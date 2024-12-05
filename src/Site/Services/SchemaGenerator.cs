using Bb;
using Bb.ComponentModel.Attributes;
using System.Reflection;

namespace Site.Services
{



    public class SchemaGenerator
    {

        private SchemaGenerator(string path, string idTemplate)
        {

            this._path = path;
            this._idTemplate = idTemplate;

            var d = this._path.AsDirectory();

            if (!d.Exists)
                d.Create();

        }

        public static void Initialize(string path, string idTemplate)
        {
            _instance = new SchemaGenerator(path, idTemplate);
        }

        public static void GenerateSchema(Type type)
        {

            _instance.GenerateSchemaImpl(type);

        }

        private void GenerateSchemaImpl(Type type)
        {

            string name = type.Name;

            var a = type.GetCustomAttribute<ExposeClassAttribute>();
            if (a != null)
                if (!string.IsNullOrEmpty(a.Name))
                    a.Name = name;

            var filename = GetFilename(name);

            var id = new Uri(string.Format(_idTemplate, name));
            var schema = type.GenerateSchemaForConfiguration(id);

            filename.Save(schema);

        }

        private FileInfo GetFilename(string name)
        {
            string path = _path.Combine(name + ".schema.json");
            var d = path.AsFile();

            if (d.Exists)
                d.Delete();
            d.Refresh();

            return d;

        }

        private readonly string _path;
        private readonly string _idTemplate;
        private static SchemaGenerator _instance;

    }


}
