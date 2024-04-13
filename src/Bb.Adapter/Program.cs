
using System.Reflection;

namespace Bb.Adapter
{
    public class Program
    {


        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var service = GetService(args);
            service.Run();
        }

        public static ServiceAdapterRunner GetService(string[] args)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(currentAssembly.Location));
            var result = new ServiceAdapterRunner(args);
            return result;
        }


    }

}