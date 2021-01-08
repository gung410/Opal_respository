IF OBJECT_ID(N'opa.CSL_Migration_activity', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_activity(
	  id int NOT NULL IDENTITY(1,1),
	  class VARCHAR(100) NOT NULL,
	  module VARCHAR(100) DEFAULT '',
	  object_model VARCHAR(100) DEFAULT '',
	  object_id int NOT NULL,
	PRIMARY KEY (id)
	)
END

GO

DBCC CHECKIDENT ('opa.CSL_Migration_activity', RESEED, 3000)