using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KmanModule
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModuleAttribute : Attribute
    {
        public string Guid { get; }
        public string Creator { get; }
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }

        public ModuleAttribute(string guid, string creator, string name, string description, string version)
        {
            Guid = guid;
            Creator = creator;
            Name = name;
            Description = description;
            Version = version;
        }
    }
}
