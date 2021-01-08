
IF OBJECT_ID(N'mig.GenerateMigrationReport', N'U') IS NULL
BEGIN
CREATE TABLE [mig].[GenerateMigrationReport](
	ID int IDENTITY(1,1) NOT NULL,
	Folder VARCHAR(255) NULL,
	Name VARCHAR(255) NOT NULL,
	Type VARCHAR(10) NOT NULL, -- CSV/DB Table
	TotalRecords INT NULL,
	TotalRecordsAfterMig INT NULL,
	StartTime DATETIME NULL,
	EndTime DATETIME NULL,
	TimeTaken BIGINT NULL,
	PRIMARY KEY (ID)
) 

END