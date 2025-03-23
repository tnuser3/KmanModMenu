using System;
using System.Collections.Generic;
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
        }

        public static List<module> getModuleHandles()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.DefinedTypes.Any())
                {
                    foreach (var moduleEntry in assembly.DefinedTypes)
                    {
                        Type[] interfaces = moduleEntry.GetInterfaces();
                        if (interfaces.Contains(typeof(KmanModule.)
                    }
                }
            }
        }
    }
}
