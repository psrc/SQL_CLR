--CLR ASSEMBLY SQL
USE Elmer;
GO
DROP FUNCTION IF EXISTS dbo.route_mi_min;
GO
DROP ASSEMBLY IF EXISTS DistMatrix;
GO

USE master;
GO
DROP LOGIN DistMatrix_assembler;
GO
DROP ASYMMETRIC KEY DistMatrix_key;
GO

-- Create asymmetric key
CREATE ASYMMETRIC KEY DistMatrix_key 
FROM EXECUTABLE FILE 
= 'D:\clr\DistMatrix.dll'
GO

-- Create a new login with the asymmetric key

USE master;
GO
CREATE LOGIN DistMatrix_assembler
FROM ASYMMETRIC KEY DistMatrix_key;

GO
GRANT UNSAFE ASSEMBLY TO DistMatrix_assembler;
GO

USE Elmer;
GO

CREATE ASSEMBLY DistMatrix
from 'D:\clr\DistMatrix.dll'
with permission_set = EXTERNAL_ACCESS;
go

CREATE FUNCTION [dbo].[route_mi_min](@o_lng [numeric](18, 15), @o_lat [numeric](18, 15), @d_lng [numeric](18, 15), @d_lat [numeric](18, 15), @tmode [nvarchar](15), @key_b [nvarchar](75))
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [DistMatrix].[UserDefinedFunctions].[route_mi_min]
GO
