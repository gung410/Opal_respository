
ALTER PROCEDURE opa.prc_CCPM_Migration_User_Insert
AS
BEGIN
	INSERT INTO opa.Migration_User (Id,ExternalId,NRIC,FullName,DisplayName,Role,Status,Gender,Nationality,MaritalStatus,DOB,AccountExpiryDate,ServiceScheme,SubServiceScheme,Division,
									Branch,Zone,Cluster,School,Organization,OfficialEmail,AlternateEmail,PreferredEmail,Phone,PhoneExtension,FaxNo,Address,LastLogin,
									CreatedDate,CreatedBy,ChangedDate,ChangedBy)
	SELECT	NEWID(),t1.ID,t1.NRIC,t1.FullName,t1.DisplayName,t1.Role,t1.Status,
			CAST(	CASE
							WHEN t1.Gender = 'female'
								THEN 0
							ELSE 1
					END AS bit) as Gender,
			'SG',NULL,t1.DOB,t1.AccountExpiry,t1.ServiceScheme,t1.SubServiceScheme,t1.Division,
			t1.Branch,t1.Zone,t1.Cluster,t1.School,t1.Organization,t1.officialemail,t1.alternateemail,t1.preferredemail,t1.phone,NULL,NULL,NULL,NULL,
			t1.ctime,NULL,GETDATE(),NULL
	FROM opa.Staging_User t1
	LEFT JOIN opa.Migration_User t2 ON t1.NRIC = t2.NRIC
	WHERE t2.NRIC IS NULL
END
GO
