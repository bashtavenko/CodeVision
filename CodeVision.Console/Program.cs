using System;

namespace CodeVision.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new CommandLineConfiguration();
            if (!CommandLine.Parser.Default.ParseArguments(args, config))
            {
                return;
            }
            
            var logger = new Logger();
            var indexer = new Indexer(logger, CodeVisionConfigurationSection.Load());
            try
            {
                indexer.Index(config.ContentPath, config.FoldersToExclude);
            }
            catch (Exception ex)
            {
                logger.Log("Fatal error", ex);
            }
        }
    }
}
