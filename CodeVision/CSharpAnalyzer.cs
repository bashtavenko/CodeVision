using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;

namespace CodeVision
{
    public class CSharpAnalyzer : Analyzer
    {
        private readonly ISet<string> _csharpStopSet;
        private readonly ISet<string> _englishStopSet;

        private static readonly string[] CSharpStopWords = 
        {
			"public","private","protected","interface","abstract",
			"null","new" ,"switch","case", "default", "base",
			"do", "if", "else","break","continue", "this", 
			"for", "static", "sealed", "readonly", "void",  
			"catch","try","throws","throw",	"class", "finally", "return" ,
			"const" , "while", "using" , "namespace" ,"true", "false",
			"a", "b", "c", "d", "e", "f", "g", "h", "i","j", "k", "l", "m", "n", "o", "p", "q", "r",
		    "s", "t", "u", "v", "w", "x", "y", "z"
	    };
	    private static readonly string[] EnglishStopWords = 
        {
	        "a", "an", "and", "are", "as", "at", "be", "but", "by",
	        "for", "if", "in", "into", "is", "it",
	        "no", "not", "of", "on", "or", "s", "such",
	        "t", "that", "the", "their", "then", "there", "these",
	        "they", "this", "to", "was", "will", "with"
	    };

        public CSharpAnalyzer()
        {
            _csharpStopSet = StopFilter.MakeStopSet(CSharpStopWords);
            _englishStopSet = StopFilter.MakeStopSet(EnglishStopWords);
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if (fieldName.Equals("comment"))
            {
                return new PorterStemFilter(new StopFilter(false, new LowerCaseTokenizer(reader), _englishStopSet));
            }
            else
            {
                return new StopFilter(false, new LowerCaseTokenizer(reader), _csharpStopSet);
            }
        }
    }
}
