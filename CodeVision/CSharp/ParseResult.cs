using System.Collections.Generic;

namespace CodeVision.CSharp
{
    // TODO: For lack of a better word
    public class ParseResult
    {
        public List<string> Usings { get; set; }
        public List<CSharpClass> Classes { get; set; }
        public List<string> Comments { get; set; }

        public ParseResult()
        {
            Usings = new List<string>();
            Classes = new List<CSharpClass>();
            Comments = new List<string>();
        }
    }
}
