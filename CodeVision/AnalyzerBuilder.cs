using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace CodeVision
{
    public class AnalyzerBuilder
    {
        public static Analyzer CreateAnalyzer()
        {
            var analyzer = new PerFieldAnalyzerWrapper(new SimpleAnalyzer());
            analyzer.AddAnalyzer(Fields.Base, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Class, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Code, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Comment, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Interface, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Method, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Parameter, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Return, new CSharpAnalyzer());
            analyzer.AddAnalyzer(Fields.Using, new CSharpAnalyzer());
            return analyzer;
        }
    }
}