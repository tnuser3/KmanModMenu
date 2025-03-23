using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KmanModMenu.Utilities.ModuleHandler
{
    internal class ModuleManager
    {
        public class module
        {
            public string guid { get; set; }
            public string creator { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string version { get; set; }
            public Assembly Module { get; set; }
            public Type Register { get; set; }
        }

        static Assembly moduleAssembly;

        private static byte[] LoadEmbeddedResource(string resourceName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Resource '{resourceName}' not found.");
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public static Task LoadModuleHandler()
        {
            byte[] dllBytes = LoadEmbeddedResource("KmanModMenu.Assets.KmanModule.dll");
            moduleAssembly = Assembly.Load(dllBytes);
            Console.WriteLine(moduleAssembly.FullName);
            return Task.CompletedTask;
        }

        public static Task LoadModules()
        {
            var path = Path.Combine(Paths.PluginPath, "kmanmodules");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return Task.CompletedTask;
            }

            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    if (File.Exists(file)) Assembly.LoadFrom(file);
                }
                catch {}
            }
            return Task.CompletedTask;
        }

        public static Task<List<module>> getModules()
        {
            return Task.FromResult(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(moduleEntry => moduleEntry.GetInterfaces().Contains(typeof(KmanModule.Module)))
                .Select(moduleEntry => new
                {
                    Attribute = moduleEntry.GetCustomAttribute<KmanModule.ModuleAttribute>(),
                    Module = moduleEntry.Assembly,
                    Type = moduleEntry.AsType()
                })
                .Where(x => x.Attribute != null)
                .Select(x => new module
                {
                    guid = x.Attribute.Guid,
                    creator = x.Attribute.Creator,
                    name = x.Attribute.Name,
                    description = x.Attribute.Description,
                    version = x.Attribute.Version,
                    Module = x.Module,
                    Register = x.Type,
                })
                .ToList());
        }
    }
}
