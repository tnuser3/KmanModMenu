using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Utilities.ModuleHandler
{
    internal class Runner : MonoBehaviour
    {
        static List<ModuleManager.module> modules = new List<ModuleManager.module>();
        public void Start()
        {
            Task.Run(LoadTask);
        }

        public Task LoadTask()
        {
            ModuleManager.LoadModules().Wait();
            var moduleTask = ModuleManager.getModules();
            moduleTask.Wait();
            modules = moduleTask.Result;
            foreach (var module in modules)
            {
                Console.WriteLine($"Loaded module {module.name}");
                (module.Register as KmanModule.Module)?.OnLoad();
                (module.Register as KmanModule.Module)?.Init();
            }
            return Task.CompletedTask;
        }

        public void Update()
        {
            foreach (var module in modules)
                (module.Register as KmanModule.Module)?.Update();
        }
    }
}
