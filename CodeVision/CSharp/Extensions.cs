using Microsoft.CodeAnalysis.CSharp;

namespace CodeVision.CSharp
{
    public static class Extensions
    {
        public static string ToFullLowerCaseString(this CSharpSyntaxNode node)
        {
            return node.ToFullString().ToLower();
        }
    }
}
