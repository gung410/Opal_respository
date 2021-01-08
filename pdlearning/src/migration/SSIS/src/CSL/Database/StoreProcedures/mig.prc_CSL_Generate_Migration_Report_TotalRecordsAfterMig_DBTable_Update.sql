IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Generate_Migration_Report_TotalRecordsAfterMig_DBTable_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CSL_Generate_Migration_Report_TotalRecordsAfterMig_DBTable_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CSL_Generate_Migration_Report_TotalRecordsAfterMig_DBTable_Update] 
@TotalRecord_contentcontainer INT,
@TotalRecord_space INT,
@TotalRecord_space_membership INT,
@TotalRecord_post INT,
@TotalRecord_user_follow INT,
@TotalRecord_comment_blog INT,
@TotalRecord_comment_forum INT,
@TotalRecord_activity INT,
@TotalRecord_content INT,
@TotalRecord_content_tag INT,
@TotalRecord_content_tag_relation INT,
@TotalRecord_file_wallpost INT,
@TotalRecord_file_forum INT,
@TotalRecord_forum_thread INT,
@TotalRecord_forum_thread_revision INT

AS
BEGIN

	DECLARE @TotalRecord_Blog INT;
	DECLARE @TotalRecord_BlogEntry INT;
	DECLARE @TotalRecord_WallPost INT;
	DECLARE @TotalRecord_Forum INT;
	DECLARE @TotalRecord_ForumThread INT;
	DECLARE @TotalRecord_BlogComment INT;
	DECLARE @TotalRecord_ForumComment INT;
	DECLARE @TotalRecord_WallPostFile INT;
	DECLARE @TotalRecord_ForumFile INT;

	DECLARE @TotalRecordError_Blog INT;
	DECLARE @TotalRecordError_BlogEntry INT;
	DECLARE @TotalRecordError_WallPost INT;
	DECLARE @TotalRecordError_Forum INT;
	DECLARE @TotalRecordError_ForumThread INT;
	DECLARE @TotalRecordError_BlogComment INT;
	DECLARE @TotalRecordError_ForumComment INT;

	DECLARE @TotalRecordBeforeMig_space INT;
	DECLARE @TotalRecordBeforeMig_space_membership INT;
	DECLARE @TotalRecordBeforeMig_forum_thread_revision INT;
	-- DECLARE @TotalRecordBeforeMig_comment INT;

	SELECT @TotalRecordBeforeMig_space = TotalRecords
	FROM mig.GenerateMigrationReport
	WHERE Folder = 'CSL' AND Type = 'DB Table' AND Name = 'space'

	SELECT @TotalRecordBeforeMig_space_membership = TotalRecords
	FROM mig.GenerateMigrationReport
	WHERE Folder = 'CSL' AND Type = 'DB Table' AND Name = 'space_membership'

	SELECT @TotalRecordBeforeMig_forum_thread_revision = TotalRecords
	FROM mig.GenerateMigrationReport
	WHERE Folder = 'CSL' AND Type = 'DB Table' AND Name = 'forum_thread_revision'

	UPDATE mig.GenerateMigrationReport
	SET TotalRecordsAfterMig = (CASE
								WHEN Name = 'contentcontainer' THEN  @TotalRecord_contentcontainer
								WHEN Name = 'space' THEN  @TotalRecord_space
								WHEN Name = 'space_membership' THEN  @TotalRecord_space_membership
								WHEN Name = 'post' THEN  @TotalRecord_post
								WHEN Name = 'user_follow' THEN  @TotalRecord_user_follow
								WHEN Name = 'comment' THEN  @TotalRecord_comment_blog + @TotalRecord_comment_forum
								WHEN Name = 'activity' THEN  @TotalRecord_activity
								WHEN Name = 'content' THEN  @TotalRecord_content
								WHEN Name = 'content_tag' THEN  @TotalRecord_content_tag
								WHEN Name = 'content_tag_relation' THEN  @TotalRecord_content_tag_relation
								WHEN Name = 'forum_thread' THEN  @TotalRecord_forum_thread
								WHEN Name = 'forum_thread_revision' THEN  @TotalRecord_forum_thread_revision
								WHEN Name = 'file' THEN  @TotalRecord_file_wallpost + @TotalRecord_file_forum
								ELSE 0 
								END)
	WHERE Type = 'DB Table' AND Folder = 'CSL'

	UPDATE mig.GenerateMigrationReport
	SET TotalRecordsAfterMig = (CASE
								WHEN Name = 'groups' THEN  @TotalRecord_space - @TotalRecordBeforeMig_space
								WHEN Name = 'group_participants' THEN  @TotalRecord_space_membership - @TotalRecordBeforeMig_space_membership
								WHEN Name = 'forum_post' THEN  @TotalRecord_forum_thread_revision + @TotalRecord_comment_forum - @TotalRecordBeforeMig_forum_thread_revision
								END)
	WHERE Type = 'CSV' AND Folder <> 'Repository';

	SELECT @TotalRecordError_Blog =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	JOIN mig.ErrorLog t2 ON t1.destination_id = t2.ErrorRecordId AND t1.source_file = '7. Blog' AND t1.source_table = 'opa.Staging_Blog' AND t2.FileName = 'opa.CSL_Migration_post'

	SELECT @TotalRecord_Blog =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	WHERE t1.source_file = '7. Blog' AND t1.source_table = 'opa.Staging_Blog'

	SELECT @TotalRecordError_BlogEntry =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	JOIN mig.ErrorLog t2 ON t1.destination_id = t2.ErrorRecordId AND t1.source_file = '7. Blog' AND t1.source_table = 'opa.Staging_BlogEntry' AND t2.FileName = 'opa.CSL_Migration_post'

	SELECT @TotalRecord_BlogEntry =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	WHERE t1.source_file = '7. Blog' AND t1.source_table = 'opa.Staging_BlogEntry'

	SELECT @TotalRecordError_WallPost =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	JOIN mig.ErrorLog t2 ON t1.destination_id = t2.ErrorRecordId AND t1.source_file = '5. Wall Posts' AND t1.source_table = 'opa.Staging_WallPost' AND t1.destination_table = 'post' AND t2.FileName = 'opa.CSL_Migration_post'

	SELECT @TotalRecord_WallPost =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	WHERE t1.source_file = '5. Wall Posts' AND t1.source_table = 'opa.Staging_WallPost' AND t1.destination_table = 'post'


-- 
	SELECT @TotalRecordError_Forum =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	JOIN mig.ErrorLog t2 ON t1.destination_id = t2.ErrorRecordId AND t1.source_file = '6. Forum' AND t1.source_table = 'opa.Staging_Forum' AND t2.FileName = 'opa.CSL_Migration_forum_thread'

	SELECT @TotalRecord_Forum =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	WHERE t1.source_file = '6. Forum' AND t1.source_table = 'opa.Staging_Forum'

	SELECT @TotalRecordError_ForumThread =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	JOIN mig.ErrorLog t2 ON t1.destination_id = t2.ErrorRecordId AND t1.source_file = '6. Forum' AND t1.source_table = 'opa.Staging_ForumThread' AND t2.FileName = 'opa.CSL_Migration_forum_thread'

	SELECT @TotalRecord_ForumThread =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	WHERE t1.source_file = '6. Forum' AND t1.source_table = 'opa.Staging_ForumThread'


	SELECT @TotalRecordError_BlogComment =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	JOIN mig.ErrorLog t2 ON t1.destination_id = t2.ErrorRecordId AND t1.source_file = '7. Blog' AND t1.source_table = 'opa.Staging_BlogComment' AND t2.FileName = 'opa.CSL_Migration_comment'

	SELECT @TotalRecord_BlogComment =  COUNT( distinct t1.destination_id) 
	FROM mig.CSL_ObjectMapping t1 
	WHERE t1.source_file = '7. Blog' AND t1.source_table = 'opa.Staging_BlogComment'


	UPDATE mig.GenerateMigrationReport
	SET TotalRecordsAfterMig = (CASE
								WHEN Name = 'group_blogs' THEN  @TotalRecord_Blog - @TotalRecordError_Blog
								WHEN Name = 'group_blog_entries' THEN  @TotalRecord_BlogEntry - @TotalRecordError_BlogEntry
								WHEN Name = 'group_wall_posts' THEN  @TotalRecord_WallPost - @TotalRecordError_WallPost
								WHEN Name = 'forum' THEN  @TotalRecord_Forum - @TotalRecordError_Forum
								WHEN Name = 'forum_thread' THEN  @TotalRecord_ForumThread - @TotalRecordError_ForumThread
								WHEN Name = 'group_blog_comments' THEN  @TotalRecord_BlogComment - @TotalRecordError_BlogComment
								END)
	WHERE Type = 'CSV' AND Folder <> 'Repository' AND Name NOT IN ('groups','group_participants','forum_post');

END