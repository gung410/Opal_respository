IF OBJECT_ID(N'opa.CSL_Migration_space', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_space(
	  id INT NOT NULL IDENTITY(1,1),
	  guid VARCHAR(45) DEFAULT NULL,
	  name NVARCHAR(1000) NOT NULL,
	  description NTEXT,
	  join_policy TINYINT DEFAULT NULL,
	  visibility TINYINT DEFAULT NULL,
	  status TINYINT NOT NULL DEFAULT '1',
	  tags TEXT,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  ldap_dn VARCHAR(255) DEFAULT NULL,
	  auto_add_new_members INT DEFAULT NULL,
	  contentcontainer_id INT DEFAULT NULL,
	  default_content_visibility TINYINT DEFAULT NULL,
	  color VARCHAR(7) DEFAULT NULL,
	  members_can_leave INT DEFAULT '1',
	  url VARCHAR(1000) DEFAULT NULL,
	PRIMARY KEY (id)
	)
END