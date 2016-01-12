using System;
using System.Collections.Generic;

namespace CodeVision.Dependencies.Nugets
{
    public class Package
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; set; }

        public string Authors { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public Uri IconUrl { get; set; }
        public Uri ProjectUrl { get; set; }
    }
}