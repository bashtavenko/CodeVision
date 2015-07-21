using System;
using System.Collections.Generic;
using System.IO;

namespace CodeVision.CSharp.Semantic
{    
    public class Module
    {
        public string Key { get; private set;} 
        public string Name
        {
            get
            {
                return string.IsNullOrEmpty(FileName) ? null : Path.GetFileNameWithoutExtension(FileName);
            }
        }
        public string FileName { get; private set; }
        public string Version { get; private set; }        
        public List<Module> References { get; private set; }

        public Module(string fileName, string version)
        {
            this.FileName = fileName;
            this.Version = version;
            this.References = new List<Module>();
            BuildKey(fileName, version);
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
                throw new ArgumentException(nameof(obj));
            }
            return this.Key.Equals(otherModule.Key, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return this.Key.GetHashCode();
        }

        private void BuildKey(string fileName, string version)
        {
            Key = string.Format("{0} {1}", fileName, version);
        }
    }
}
