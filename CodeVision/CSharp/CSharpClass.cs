using System.Collections.Generic;

namespace CodeVision.CSharp
{
    public class CSharpClass
    {
        public string ClassName { get; set; }
        public List<string> Interfaces { get; set; }
        public string BaseClassName { get; set; }
        public List<CSharpMethod> Methods { get; set; }

        public CSharpClass()
        {
            Methods = new List<CSharpMethod>();
        }
    }
}
