

----------------------------- Migrate Designation from SAM

INSERT INTO [CourseDb].[dbo].[UserMetadata] ([Id], CreatedDate, UserId, [Type], [Value])
SELECT NEWID(), GETDATE(), TRY_CONVERT(uniqueidentifier,TempTable.ExtID), 'Designation', TempTable.Designation
FROM ( SELECT [ExtID], JSON_VALUE([DynamicAttributes],'$.designation') Designation  
		FROM [db_sam_dev].[org].[User] sUser
		JOIN [CourseDb].[dbo].[Users] cUser ON CONVERT(nvarchar(256),cUser.Id) = sUser.ExtID
		WHERE [DynamicAttributes] LIKE '%designation%' AND JSON_VALUE([DynamicAttributes],'$.designation') IS NOT NULL) TempTable
Where NOT EXISTS (SELECT * FROM [CourseDb].[dbo].[UserMetadata] uTemp WHERE CONVERT(nvarchar(256),uTemp.UserId) = TempTable.ExtID AND 
																									uTemp.[Value] = TempTable.Designation)

------------------------------ Migrate LearningFramework from SAM
IF (OBJECT_ID('tempdb..#Temp2') IS NOT NULL) 
	BEGIN DROP TABLE #Temp2 
END 

BEGIN 
	BEGIN 
	select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
	INTO #Temp2
	from [db_sam_dev].[org].[User] U 
	JOIN [db_sam_dev].[org].[UT_U] UTU ON U.UserID = UTU.UserID 
	JOIN [db_sam_dev].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 51 
	JOIN [CourseDb].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
	WHERE NOT EXISTS (SELECT * FROM [CourseDb].[dbo].[UserMetadata] m
							WHERE cUser.Id = m.[UserId] AND m.[Value] = UT.ExtID) 
	END 

	BEGIN
		INSERT INTO [CourseDb].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'LearningFramework', [value]
		FROM #Temp2 
	END
END 

IF (OBJECT_ID('tempdb..#Temp2') IS NOT NULL) 
	BEGIN DROP TABLE #Temp2 
END 
