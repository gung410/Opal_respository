

-- step 1: create assembly on migration db
ALTER DATABASE CURRENT SET TRUSTWORTHY ON
EXEC sp_changedbowner 'sa'
GO
CREATE ASSEMBLY ReverseMarkdown from 'C:\Users\nguyet.tran\Orient Software Development Corporation\Toan Nguyen - Conexus\OSD-MOE-Learner&CCPM\Migration\Humhub\net46\ReverseMarkdown.dll' 
WITH PERMISSION_SET = UNSAFE;

-- step 2: create function
CREATE FUNCTION [dbo].[ConvertHTML](@String [nvarchar](max))
RETURNS [nvarchar](max) WITH EXECUTE AS CALLER
AS 
    EXTERNAL NAME [ReverseMarkdown].[ReverseMarkdown.StaticConvert].[Convert]
GO

-- step 3: enable clr 

EXEC sp_configure 'clr enabled', 1;  
RECONFIGURE;  
GO  

-- example 
SELECT [dbo].[ConvertHTML]('This a sample <strong>paragraph</strong> from <a href=\"http://test.com\">my site</a>')

SELECT [dbo].[ConvertHTML](ISNULL(about,''))-- , about
from opa.Staging_Aggregate

SELECT  [dbo].[ConvertHTML](ISNULL(REPLACE(CAST(t1.about AS NVARCHAR(MAX)),'<br>','CHAR(13)'),''))
 from opa.Staging_Aggregate t1


 SELECT  REPLACE(CAST([dbo].[ConvertHTML](ISNULL(about,'')) AS NVARCHAR(MAX)),'<br>','CHAR(13)')
 from opa.Staging_Aggregate t1







 SELECT  REPLACE(t1.about,'<br>','CHAR(13)'), about
 from opa.Staging_Aggregate t1
