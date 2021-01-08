BEGIN
	UPDATE [CourseDb].[dbo].[Departments]
	SET 
		[CourseDb].[dbo].[Departments].[Name] = CONVERT(nvarchar(256), originalDepartment.[Name])
	FROM
		[CourseDb].[dbo].[Departments] courseDepartment
		JOIN [competence-opal-at6qr].[dbo].[Department] originalDepartment
		on originalDepartment.DepartmentID = courseDepartment.DepartmentID
END
