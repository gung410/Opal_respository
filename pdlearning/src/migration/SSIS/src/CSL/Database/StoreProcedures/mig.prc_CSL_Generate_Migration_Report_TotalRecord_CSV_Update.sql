IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE name = N'prc_CSL_Generate_Migration_Report_TotalRecord_CSV_Update' AND type IN ( N'P', N'PC' )  AND SCHEMA_NAME(schema_id) = 'mig')
	EXEC ('CREATE PROC [mig].[prc_CSL_Generate_Migration_Report_TotalRecord_CSV_Update] AS ')
GO
ALTER PROCEDURE [mig].[prc_CSL_Generate_Migration_Report_TotalRecord_CSV_Update]
AS
BEGIN

	DECLARE @TotalRecord_Aggregate INT
	DECLARE @TotalRecord_Aggregate_Participants INT
	DECLARE @TotalRecord_Blog INT
	DECLARE @TotalRecord_BlogEntry INT
	DECLARE @TotalRecord_BlogComment INT
	DECLARE @TotalRecord_WallPost INT
	DECLARE @TotalRecord_Forum INT
	DECLARE @TotalRecord_ForumPost INT
	DECLARE @TotalRecord_ForumThread INT
	DECLARE @TotalRecord_ForumThreadCategory INT
	DECLARE @TotalRecord_File INT

	SELECT @TotalRecord_Aggregate =	COUNT(id)
	FROM opa.Raw_Aggregate;

	SELECT @TotalRecord_Aggregate_Participants =	COUNT(id)
	FROM opa.Raw_CourseAndGroupParticipants;

	SELECT @TotalRecord_Blog =	COUNT(id)
	FROM opa.Raw_Blog;

	SELECT @TotalRecord_BlogEntry =	COUNT(id)
	FROM opa.Raw_BlogEntry;

	SELECT @TotalRecord_BlogComment =	COUNT(id)
	FROM opa.Raw_BlogComments;

	SELECT @TotalRecord_WallPost =	COUNT(id)
	FROM opa.Raw_WallPost;

	SELECT @TotalRecord_Forum =	COUNT(id)
	FROM opa.Raw_Forum;

	SELECT @TotalRecord_ForumPost =	COUNT(id)
	FROM opa.Raw_ForumPost;

	SELECT @TotalRecord_ForumThread =	COUNT(id)
	FROM opa.Raw_ForumThread;

	SELECT @TotalRecord_ForumThreadCategory =	COUNT(id)
	FROM opa.Raw_ForumThreadCategory;

	UPDATE mig.GenerateMigrationReport
	SET TotalRecords = (CASE
						WHEN Name = 'groups' THEN  @TotalRecord_Aggregate
						WHEN Name = 'group_participants' THEN  @TotalRecord_Aggregate_Participants
						WHEN Name = 'group_blogs' THEN  @TotalRecord_Blog
						WHEN Name = 'group_blog_entries' THEN  @TotalRecord_BlogEntry
						WHEN Name = 'group_blog_comments' THEN  @TotalRecord_BlogComment
						WHEN Name = 'group_wall_posts' THEN  @TotalRecord_WallPost
						WHEN Name = 'forum' THEN  @TotalRecord_Forum
						WHEN Name = 'forum_post' THEN  @TotalRecord_ForumPost
						WHEN Name = 'forum_thread' THEN  @TotalRecord_ForumThread
						WHEN Name = 'forum_thread_category' THEN  @TotalRecord_ForumThreadCategory
						ELSE 0 
						END)
	WHERE Type = 'CSV' AND Folder <> 'Repository'

	UPDATE mig.GenerateMigrationReport
	SET EndTime = GETDATE(),
		TimeTaken = DATEDIFF(SECOND, StartTime, GETDATE())
	WHERE Folder = 'CSL' AND Name = 'CSL_6.1_InsertMigrationReport' AND Type = 'Package'
END
