BEGIN
	UPDATE [CourseDb].[dbo].[Users]
SET
	[CourseDb].[dbo].[Users].[PhoneNumber] = CONVERT(nvarchar(50), u.Mobile)
FROM
	[CourseDb].[dbo].[Users] cU
	INNER JOIN [db_sam_dev].[org].[User] U 
	on u.ExtID = CONVERT(nvarchar(256), cU.Id)
END
