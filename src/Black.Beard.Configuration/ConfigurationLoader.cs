using Bb;
using System.Collections;
using System.Reflection;


namespace Bb.Configuration
{

    public class ConfigurationLoader : IEnumerable<IGrouping<string, ConfigurationFile>>
    {

        static ConfigurationLoader()
        {
            _environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? null;
        }

        public ConfigurationLoader(string pattern, Func<FileInfo, bool> filter = null)
        {

            if (string.IsNullOrEmpty(pattern))
                pattern = $"*.json";

            List<FileInfo> items = new List<FileInfo>();

            var contentRootPath = Assembly.GetEntryAssembly()
                .Location
                .AsFile()
            .Directory;

            _i = GetFiles(filter, contentRootPath, pattern).GroupBy(c => c.Name).ToList();

            foreach (var item in ConfigurationFolder.Instance)
            {
                var j = GetFiles(filter, item, pattern).GroupBy(c => c.Name).ToList();
                foreach (var file in j)
                    _i.Add(file);
            }

        }

        private static List<ConfigurationFile> GetFiles(Func<FileInfo, bool> filter, DirectoryInfo item, string pattern)
        {

            List<ConfigurationFile> items = new List<ConfigurationFile>();
            item.Refresh();

            var files = item.GetFiles(pattern);
            foreach (var file in files)
                if (filter == null || filter(file))
                {
                    var a = new ConfigurationFile() { FileInfo = file, Name = ComputeName(file.Name), Environment = ComputeEnvironmentName(file.Name) };
                    if (a.Environment == _environmentName || a.Environment == null)
                        items.Add(a);
                }
            return items;

        }

        private static string ComputeName(string name)
        {
            var n = name.Split('.');
            return n[0];
        }

        private static string? ComputeEnvironmentName(string name)
        {
            var n = name.Split('.');
            if (n.Length > 2)
                return n[1];
            return null;
        }



        public IEnumerator<IGrouping<string, ConfigurationFile>> GetEnumerator()
        {
            return _i.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _i.GetEnumerator();
        }
   
        private static string? _environmentName;
        private readonly List<IGrouping<string, ConfigurationFile>> _i;
    }


    public struct ConfigurationFile
    {
        public string Name;
        public FileInfo FileInfo;
        public string? Environment;
    }


}
