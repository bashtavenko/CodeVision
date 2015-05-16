using CommandLine;
using CommandLine.Text;

namespace CodeVision.Console
{
    public class CommandLineConfiguration
    {
        [Option('c', "content", Required = true, HelpText = "Content path")]
        public string ContentPath { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }   
    }
}
