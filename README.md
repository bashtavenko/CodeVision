#Code Vision

This is complete code search solution based on [Apache Lucene.net](https://github.com/StanBPublic/CodeChurnLoader)
search engine library. It consists of two parts - indexer (console application) and
web front end(Asp.Net MVC application). Indexer and web site should run on the same server.
You would need a mechanism of copying the source files to that server.

Three type of files are supported, but adding more types would be very simple:

* C# files are indexed as text and also parsed by [Roslyn](https://github.com/dotnet/roslyn) to extract syntax attributes such as
classes, methods, parameters and comments
* JavaScript files indexed as text. Common third party libraries are excluded
* SQL files indexed as text

##Local Setup

1. Run all unit tests. This will build the search index based on the included sample files
2. Ctrl+F5 CodeVision.Web This should create initial IISExpress configuration.
3. Run **CreateVdirsIISExpress.ps1** to add two virtual directories the IISExpress site.
4. Start web site again. This time you should be able to search by say "Apache". See other search terms in the unit tests.

##Server Setup
* Compile CodeVision.Console (indexer) and move it to the server
* Adjust path where you want the index built in CodeVision.Console.exe.config:
```
<CodeVision IndexPath="C:\CodeVision\Index" ContentRoot="C:\Jenkins\workspace" />
```

* Run Indexer with one parameter which is root folder for the source files

```
$root='C:\Jenkins\workspace'
$console='C:\CodeVision\Bin\CodeVision.Console.exe'
.$console -c $root
```

* Deploy web site with MSWebDeploy or any other method
* Setup two virtual directories `\searchcontent` and `\searchindex` and point them to the folders 
with source files and index

**Happy Searching!**
