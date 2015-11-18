using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using NUnit.Framework;
using System.IO;
using System.Linq;


namespace CodeVision.Tests
{
    [TestFixture]
    public class CSharpSemanticModelTests
    {
        [TestCase("Lucene.Net.Memory\\MemoryIndex.cs")]
        public void CSharpSemantic_CanBuildModel(string fileName)
        {
            string text = GetFileText(fileName);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(text);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            var compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location)})
                .AddSyntaxTrees(tree);

            var members = compilation.GetTypeByMetadataName("System.String").GetMembers();
            var baseType = compilation.GetTypeByMetadataName("System.String").BaseType;

            var model = compilation.GetSemanticModel(tree);
            var classDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();
            var classSymbol = model.GetDeclaredSymbol(classDecl);            
        }

        [Test]
        public void CSharpSemantic_Workspace()
        {
            string solutionPath = @"..\..\..\CodeVision.sln";
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionPath).Result;
            var project = solution.Projects.Single(s => s.Name == "CodeVision.Console");
            var refs = project.MetadataReferences;
        }

        internal string GetFileText(string fileName)
        {            
            var filePath = Path.Combine("..\\..\\Content\\", fileName);
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }
    }    
}
