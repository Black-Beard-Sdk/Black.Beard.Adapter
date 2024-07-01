using Bb;
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Black.Beard.Configuration.Git
{
    internal static class ConfigurationFileLoader
    {
        public static IConfigurationBuilder LoadConfigurationFile(this IConfigurationBuilder config,
            string[] paths,
            string pattern = null,
            Func<FileInfo, bool> filter = null)
        {

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var contentRootPath = Assembly.GetEntryAssembly()
                .Location
                .AsFile()
                .Directory.FullName;

            var dirs = new List<DirectoryInfo>()
            {
                contentRootPath.AsDirectory()
            };

            if (paths != null)
                foreach (var path in paths)
                {

                    DirectoryInfo c;

                    if (path.FilePathIsAbsolute())
                        c = path.AsDirectory();
                    else
                    {
                        var p = contentRootPath;
                        if (!string.IsNullOrEmpty(path))
                            p = p.Combine(path);
                        c = p.AsDirectory();
                    }

                    if (c != null && c.Exists)
                        dirs.Add(c);

                }

            if (string.IsNullOrEmpty(pattern))
                pattern = $"*.{environmentName}.json";

            foreach (var dir in dirs)
            {
                foreach (var file in dir.GetFiles(pattern))
                    if (filter == null || filter(file))
                    {
                        config.AddJsonFile(file.FullName, optional: false, reloadOnChange: false);
                        // _logger.Debug($"configuration file {file.FullName} is loaded.");
                    }
            }

            return config;

        }
    }

}
