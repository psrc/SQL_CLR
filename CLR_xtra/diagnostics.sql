SELECT * from sys.assembly_modules
SELECT * from sys.assembly_files
SELECT
  a.name AssemblyName,
  f.name FileName,
  f.content Dll
FROM sys.assemblies a
JOIN sys.assembly_files f ON f.assembly_id = a.assembly_id

select * from sys.dm_clr_properties

SELECT *
FROM sys.assemblies a
JOIN sys.assembly_modules af ON a.assembly_id = af.assembly_id
WHERE a.name = 'SqlRegEx';


SELECT *
FROM sys.assembly_modules;

SELECT *
FROM sys.assemblies a
JOIN sys.assembly_modules af ON a.assembly_id = af.assembly_id

SELECT * FROM sys.dm_clr_loaded_assemblies;

SELECT SERVERPROPERTY('Edition'), SERVERPROPERTY('ProductVersion'), SERVERPROPERTY('EngineEdition');