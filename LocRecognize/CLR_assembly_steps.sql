--CLR ASSEMBLY SQL
USE Elmer;

DROP FUNCTION IF EXISTS dbo.loc_recognize;
GO
DROP ASSEMBLY IF EXISTS LocRecognize;   
USE master;
GO

-- Create asymmetric key
DROP LOGIN LocRecognize_assembler;
GO
DROP ASYMMETRIC KEY LocRecognize_key
GO
USE master;
GO
CREATE ASYMMETRIC KEY LocRecognize_key 
FROM EXECUTABLE FILE 
= 'D:\clr\LocRecognize.dll'
GO

-- Create a new login with the asymmetric key

USE master;
GO
CREATE LOGIN LocRecognize_assembler
FROM ASYMMETRIC KEY LocRecognize_key

GO
GRANT UNSAFE ASSEMBLY TO LocRecognize_assembler;
GO

USE Elmer;
GO

CREATE ASSEMBLY LocRecognize
from 'D:\clr\LocRecognize.dll'
with permission_set = UNSAFE
GO


CREATE FUNCTION [dbo].[loc_recognize](@lng [numeric](18, 15), @lat [numeric](18, 15), @google_key [nvarchar](75))
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [LocRecognize].[LocRecognizeFunctions].[loc_recognize]
GO
