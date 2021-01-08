GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverter
GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverterLevel2
GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverterLevel3
GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverterSingle
GO
-- CREATE STORE PROCEDURE --

-- CREATE STORE 1
create PROC  sp_TagIdJsonConverter(@CourseId uniqueidentifier, @GroupCode varchar(max), @output NVARCHAR(MAX) OUTPUT)
as
begin
declare @json NVARCHAR(MAX);
	set @json = (select lower(MetadataTags.TagId) TagId
			FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags MetadataTags
			WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
			AND Resources.ResourceId = @CourseId
			AND MetadataTags.GroupCode = @GroupCode
			FOR JSON AUTO)
	set @output = replace (replace(replace(@json, '"TagId":', ''),'{',''),'}','')
end

GO
-- CREATE STORE 2
create PROC  sp_TagIdJsonConverterLevel2(@CourseId uniqueidentifier, @GroupCode varchar(max), @output NVARCHAR(MAX) OUTPUT)
as
begin
declare @json NVARCHAR(MAX);
	set @json = (SELECT lower(MetadataTags.TagId) TagId
        FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags
        WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
		AND Resources.ResourceId = @CourseId
		AND MetadataTags.ParentTagId in (
		   SELECT MetadataTags.TagId
		   FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags
		   WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
		   AND Resources.ResourceId = @CourseId
		   AND MetadataTags.GroupCode = @GroupCode)
			FOR JSON AUTO)
	set @output = replace (replace(replace(@json, '"TagId":', ''),'{',''),'}','')
end

GO
-- Create store 3
create PROC  sp_TagIdJsonConverterLevel3(@CourseId uniqueidentifier, @GroupCode varchar(max), @output NVARCHAR(MAX) OUTPUT)
as
begin
declare @json NVARCHAR(MAX);
	set @json = (SELECT lower(MetadataTags.TagId) TagId
        FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags
        WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
  AND Resources.ResourceId = @CourseId
  AND MetadataTags.ParentTagId in (
	   SELECT MetadataTags.TagId
	   FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags
	   WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
	   AND Resources.ResourceId = @CourseId
	   AND MetadataTags.ParentTagId in (
			SELECT MetadataTags.TagId
			FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags
			WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
			AND Resources.ResourceId = @CourseId
			AND MetadataTags.GroupCode = @GroupCode)
		)FOR JSON AUTO)
	set @output = replace (replace(replace(@json, '"TagId":', ''),'{',''),'}','')
end

GO

-- CREATE STORE 4
create PROC  sp_TagIdJsonConverterSingle(@CourseId uniqueidentifier, @GroupCode varchar(max), @output NVARCHAR(MAX) OUTPUT)
as
begin
declare @json NVARCHAR(MAX);
	set @json = (select TOP 1 lower(MetadataTags.TagId) TagId
			FROM [MOE_Suggestion].[dbo].Resources, [MOE_Suggestion].[dbo].TaggedWith, [MOE_Suggestion].[dbo].MetadataTags MetadataTags
			WHERE MATCH(Resources-(TaggedWith)->MetadataTags)
			AND Resources.ResourceId = @CourseId
			AND MetadataTags.GroupCode = @GroupCode)
	set @output = replace (replace(replace(@json, '"TagId":', ''),'{',''),'}','')
end

GO

-- BEGIN MAIN MIGRATE SCRIPT --

DECLARE @CourseId uniqueidentifier;
DECLARE @result varchar(max)
	----Create cursor for loop throught Course table
	DECLARE cursorCourse CURSOR FOR SELECT Id FROM [MOE_Course].[dbo].[Course] c JOIN [MOE_Suggestion].[dbo].[Resources] r ON c.Id = r.ResourceId WHERE PDActivityType = 'db13d0f8-d595-11e9-baec-0242ac120004'
	OPEN cursorCourse
	FETCH NEXT FROM cursorCourse INTO @CourseId
	WHILE @@FETCH_STATUS = 0
	BEGIN

		-- Target column ServiceSchemeIds
		exec sp_TagIdJsonConverter @CourseId,'SERVICESCHEMES', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET ServiceSchemeIds = @result
		WHERE Id = @CourseId

		-- Target column CourseLevel
		exec sp_TagIdJsonConverterSingle @CourseId,'COURSE-LEVELS', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET CourseLevel = @result
		WHERE Id = @CourseId

        -- Target column LearningMode
        UPDATE [MOE_Course].[dbo].[Course] SET LearningMode = LOWER('5DF4DFDA-DB9F-11E9-B8D9-0242AC120004')
		WHERE Id = @CourseId
        INSERT INTO [MOE_Suggestion].[dbo].[TaggedWith] ($from_id, $to_id) VALUES (
            (SELECT $node_id FROM [MOE_Suggestion].[dbo].[Resources] WHERE ResourceId = @CourseId), 
            (SELECT $node_id FROM [MOE_Suggestion].[dbo].[MetadataTags] WHERE TagId = '5DF4DFDA-DB9F-11E9-B8D9-0242AC120004'))

		-- Target column DevelopmentalRoleIds
		exec sp_TagIdJsonConverter @CourseId,'DEVROLES', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET DevelopmentalRoleIds = @result
		WHERE Id = @CourseId

		-- Target columnn SubjectAreaIds
		exec sp_TagIdJsonConverter @CourseId,'PDO-TAXONOMY', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET SubjectAreaIds = @result
		WHERE Id = @CourseId

		-- Target column LearningFrameworkIds
		exec sp_TagIdJsonConverter @CourseId,'LEARNING-FXS', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET LearningFrameworkIds = @result
		WHERE Id = @CourseId

		-- Target column LearningDimensionIds
		exec sp_TagIdJsonConverterLevel2 @CourseId,'LEARNING-FXS', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET  LearningDimensionIds = @result
		WHERE Id = @CourseId

		-- Target column LearningAreaIds
		exec sp_TagIdJsonConverterLevel3 @CourseId,'LEARNING-FXS', @output = @result OUTPUT
		UPDATE [MOE_Course].[dbo].[Course] SET  LearningAreaIds = @result
		WHERE Id = @CourseId

		FETCH NEXT FROM cursorCourse INTO @CourseId
	END
	CLOSE cursorCourse;
	DEALLOCATE cursorCourse
GO
-- END MAIN MIGRATE SCRIPT --

DROP PROCEDURE IF EXISTS sp_TagIdJsonConverter
GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverterLevel2
GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverterLevel3
GO
DROP PROCEDURE IF EXISTS sp_TagIdJsonConverterSingle
GO
