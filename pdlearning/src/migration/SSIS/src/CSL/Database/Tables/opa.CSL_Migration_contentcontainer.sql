IF OBJECT_ID(N'opa.CSL_Migration_contentcontainer', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_contentcontainer(
	  id INT NOT NULL IDENTITY(1,1),
	  guid CHAR(36) NOT NULL,
	  class CHAR(60) NOT NULL,
	  pk INT DEFAULT NULL,
	  owner_user_id INT DEFAULT NULL,
	PRIMARY KEY (id)
	)
END