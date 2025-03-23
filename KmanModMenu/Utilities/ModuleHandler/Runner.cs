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
        static List<Tuple<MethodInfo, object>> updateListeners = new List<Tuple<MethodInfo, object>>();

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
                var moduleInstance = Activator.CreateInstance(module.Register);
                module.Register.GetMethod("OnLoad")?.Invoke(moduleInstance, []);
                module.Register.GetMethod("Init")?.Invoke(moduleInstance, []);
                var updateMethod = module.Register.GetMethod("Update");
                if (updateMethod != null)
                {
                    updateListeners.Add(new Tuple<MethodInfo, object>(updateMethod, moduleInstance));
                }
            }
            return Task.CompletedTask;
        }

        public void Update()
        {
            foreach (var update in updateListeners)
            {
                update.Item1?.Invoke(update.Item2, []); // Pass the instance to the Update method
            }
        }
    }
}
