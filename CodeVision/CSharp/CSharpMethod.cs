using System.Collections.Generic;

namespace CodeVision.CSharp
{
    public class CSharpMethod
    {
        public string MethodName { get; set; }
        public List<string> Parameters { get; set; }
        public string Body{ get; set; }
        public string ReturnType { get; set; }

        public CSharpMethod()
        {
            Parameters = new List<string>();    
        }
    }
}
