using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static UnityEngine.ParticleSystem;

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

        public static byte[] LoadEmbeddedResource(string resourceName)
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

        public static void LoadModuleHandler()
        {
            byte[] dllBytes = LoadEmbeddedResource("KmanModMenu.Assets.KmanModule.dll");
            moduleAssembly = Assembly.Load(dllBytes);

            Console.WriteLine(moduleAssembly.FullName);
        }

        public static void  LoadModules()
        {
            var path = Path.Combine(Paths.PluginPath, "kmanmodules");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }

            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    if (File.Exists(file)) Assembly.LoadFrom(file);
                }
                catch {}
            }
        }
        public static List<module> getModules()
        {
            var moduleType = moduleAssembly.GetType("KmanModule.Module");

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly != null &&
                                  !assembly.FullName.ToLower().Contains("unity") &&
                                  !assembly.FullName.ToLower().Contains("system"))
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(moduleEntry => moduleEntry.GetInterfaces().Contains(moduleType))
                .Select(moduleEntry => new
                {
                    Attribute = moduleEntry.GetCustomAttributesData()
                        .FirstOrDefault(attr => attr.AttributeType.Name == "ModuleAttribute"),
                    Module = moduleEntry.Assembly,
                    Type = moduleEntry.AsType()
                })
                .Where(x => x.Attribute != null)
                .Select(x => new module
                {
                    guid = (string)x.Attribute.ConstructorArguments[0].Value,
                    creator = (string)x.Attribute.ConstructorArguments[1].Value,
                    name = (string)x.Attribute.ConstructorArguments[2].Value,
                    description = (string)x.Attribute.ConstructorArguments[3].Value,
                    version = (string)x.Attribute.ConstructorArguments[4].Value,
                    Module = x.Module, 
                    Register = x.Type
                })
                .ToList();
        }
    }
}
