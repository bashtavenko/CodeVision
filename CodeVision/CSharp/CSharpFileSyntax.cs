using System.Collections.Generic;

namespace CodeVision.CSharp
{
    public class CSharpFileSyntax
    {
        public List<string> Usings { get; set; }
        public List<CSharpClass> Classes { get; set; }
        public List<string> Comments { get; set; }

        public CSharpFileSyntax()
        {
            Usings = new List<string>();
            Classes = new List<CSharpClass>();
            Comments = new List<string>();
        }
    }
}
