using System;
using System.Collections.Generic;

namespace CodeVision.Dependencies.Modules
{
    public class Module : Symbol
    {
        public string Name { get; }      
        public string Version { get; }        
        public string Description { get; }        
        public List<Module> References { get; }

        public Module(string name, string version)
            : this(name, version, null, null)
        {
        }

        public Module(string name, string version, string description)
            : this (name, version, description, null)
        {            
        }

        public Module(string name, string version, string description, int? id)
        {
            this.Name = name;
            this.Version = version;
            this.Description = description;
            this.References = new List<Module>();
            this.Id = id;
        }

        public void AddReference (Module module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }
            this.References.Add(module);
        }              
        
        public override bool Equals(object obj)
        {
            var otherModule = obj as Module;
            if (otherModule == null)
            {
                return false;
            }
            var thisKey = BuildUniqueKey(Name, Version);
            var otherKey = BuildUniqueKey(otherModule.Name, otherModule.Version);
            return thisKey.Equals(otherKey, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return BuildUniqueKey(Name, Version).GetHashCode();
        }

        private string BuildUniqueKey(string name, string version)
        {
            return $"{name} {version}";
        }
    }
}
