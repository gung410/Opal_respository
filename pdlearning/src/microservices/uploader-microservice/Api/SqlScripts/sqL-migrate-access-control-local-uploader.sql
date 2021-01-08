BEGIN TRANSACTION [MigrateAccessControlData]

BEGIN TRY
    /* 1. Migrate data from SAM to Uploader DB */
    /****** [Department]  ******/
    INSERT INTO [UploaderDb].[dbo].[Departments]
      ([Id], [DepartmentID])
    SELECT NEWID() AS Id, [DepartmentID]
    FROM [db_sam_dev].[org].[Department]

    /****** [DepartmentTypes]  ******/
    INSERT INTO [UploaderDb].[dbo].[DepartmentTypes]
      ([Id], [DepartmentTypeID], [ExtID])
    SELECT NEWID() AS Id, [DepartmentTypeID], [ExtID]
    FROM [db_sam_dev].[org].[DepartmentType]

    /****** [DepartmentTypeDepartments]  ******/
    INSERT INTO [UploaderDb].[dbo].[DepartmentTypeDepartments]
      ([Id], [DepartmentTypeID], [DepartmentID])
    SELECT NEWID() AS Id, [DepartmentTypeID], [DepartmentID]
    FROM [db_sam_dev].[org].[DT_D]

    /****** Users  ******/
    INSERT INTO [UploaderDb].[dbo].[Users]
      ([Id], [UserID], [FirstName], [LastName], [Email], [DepartmentId])
    SELECT ExtID, UserID, FirstName, LastName, Email, DepartmentID
    FROM [db_sam_dev].[org].[User]
    WHERE [ExtID]
	      IN	(SELECT DISTINCT [ExtID]
    FROM [db_sam_dev].[org].[User]
    WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

    /****** [HierarchyDepartments]  ******/
    INSERT INTO [UploaderDb].[dbo].[HierarchyDepartments]
      ([Id],[HDID],[DepartmentID],[ParentID],[Path])
    SELECT NEWID() AS ID, [HDID], [DepartmentID], [ParentID], [Path]
    FROM [db_sam_dev].[org].[H_D]

    /****** [Personal Spaces]  ******/
    INSERT INTO [UploaderDb].[dbo].[PersonalSpaces]
      ([Id],[UserId], [TotalSpace], [TotalUsed], [CreatedDate])
    SELECT NEWID() AS ID, [ExtID] As UserId, CAST(10485760 as int) As TotalSpace, CAST(0 as int) As TotalUsed, GETDATE() as CreatedDate
	FROM [db_sam_dev].[org].[User]
    WHERE [ExtID]
	      IN	(SELECT DISTINCT [ExtID]
    FROM [db_sam_dev].[org].[User]
    WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

COMMIT TRANSACTION [MigrateAccessControlData]
END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateAccessControlData]
END CATCH

