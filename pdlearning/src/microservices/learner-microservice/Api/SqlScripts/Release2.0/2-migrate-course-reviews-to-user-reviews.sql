-- Migrate data: CourseReviews to UserReviews on LEARNER
BEGIN TRY
	BEGIN TRANSACTION [MigrateCourseReviews]

		-- Set Id = NEWID()
		-- Set default ItemType is 'Course' because Release 1 only Course
		INSERT INTO [MOE_Learner].[dbo].[UserReviews](Id,CreatedDate,ChangedDate,ParentCommentId,UserId,ItemId,[Version],ItemName,
												UserFullName,CommentTitle,CommentContent,Rate,IsDeleted,CreatedBy,ChangedBy,ItemType)
												SELECT NEWID() as Id,CreatedDate,ChangedDate,ParentCommentId,UserId,CourseId,[Version],ItemName,
												UserFullName,CommentTitle,CommentContent,Rate,IsDeleted,CreatedBy,ChangedBy,'Course'
												FROM [MOE_Learner].[dbo].[CourseReviews]

	COMMIT TRANSACTION [MigrateCourseReviews]
END TRY
BEGIN CATCH
	print('There was an error.')
	ROLLBACK TRANSACTION [MigrateCourseReviews]
END CATCH
