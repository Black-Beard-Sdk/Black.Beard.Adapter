using Bb.ComponentModel.Loaders;
using Bb;

namespace Site.SiteExtensions
{

    public class Assemblies
    {

        /// <summary>
        /// Ensure assemblies are loaded
        /// </summary>
        public static void Load()
        {

            var _currentDirectory = Directory.GetCurrentDirectory();
            var dir = _currentDirectory.Combine("Configs").AsDirectory();
            var files = dir.GetFiles("ExposedAssemblyRepositories*.json", SearchOption.AllDirectories);
            foreach (var file in files)
                try
                {

                    var payload = file.LoadFromFile();

                    ExposedAssemblyRepositories assemblies = file.LoadFromFileAndDeserialize<ExposedAssemblyRepositories>();
                    assemblies.Load();
                }
                catch (Exception ex)
                {
                    throw;
                }




        }

    }
}
