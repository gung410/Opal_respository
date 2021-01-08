DELIMITER //
DROP PROCEDURE IF EXISTS prc_CSL_Generate_Migration_Report_TotalRecord_DBTable_Get//
CREATE PROCEDURE prc_CSL_Generate_Migration_Report_TotalRecord_DBTable_Get()
BEGIN
	DECLARE pTotalRecord_contentcontainer INT;
	DECLARE pTotalRecord_space INT;
	DECLARE pTotalRecord_space_membership INT;
	DECLARE pTotalRecord_post INT;
	DECLARE pTotalRecord_user_follow INT;
	DECLARE pTotalRecord_comment_blog INT;
    DECLARE pTotalRecord_comment_forum INT;
	DECLARE pTotalRecord_activity INT;
	DECLARE pTotalRecord_content INT;
	DECLARE pTotalRecord_content_tag INT;
	DECLARE pTotalRecord_content_tag_relation INT;
	DECLARE pTotalRecord_file_wallpost INT;
    DECLARE pTotalRecord_file_forum INT;
    DECLARE pTotalRecord_forum_thread INT;
    DECLARE pTotalRecord_forum_thread_revision INT;

	SELECT COUNT(id) INTO pTotalRecord_contentcontainer 
	FROM contentcontainer;

	SELECT COUNT(id) INTO pTotalRecord_space
	FROM space;

	SELECT COUNT(id) INTO pTotalRecord_space_membership
	FROM space_membership;

	SELECT COUNT(id) INTO pTotalRecord_post
	FROM post;

	SELECT COUNT(id) INTO pTotalRecord_user_follow
	FROM user_follow;

	SELECT COUNT(id) INTO pTotalRecord_comment_blog
	FROM comment
    WHERE object_model <> 'humhub\modules\forum\models\ForumThread';
    
	SELECT COUNT(id) INTO pTotalRecord_comment_forum
	FROM comment
    WHERE object_model = 'humhub\modules\forum\models\ForumThread';

	SELECT COUNT(id) INTO pTotalRecord_activity
	FROM activity;

	SELECT COUNT(id) INTO pTotalRecord_content
	FROM content;

	SELECT COUNT(id) INTO pTotalRecord_content_tag
	FROM content_tag;

	SELECT COUNT(id) INTO pTotalRecord_content_tag_relation
	FROM content_tag_relation;
    
	SELECT COUNT(id) INTO pTotalRecord_file_wallpost
	FROM comment
    WHERE object_model <> 'humhub\modules\forum\models\ForumThread';
    
	SELECT COUNT(id) INTO pTotalRecord_file_forum
	FROM comment
    WHERE object_model = 'humhub\modules\forum\models\ForumThread';
    
	SELECT COUNT(id) INTO pTotalRecord_forum_thread
	FROM forum_thread;
    
	SELECT COUNT(id) INTO pTotalRecord_forum_thread_revision
	FROM forum_thread_revision;

	SELECT	pTotalRecord_contentcontainer TotalRecord_contentcontainer, pTotalRecord_space TotalRecord_space, pTotalRecord_space_membership TotalRecord_space_membership,
			pTotalRecord_post TotalRecord_post, pTotalRecord_user_follow TotalRecord_user_follow, pTotalRecord_comment_blog TotalRecord_comment_blog, pTotalRecord_comment_forum TotalRecord_comment_forum,
            pTotalRecord_activity TotalRecord_activity, pTotalRecord_content TotalRecord_content, pTotalRecord_content_tag TotalRecord_content_tag, 
            pTotalRecord_content_tag_relation TotalRecord_content_tag_relation, pTotalRecord_file_wallpost TotalRecord_file_wallpost, pTotalRecord_file_forum TotalRecord_file_forum,
            pTotalRecord_forum_thread TotalRecord_forum_thread, pTotalRecord_forum_thread_revision TotalRecord_forum_thread_revision;
END //
