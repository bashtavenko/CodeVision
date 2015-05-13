using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CodeVision.CSharp
{
    public class CSharpParser
    {
        public ParseResult Parse(string filePath)
        {
            var result = new ParseResult();
            string text;
            using (var sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();
            }

            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            var root = (CompilationUnitSyntax) tree.GetRoot();

            foreach (var usingDirective in root.Usings)
            {
                result.Usings.Add(usingDirective.Name.ToFullString());
            }
            
            foreach (var cls in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var @class = new CSharpClass();
                @class.ClassName = cls.Identifier.Text;
                foreach (var methodDeclarationSyntax in cls.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var method = new CSharpMethod()
                    {
                        MethodName = methodDeclarationSyntax.Identifier.Text,
                        Body = methodDeclarationSyntax.Body.ToFullString(),
                        ReturnType = methodDeclarationSyntax.ReturnType.ToFullString()
                    };
                    
                    foreach (var param in methodDeclarationSyntax.ParameterList.Parameters)
                    {
                        method.Parameters.Add(param.Identifier.Text);
                    }
                    @class.Methods.Add(method);
                }
                result.Classes.Add(@class);
            }        

            return result;
        }
    }
}
