using Microsoft.CodeAnalysis.CSharp;

namespace CodeVision.CSharp
{
    public static class Extensions
    {
        public static string ToFullStringLowerNormalized(this CSharpSyntaxNode node)
        {
            if (node == null)
            {
                return null;
            }
            string identifer = node.ToFullString().Replace("\r\n", string.Empty);
            return identifer.Trim().ToLowerInvariant();
        }
    }
}
