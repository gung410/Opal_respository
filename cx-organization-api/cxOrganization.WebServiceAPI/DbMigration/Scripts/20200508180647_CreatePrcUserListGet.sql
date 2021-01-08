-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP PROCEDURE IF EXISTS dbo.prc_UserList_get
GO
-- =============================================
-- Author:		<Kian, CXVN>
-- Create date: <2020-05-08T17:07:19>
-- Description:	<For using in OPAL>
-- =============================================
CREATE PROCEDURE dbo.prc_UserList_get
	-- Add the parameters for the stored procedure here
	@TotalRow INT OUTPUT,
	@LanguageId INT=NULL,
	@PageSize INT =500 ,
	@PageIndex INT =1 ,
	@OwnerId INT = NULL ,
    @CustomerId INT = NULL,
	@SearchKey NVARCHAR(200) = null,
	@EnableSearchingSsn BIT = null,
	@GenderFilter VARCHAR(20)= null,
    @EntityStatusIdsFilter VARCHAR(50)= null,
	@UserIdsFilter VARCHAR(MAX)= null,
	@UserArchetypeIdsFilter VARCHAR(MAX)= null,
	@DepartmentIdsFilter VARCHAR(MAX)= null,
	@AgeRangeFilter VARCHAR(500)= null,--Expected format '0-19=null,20-29=null,30-39'
	@JsonDynamicAttributeFilter NVARCHAR(MAX) = null, -- '$.designation=1213485&&$.jobFamily[]=bc461658-b43a-11e9-8d38-0242ac120004=null,bc4a4a70-b43a-11e9-8441-0242ac120004=null,bc4a4d90-b43a-11e9-94d8-0242ac120004'
	@IdpNeedActivityId INT = NULL,
	@IdpPlanActivityId INT = NULL,
	@IdpNeedResultStatusTypeIdsFilter VARCHAR(50)= null,
    @IdpPlanResultStatusTypeIdsFilter VARCHAR(50)= null,
	@IdpNeedResultAllowedStatusTypeIds VARCHAR(50)= null,
	@IdpPlanResultAllowedStatusTypeIds VARCHAR(50)= null,
	@IdpNeedDefaultStatusTypeId INT = NULL,
	@IdpPlanDefaultStatusTypeId INT = NULL,
	@MultiUserTypeIdFilter VARCHAR(MAX) = null,
	@MultiUserGroupIdFilter VARCHAR(MAX) = null,
	@DepartmentTypeIdsFilter VARCHAR(MAX) = NULL,
	@CreatedAfter DATETIME = NULL,
	@CreatedBefore DATETIME = NULL,
	@ExpirationDateAfter DATETIME = NULL,
	@ExpirationDateBefore DATETIME = NULL,
	@Locked SMALLINT = NULL,
	@OrderBy VARCHAR(500) = NULL

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;

DECLARE	@SearchByFullName NVARCHAR(200);
DECLARE @idpNeedDefaultStatusTypeNo INT;
DECLARE @idpNeedDefaultStatusTypeCode NVARCHAR(50);
DECLARE @idpNeedDefaultStatusTypeName NVARCHAR(100);
DECLARE @idpNeedDefaultStatusTypeDescription NVARCHAR(500);;
DECLARE @idpPlanDefaultStatusTypeNo INT;
DECLARE @idpPlanDefaultStatusTypeCode NVARCHAR(50);
DECLARE @idpPlanDefaultStatusTypeName NVARCHAR(100);
DECLARE @idpPlanDefaultStatusTypeDescription NVARCHAR(500);

DECLARE @getIdpNeedFirst BIT;
DECLARE @getIdpPlanFirst BIT;


IF(@PageSize IS NULL OR @PageSize<=0)
 SET @PageSize=100000
IF(@PageIndex IS NULL OR @PageIndex<=0)
 SET @PageIndex=1



IF(@languageId IS NULL OR @languageId<=0)
   SELECT TOP 1 @languageId=l.LanguageID FROM dbo.[Language] l WHERE l.LanguageCode='en-US'

SET @OwnerId = NULLIF(@OwnerId,0)
SET @CustomerId = NULLIF(@CustomerId,0)
SET @SearchKey = NULLIF(@SearchKey, '')
SET @GenderFilter = NULLIF(@GenderFilter, '')
SET @EntityStatusIdsFilter = NULLIF(@EntityStatusIdsFilter, '')
SET @DepartmentIdsFilter = NULLIF(@DepartmentIdsFilter, '')
SET @UserIdsFilter = NULLIF(@UserIdsFilter, '')
SET @AgeRangeFilter = NULLIF(@AgeRangeFilter, '')


SET @MultiUserTypeIdFilter = NULLIF(@MultiUserTypeIdFilter, '')
SET @MultiUserGroupIdFilter = NULLIF(@MultiUserGroupIdFilter, '')

SET @DepartmentTypeIdsFilter=NULLIF(@DepartmentTypeIdsFilter,'')

SET @JsonDynamicAttributeFilter = NULLIF(@JsonDynamicAttributeFilter, '')

SET @IdpNeedResultStatusTypeIdsFilter = NULLIF(@IdpNeedResultStatusTypeIdsFilter, '')
SET @IdpPlanResultStatusTypeIdsFilter = NULLIF(@IdpPlanResultStatusTypeIdsFilter, '')

SET @IdpNeedResultAllowedStatusTypeIds = NULLIF(@IdpNeedResultAllowedStatusTypeIds, '') 
SET @IdpPlanResultAllowedStatusTypeIds = NULLIF(@IdpPlanResultAllowedStatusTypeIds,'')


IF(@OrderBy IS NOT NULL OR @OrderBy !='')
 SET @OrderBy='ORDER BY '+@OrderBy
ELSE SET @OrderBy='ORDER BY FirstName';--Deafult sorting


IF(@IdpNeedResultStatusTypeIdsFilter IS NOT NULL
OR CHARINDEX('NeedResultDueDate', @OrderBy)>0
OR CHARINDEX('NeedStatusTypeNo', @OrderBy)>0
OR CHARINDEX('NeedStatusTypeName', @OrderBy)>0)
BEGIN
	SET @getIdpNeedFirst=1
END 
	ELSE SET @getIdpNeedFirst=0

IF(@IdpPlanResultStatusTypeIdsFilter IS NOT NULL
OR CHARINDEX('PlanResultDueDate', @OrderBy)>0
OR CHARINDEX('PlanStatusTypeNo', @OrderBy)>0
OR CHARINDEX('PlanStatusTypeName', @OrderBy)>0)
BEGIN
	SET @getIdpPlanFirst=1
END ELSE SET @getIdpPlanFirst=0


IF (@SearchKey IS NOT NULL	AND @SearchKey != '')
BEGIN

	--If searchkey contain space (' '), we consider that is searching for user full name
	IF (CHARINDEX(' ', @SearchKey) > 0)
	BEGIN
		SET @SearchByFullName = '%' + REPLACE(@SearchKey, ' ', '%') + '%';
		SET @SearchKey = NULL;
	END
	ELSE
	BEGIN
		SET @SearchKey = '%' + @SearchKey + '%'
	END

END


SELECT	[Value] UserId INTO #userIdsFilterTbl FROM dbo.funcListToTableInt(@UserIdsFilter, ',')


SELECT
st.StatusTypeID
,st.CodeName StatusCodeName
,ISNULL([as].No, st.No) StatusTypeNo
,ISNULL(NULLIF(las.StatusName, ''), ls.Name) StatusName
,ISNULL(NULLIF(las.StatusDescription, ''), ls.Description) StatusDescription 
INTO #idpNeedStatusTypeTbl
FROM at.StatusType st
LEFT JOIN at.A_S [as] ON [as].StatusTypeID = st.StatusTypeID	AND [as].ActivityID = @idpNeedActivityId
LEFT JOIN at.LT_StatusType ls ON st.StatusTypeID = ls.StatusTypeID	AND ls.LanguageID = @languageId
LEFT JOIN at.LT_A_S las	ON las.ASID = [as].ASID	AND las.LanguageID =@languageId; 

SELECT
st2.StatusTypeID
,st2.CodeName StatusCodeName
,ISNULL([as].No, st2.No) StatusTypeNo
,ISNULL(NULLIF(las.StatusName, ''), ls.Name) StatusName
,ISNULL(NULLIF(las.StatusDescription, ''), ls.Description) StatusDescription 
INTO #idpPlanStatusTypeTbl
FROM at.StatusType st2
LEFT JOIN at.A_S [as]ON [as].StatusTypeID = st2.StatusTypeID	AND [as].ActivityID = @idpPlanActivityId
LEFT JOIN at.LT_StatusType ls ON st2.StatusTypeID = ls.StatusTypeID	AND ls.LanguageID = @languageId
LEFT JOIN at.LT_A_S las	ON las.ASID = [as].ASID		AND las.LanguageID = @languageId;

SELECT
	@IdpNeedDefaultStatusTypeId=s.StatusTypeID
   ,@idpNeedDefaultStatusTypeNo = s.StatusTypeNo
   ,@idpNeedDefaultStatusTypeName = s.StatusName
   ,@idpNeedDefaultStatusTypeCode=s.StatusCodeName
   ,@idpNeedDefaultStatusTypeDescription = s.StatusDescription
FROM #idpNeedStatusTypeTbl s
WHERE s.StatusTypeID = @idpNeedDefaultStatusTypeId;


SELECT
	@IdpPlanDefaultStatusTypeId=s.StatusTypeID
	,@idpPlanDefaultStatusTypeNo = s.StatusTypeNo
   ,@idpPlanDefaultStatusTypeName = s.StatusName
   ,@idpPlanDefaultStatusTypeCode= s.StatusCodeName
   ,@idpPlanDefaultStatusTypeDescription = s.StatusDescription
FROM #idpPlanStatusTypeTbl s
WHERE s.StatusTypeID = @IdpPlanDefaultStatusTypeId;


--For for each activity, if an user have multiple valid results (multiple version),  
--we get the latest one that matches with allowed status type ids (these status types not for fitering)/
--Otherwise, if user has only one result, we get that result

IF(@IdpNeedResultAllowedStatusTypeIds IS NOT NULL)
	SET @IdpNeedResultAllowedStatusTypeIds=','+@IdpNeedResultAllowedStatusTypeIds+',' --combine this for checking contain value exactly 





IF(@IdpPlanResultAllowedStatusTypeIds IS NOT NULL)
	SET @IdpPlanResultAllowedStatusTypeIds=','+@IdpPlanResultAllowedStatusTypeIds+','


DECLARE @HasSortingOnUserType BIT
DECLARE @HasSortingOnUserGroup BIT
IF(CHARINDEX('CareerPaths', @OrderBy) >0
 OR CHARINDEX('DevelopmentalRoles', @OrderBy) >0
 OR CHARINDEX('ExperienceCategories', @OrderBy) >0
 OR CHARINDEX('LearningFrameworks', @OrderBy) >0 
 OR CHARINDEX('PersonnelGroups', @OrderBy) >0 
 OR CHARINDEX('RoleInfos', @OrderBy) >0
 OR CHARINDEX('SystemRoleInfos', @OrderBy) >0)
 BEGIN
 --If There is any order by on user type, then we need to lookup name for sorting
SET @HasSortingOnUserType=1

SELECT piv.UserID,
CareerPath  CareerPaths,
DevelopmentalRole DevelopmentalRoles,
ExperienceCategory ExperienceCategories,
LearningFrameworkList LearningFrameworks,
PersonnelGroup PersonnelGroups,
[Role] RoleInfos,
SystemRole SystemRoleInfos
INTO #userUserTypeTbl
FROM (
SELECT uu.UserID ,a.CodeName ArchetypeCodeName  ,STRING_AGG(lut.Name, ',') WITHIN GROUP (ORDER BY lut.Name ASC) AS UTNames
		FROM org.UT_U uu
		INNER JOIN org.UserType ut	ON uu.UserTypeID = ut.UserTypeID
		INNER JOIN dbo.Archetype a ON ut.ArchetypeID = a.ArchetypeID
		LEFT JOIN org.LT_UserType lut	ON ut.UserTypeID = lut.UserTypeID	AND lut.LanguageID = 1
		WHERE (@UserIdsFilter IS NULL	OR EXISTS (SELECT 1	FROM #userIdsFilterTbl input WHERE uu.UserID = input.UserID))
		GROUP BY uu.UserID, a.CodeName) a
PIVOT(MAX(UTNames) FOR ArchetypeCodeName IN (CareerPath,
DevelopmentalRole,
ExperienceCategory,
LearningFrameworkList,
PersonnelGroup,
[Role],
SystemRole)) piv
 END

 IF(CHARINDEX('ApprovalGroups', @OrderBy) >0
 OR CHARINDEX('UserPools', @OrderBy) >0
 OR CHARINDEX('OtherGroups', @OrderBy) >0)
 BEGIN
 SET @HasSortingOnUserGroup=1

	SELECT piv.UserID,
	ApprovalGroup ApprovalGroups,
	UserPool UserPools,
	OtherGroupArchetype OtherGroups
	INTO #userGroupTbl
	FROM (SELECT UserID ,ArchetypeCodeName ,STRING_AGG(GroupName, ',') WITHIN GROUP (ORDER BY GroupName ASC) AS UGNames
		FROM (SELECT DISTINCT	ugm.UserID	 ,IIF(a.CodeName='ApprovalGroup' OR a.CodeName='UserPool', a.CodeName, 'OtherGroupArchetype') ArchetypeCodeName,ugm.UserGroupID	,IIF(a.CodeName = 'ApprovalGroup', TRIM(u.FirstName + ' ' + u.LastName), ug.Name) GroupName
			FROM org.UGMember ugm
			INNER JOIN org.UserGroup ug	ON ug.UserGroupID = ugm.UserGroupID
			LEFT JOIN org.[User] u	ON u.UserID = ug.UserID
			LEFT JOIN org.Department d  ON u.DepartmentID=d.DepartmentID
			INNER JOIN dbo.Archetype a	ON ug.ArchetypeID = a.ArchetypeID
			WHERE ugm.Deleted IS NULL AND ug.Deleted IS NULL AND (ug.UserID IS NULL OR u.Deleted IS NULL) AND (ug.DepartmentID IS NULL OR D.DepartmentID IS NULL) AND ugm.UserID IS NOT NULL
			AND  (@UserIdsFilter IS NULL OR EXISTS (SELECT 1 FROM #userIdsFilterTbl input WHERE ugm.UserID = input.UserID))
			AND ug.EntityStatusID = 1
			AND ugm.EntityStatusID = 1
			AND (ugm.ValidFrom IS NULL	OR ugm.ValidFrom <= GETDATE())
			AND (ugm.ValidTo IS NULL OR ugm.ValidTo >= GETDATE()))
			AS DistinctUGMember
		GROUP BY UserID,ArchetypeCodeName) a

		PIVOT(MAX(UGNames) FOR ArchetypeCodeName IN (ApprovalGroup,
	UserPool,
	OtherGroupArchetype)) piv
END

 DECLARE @userTempTbl TABLE(
		DepartmentID INT,
		DepartmentArchetypeID INT,
        DepartmentExtId NVARCHAR(256),
	    DepartmentName NVARCHAR(256),
		DepartmentDescription NVARCHAR(256),
		UserID INT,
		ExtID NVARCHAR(256),
		FirstName NVARCHAR(256),
		LastName NVARCHAR(256),
		Email NVARCHAR(256),
		ArchetypeID INT,
		EntityStatusID INT,
		EntityStatusReasonID INT,
		Deleted DATETIME2(7),
		Created SMALLDATETIME,
		LastUpdated DATETIME2(7),
		LastUpdatedBy  INT,
		EntityActiveDate DATETIME2(7),
		EntityExpirationDate DATETIME2(7),
		LastSynchronized DATETIME2(7),
		DynamicAttributes NVARCHAR(MAX),
		Locked SMALLINT,
		NeedResultId BIGINT,
		NeedResultExtId NVARCHAR(256),
		NeedResultDueDate DATETIME2(7),
		NeedStatusTypeId INT,
	    NeedStatusTypeNo INT,
		NeedStatusTypeCodeName NVARCHAR(256),
		NeedStatusTypeName NVARCHAR(512),
		NeedStatusTypeDescription NVARCHAR(512),
		PlanResultId BIGINT,
		PlanResultExtId	NVARCHAR(256),
		PlanResultDueDate DATETIME2(7),
		PlanStatusTypeId INT,
		PlanStatusTypeNo INT,
		PlanStatusTypeCodeName NVARCHAR(256),
		PlanStatusTypeName NVARCHAR(512),
		PlanStatusTypeDescription NVARCHAR(512),
		CareerPaths NVARCHAR(MAX),		
		DevelopmentalRoles NVARCHAR(MAX),	
		ExperienceCategories NVARCHAR(MAX),
		LearningFrameworks NVARCHAR(MAX),	
		PersonnelGroups NVARCHAR(MAX),	
		RoleInfos NVARCHAR(MAX),	
		SystemRoleInfos NVARCHAR(MAX),			
		ApprovalGroups NVARCHAR(MAX),
		UserPools NVARCHAR(MAX),
		OtherGroups NVARCHAR(MAX))


    DECLARE @SqlStatement NVARCHAR(MAX)


	--Start building dynamic SQL statment
	DECLARE @WhereStatement NVARCHAR(MAX)
	SET @WhereStatement='WHERE u.Deleted IS NULL and d.Deleted IS NULL '
	
	IF (@OwnerId IS NOT NULL)
		SET @WhereStatement = @WhereStatement+' AND u.OwnerId='+CONVERT(VARCHAR(20), @OwnerId);

	IF(@CustomerId IS NOT NULL)
		SET @WhereStatement = @WhereStatement+' AND  u.CustomerId='+CONVERT(VARCHAR(20),@CustomerId)

	IF (@UserIdsFilter IS NOT NULL)
		SET @WhereStatement = @WhereStatement + ' AND  u.UserID  in ('+@UserIdsFilter+')'

	IF (@DepartmentIdsFilter IS NOT NULL)
		SET @WhereStatement = @WhereStatement + ' AND u.DepartmentID in ('+@DepartmentIdsFilter+')'

	IF (@EntityStatusIdsFilter IS NOT NULL)
		SET @WhereStatement = @WhereStatement + ' AND  u.EntityStatusID  in ('+@EntityStatusIdsFilter+')'

	IF (@SearchByFullName IS NOT NULL)
		SET @WhereStatement = @WhereStatement + ' AND TRIM(U.FirstName+'' ''+ U.LastName) LIKE '''+@SearchByFullName+''''

	IF (@SearchKey IS NOT NULL)
		SET @WhereStatement = @WhereStatement + ' AND (U.FirstName LIKE '''+@SearchKey+''' OR U.LastName LIKE '''+@SearchKey+'''  OR U.Email LIKE'''+@SearchKey+'''  OR U.Mobile LIKE '''+@SearchKey+'''  OR ('+CONVERT(VARCHAR(1), @EnableSearchingSsn)+'=1 AND U.SSN LIKE '''+@SearchKey+'''))'

	IF (@GenderFilter IS NOT NULL)
		SET @WhereStatement = @WhereStatement + ' AND u.Gender in ('+@GenderFilter+')'	

	IF (@IdpNeedResultStatusTypeIdsFilter IS NOT NULL)
	BEGIN
		IF (@IdpNeedDefaultStatusTypeId IS NOT NULL)
			SET @WhereStatement = @WhereStatement + ' AND  (rNeed.StatusTypeID in ('+@IdpNeedResultStatusTypeIdsFilter+') OR (rNeed.ResultId IS NULL AND '+CONVERT(VARCHAR(5), @IdpNeedDefaultStatusTypeId)+' in ('+@IdpNeedResultStatusTypeIdsFilter+')))'
		ELSE
			SET @WhereStatement = @WhereStatement + ' AND  rNeed.StatusTypeID in ('+@IdpNeedResultStatusTypeIdsFilter+')'

	END
	IF (@IdpPlanResultStatusTypeIdsFilter IS NOT NULL)
	BEGIN
		IF(@IdpPlanDefaultStatusTypeId IS NOT NULL)
			SET @WhereStatement = @WhereStatement + ' AND (rPlan.StatusTypeID in ('+@IdpPlanResultStatusTypeIdsFilter+') OR  (rPlan.ResultId IS NULL AND '+CONVERT(VARCHAR(5), @IdpPlanDefaultStatusTypeId)+' in ('+@IdpPlanResultStatusTypeIdsFilter+')))'
		ELSE
			SET @WhereStatement = @WhereStatement + ' AND rPlan.StatusTypeID in ('+@IdpPlanResultStatusTypeIdsFilter+')'

    END
   IF(@DepartmentTypeIdsFilter IS NOT NULL)
   		SET @WhereStatement = @WhereStatement + ' AND EXISTS(SELECT 1 FROM org.DT_D dd WHERE dd.DepartmentID=D.DepartmentID AND dd.DepartmentTypeID IN ('+@DepartmentTypeIdsFilter+'))'
 
	IF(@CreatedAfter IS NOT NULL)	
		SET @WhereStatement = @WhereStatement + ' AND U.Created>='''+CONVERT(VARCHAR, @CreatedAfter,121)+'''' --DateTime style 121: 2020-05-10 21:33:31.027

	IF(@CreatedBefore IS NOT NULL)	
		SET @WhereStatement = @WhereStatement + ' AND U.Created<='''+CONVERT(VARCHAR, @CreatedBefore,121)+'''' 

	IF(@ExpirationDateAfter IS NOT NULL)	
		SET @WhereStatement = @WhereStatement + ' AND U.EntityExpirationDate>='''+CONVERT(VARCHAR, @ExpirationDateAfter,121)+'''' 

	IF(@ExpirationDateBefore IS NOT NULL)	
		SET @WhereStatement = @WhereStatement + ' AND U.EntityExpirationDate<='''+CONVERT(VARCHAR, @ExpirationDateBefore,121)+''''
		
	IF(@Locked IS NOT NULL)	
		SET @WhereStatement = @WhereStatement + ' AND U.Locked='+CONVERT(VARCHAR(5),@Locked)

	IF(@MultiUserTypeIdFilter IS NOT NULL)
	BEGIN
	  DECLARE @WhereOnUserType NVARCHAR(MAX) =''

      SELECT @WhereOnUserType= @WhereOnUserType+ ' AND EXISTS (SELECT 1 FROM org.UT_U uu WHERE u.UserID= uu.UserID AND uu.UserTypeID IN ('+sta.ParseValue+')) '
	  FROM dbo.StringToArray(@MultiUserTypeIdFilter,'&&')  sta
	  WHERE sta.ParseValue!=''

		SET @WhereStatement = @WhereStatement + @WhereOnUserType
	END
	IF(@MultiUserGroupIdFilter IS NOT NULL)
	BEGIN
	    DECLARE @WhereOnUserGroup NVARCHAR(MAX) =''
		
       SELECT @WhereOnUserGroup= @WhereOnUserGroup+ ' AND EXISTS (SELECT 1 FROM org.UGMember ugm WHERE ugm.UserID=u.UserID	AND ugm.EntityStatusID = 1 AND (ugm.ValidFrom IS NULL	OR ugm.ValidFrom <= GETDATE())	AND (ugm.ValidTo IS NULL OR ugm.ValidTo >= GETDATE()) AND ugm.UserGroupID IN('+sta.ParseValue+')) '
	   FROM dbo.StringToArray(@MultiUserGroupIdFilter,'&&')  sta
	   WHERE sta.ParseValue!=''

		SET @WhereStatement = @WhereStatement + @WhereOnUserGroup

	END
	IF(@AgeRangeFilter IS NOT NULL)
	BEGIN
		DECLARE @WhereOnAgeRange NVARCHAR(MAX)


		 SELECT @WhereOnAgeRange=  COALESCE(@WhereOnAgeRange + ' OR ', '')+'(U.DateOfBirth BETWEEN ''' +  CONVERT(VARCHAR(5), YEAR(GETDATE()) - CONVERT(INT, RIGHT(sta.ParseValue, LEN(sta.ParseValue) - CHARINDEX('-', sta.ParseValue)))) + '-01-01'' AND '''+ CONVERT(VARCHAR(5), YEAR(GETDATE()) - CONVERT(INT, LEFT(sta.ParseValue, CHARINDEX('-', sta.ParseValue) - 1))) + '-12-31'')'
		 FROM dbo.StringToArray(@AgeRangeFilter, ',') sta
		 WHERE sta.ParseValue!=''

		 SET @WhereStatement= @WhereStatement +' AND ('+@WhereOnAgeRange+')'
     
	END

  --Start building expresiong filter on u.DynamicAttributes
  --Example give json filter:  '$.designation=1213485&&$.jobFamily[]=bc461658-b43a-11e9-8d38-0242ac120004,bc4a4a70-b43a-11e9-8441-0242ac120004,bc4a4d90-b43a-11e9-94d8-0242ac120004'

  --For each json expression splited by '&&', we use operartor 'AND'. In side each json expresion we use operator 'OR' for filter on muitple values (contain any value)

  IF(@JsonDynamicAttributeFilter IS NOT NULL)
  BEGIN

  	SELECT sta.ParseValue JsonExpresion ,ROW_NUMBER() OVER (ORDER BY sta.ParseValue) rn INTO #temJsonTbl	FROM dbo.StringToArray(@JsonDynamicAttributeFilter, '&&') sta


	DECLARE @JsonExpresion NVARCHAR(500)
	DECLARE JSON_CURSOR CURSOR
	LOCAL STATIC READ_ONLY FORWARD_ONLY FOR SELECT DISTINCT
		JsonExpresion
	FROM #temJsonTbl

	OPEN JSON_CURSOR
	FETCH NEXT FROM JSON_CURSOR INTO @JsonExpresion
	WHILE @@fetch_status = 0
	BEGIN

	DECLARE @jsonField NVARCHAR(100)
	DECLARE @jsonValues NVARCHAR(MAX)

	SET @jsonField = LEFT(@JsonExpresion, CHARINDEX('=', @JsonExpresion) - 1)
	SET @jsonValues = RIGHT(@JsonExpresion, LEN(@JsonExpresion) - CHARINDEX('=', @JsonExpresion))

	IF (CHARINDEX('[]', @jsonField) > 0)--If json property is an array, we use JSON_QUERY
	BEGIN
		SET @jsonField = REPLACE(@jsonField, '[]', '')
		SET @WhereStatement = @WhereStatement + ' AND (' + (SELECT	STRING_AGG('JSON_QUERY(u.DynamicAttributes, ''' + @jsonField + ''') like ''%' + sta.ParseValue + '%''', ' or ')	FROM dbo.StringToArray(@jsonValues, ',') sta)+ ')'
	END

	ELSE --If json property is an single value, we use JSON_VALUE
	BEGIN
		SET @WhereStatement = @WhereStatement + ' AND (' + (SELECT	STRING_AGG('JSON_QUERY(u.DynamicAttributes, ''' + @jsonField + ''') = ''' + sta.ParseValue + '''', ' or ')	FROM dbo.StringToArray(@jsonValues, ',') sta)+ ')'
	END

	FETCH NEXT FROM JSON_CURSOR INTO @JsonExpresion
	END
	CLOSE JSON_CURSOR
	DEALLOCATE JSON_CURSOR
	DROP TABLE #temJsonTbl
	--End building expresiong filter on u.DynamicAttributes

	--END building dynamic SQL statment
  END
 
 DECLARE @SqlSelect NVARCHAR(MAX)
 DECLARE @SqlFrom NVARCHAR(MAX)
 SET @SqlSelect='SELECT	d.DepartmentID,
		d.ArchetypeID DepartmentArchetypeID,
	    d.ExtID DepartmentExtId,
	    d.Name DepartmentName,
		d.Description DepartmentDescription,
		u.UserID,
		U.ExtID,
		u.FirstName,
		u.LastName,
		u.Email,
		u.ArchetypeID,
		U.EntityStatusID,
		U.EntityStatusReasonID,
		U.Deleted,
		U.Created,
		U.LastUpdated,
		U.LastUpdatedBy,
		u.EntityActiveDate,
		u.EntityExpirationDate,
		u.LastSynchronized,
		U.DynamicAttributes,
		U.Locked'

		SET @SqlFrom=' FROM org.[User] u		
	   INNER JOIN org.Department d ON d.DepartmentID=u.DepartmentID	 '


    IF(@getIdpNeedFirst=1)
	BEGIN

		SELECT
		rTemp.ResultID
	   ,rTemp.ExtID
	   ,rTemp.UserID
	   ,rTemp.ActivityID
	   ,rTemp.SurveyID
	   ,rTemp.ValidFrom
	   ,rTemp.ValidTo
	   ,rTemp.Created
	   ,rTemp.EntityStatusID
	   ,rTemp.StatusTypeID
	   ,rTemp.DueDate
	   ,st.StatusName   
	   ,st.StatusCodeName
	   ,st.StatusDescription
	   ,st.StatusTypeNo
	   INTO #idNeedResultTblFirst

	FROM (SELECT s.ActivityID  ,r.*	,ROW_NUMBER() OVER (PARTITION BY r.UserID,s.ActivityID	ORDER BY IIF(@IdpNeedResultAllowedStatusTypeIds  IS NULL OR @IdpNeedResultAllowedStatusTypeIds LIKE (','+ CONVERT(VARCHAR(5), r.StatusTypeID)+','),0, 1), r.Created DESC) AS rownumber
		FROM dbo.Result r
		INNER JOIN at.Survey s	ON r.SurveyID = s.SurveyID
		WHERE r.Deleted IS NULL AND s.ActivityID = @idpNeedActivityId
		AND r.EntityStatusID = 1	
		AND (r.ValidFrom IS NULL OR r.ValidFrom <= GETDATE())		
		AND (r.ValidTo IS  NULL	OR r.ValidTo >= GETDATE())
		AND (@UserIdsFilter IS NULL	OR EXISTS (SELECT 1	FROM #userIdsFilterTbl input WHERE r.UserID = input.UserID)	))
		rTemp
	LEFT JOIN #idpNeedStatusTypeTbl st	ON st.StatusTypeID = rTemp.StatusTypeID

	WHERE rownumber = 1;--The latest result of user on an activity ;

		IF(@IdpNeedDefaultStatusTypeId IS NOT NULL)
		BEGIN
    			SET @SqlSelect=@SqlSelect+',
				rNeed.ResultID NeedResultId,
				rNeed.ExtID NeedResultExtId,
				rNeed.DueDate NeedResultDueDate,
				IIF(rNeed.ResultID is null, '+   CONVERT(VARCHAR (5),@idpNeedDefaultStatusTypeId)+', rNeed.StatusTypeID) NeedStatusTypeId,
				IIF(rNeed.ResultID is null,  '+  ISNULL( CONVERT(VARCHAR (5),  @idpNeedDefaultStatusTypeNo),'')+',rNeed.StatusTypeNo) NeedStatusTypeNo,
				IIF(rNeed.ResultID is null, '''+ ISNULL( @idpNeedDefaultStatusTypeCode,'')+''', rNeed.StatusCodeName) NeedStatusTypeCodeName,
				IIF(rNeed.ResultID is null, '''+ ISNULL( @idpNeedDefaultStatusTypeName,'')+''', rNeed.StatusName) NeedStatusTypeName,
				IIF(rNeed.ResultID is null,'''+  ISNULL( @idpNeedDefaultStatusTypeDescription,'')+''', rNeed.StatusDescription) NeedStatusTypeDescription '
		END
		ELSE
		BEGIN
				SET @SqlSelect=@SqlSelect+',
				rNeed.ResultID NeedResultId,
				rNeed.ExtID NeedResultExtId,
				rNeed.DueDate NeedResultDueDate,
				rNeed.StatusTypeID NeedStatusTypeId,
				rNeed.StatusTypeNo NeedStatusTypeNo,
				rNeed.StatusCodeName NeedStatusTypeCodeName,
				rNeed.StatusName NeedStatusTypeName,
				rNeed.StatusDescription NeedStatusTypeDescription '
		END
	

	SET @SqlFrom=@SqlFrom +'  LEFT JOIN #idNeedResultTblFirst rNeed ON rNeed.UserID = u.UserID	'

	END
	ELSE
	BEGIN
	SET @SqlSelect=@SqlSelect+',
		null NeedResultId,
		null NeedResultExtId,
		null NeedResultDueDate,
	    null NeedStatusTypeId,
		null NeedStatusTypeNo,
		null NeedStatusTypeCodeName,
		null NeedStatusTypeName,
		null NeedStatusTypeDescription '
	END


	IF(@getIdpPlanFirst=1)
	BEGIN
	
		SELECT
			rTemp.ResultID
		   ,rTemp.ExtID
		   ,rTemp.UserID
		   ,rTemp.ActivityID
		   ,rTemp.SurveyID
		   ,rTemp.ValidFrom
		   ,rTemp.ValidTo
		   ,rTemp.Created
		   ,rTemp.EntityStatusID
		   ,rTemp.StatusTypeID
		   ,rTemp.DueDate
		   ,st.StatusName
		   ,st.StatusCodeName
		   ,st.StatusDescription
		   ,st.StatusTypeNo 
		   INTO #idpPlanResultTblFirst
		FROM (SELECT s.ActivityID,r.*,ROW_NUMBER() OVER (PARTITION BY r.UserID,s.ActivityID	ORDER BY IIF(@IdpPlanResultAllowedStatusTypeIds  IS NULL OR @IdpPlanResultAllowedStatusTypeIds LIKE (','+ CONVERT(VARCHAR(5), r.StatusTypeID)+','),0, 1), r.Created DESC) AS rownumber
			FROM dbo.Result r
			INNER JOIN at.Survey s	ON r.SurveyID = s.SurveyID
			WHERE r.Deleted IS NULL AND  s.ActivityID = @idpPlanActivityId
			AND r.EntityStatusID = 1	
			AND (r.ValidFrom IS NULL OR r.ValidFrom <= GETDATE())
			AND (r.ValidTo IS  NULL	OR r.ValidTo >= GETDATE())
			AND (@UserIdsFilter IS NULL	OR EXISTS (SELECT 1 FROM #userIdsFilterTbl input	WHERE r.UserID = input.UserID)
			)) rTemp
		LEFT JOIN #idpPlanStatusTypeTbl st ON st.StatusTypeID = rTemp.StatusTypeID
		WHERE rownumber = 1;--The latest result of user on an activity ;

		IF(@IdpPlanDefaultStatusTypeId IS NOT NULL)
		BEGIN
				SET @SqlSelect=@SqlSelect+',
				rPlan.ResultID PlanResultId	,
				rPlan.ExtID PlanResultExtId	,
				rPlan.DueDate PlanResultDueDate,
				IIF(rPlan.ResultID is null,'+  CONVERT(VARCHAR (5),@IdpPlanDefaultStatusTypeId)+', rPlan.StatusTypeID) PlanStatusTypeId,
				IIF(rPlan.ResultID is null,'+  ISNULL( CONVERT(VARCHAR (5),@idpPlanDefaultStatusTypeNo),'')+', rPlan.StatusTypeNo) PlanStatusTypeNo,
				IIF(rPlan.ResultID is null, '''+ ISNULL( @idpPlanDefaultStatusTypeCode,'')+''',rPlan.StatusCodeName) PlanStatusTypeCodeName,
				IIF(rPlan.ResultID is null, '''+ ISNULL( @idpPlanDefaultStatusTypeName,'')+''',rPlan.StatusName) PlanStatusTypeName,
				IIF(rPlan.ResultID is null,'''+  ISNULL( @idpPlanDefaultStatusTypeDescription,'')+''',rPlan.StatusDescription) PlanStatusTypeDescription '
		END
		ELSE
		BEGIN
			SET @SqlSelect=@SqlSelect+',
				rPlan.ResultID PlanResultId	,
				rPlan.ExtID PlanResultExtId	,
				rPlan.DueDate PlanResultDueDate,
				rPlan.StatusTypeID PlanStatusTypeId,
				rPlan.StatusTypeNo PlanStatusTypeNo,
				rPlan.StatusCodeName PlanStatusTypeCodeName,
				rPlan.StatusName PlanStatusTypeName,
				rPlan.StatusDescription PlanStatusTypeDescription '
		END

		SET @SqlFrom=@SqlFrom +'  LEFT JOIN #idpPlanResultTblFirst rPlan ON rPlan.UserID = u.UserID	'

	END
	ELSE
	BEGIN
	SET @SqlSelect=@SqlSelect+',
		null PlanResultId	,
		null PlanResultExtId	,
		null PlanResultDueDate,
		null PlanStatusTypeId,
		null PlanStatusTypeNo,
		null PlanStatusTypeCodeName,
		null PlanStatusTypeName,
		null PlanStatusTypeDescription '
	END


	IF(@HasSortingOnUserType=1)
	BEGIN
		SET @SqlSelect=@SqlSelect+',
			uutt.CareerPaths,		
			uutt.DevelopmentalRoles,
			uutt.ExperienceCategories,
			uutt.LearningFrameworks,
			uutt.PersonnelGroups,
			uutt.RoleInfos,
			uutt.SystemRoleInfos '
				
		SET @SqlFrom=@SqlFrom +' LEFT JOIN #userUserTypeTbl uutt	ON uutt.UserID = u.UserID '
			
	END
	ELSE
	BEGIN
	SET @SqlSelect=@SqlSelect+',
			null CareerPaths,		
			null DevelopmentalRoles,
			null ExperienceCategories,
			null LearningFrameworks,
			null PersonnelGroups,
			null RoleInfos,
			null SystemRoleInfos '
	END

	IF(@HasSortingOnUserGroup=1)
	BEGIN
		SET @SqlSelect=@SqlSelect+',ugt.ApprovalGroups,
		ugt.UserPools,
		ugt.OtherGroups'

		SET @SqlFrom=@SqlFrom +' LEFT JOIN #userGroupTbl ugt	ON ugt.UserID = u.UserID '
	END
	ELSE
	BEGIN
	SET @SqlSelect=@SqlSelect+',
			null ApprovalGroups,		
			null UserPools,
			null OtherGroups'
			
	END

DECLARE @SqlPaging NVARCHAR(200)='OFFSET '+CONVERT(VARCHAR(10),(@PageSize * (@PageIndex - 1)))+' ROWS FETCH NEXT '+CONVERT(VARCHAR(10),@PageSize)+' ROWS ONLY' ;

IF(@OrderBy IS NULL OR @OrderBy ='')
	SET @OrderBy=' (SELECT NULL) '

		
SET @SqlStatement=@SqlSelect
+ ' '+@SqlFrom
+ ' '+@WhereStatement
+ ' '+@OrderBy
+ ' '+@SqlPaging


DECLARE @SqlCount NVARCHAR(MAX)='select @totalRow=count(*)'+ ' '+@SqlFrom+ ' '+@WhereStatement;

EXEC sp_executesql @SqlCount, N'@totalRow INT OUTPUT', @totalRow=@totalRow OUTPUT;

INSERT into @userTempTbl  EXEC( @SqlStatement)


--SELECT u.*
--INTO @userTempTbl
--FROM @userTempTbl u
--ORDER BY (SELECT NULL)
--OFFSET @PageSize * (@PageIndex - 1) ROWS
--FETCH NEXT @PageSize ROWS ONLY

IF(@getIdpNeedFirst=0)
BEGIN
SELECT
		rTemp.ResultID
	   ,rTemp.ExtID
	   ,rTemp.UserID
	   ,rTemp.ActivityID
	   ,rTemp.SurveyID
	   ,rTemp.ValidFrom
	   ,rTemp.ValidTo
	   ,rTemp.Created
	   ,rTemp.EntityStatusID
	   ,rTemp.StatusTypeID
	   ,rTemp.DueDate
	   ,st.StatusName   
	   ,st.StatusCodeName
	   ,st.StatusDescription
	   ,st.StatusTypeNo
	   INTO #idNeedResultTblLast

	FROM (SELECT s.ActivityID  ,r.*	,ROW_NUMBER() OVER (PARTITION BY r.UserID,s.ActivityID	ORDER BY IIF(@IdpNeedResultAllowedStatusTypeIds  IS NULL OR @IdpNeedResultAllowedStatusTypeIds LIKE (','+ CONVERT(VARCHAR(5), r.StatusTypeID)+','),0, 1), r.Created DESC) AS rownumber
		FROM dbo.Result r
		INNER JOIN @userTempTbl u ON u.UserID=r.UserID
		INNER JOIN at.Survey s	ON r.SurveyID = s.SurveyID
		WHERE r.Deleted IS NULL AND s.ActivityID = @idpNeedActivityId
		AND r.EntityStatusID = 1	
		AND (r.ValidFrom IS NULL OR r.ValidFrom <= GETDATE())		
		AND (r.ValidTo IS  NULL	OR r.ValidTo >= GETDATE()		))
		rTemp
	LEFT JOIN #idpNeedStatusTypeTbl st	ON st.StatusTypeID = rTemp.StatusTypeID

	WHERE rownumber = 1;--The latest result of user on an activity ;

END

IF(@getIdpPlanFirst = 0)
BEGIN
		SELECT
			rTemp.ResultID
		   ,rTemp.ExtID
		   ,rTemp.UserID
		   ,rTemp.ActivityID
		   ,rTemp.SurveyID
		   ,rTemp.ValidFrom
		   ,rTemp.ValidTo
		   ,rTemp.Created
		   ,rTemp.EntityStatusID
		   ,rTemp.StatusTypeID
		   ,rTemp.DueDate
		   ,st.StatusName
		   ,st.StatusCodeName
		   ,st.StatusDescription
		   ,st.StatusTypeNo 
		   INTO #idpPlanResultTblLast
		FROM (SELECT s.ActivityID,r.*,ROW_NUMBER() OVER (PARTITION BY r.UserID,s.ActivityID	ORDER BY IIF(@IdpPlanResultAllowedStatusTypeIds  IS NULL OR @IdpPlanResultAllowedStatusTypeIds LIKE (','+ CONVERT(VARCHAR(5), r.StatusTypeID)+','),0, 1), r.Created DESC) AS rownumber
			FROM dbo.Result r
			INNER JOIN @userTempTbl u ON u.UserID=r.UserID
			INNER JOIN at.Survey s	ON r.SurveyID = s.SurveyID
			WHERE s.ActivityID = @idpPlanActivityId
			AND r.EntityStatusID = 1	
			AND (r.ValidFrom IS NULL OR r.ValidFrom <= GETDATE())
			AND (r.ValidTo IS  NULL	OR r.ValidTo >= GETDATE())) rTemp
		LEFT JOIN #idpPlanStatusTypeTbl st ON st.StatusTypeID = rTemp.StatusTypeID
		WHERE rownumber = 1;--The latest result of user on an activity ;
END

IF(@getIdpNeedFirst=1 AND @getIdpPlanFirst=1)
BEGIN
SELECT  DepartmentID ,
		DepartmentArchetypeID,
        DepartmentExtId ,
	    DepartmentName ,
		DepartmentDescription,
		UserID,
		ExtID ,
		FirstName ,
		LastName ,
		Email,
		ArchetypeID,
		EntityStatusID,
		EntityStatusReasonID,
		Deleted,
		Created ,
		LastUpdated,
		LastUpdatedBy ,
		EntityActiveDate,
		EntityExpirationDate,
		LastSynchronized, 
		DynamicAttributes ,
		Locked,
		NeedResultId ,
		NeedResultExtId ,
		NeedResultDueDate,
		NeedStatusTypeId,
	    NeedStatusTypeNo,
		NeedStatusTypeCodeName,
		NeedStatusTypeName,
		NeedStatusTypeDescription ,
		PlanResultId,
		PlanResultExtId	,
		PlanResultDueDate,
		PlanStatusTypeId,
		PlanStatusTypeNo ,
		PlanStatusTypeCodeName,
		PlanStatusTypeName ,
		PlanStatusTypeDescription ,

	(SELECT ut.UserTypeID Id, ut.ArchetypeID, ut.ExtID, lut.Name,lut.Description
	FROM org.UT_U uu
	INNER JOIN org.UserType ut ON ut.UserTypeID=uu.UserTypeID
	LEFT JOIN org.LT_UserType lut ON lut.UserTypeID=ut.UserTypeID AND lut.LanguageID=2
	WHERE uu.UserID=U.UserID
	ORDER BY lut.Name
	FOR JSON PATH) UserTypes,

	(SELECT DISTINCT ug.UserGroupID Id, ug.ExtID, Ug.ArchetypeID, ug.UserGroupTypeID, IIF(a.CodeName = 'ApprovalGroup', TRIM(uugm.FirstName + ' ' + u.LastName), ug.Name) [Name], ug.Description,
	ug.UserID, ug.DepartmentID
	FROM org.UGMember ugm
	INNER JOIN org.UserGroup ug	ON ug.UserGroupID = ugm.UserGroupID
	LEFT JOIN org.[User] uugm	ON uugm.UserID = ug.UserID
	INNER JOIN dbo.Archetype a	ON ug.ArchetypeID = a.ArchetypeID
	WHERE ug.Deleted IS NULL and ugm.Deleted IS NULL AND ugm.UserID = u.UserID
	AND ug.EntityStatusID = 1
	AND ugm.EntityStatusID = 1
	AND (ugm.ValidFrom IS NULL	OR ugm.ValidFrom <= GETDATE())
	AND (ugm.ValidTo IS NULL OR ugm.ValidTo >= GETDATE())
	ORDER BY [Name]
	FOR JSON PATH) UserGroups

FROM @userTempTbl u  

END
ELSE IF(@getIdpNeedFirst=0 AND @getIdpPlanFirst=1)
BEGIN
SELECT  DepartmentID ,
		DepartmentArchetypeID,
        DepartmentExtId ,
	    DepartmentName ,
		DepartmentDescription,
		u.UserID,
		u.ExtID ,
		FirstName ,
		LastName ,
		Email,
		ArchetypeID,
		u.EntityStatusID,
		EntityStatusReasonID,
		Deleted,
		u.Created ,
		LastUpdated,
		LastUpdatedBy ,
		EntityActiveDate,
		EntityExpirationDate,
		LastSynchronized, 
		DynamicAttributes ,
		Locked,
		rNeed.ResultID NeedResultId,
		rNeed.ExtID NeedResultExtId,
		rNeed.DueDate NeedResultDueDate,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeId, rNeed.StatusTypeID) NeedStatusTypeId,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeNo,rNeed.StatusTypeNo) NeedStatusTypeNo,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeCode, rNeed.StatusCodeName) NeedStatusTypeCodeName,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeName, rNeed.StatusName) NeedStatusTypeName,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeDescription, rNeed.StatusDescription) NeedStatusTypeDescription, 
		PlanResultId,
		PlanResultExtId	,
		PlanResultDueDate,
		PlanStatusTypeId,
		PlanStatusTypeNo ,
		PlanStatusTypeCodeName,
		PlanStatusTypeName ,
		PlanStatusTypeDescription ,

	(SELECT ut.UserTypeID Id, ut.ArchetypeID, ut.ExtID, lut.Name,lut.Description
	FROM org.UT_U uu
	INNER JOIN org.UserType ut ON ut.UserTypeID=uu.UserTypeID
	LEFT JOIN org.LT_UserType lut ON lut.UserTypeID=ut.UserTypeID AND lut.LanguageID=2
	WHERE uu.UserID=U.UserID
	ORDER BY lut.Name
	FOR JSON PATH) UserTypes,

	(SELECT DISTINCT ug.UserGroupID Id, ug.ExtID, Ug.ArchetypeID, ug.UserGroupTypeID, IIF(a.CodeName = 'ApprovalGroup', TRIM(uugm.FirstName + ' ' + u.LastName), ug.Name) [Name], ug.Description,
	ug.UserID, ug.DepartmentID
	FROM org.UGMember ugm
	INNER JOIN org.UserGroup ug	ON ug.UserGroupID = ugm.UserGroupID
	LEFT JOIN org.[User] uugm	ON uugm.UserID = ug.UserID
	INNER JOIN dbo.Archetype a	ON ug.ArchetypeID = a.ArchetypeID
	WHERE ug.Deleted IS NULL AND ugm.Deleted IS NULL AND ugm.UserID = u.UserID
	AND ug.EntityStatusID = 1
	AND ugm.EntityStatusID = 1
	AND (ugm.ValidFrom IS NULL	OR ugm.ValidFrom <= GETDATE())
	AND (ugm.ValidTo IS NULL OR ugm.ValidTo >= GETDATE())
	ORDER BY [Name]
	FOR JSON PATH) UserGroups

FROM @userTempTbl u  
LEFT JOIN #idNeedResultTblLast rNeed ON rNeed.UserID=u.UserID
END
ELSE IF(@getIdpPlanFirst=0 AND @getIdpNeedFirst=1)
BEGIN
	SELECT  DepartmentID ,
		DepartmentArchetypeID,
        DepartmentExtId ,
	    DepartmentName ,
		DepartmentDescription,
		u.UserID,
		u.ExtID ,
		FirstName ,
		LastName ,
		Email,
		ArchetypeID,
		u.EntityStatusID,
		EntityStatusReasonID,
		Deleted,
		u.Created ,
		LastUpdated,
		LastUpdatedBy ,
		EntityActiveDate,
		EntityExpirationDate,
		LastSynchronized, 
		DynamicAttributes ,
		Locked,
		NeedResultId ,
		NeedResultExtId ,
		NeedResultDueDate,
		NeedStatusTypeId,
	    NeedStatusTypeNo,
		NeedStatusTypeCodeName,
		NeedStatusTypeName,
		NeedStatusTypeDescription ,
		rPlan.ResultID PlanResultId	,
		rPlan.ExtID PlanResultExtId	,
		rPlan.DueDate PlanResultDueDate,
		IIF(rPlan.ResultID is null,@IdpPlanDefaultStatusTypeId, rPlan.StatusTypeID) PlanStatusTypeId,
		IIF(rPlan.ResultID is null,@idpPlanDefaultStatusTypeNo, rPlan.StatusTypeNo) PlanStatusTypeNo,
		IIF(rPlan.ResultID is null, @idpPlanDefaultStatusTypeCode,rPlan.StatusCodeName) PlanStatusTypeCodeName,
		IIF(rPlan.ResultID is null, @idpPlanDefaultStatusTypeName,rPlan.StatusName) PlanStatusTypeName,
		IIF(rPlan.ResultID is null, @idpPlanDefaultStatusTypeDescription,rPlan.StatusDescription) PlanStatusTypeDescription ,

	(SELECT ut.UserTypeID Id, ut.ArchetypeID, ut.ExtID, lut.Name,lut.Description
	FROM org.UT_U uu
	INNER JOIN org.UserType ut ON ut.UserTypeID=uu.UserTypeID
	LEFT JOIN org.LT_UserType lut ON lut.UserTypeID=ut.UserTypeID AND lut.LanguageID=2
	WHERE uu.UserID=U.UserID
	ORDER BY lut.Name
	FOR JSON PATH) UserTypes,

	(SELECT DISTINCT ug.UserGroupID Id, ug.ExtID, Ug.ArchetypeID, ug.UserGroupTypeID, IIF(a.CodeName = 'ApprovalGroup', TRIM(uugm.FirstName + ' ' + u.LastName), ug.Name) [Name], ug.Description,
	ug.UserID, ug.DepartmentID
	FROM org.UGMember ugm
	INNER JOIN org.UserGroup ug	ON ug.UserGroupID = ugm.UserGroupID
	LEFT JOIN org.[User] uugm	ON uugm.UserID = ug.UserID
	INNER JOIN dbo.Archetype a	ON ug.ArchetypeID = a.ArchetypeID
	WHERE ug.Deleted IS NULL AND ugm.Deleted IS NULL and  ugm.UserID = u.UserID
	AND ug.EntityStatusID = 1
	AND ugm.EntityStatusID = 1
	AND (ugm.ValidFrom IS NULL	OR ugm.ValidFrom <= GETDATE())
	AND (ugm.ValidTo IS NULL OR ugm.ValidTo >= GETDATE())
	ORDER BY [Name]
	FOR JSON PATH) UserGroups

FROM @userTempTbl u  
LEFT JOIN #idpPlanResultTblLast rPlan ON rPlan.UserID=u.UserID

END
ELSE 
BEGIN
SELECT  DepartmentID ,
		DepartmentArchetypeID,
        DepartmentExtId ,
	    DepartmentName ,
		DepartmentDescription,
		u.UserID,
		u.ExtID ,
		FirstName ,
		LastName ,
		Email,
		ArchetypeID,
		u.EntityStatusID,
		EntityStatusReasonID,
		Deleted,
		u.Created ,
		LastUpdated,
		LastUpdatedBy ,
		EntityActiveDate,
		EntityExpirationDate,
		LastSynchronized, 
		DynamicAttributes ,
		Locked,
		rNeed.ResultID NeedResultId,
		rNeed.ExtID NeedResultExtId,
		rNeed.DueDate NeedResultDueDate,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeId, rNeed.StatusTypeID) NeedStatusTypeId,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeNo,rNeed.StatusTypeNo) NeedStatusTypeNo,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeCode, rNeed.StatusCodeName) NeedStatusTypeCodeName,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeName, rNeed.StatusName) NeedStatusTypeName,
		IIF(rNeed.ResultID is null,  @idpNeedDefaultStatusTypeDescription, rNeed.StatusDescription) NeedStatusTypeDescription, 
		rPlan.ResultID PlanResultId	,
		rPlan.ExtID PlanResultExtId	,
		rPlan.DueDate PlanResultDueDate,
		IIF(rPlan.ResultID is NULL ,@IdpPlanDefaultStatusTypeId, rPlan.StatusTypeID) PlanStatusTypeId,
		IIF(rPlan.ResultID is NULL ,@idpPlanDefaultStatusTypeNo, rPlan.StatusTypeNo) PlanStatusTypeNo,
		IIF(rPlan.ResultID is null, @idpPlanDefaultStatusTypeCode,rPlan.StatusCodeName) PlanStatusTypeCodeName,
		IIF(rPlan.ResultID is null, @idpPlanDefaultStatusTypeName,rPlan.StatusName) PlanStatusTypeName,
		IIF(rPlan.ResultID is null, @idpPlanDefaultStatusTypeDescription,rPlan.StatusDescription) PlanStatusTypeDescription ,

	(SELECT ut.UserTypeID Id, ut.ArchetypeID, ut.ExtID, lut.Name,lut.Description
	FROM org.UT_U uu
	INNER JOIN org.UserType ut ON ut.UserTypeID=uu.UserTypeID
	LEFT JOIN org.LT_UserType lut ON lut.UserTypeID=ut.UserTypeID AND lut.LanguageID=2
	WHERE uu.UserID=U.UserID
	ORDER BY lut.Name
	FOR JSON PATH) UserTypes,

	(SELECT DISTINCT ug.UserGroupID Id, ug.ExtID, Ug.ArchetypeID, ug.UserGroupTypeID, IIF(a.CodeName = 'ApprovalGroup', TRIM(uugm.FirstName + ' ' + u.LastName), ug.Name) [Name], ug.Description,
	ug.UserID, ug.DepartmentID
	FROM org.UGMember ugm
	INNER JOIN org.UserGroup ug	ON ug.UserGroupID = ugm.UserGroupID
	LEFT JOIN org.[User] uugm	ON uugm.UserID = ug.UserID
	INNER JOIN dbo.Archetype a	ON ug.ArchetypeID = a.ArchetypeID
	WHERE ug.Deleted IS NULL AND ugm.Deleted IS NULL AND  ugm.UserID = u.UserID
	AND ug.EntityStatusID = 1
	AND ugm.EntityStatusID = 1
	AND (ugm.ValidFrom IS NULL	OR ugm.ValidFrom <= GETDATE())
	AND (ugm.ValidTo IS NULL OR ugm.ValidTo >= GETDATE())
	ORDER BY [NAME]
	FOR JSON PATH) UserGroups

FROM @userTempTbl u  
LEFT JOIN #idNeedResultTblLast rNeed ON rNeed.UserID=u.UserID
LEFT JOIN #idpPlanResultTblLast rPlan ON rPlan.UserID=u.UserID

END


DROP TABLE #idpNeedStatusTypeTbl;
DROP TABLE #idpPlanStatusTypeTbl
DROP TABLE #userIdsFilterTbl

IF(@getIdpNeedFirst=1)
	DROP TABLE #idNeedResultTblFirst
ELSE DROP TABLE #idNeedResultTblLast
IF(@getIdpPlanFirst=1)
	DROP TABLE #idpPlanResultTblFirst
ELSE DROP TABLE #idpPlanResultTblLast

 IF(@HasSortingOnUserGroup=1)
   DROP TABLE #userGroupTbl
 IF (@HasSortingOnUserType=1)
   DROP TABLE #userUserTypeTbl
END
   	 
GO

