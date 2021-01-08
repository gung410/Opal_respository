IF OBJECT_ID(N'opa.CSL_Migration_user_follow', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_user_follow(
	  id INT NOT NULL IDENTITY(1,1),
	  object_model VARCHAR(100) NOT NULL,
	  object_id INT NOT NULL,
	  user_id INT NOT NULL,
	  send_notifications TINYINT DEFAULT '1',
	PRIMARY KEY (id)
	)
END

GO

DBCC CHECKIDENT ('opa.CSL_Migration_user_follow', RESEED, 3000)