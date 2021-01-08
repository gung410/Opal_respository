
CREATE PROCEDURE opa.prc_CCPM_Staging_User_Insert
AS
BEGIN
	INSERT INTO opa.Staging_User (	ID,Status,NRIC,FullName,DisplayName,AccountExpiry,Role,ServiceScheme,SubServiceScheme,Gender,DOB,Designation,
									Division,Branch,Zone,Cluster,School,Organization,officialemail,alternateemail,preferredemail,phone,ctime)
	SELECT t1.ID,t1.Status,t1.NRIC,t1.FullName,t1.DisplayName,t1.AccountExpiry,t1.Role,t1.ServiceScheme,t1.SubServiceScheme,t1.Gender,t1.DOB,t1.Designation,
		   t1.Division,t1.Branch,t1.Zone,t1.Cluster,t1.School,t1.Organization,t1.officialemail,t1.alternateemail,t1.preferredemail,t1.phone,t1.ctime
	FROM opa.Raw_User t1
	LEFT JOIN opa.Staging_User t2 ON t1.ID = t2.ID
	WHERE t2.ID IS NULL
END
GO
