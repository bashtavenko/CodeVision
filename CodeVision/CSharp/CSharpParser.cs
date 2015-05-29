using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeVision.CSharp
{
    public class CSharpParser
    {
        public CSharpFileSyntax Parse(string filePath)
        {
            var result = new CSharpFileSyntax();
            string text;
            using (var sr = new StreamReader(filePath))
            {
                text = sr.ReadToEnd();
            }

            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            foreach (var usingDirective in root.Usings)
            {
                result.Usings.Add(usingDirective.Name.ToFullStringLowerNormalized());
            }

            result.Comments =  GetComments(root.DescendantTrivia());
            
            foreach (var cls in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var @class = new CSharpClass {ClassName = cls.Identifier.Text.ToLower()};
                
                if (cls.BaseList != null && cls.BaseList.Types.Count >= 1)
                {
                    @class.BaseClassName = cls.BaseList.Types[0].ToFullStringLowerNormalized();
                }
                foreach (var methodDeclarationSyntax in cls.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    var method = new CSharpMethod()
                    {
                        MethodName = methodDeclarationSyntax.Identifier.Text.ToLower(),
                        ReturnType = methodDeclarationSyntax.ReturnType.ToFullStringLowerNormalized()
                    };

                    if (methodDeclarationSyntax.Body != null)
                    {
                        method.Body = methodDeclarationSyntax.Body.ToFullString().ToLower();
                    }
                    
                    foreach (var param in methodDeclarationSyntax.ParameterList.Parameters)
                    {
                        method.Parameters.Add(param.Identifier.Text.ToLower());
                    }
                    @class.Methods.Add(method);
                }
                result.Classes.Add(@class);
            }        

            return result;
        }

        private List<string> GetComments(IEnumerable<SyntaxTrivia> syntaxTriva)
        {
            return syntaxTriva
                .Where(x => x.Kind() == SyntaxKind.MultiLineCommentTrivia ||
                            x.Kind() == SyntaxKind.SingleLineCommentTrivia)
                .Select(s => s.ToFullString())
                .ToList();
        }
    }
}
