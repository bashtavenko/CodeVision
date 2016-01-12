using System;
using System.Collections.Generic;

namespace CodeVision.Dependencies.SqlStorage
{
    public class Package
    {
        public int PackageId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; set; }
        public string Authors { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string IconUrl { get; set; }
        public string ProjectUrl { get; set; }
        public virtual List<Project> Projects { get; set; }

        public Package()
        {
            Projects = new List<Project>();
        }
    }
}