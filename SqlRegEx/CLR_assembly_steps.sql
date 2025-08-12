--CLR ASSEMBLY SQL
USE Elmer;
GO
DROP FUNCTION IF EXISTS dbo.rgx_extract, dbo.rgx_replace, dbo.rgx_find, dbo.rgx_matches;
GO

USE master;
GO

-- Create asymmetric key
DROP LOGIN SqlRegEx_assembler;
GO
DROP ASYMMETRIC KEY SqlRegEx_key;

CREATE ASYMMETRIC KEY SqlRegEx_key 
FROM EXECUTABLE FILE 
= 'D:\clr\SqlRegEx.dll'
GO

-- Create a new login with the asymmetric key

USE master;
GO
CREATE LOGIN SqlRegEx_assembler
FROM ASYMMETRIC KEY SqlRegEx_key
GO


USE Elmer;
GO
DROP ASSEMBLY SqlRegEx;
GO
CREATE ASSEMBLY SqlRegEx
from 'D:\clr\SqlRegEx.dll'
with permission_set = SAFE
go

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE FUNCTION [dbo].[rgx_find](@Input [nvarchar](max), @Pattern [nvarchar](max), @IgnoreCase [bit] = False)
RETURNS [bit] WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlRegEx].[SqlClrTools.SqlRegEx].[RegExIsMatch]
GO

CREATE FUNCTION [dbo].[rgx_matches](@Input [nvarchar](max), @Pattern [nvarchar](max), @IgnoreCase [bit] = False)
RETURNS  TABLE (
	[match] [nvarchar](max) NULL,
	[match_index] [int] NULL,
	[match_length] [int] NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlRegEx].[SqlClrTools.SqlRegEx].[RegExMatches];
GO

CREATE FUNCTION [dbo].[rgx_split](@Input [nvarchar](max), @Pattern [nvarchar](max), @IgnoreCase [bit] = False)
RETURNS  TABLE (
	[match] [nvarchar](max) NULL
) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlRegEx].[SqlClrTools.SqlRegEx].[RegExSplit];
GO

CREATE FUNCTION [dbo].[rgx_replace](@Input [nvarchar](max), @Pattern [nvarchar](max), @Replacement [nvarchar](max), @IgnoreCase [bit])
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME [SqlRegEx].[SqlClrTools.SqlRegEx].[RegExReplace];
GO

CREATE FUNCTION [dbo].[rgx_extract](@Input [nvarchar](max), @Pattern [nvarchar](max), @IgnoreCase [bit] = 0)
RETURNS nvarchar(max)
WITH EXECUTE AS CALLER
BEGIN
    DECLARE @greedy nvarchar(max);
    WITH t1 AS (SELECT match, match_length FROM dbo.rgx_matches(@Input, @Pattern, @IgnoreCase))
    SELECT @greedy = t1.match FROM t1
        ORDER BY t1.match_length DESC OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
    RETURN @greedy
END
GO
