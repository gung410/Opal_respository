/**
* Script migrate data from SAM to CCPM/CAM
* Date: 2020/04/16
* Note: Please replace Database name to migrate data
*		- db_tmp: name of platform database from SAM such as: systemtest-competence-opal-at6qr
*		- [MOE_DigitalContent]/MOE_Form: systemtest-digital-content-opal-db/systemtest-omnicore-opal-forms-db
*/

/****** [Department]  ******/
INSERT INTO [MOE_DigitalContent].[dbo].[Departments] 
	([Id], [DepartmentID])
SELECT NEWID() AS Id, [DepartmentID]
FROM db_tmp.[org].[Department]

/****** [DepartmentTypes]  ******/
INSERT INTO [MOE_DigitalContent].[dbo].[DepartmentTypes]
	([Id], [DepartmentTypeID], [ExtID])
SELECT  NEWID() AS Id, [DepartmentTypeID], [ExtID]
FROM [db_tmp].[org].[DepartmentType]

/****** [[DepartmentTypeDepartments]]  ******/
INSERT INTO [MOE_DigitalContent].[dbo].[DepartmentTypeDepartments]
	([Id], [DepartmentTypeID], [DepartmentID])
SELECT NEWID() AS Id, [DepartmentTypeID], [DepartmentID]
  FROM [db_tmp].[org].[DT_D]

/****** Users  ******/
INSERT INTO [MOE_DigitalContent].[dbo].[Users]
	([Id], [UserID], [FirstName], [LastName], [Email], [DepartmentId])
SELECT ExtID, UserID, FirstName, LastName, Email, DepartmentID
FROM db_tmp.[org].[User]
WHERE [ExtID] 
	  IN	(SELECT DISTINCT [ExtID] 
			FROM db_tmp.[org].[User]
			WHERE TRY_CONVERT(UNIQUEIDENTIFIER, [ExtID]) IS NOT NULL)

/****** [HierarchyDepartments]  ******/
INSERT INTO [MOE_DigitalContent].[dbo].[HierarchyDepartments] 
([Id],[HDID],[DepartmentID],[ParentID],[Path])
SELECT NEWID() AS ID,[HDID],[DepartmentID],[ParentID],[Path]
  FROM db_tmp.[org].[H_D]
