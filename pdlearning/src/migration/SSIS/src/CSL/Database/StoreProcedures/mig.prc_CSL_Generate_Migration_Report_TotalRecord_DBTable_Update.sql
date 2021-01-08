IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Generate_Migration_Report_TotalRecord_DBTable_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CSL_Generate_Migration_Report_TotalRecord_DBTable_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CSL_Generate_Migration_Report_TotalRecord_DBTable_Update] 
@TotalRecord_contentcontainer INT,
@TotalRecord_space INT,
@TotalRecord_space_membership INT,
@TotalRecord_post INT,
@TotalRecord_user_follow INT,
@TotalRecord_comment INT,
@TotalRecord_activity INT,
@TotalRecord_content INT,
@TotalRecord_content_tag INT,
@TotalRecord_content_tag_relation INT,
@TotalRecord_forum_thread INT,
@TotalRecord_forum_thread_revision INT,
@TotalRecord_file INT
AS
BEGIN


	UPDATE mig.GenerateMigrationReport
	SET TotalRecords = (CASE
						WHEN Name = 'contentcontainer' THEN  @TotalRecord_contentcontainer
						WHEN Name = 'space' THEN  @TotalRecord_space
						WHEN Name = 'space_membership' THEN  @TotalRecord_space_membership
						WHEN Name = 'post' THEN  @TotalRecord_post
						WHEN Name = 'user_follow' THEN  @TotalRecord_user_follow
						WHEN Name = 'comment' THEN  @TotalRecord_comment
						WHEN Name = 'activity' THEN  @TotalRecord_activity
						WHEN Name = 'content' THEN  @TotalRecord_content
						WHEN Name = 'content_tag' THEN  @TotalRecord_content_tag
						WHEN Name = 'content_tag_relation' THEN  @TotalRecord_content_tag_relation
						WHEN Name = 'forum_thread' THEN  @TotalRecord_forum_thread
						WHEN Name = 'forum_thread_revision' THEN  @TotalRecord_forum_thread_revision
						WHEN Name = 'file' THEN  @TotalRecord_file
						ELSE 0 
						END)
	WHERE Type = 'DB Table' AND Folder = 'CSL'
END