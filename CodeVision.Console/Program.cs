using System;
using System.Linq;
using CodeVision.Dependencies;
using CodeVision.Dependencies.Database;
using CodeVision.Dependencies.Modules;

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
                if (!string.IsNullOrEmpty(commandLine.ContentPath))
                {
                    var indexer = new Indexer(logger, configFile);
                    indexer.Index(commandLine.ContentPath, commandLine.FoldersToExclude);
                }
                
                if (commandLine.SolutionPaths != null)
                {
                    var collector = new ModulesGraphCollector(configFile.DependencyGraphConnectionString, logger);
                    collector.CollectDependencies(commandLine.SolutionPaths);
                }

                if (commandLine.Databases != null)
                {
                    var collector = new DatabaseObjectGraphCollector(configFile.TargetDatabaseConnectionString, configFile.DependencyGraphConnectionString, logger);
                    collector.CollectDependencies(commandLine.Databases);
                }
            }
            catch (Exception ex)
            {
                logger.Log("Fatal error", ex);
            }
        }
    }
}
