IF OBJECT_ID(N'opa.CSL_Migration_space_membership', N'U') IS NULL
BEGIN
	CREATE TABLE opa.CSL_Migration_space_membership(
	  space_id INT NOT NULL,
	  user_id INT NOT NULL,
	  originator_user_id VARCHAR(45) DEFAULT NULL,
	  status TINYINT DEFAULT NULL,
	  request_message TEXT,
	  last_visit DATETIME DEFAULT NULL,
	  created_at DATETIME DEFAULT NULL,
	  created_by INT DEFAULT NULL,
	  updated_at DATETIME DEFAULT NULL,
	  updated_by INT DEFAULT NULL,
	  group_id VARCHAR(255) DEFAULT 'member',
	  show_at_dashboard TINYINT DEFAULT '1',
	  can_cancel_membership INT DEFAULT '1',
	  send_notifications TINYINT DEFAULT '0',
	  id INT NOT NULL IDENTITY(1,1),
	  authclient_id VARCHAR(20) DEFAULT NULL,
	PRIMARY KEY (id)
	)
END