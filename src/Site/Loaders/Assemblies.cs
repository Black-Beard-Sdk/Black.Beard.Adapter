using Bb.ComponentModel.Loaders;
using Bb;
using System.Reflection;
using static MudBlazor.CategoryTypes;
using Bb.ComponentModel.Attributes;

namespace Site.Loaders
{

    public static class Assemblies
    {

        /// <summary>
        /// Ensure required assemblies are loaded
        /// </summary>
        public static ExposedAssemblyRepositories Resolve(string filename, string[] configPaths)
        {

            foreach (var repertoireConfig in configPaths)
            {

                var _currentDirectory = Directory.GetCurrentDirectory();
                var dir = _currentDirectory.Combine(repertoireConfig).AsDirectory();
                var files = dir.GetFiles(filename, SearchOption.AllDirectories);
                foreach (var file in files)
                    try
                    {
                        ExposedAssemblyRepositories assemblies
                            = file.LoadFromFileAndDeserialize<ExposedAssemblyRepositories>();
                        
                        return assemblies;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
            }

            return null;

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

        private static bool ContainPage2(this Assembly assembly)
        {

            var name = typeof(ExposeClassAttribute).Assembly.FullName;
            AssemblyName[] m = assembly.GetReferencedAssemblies();

            if (m.Any(c => c.Name.StartsWith(name)))
                return assembly.ExportedTypes.Any(c => c.GetCustomAttributes().OfType<Microsoft.AspNetCore.Components.RouteAttribute>().Any());

            return false;

        }

        private static bool ContainPage2(this Assembly assembly, string textToSearch)
        {
            AssemblyName[] m = assembly.GetReferencedAssemblies();
            if (m.Any(c => c.Name.StartsWith(textToSearch)))
                return assembly.ExportedTypes.Any(c => c.GetCustomAttributes().OfType<Microsoft.AspNetCore.Components.RouteAttribute>().Any());

            return false;

        }

    }
}
