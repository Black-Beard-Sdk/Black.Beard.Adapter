using System.Reflection;

namespace Bb.Modules.Sgbd.Models
{


    public static class ResourceHelper
    {

        public static IEnumerable<Resource> GetResources(string filter)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var item in assembly.GetManifestResourceNames())
                    if (item.Contains(filter))
                        yield return new Resource(assembly, item);
        }


        public static IEnumerable<Resource> GetResources(Predicate<string> filter = null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var item in assembly.GetManifestResourceNames())
                    if (filter == null || filter(item))
                        yield return new Resource(assembly, item);
        }


        public static IEnumerable<Resource> GetResources(this Assembly assembly)
        {
            foreach (var item in assembly.GetManifestResourceNames())
                yield return new Resource(assembly, item);
        }


        public static string? GetResource(this Assembly assembly, string name)
        {
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(c => c.EndsWith(name));
            if (resourceName == null)
                return null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


    }


    public struct Resource
    {


        public Resource(Assembly assembly, string resourceNamee)
        {
            Assembly = assembly;
            ResourceName = resourceNamee;
        }


        Assembly Assembly { get; }


        public string ResourceName { get; }


        public string? GetResource()
        {

            using (Stream stream = this.Assembly.GetManifestResourceStream(ResourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


    }


}
