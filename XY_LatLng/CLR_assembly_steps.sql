--CLR ASSEMBLY SQL
USE Elmer;
DROP FUNCTION IF EXISTS ToXY, ToLngLat;
GO
DROP ASSEMBLY IF EXISTS XY_LatLng;

USE master;
GO

-- Create asymmetric key
DROP LOGIN XY_LatLng_assembler;
GO
DROP ASYMMETRIC KEY XY_LatLng_key
GO
USE master;
GO
CREATE ASYMMETRIC KEY XY_LatLng_key 
FROM EXECUTABLE FILE 
= 'D:\clr\XY_LatLng.dll'
GO

-- Create a new login with the asymmetric key

USE master;
GO
CREATE LOGIN XY_LatLng_assembler
FROM ASYMMETRIC KEY XY_LatLng_key

GO
GRANT UNSAFE ASSEMBLY TO XY_LatLng_assembler;
GO

USE Elmer;
GO


CREATE ASSEMBLY XY_LatLng
from 'D:\clr\XY_LatLng.dll'
with permission_set = UNSAFE
go

CREATE FUNCTION [dbo].[ToLngLat](@x [numeric](18, 11), @y [numeric](18, 11))
RETURNS [geometry] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [XY_LatLng].[UserDefinedFunctions].[ToLngLat]
GO

CREATE FUNCTION [dbo].[ToXY](@lng [numeric](18, 15), @lat [numeric](18, 15))
RETURNS [geometry] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [XY_LatLng].[UserDefinedFunctions].[ToXY]
GO
