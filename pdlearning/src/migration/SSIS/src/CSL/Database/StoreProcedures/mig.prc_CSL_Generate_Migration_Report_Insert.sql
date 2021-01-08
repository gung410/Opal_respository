IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Generate_Migration_Report_Insert' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CSL_Generate_Migration_Report_Insert] AS ')
GO
ALTER PROCEDURE [mig].[prc_CSL_Generate_Migration_Report_Insert]
AS
BEGIN
	INSERT INTO mig.GenerateMigrationReport(Folder, Name, Type, TotalRecords, StartTime, EndTime, TimeTaken)
	VALUES('Group Collaboration','groups','CSV',NULL,NULL,NULL,NULL),
	('Group Collaboration','group_participants','CSV',NULL,NULL,NULL,NULL),
	('Blog','group_blogs','CSV',NULL,NULL,NULL,NULL),
	('Blog','group_blog_entries','CSV',NULL,NULL,NULL,NULL),
	('Blog','group_blog_comments','CSV',NULL,NULL,NULL,NULL),
	('Wall','group_wall_posts','CSV',NULL,NULL,NULL,NULL),
	('Forum','forum','CSV',NULL,NULL,NULL,NULL),
	('Forum','forum_post','CSV',NULL,NULL,NULL,NULL),
	('Forum','forum_thread','CSV',NULL,NULL,NULL,NULL),
	('Forum','forum_thread_category','CSV',NULL,NULL,NULL,NULL),
	('CSL','contentcontainer','DB Table',NULL,NULL,NULL,NULL),
	('CSL','space','DB Table',NULL,NULL,NULL,NULL),
	('CSL','space_membership','DB Table',NULL,NULL,NULL,NULL),
	('CSL','post','DB Table',NULL,NULL,NULL,NULL),
	('CSL','user_follow','DB Table',NULL,NULL,NULL,NULL),
	('CSL','comment','DB Table',NULL,NULL,NULL,NULL),
	('CSL','activity','DB Table',NULL,NULL,NULL,NULL),
	('CSL','content','DB Table',NULL,NULL,NULL,NULL),
	('CSL','content_tag','DB Table',NULL,NULL,NULL,NULL),
	('CSL','content_tag_relation','DB Table',NULL,NULL,NULL,NULL),
	('CSL','forum_thread','DB Table',NULL,NULL,NULL,NULL),
	('CSL','forum_thread_revision','DB Table',NULL,NULL,NULL,NULL),
	('CSL','file','DB Table',NULL,NULL,NULL,NULL),
	('CSL','CSL_6.1_InsertMigrationReport','Package',NULL,GETDATE(),NULL,NULL)
END