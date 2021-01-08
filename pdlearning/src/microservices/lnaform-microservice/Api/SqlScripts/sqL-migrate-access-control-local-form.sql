BEGIN TRANSACTION [MigrateAccessControlData]

BEGIN TRY
    /* 1. Migrate data from SAM to Form DB */

    /****** [Department]  ******/
    INSERT INTO [MOE_Form].[dbo].[Departments]
      ([Id], [DepartmentID])
    SELECT NEWID() AS Id, [DepartmentID]
    FROM [db_sam_dev].[org].[Department]

    /****** [DepartmentTypes]  ******/
    INSERT INTO [MOE_Form].[dbo].[DepartmentTypes]
      ([Id], [DepartmentTypeID], [ExtID])
    SELECT NEWID() AS Id, [DepartmentTypeID], [ExtID]
    FROM [db_sam_dev].[org].[DepartmentType]

    /****** [[DepartmentTypeDepartments]]  ******/
    INSERT INTO [MOE_Form].[dbo].[DepartmentTypeDepartments]
      ([Id], [DepartmentTypeID], [DepartmentID])
    SELECT NEWID() AS Id, [DepartmentTypeID], [DepartmentID]
    FROM [db_sam_dev].[org].[DT_D]

    /****** Users  ******/
    INSERT INTO [MOE_Form].[dbo].[Users]
      ([Id], [UserID], [FirstName], [LastName], [Email], [DepartmentId])
    SELECT ExtID, UserID, FirstName, LastName, Email, DepartmentID
    FROM [db_sam_dev].[org].[User]
    WHERE [ExtID]
	      IN	(SELECT DISTINCT [ExtID]
    FROM [db_sam_dev].[org].[User]
    WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

    /****** [HierarchyDepartments]  ******/
    INSERT INTO [MOE_Form].[dbo].[HierarchyDepartments]
      ([Id],[HDID],[DepartmentID],[ParentID],[Path])
    SELECT NEWID() AS ID, [HDID], [DepartmentID], [ParentID], [Path]
    FROM [db_sam_dev].[org].[H_D]

COMMIT TRANSACTION [MigrateAccessControlData]

BEGIN TRANSACTION [MigrateCourseDepartment]
    /* 2. Update access control for table: Form  ******/
    UPDATE  f
    SET f.[DepartmentId] = u.DepartmentId
    FROM [MOE_Form].[dbo].[Forms] f
      JOIN [MOE_Form].[dbo].[Users] u on f.OwnerId = u.Id
COMMIT TRANSACTION [MigrateCourseDepartment]
END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateAccessControlData]
END CATCH  

