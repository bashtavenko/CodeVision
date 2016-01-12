using System.Collections.Generic;

namespace CodeVision.Dependencies.Nugets
{
    public class Project
    {
        public string Name { get; set; }
        public string OutputKind { get; set; }
        public string Platform { get; set; }
        public IList<Package> Packages { get; set; }

        public Project()
        {
            Packages = new List<Package>();
        }
    }
}
