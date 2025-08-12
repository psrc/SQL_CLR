--CLR ASSEMBLY SQL
USE Elmer;
DROP FUNCTION IF EXISTS geocode_b, geocode_g;
GO
DROP ASSEMBLY IF EXISTS ApiGeocode;

USE master;
GO

-- Create asymmetric key
DROP LOGIN ApiGeocode_assembler;
GO
DROP ASYMMETRIC KEY ApiGeocode_key
GO
USE master;
GO
CREATE ASYMMETRIC KEY ApiGeocode_key 
FROM EXECUTABLE FILE 
= 'D:\clr\ApiGeocode.dll'
GO

-- Create a new login with the asymmetric key

USE master;
GO
CREATE LOGIN ApiGeocode_assembler
FROM ASYMMETRIC KEY ApiGeocode_key

GO
GRANT UNSAFE ASSEMBLY TO ApiGeocode_assembler;
GO

USE Elmer;
GO


CREATE ASSEMBLY ApiGeocode
from 'D:\clr\ApiGeocode.dll'
with permission_set = UNSAFE
go

CREATE FUNCTION [dbo].[geocode_b](@addressLine [nvarchar](50), @locality [nvarchar](30), @postalCode [nchar](5), @BingKey [nvarchar](70))
RETURNS [geometry] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [ApiGeocode].[Geocoders].[geocode_b]
GO

CREATE FUNCTION [dbo].[geocode_g](@addressLine [nvarchar](50), @locality [nvarchar](30), @postalCode [nchar](5), @googleKey [nvarchar](70))
RETURNS [geometry] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [ApiGeocode].[Geocoders].[geocode_g]
GO
