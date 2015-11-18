using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace CodeVision.Console
{
    public class CommandLineConfiguration
    {
        public CommandLineConfiguration()
        {
            FoldersToExclude = new List<string>();
            SolutionPaths = new List<string>();
        }

        [Option('c', "content", Required = false, HelpText = "Content path", MutuallyExclusiveSet = "index")]        
        public string ContentPath { get; set; }

        [Option('i', "index", Required = false, HelpText = "Index path", MutuallyExclusiveSet = "index")]
        public string IndexPath { get; set; }

        [OptionList('e', "exclude", Required = false, HelpText = "Colon(:) separated folders within content path to exclude from indexing", MutuallyExclusiveSet = "index")]
        public List<string> FoldersToExclude { get; set; }

        [OptionList('s', "solutions", Required = false, HelpText = "Colon(:) separated absolute path to the solutions for module dependency graph", MutuallyExclusiveSet = "ModuleGraph")]
        public List<string> SolutionPaths { get; set; }

        [OptionList('d', "databases", Required = false, HelpText = "Colon(:) separated database names for database dependency graph", MutuallyExclusiveSet = "DatabseGraph")]
        public List<string> Databases { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }   
    }
}
