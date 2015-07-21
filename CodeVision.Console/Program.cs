using System;
using CodeVision.CSharp.Semantic;

namespace CodeVision.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandLine = new CommandLineConfiguration();
            if (!CommandLine.Parser.Default.ParseArguments(args, commandLine))
            {
                return;
            }
            
            var logger = new Logger();            
            try
            {
                var configFile = CodeVisionConfigurationSection.Load();
                if (commandLine.SolutionPaths.Count == 0)
                {
                    var indexer = new Indexer(logger, configFile);
                    indexer.Index(commandLine.ContentPath, commandLine.FoldersToExclude);
                }
                else
                {
                    var collector = new DependencyGraphCollector(configFile.DependencyGraphConnectionString, logger);
                    collector.CollectDependencies(commandLine.SolutionPaths);
                }
            }
            catch (Exception ex)
            {
                logger.Log("Fatal error", ex);
            }
        }
    }
}
