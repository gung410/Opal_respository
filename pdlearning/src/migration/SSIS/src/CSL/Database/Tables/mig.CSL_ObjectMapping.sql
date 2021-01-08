IF OBJECT_ID(N'mig.CSL_ObjectMapping', N'U') IS NULL
BEGIN
	CREATE TABLE mig.CSL_ObjectMapping(
	  id int NOT NULL IDENTITY(1,1),
	  source_file VARCHAR(100) NOT NULL,
	  source_table VARCHAR(100) NOT NULL,
	  source_id VARCHAR(100) ,
	  destination_table VARCHAR(100) ,
	  destination_id VARCHAR(100) NOT NULL,
	  PRIMARY KEY (id)
	)
END

GO

DBCC CHECKIDENT ('mig.CSL_ObjectMapping', RESEED, 1)