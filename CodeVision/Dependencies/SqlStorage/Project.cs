using System.Collections.Generic;

namespace CodeVision.Dependencies.SqlStorage
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string OutputKind { get; set; }
        public string Platform { get; set; }
        public virtual List<Package> Packages { get; set; }

        public Project()
        {
            Packages = new List<Package>();
        }
    }
}