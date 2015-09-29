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
        }

        [Option('c', "content", Required = true, HelpText = "Content path")]
        public string ContentPath { get; set; }

        [Option('i', "index", Required = false, HelpText = "Index path")]
        public string IndexPath { get; set; }

        [OptionList('e', "exclude", Required = false, HelpText = "Colon(:) separated folders within content path to exclude from indexing")]
        public List<string> FoldersToExclude { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }   
    }
}
