
CREATE PROCEDURE opa.prc_Humhub_Staging_ForumPosts_Insert
AS
BEGIN
	INSERT INTO opa.Staging_ForumPosts (id,thread__id,parent__id,first_post__id,owner__id,name,content,status,attach1_slot,attach1_filename,attach1_size,attach2_slot,attach2_filename,
										attach2_size,attach3_slot,attach3_filename,attach3_size,attach4_slot,attach4_filename,attach4_size,
										attach5_slot,attach5_filename,attach5_size,last_edit_uid,last_edit_time,ctime)
	SELECT  t1.id,t1.thread__id,t1.parent__id,t1.first_post__id,t1.owner__id,t1.name,t1.content,t1.status,t1.attach1_slot,t1.attach1_filename,t1.attach1_size,t1.attach2_slot,t1.attach2_filename,t1.
			attach2_size,t1.attach3_slot,t1.attach3_filename,t1.attach3_size,t1.attach4_slot,t1.attach4_filename,t1.attach4_size,t1.
			attach5_slot,t1.attach5_filename,t1.attach5_size,t1.last_edit_uid,t1.last_edit_time,t1.ctime
	FROM opa.Raw_ForumPosts t1
	LEFT JOIN opa.Staging_ForumPosts t2 ON t1.id = t2.id
	WHERE t2.id IS NULL
END
GO
