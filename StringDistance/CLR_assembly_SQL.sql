-- CLR Assembly SQL for String Distance
USE master;
GO
CREATE ASYMMETRIC KEY StringDistance_key 
FROM EXECUTABLE FILE 
= 'D:\clr\StringDistance.dll'
GO

-- Create a new login with the asymmetric key
USE master;
GO
CREATE LOGIN StringDistance_assembler
FROM ASYMMETRIC KEY StringDistance_key
GO

USE Elmer;
GO

CREATE ASSEMBLY StringDistance
from 'D:\clr\StringDistance.dll'
with permission_set = SAFE
go

CREATE FUNCTION [dbo].[dl_distance](@string_a [nvarchar](4000), @string_b [nvarchar](4000))
RETURNS [int] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [StringDistance].[StringDistance].[GetDamerauLevenshteinDistance]
GO
