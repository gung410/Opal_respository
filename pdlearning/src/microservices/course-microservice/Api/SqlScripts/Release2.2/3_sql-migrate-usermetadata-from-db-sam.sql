-----UPDATE USERMETADATE CASE TYPE = 'teachingLevels'
INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] ([Id], CreatedDate, UserId, [Type], [Value])
SELECT NEWID(), GETDATE(), TRY_CONVERT(uniqueidentifier, TempTable.ExtID), 'TeachingLevel', TempTable.TeachingLevel
FROM (Select T1.ExtID, T2.TeachingLevel TeachingLevel
		from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.teachingLevels') TeachingLevels  
		FROM [uat-next-competence-opal-at6qr].[org].[User] sUser
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
		WHERE [DynamicAttributes] LIKE '%teachingLevels%' AND JSON_QUERY([DynamicAttributes],'$.teachingLevels') IS NOT NULL) T1
		cross apply OPENJSON(T1.TeachingLevels) with (TeachingLevel UNIQUEIDENTIFIER '$') as T2) TempTable
Where NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.TeachingLevel)

-----UPDATE USERMETADATE CASE TYPE = 'teachingSubjects'
INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] ([Id], CreatedDate, UserId, [Type], [Value])
SELECT NEWID(), GETDATE(), TRY_CONVERT(uniqueidentifier,TempTable.ExtID), 'TeachingSubject', TempTable.TeachingSubject
FROM (Select T1.ExtID, T2.TeachingSubject TeachingSubject
		from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.teachingSubjects') TeachingSubjects  
		FROM [uat-next-competence-opal-at6qr].[org].[User] sUser
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
		WHERE [DynamicAttributes] LIKE '%teachingSubjects%' AND JSON_QUERY([DynamicAttributes],'$.teachingSubjects') IS NOT NULL) T1
		cross apply OPENJSON(T1.TeachingSubjects) with (TeachingSubject UNIQUEIDENTIFIER '$') as T2) TempTable
Where NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.TeachingSubject)

-----UPDATE USERMETADATE CASE TYPE = 'teachingCourseOfStudy'
INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] ([Id], CreatedDate, UserId, [Type], [Value])
SELECT NEWID(), GETDATE(), TRY_CONVERT(uniqueidentifier,TempTable.ExtID), 'TeachingCourseOfStudy', TempTable.TeachingCourseOfStudy
FROM (Select T1.ExtID, T2.TeachingCourseOfStudy TeachingCourseOfStudy
		from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.teachingCourseOfStudy') TeachingCourseOfStudy  
		FROM [uat-next-competence-opal-at6qr].[org].[User] sUser
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
		WHERE [DynamicAttributes] LIKE '%teachingCourseOfStudy%' AND JSON_QUERY([DynamicAttributes],'$.teachingCourseOfStudy') IS NOT NULL) T1
		cross apply OPENJSON(T1.TeachingCourseOfStudy) with (TeachingCourseOfStudy UNIQUEIDENTIFIER '$') as T2) TempTable
Where NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.TeachingCourseOfStudy)

-----UPDATE USERMETADATE CASE TYPE = 'CocurricularActivity'
INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] ([Id], CreatedDate, UserId, [Type], [Value])
SELECT NEWID(), GETDATE(), TRY_CONVERT(uniqueidentifier,TempTable.ExtID), 'CocurricularActivity', TempTable.CocurricularActivity
FROM (Select T1.ExtID, T2.CocurricularActivity CocurricularActivity
		from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.cocurricularActivities') CocurricularActivity  
		FROM [uat-next-competence-opal-at6qr].[org].[User] sUser
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
		WHERE [DynamicAttributes] LIKE '%cocurricularActivities%' AND JSON_QUERY([DynamicAttributes],'$.cocurricularActivities') IS NOT NULL) T1
		cross apply OPENJSON(T1.CocurricularActivity) with (CocurricularActivity UNIQUEIDENTIFIER '$') as T2) TempTable
Where NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.CocurricularActivity)

-----UPDATE USERMETADATE CASE TYPE = 'JobFamily'
INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] ([Id], CreatedDate, UserId, [Type], [Value])
SELECT NEWID(), GETDATE(), TRY_CONVERT(uniqueidentifier,TempTable.ExtID), 'JobFamily', TempTable.JobFamily
FROM (Select T1.ExtID, T2.JobFamily JobFamily
		from (SELECT [ExtID], JSON_QUERY([DynamicAttributes],'$.jobFamily') JobFamily  
		FROM [uat-next-competence-opal-at6qr].[org].[User] sUser
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser ON cUser.Id = TRY_CONVERT(uniqueidentifier ,sUser.ExtID)
		WHERE [DynamicAttributes] LIKE '%jobFamily%' AND JSON_QUERY([DynamicAttributes],'$.jobFamily') IS NOT NULL) T1
		cross apply OPENJSON(T1.JobFamily) with (JobFamily UNIQUEIDENTIFIER '$') as T2) TempTable
Where NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] uTemp WHERE CONVERT(varchar(256),uTemp.UserId) = TempTable.ExtID AND uTemp.Value = TempTable.JobFamily)

-----UPDATE USERMETADATE CASE TYPE = 'Track'
IF (OBJECT_ID('tempdb.#Temp1') IS NOT NULL)
BEGIN
    DROP TABLE #Temp1
END

BEGIN
	BEGIN
		select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
		INTO #Temp1
		from [uat-next-competence-opal-at6qr].[org].[User] U
		JOIN [uat-next-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
		JOIN [uat-next-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 45
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] m WHERE cUser.Id=m.[UserId]
                                                                                                AND m.[Value] = UT.ExtID)
	END
	BEGIN	
		INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'Track', [value]
		FROM #Temp1 
	END
END

IF (OBJECT_ID('tempdb.#Temp1') IS NOT NULL)
BEGIN
    DROP TABLE #Temp1
END

-----UPDATE USERMETADATE CASE TYPE = 'DevelopmentalRole'
IF (OBJECT_ID('tempdb.#Temp2') IS NOT NULL)
BEGIN
    DROP TABLE #Temp2
END

BEGIN
	BEGIN
		select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
		INTO #Temp2
		from [uat-next-competence-opal-at6qr].[org].[User] U
		JOIN [uat-next-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
		JOIN [uat-next-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 49
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
		WHERE NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] m WHERE cUser.Id=m.[UserId]
                                                                                                    AND m.[Value] = UT.ExtID)
	END
	BEGIN	
		INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'DevelopmentalRole', [value]
		FROM #Temp2
	END
END

IF (OBJECT_ID('tempdb.#Temp2') IS NOT NULL)
BEGIN
    DROP TABLE #Temp2
END

-----UPDATE USERMETADATE CASE TYPE = 'EasSubstantiveGradeBanding'
BEGIN
    INSERT INTO [uat-next-courses-opal-db].[dbo].[UserMetadata] (Id, CreatedDate, [Type], UserId, [Value])
    SELECT NEWID(), Getdate(), 'EasSubstantiveGradeBanding', userId, ExtID
    FROM (
        select distinct cUser.Id as userId ,UT.ExtID
						    from [uat-next-competence-opal-at6qr].[org].[User] U
						    JOIN [uat-next-competence-opal-at6qr].[org].[UT_U] UTU ON U.UserID = UTU.UserID
						    JOIN [uat-next-competence-opal-at6qr].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 51 AND UT.ParentID=104
						    JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
							WHERE NOT EXISTS (SELECT 1 FROM [uat-next-courses-opal-db].[dbo].[UserMetadata] m WHERE cUser.Id=m.[UserId])
    ) AS subquery
END

-----UPDATE USER STATUS
IF (OBJECT_ID('tempdb.#Temp3') IS NOT NULL)
BEGIN
    DROP TABLE #Temp3
END

BEGIN 
	BEGIN
		SELECT cUser.Id as [Id] ,E.CodeName AS [Status]
		INTO #Temp3
		from [uat-next-competence-opal-at6qr].[org].[User] U
		JOIN [uat-next-competence-opal-at6qr].[dbo].[EntityStatus] E ON U.EntityStatusID = E.EntityStatusID
		JOIN [uat-next-courses-opal-db].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
	END

	BEGIN	
		UPDATE 
			[uat-next-courses-opal-db].[dbo].[Users]
		SET
			[uat-next-courses-opal-db].[dbo].[Users].[Status] = TempTbl.[Status]
		FROM
			[uat-next-courses-opal-db].[dbo].[Users]
			INNER JOIN #Temp3 TempTbl
		ON [uat-next-courses-opal-db].[dbo].[Users].Id = TempTbl.Id
	END
END

IF (OBJECT_ID('tempdb.#Temp3') IS NOT NULL)
BEGIN
    DROP TABLE #Temp3
END
