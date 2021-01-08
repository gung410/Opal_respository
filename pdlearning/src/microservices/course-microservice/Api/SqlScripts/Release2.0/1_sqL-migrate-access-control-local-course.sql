BEGIN TRANSACTION [MigrateAccessControlData]

BEGIN TRY
	/* 1. Migrate data from SAM to Course DB */
	/****** [Department]  ******/
	INSERT INTO [MOE_Course].[dbo].[Departments]
	  ([Id], [DepartmentID])
	SELECT NEWID() AS Id, [DepartmentID]
	FROM [db_sam_dev].[org].[Department]

	/****** [DepartmentTypes]  ******/
	INSERT INTO [MOE_Course].[dbo].[DepartmentTypes]
	  ([Id], [DepartmentTypeID], [ExtID])
	SELECT NEWID() AS Id, [DepartmentTypeID], [ExtID]
	FROM [db_sam_dev].[org].[DepartmentType]

	/****** [[DepartmentTypeDepartments]]  ******/
	INSERT INTO [MOE_Course].[dbo].[DepartmentTypeDepartments]
	  ([Id], [DepartmentTypeID], [DepartmentID])
	SELECT NEWID() AS Id, [DepartmentTypeID], [DepartmentID]
	FROM [db_sam_dev].[org].[DT_D]

	/****** Users  ******/
	INSERT INTO [MOE_Course].[dbo].[Users]
	  ([Id], [UserID], [FirstName], [LastName], [Email], [DepartmentId])
	SELECT ExtID, UserID, FirstName, LastName, Email, DepartmentID
	FROM [db_sam_dev].[org].[User]
	WHERE [ExtID]
		  IN	(SELECT DISTINCT [ExtID]
	FROM [db_sam_dev].[org].[User]
	WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

	/****** [HierarchyDepartments]  ******/
	INSERT INTO [MOE_Course].[dbo].[HierarchyDepartments]
	  ([Id],[HDID],[DepartmentID],[ParentID],[Path])
	SELECT NEWID() AS ID, [HDID], [DepartmentID], [ParentID], [Path]
	FROM [db_sam_dev].[org].[H_D]

COMMIT TRANSACTION [MigrateAccessControlData]


BEGIN TRANSACTION [MigrateCourseDepartment]
	/* 2. Update access control for table: Course */
	UPDATE  c
	SET c.[DepartmentId] = u.DepartmentId
	FROM [MOE_Course].[dbo].[Course] c
	  JOIN [MOE_Course].[dbo].[Users] u on c.CreatedBy = u.Id
COMMIT TRANSACTION [MigrateCourseDepartment]
END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateAccessControlData]
END CATCH  

