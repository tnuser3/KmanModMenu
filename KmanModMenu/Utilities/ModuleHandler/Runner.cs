using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Utilities.ModuleHandler
{
    internal class Runner : MonoBehaviour
    {
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

        static List<ModuleManager.module> modules = new List<ModuleManager.module>();
        static List<MethodInfo> updateListeners = new List<MethodInfo>();

        public void Start()
        {
            LoadTask();
        }

        public Task LoadTask()
        {
            ModuleManager.LoadModuleHandler();
            ModuleManager.LoadModules();
            var moduleTask = ModuleManager.getModules();
            foreach (var module in moduleTask)
            {
                Console.WriteLine($"Loaded module {module.name}");
                module.Register?.GetMethod("OnLoad")?.Invoke(module.Register, []);
                module.Register?.GetMethod("Init")?.Invoke(module.Register, []);
                updateListeners.Add(module.Register?.GetMethod("Update"));
            }
            return Task.CompletedTask;
        }

        public void Update()
        {
            foreach (var update in updateListeners)
                update?.Invoke(update.DeclaringType, []);
        }
    }
}
