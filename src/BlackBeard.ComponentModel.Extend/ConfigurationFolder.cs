namespace Bb
{
    public class ConfigurationFolder : List<DirectoryInfo>
    {

        private ConfigurationFolder()
        {

        }

        public static void AddDirectoryIfExists(string folder)
        {
            AddDirectoryIfExists(new DirectoryInfo(folder));
        }

        public static void AddDirectoryIfExists(DirectoryInfo folder)
        {
            folder.Refresh();
            if (folder.Exists)
                Instance.Add(folder);
        }

        public static void AddDirectory(string folder)
        {
            AddDirectory(new DirectoryInfo(folder));
        }

        public static void AddDirectory(DirectoryInfo folder)
        {
            Instance.Add(folder);
        }

      

        public static string[] GetPaths()
        {

            List<string> result = new List<string>();

            foreach (var item in Instance)
                result.Add(item.FullName);

            return result.ToArray();

        }

        public static ConfigurationFolder Instance
        {
            get
            {

                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                        {
                            _instance = new ConfigurationFolder();
                            AddDirectoryIfExists(Path.Combine(AppContext.BaseDirectory, "Configs"));
                        }
                return _instance;
            }
        }

        private static ConfigurationFolder _instance;
        private static object _lock = new object();

    }

}
