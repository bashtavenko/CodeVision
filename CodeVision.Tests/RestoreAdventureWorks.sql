-- Download AdventureWorks2012 database from http://msftdbprodsamples.codeplex.com/downloads/get/478214
create database AdventureWorks2012 on
(filename = 'C:\My\CodeVision\AdventureWorks2012_Database\AdventureWorks2012_Data.mdf'),
(filename = 'C:\My\CodeVision\AdventureWorks2012_Database\AdventureWorks2012_Log.ldf') for attach;

-- drop database AdventureWorks2012;