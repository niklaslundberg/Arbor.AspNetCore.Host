using System;
using System.IO;
using System.Runtime.Loader;
using Arbor.App.Extensions.ExtensionMethods;
using Serilog;

namespace Arbor.AspNetCore.Host.Mvc
{
    public static class ViewAssemblyLoader
    {
        public static void LoadViewAssemblies(ILogger logger,
            AssemblyLoadContext? assemblyLoadContext = null,
            string? basePath = null)
        {
            string? applicationDirectory = basePath ?? AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrWhiteSpace(applicationDirectory))
            {
                return;
            }

            assemblyLoadContext ??= AssemblyLoadContext.Default;

            var appDirectory = new DirectoryInfo(applicationDirectory);

            var viewDllFiles = appDirectory.GetFiles("*.Views.dll");

            if (viewDllFiles.Length == 0)
            {
                logger.Debug("Could not find any view DLL files in directory {AppDirectory}", applicationDirectory);
                return;
            }

            logger.Debug("Found {ViewDllCount} view dll files in directory {AppDirectory}",
                viewDllFiles.Length,
                applicationDirectory);

            foreach (var fileInfo in viewDllFiles)
            {
                try
                {
                    var assembly = assemblyLoadContext.LoadFromAssemblyPath(fileInfo.FullName);

                    logger.Debug("Successfully loaded assembly {Assembly} from DLL file {DllFile}",
                        assembly.FullName,
                        fileInfo.FullName);
                }
                catch (Exception ex) when (!ex.IsFatal())
                {
                    logger.Error(ex, "Could not load assembly from DLL file {Dll}", fileInfo.FullName);
                }
            }
        }
    }
}