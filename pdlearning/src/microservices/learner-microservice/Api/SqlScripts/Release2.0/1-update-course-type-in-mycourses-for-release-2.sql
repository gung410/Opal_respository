-- Currently, EF core migration is setting the default value to be empty in LEARNER
-- Need to update CourseType values
-- Because CourseType column has been created in Release 2 and is a non-null column
-- Set default CourseType is 'Microlearning' because Release 1 has a course type is microlearning
BEGIN TRY
	BEGIN TRANSACTION [UpdateCourseTypeInMyCourse]
		UPDATE [MOE_Learner].[dbo].[MyCourses]
		set CourseType = 'Microlearning'

	COMMIT TRANSACTION [UpdateCourseTypeInMyCourse]
END TRY  
BEGIN CATCH 
	print('There was an error.')
    ROLLBACK TRANSACTION [UpdateCourseTypeInMyCourse]
END CATCH
