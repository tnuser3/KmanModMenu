using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleExample
{
    [KmanModule.Module("com.Kman.ExamplePlugin", "Kman/Tnuser", "ExamplePlugin", "Example Description", "1.0.0")]
    public class Plugin : KmanModule.Module
    {
        public void Init()
        {
            Console.WriteLine("Initialized ExamplePlugin");
        }

        public void OnLoad()
        {
            Console.WriteLine("Loaded ExamplePlugin");
        }

        public void Update()
        {
            Console.WriteLine("Update Ran!");
        }
    }
}
