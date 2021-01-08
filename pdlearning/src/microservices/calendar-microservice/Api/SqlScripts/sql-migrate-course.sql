/* MIGRATE COURSE DATA */

BEGIN TRANSACTION [MigrateCourseData]

BEGIN TRY

INSERT INTO [CalendarDb].[dbo].[Courses]
	([Id], [CourseName], [Status], [CreatedDate], [ChangedDate], [DeletedDate])
	SELECT [Id], [CourseName], [Status], [CreatedDate], [ChangedDate], [DeletedDate]
	FROM [db_course_local].[dbo].[Course]

COMMIT TRANSACTION [MigrateCourseData]

END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateCourseData]
END CATCH
