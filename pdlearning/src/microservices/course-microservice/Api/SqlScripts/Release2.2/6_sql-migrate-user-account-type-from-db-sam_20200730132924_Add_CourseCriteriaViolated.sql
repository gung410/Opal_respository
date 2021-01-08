BEGIN
	BEGIN
		UPDATE 
			[MOE_Course].[dbo].[Users]
		SET
			[MOE_Course].[dbo].[Users].[AccountType] = 'Internal'
		FROM
			[MOE_Course].[dbo].[Users]
			INNER JOIN (SELECT cUser.Id , U.Locked
							from [db_sam_dev].[org].[User] U
							JOIN [MOE_Course].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
							Where U.Locked = 1) TempTbl
		ON [MOE_Course].[dbo].[Users].Id = TempTbl.Id
	END
	BEGIN
		UPDATE 
			[MOE_Course].[dbo].[Users]
		SET
			[MOE_Course].[dbo].[Users].[AccountType] = 'External'
		FROM
			[MOE_Course].[dbo].[Users]
			INNER JOIN (SELECT cUser.Id, U.Locked
							from [db_sam_dev].[org].[User] U
							JOIN [MOE_Course].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
							Where U.Locked = 0) TempTbl1
		ON [MOE_Course].[dbo].[Users].Id = TempTbl1.Id
	END
END

-----UPDATE USERMETADATE CASE TYPE = 'DevelopmentalRole'
IF (OBJECT_ID('tempdb..#Temp2') IS NOT NULL)
BEGIN
    DROP TABLE #Temp2
END

BEGIN
	BEGIN
		select DISTINCT cUser.Id as [userId], UT.ExtID as [value]
		INTO #Temp2
		from [db_sam_dev].[org].[User] U
		JOIN [db_sam_dev].[org].[UT_U] UTU ON U.UserID = UTU.UserID
		JOIN [db_sam_dev].[org].[UserType] UT ON UTU.UserTypeID = UT.UserTypeID AND UT.ArchetypeID = 44
		JOIN [MOE_Course].[dbo].[Users] cUser on u.ExtID = CONVERT(nvarchar(256), cUser.Id)
	END
	BEGIN	
		INSERT INTO [MOE_Course].[dbo].[UserMetadata] (Id, CreatedDate, UserId, [Type], [Value])
		SELECT NEWID(), GETDATE(), CONVERT(uniqueidentifier,[userId]), 'ServiceScheme', [value]
		FROM #Temp2
	END
END

IF (OBJECT_ID('tempdb..#Temp2') IS NOT NULL)
BEGIN
    DROP TABLE #Temp2
END

-------SYNC DEPARTMENT_UNIT_TYPE FROM SAM DB
INSERT INTO [MOE_Course].[dbo].[DepartmentUnitTypeDepartment] ([Id], [CreatedDate],[DepartmentId], [DepartmentUnitTypeId], [IsDeleted])
SELECT NEWID(), GETDATE(), TempTable.[DepartmentID], CONVERT(UNIQUEIDENTIFIER, TempTable.[DepartmentUnitTypeId]), 0
FROM (SELECT [DepartmentID], JSON_VALUE([DynamicAttributes],'$.typeOfOrganizationUnits') DepartmentUnitTypeId  FROM  [db_sam_dev].[org].[Department]
		WHERE [DynamicAttributes] LIKE '%typeOfOrganizationUnits%' AND 
		JSON_VALUE([DynamicAttributes],'$.typeOfOrganizationUnits') <> '' AND
		JSON_VALUE([DynamicAttributes],'$.typeOfOrganizationUnits') IS NOT NULL) TempTable
Where NOT EXISTS (SELECT 1 FROM [MOE_Course].[dbo].[DepartmentUnitTypeDepartment] d WHERE TempTable.DepartmentID = d.DepartmentId)
