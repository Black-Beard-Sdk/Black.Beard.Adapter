using Bb.ComponentModel.Loaders;
using Bb;
using System.Reflection;
using static MudBlazor.CategoryTypes;

namespace Site.Loaders
{

    public static class Assemblies
    {

        /// <summary>
        /// Ensure required assemblies are loaded
        /// </summary>
        public static void Load(string filename = "ExposedAssemblyRepositories.json")
        {

            var _currentDirectory = Directory.GetCurrentDirectory();
            var dir = _currentDirectory.Combine("Configs").AsDirectory();
            var files = dir.GetFiles(filename, SearchOption.AllDirectories);
            foreach (var file in files)
                try
                {
                    ExposedAssemblyRepositories assemblies
                        = file.LoadFromFileAndDeserialize<ExposedAssemblyRepositories>();
                    assemblies.Load();
                }
                catch (Exception ex)
                {
                    throw;
                }

        }

        /// <summary>
        /// Return assembly to add for discover routes
        /// </summary>
        public static IEnumerable<Assembly> AdditionalAssemblies
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                return WebAssemblies.Where(c => c != assembly);
            }
        }

        /// <summary>
        /// Return all assemblies that contain type with route attribute
        /// </summary>
        public static IEnumerable<Assembly> WebAssemblies
        {
            get
            {

                var _assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var item in _assemblies)
                    if (item.ContainPage())
                        yield return item;
            }
        }


        private static bool ContainPage(this Assembly assembly)
        {

            AssemblyName[] m = assembly.GetReferencedAssemblies();

            if (m.Any(c => c.Name.StartsWith("Microsoft.AspNetCore.Components")))
                return assembly.ExportedTypes.Any(c => c.GetCustomAttributes().OfType<Microsoft.AspNetCore.Components.RouteAttribute>().Any());

            return false;

        }


    }
}
