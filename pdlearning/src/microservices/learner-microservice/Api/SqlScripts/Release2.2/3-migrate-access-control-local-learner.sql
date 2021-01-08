BEGIN TRANSACTION [MigrateAccessControlData]

BEGIN TRY
    /* 1. Migrate data from SAM to Learner DB */

    /****** [Department]  ******/
    INSERT INTO [MOE_Learner].[dbo].[Departments]
      ([Id], [DepartmentID])
    SELECT NEWID() AS Id, [DepartmentID]
    FROM [db_sam_dev].[org].[Department]

    /****** [DepartmentTypes]  ******/
    INSERT INTO [MOE_Learner].[dbo].[DepartmentTypes]
      ([Id], [DepartmentTypeID], [ExtID])
    SELECT NEWID() AS Id, [DepartmentTypeID], [ExtID]
    FROM [db_sam_dev].[org].[DepartmentType]

    /****** [[DepartmentTypeDepartments]]  ******/
    INSERT INTO [MOE_Learner].[dbo].[DepartmentTypeDepartments]
      ([Id], [DepartmentTypeID], [DepartmentID])
    SELECT NEWID() AS Id, [DepartmentTypeID], [DepartmentID]
    FROM [db_sam_dev].[org].[DT_D]

    /****** Users  ******/
    INSERT INTO [MOE_Learner].[dbo].[Users]
      ([Id], [UserID], [FirstName], [LastName], [Email], [DepartmentId])
    SELECT ExtID, UserID, FirstName, LastName, Email, DepartmentID
    FROM [db_sam_dev].[org].[User]
    WHERE [ExtID]
	      IN	(SELECT DISTINCT [ExtID]
    FROM [db_sam_dev].[org].[User]
    WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

    /****** [HierarchyDepartments]  ******/
    INSERT INTO [MOE_Learner].[dbo].[HierarchyDepartments]
      ([Id],[HDID],[DepartmentID],[ParentID],[Path])
    SELECT NEWID() AS ID, [HDID], [DepartmentID], [ParentID], [Path]
    FROM [db_sam_dev].[org].[H_D]

COMMIT TRANSACTION [MigrateAccessControlData]
END TRY

BEGIN CATCH
	print('-> There was an error when exec your command. Nothing was updated!');
	ROLLBACK TRANSACTION [MigrateAccessControlData]
END CATCH  

