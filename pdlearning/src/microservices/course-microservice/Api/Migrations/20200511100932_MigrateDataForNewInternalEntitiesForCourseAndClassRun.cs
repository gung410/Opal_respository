using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class MigrateDataForNewInternalEntitiesForCourseAndClassRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROC IF EXISTS sp_insertCourseInternalValues
                                    GO
                                    Create PROC  sp_insertCourseInternalValues(@courseId uniqueidentifier, @coursesType varchar(50), @jsonValue varchar(max))
                                    AS
                                    Begin
                                     declare @i int;
                                     declare @length int;
                                     SET @i = 0;
                                            SET @length = 
                                            (SELECT COUNT(*) 
                                             FROM OPENJSON(@jsonValue))
                                            WHILE @i < @length
                                            BEGIN
                                                DECLARE @jsonValueItem NVARCHAR(100);
                                                SET @jsonValueItem = JSON_VALUE(@jsonValue,CONCAT('$[',@i,']'));
			                                    DELETE dbo.CourseInternalValue WHERE CourseId = @courseId and [Type] = @coursesType and [Value] = @jsonValueItem;
			                                    insert into dbo.CourseInternalValue(CourseId,[Type], [Value]) values(@courseId, @coursesType, @jsonValueItem)
                                                SET @i = @i +1;
                                            END
                                    End");

            migrationBuilder.Sql(@"DECLARE @id uniqueidentifier;
	                                DECLARE @CourseFacilitatorIds varchar(max);
	                                DECLARE @CourseCoFacilitatorIds varchar(max);
	                                DECLARE @CollaborativeContentCreatorIds varchar(max);
	                                ----Create cursor for loop throught Course table
	                                DECLARE cursorCourseInternal CURSOR FOR SELECT id, CourseFacilitatorIds, CourseCoFacilitatorIds, CollaborativeContentCreatorIds FROM dbo.Course
	                                OPEN cursorCourseInternal
	                                FETCH NEXT FROM cursorCourseInternal INTO @id, @CourseFacilitatorIds, @CourseCoFacilitatorIds, @CollaborativeContentCreatorIds
	                                WHILE @@FETCH_STATUS = 0
	                                BEGIN
		                                EXEC sp_insertCourseInternalValues @id, 'CourseFacilitatorIds', @CourseFacilitatorIds;
		                                EXEC sp_insertCourseInternalValues @id, 'CourseCoFacilitatorIds', @CourseCoFacilitatorIds;
		                                EXEC sp_insertCourseInternalValues @id, 'CollaborativeContentCreatorIds', @CollaborativeContentCreatorIds;
		                                FETCH NEXT FROM cursorCourseInternal INTO @id, @CourseFacilitatorIds, @CourseCoFacilitatorIds, @CollaborativeContentCreatorIds
	                                END
	                                CLOSE cursorCourseInternal;
	                                DEALLOCATE cursorCourseInternal");

            migrationBuilder.Sql(@"DROP PROC IF EXISTS sp_insertCourseInternalValues");

            migrationBuilder.Sql(@"DROP PROC IF EXISTS sp_insertClassRunInternalValues
                                    GO
                                    Create PROC  sp_insertClassRunInternalValues(@classRunId uniqueidentifier, @classRunType varchar(50), @jsonValue varchar(max))
                                    AS
                                    Begin
                                     declare @i int;
                                     declare @length int;
                                     SET @i = 0;
                                            SET @length = 
                                            (SELECT COUNT(*) 
                                             FROM OPENJSON(@jsonValue))
                                            WHILE @i < @length
                                            BEGIN
                                                DECLARE @jsonValueItem NVARCHAR(100);
                                                SET @jsonValueItem = JSON_VALUE(@jsonValue,CONCAT('$[',@i,']'));
			                                    DELETE dbo.ClassRunInternalValue WHERE ClassRunId = @classRunId and [Type] = @classRunType and [Value] = @jsonValueItem;
			                                    insert into dbo.ClassRunInternalValue(ClassRunId,[Type], [Value]) values(@classRunId, @classRunType, @jsonValueItem)
                                                SET @i = @i +1;
                                            END
                                    End");

            migrationBuilder.Sql(@"DECLARE @id uniqueidentifier;
	                                DECLARE @ClassRunFacilitatorIds varchar(max);
	                                DECLARE @ClassRunCoFacilitatorIds varchar(max);
	                                DECLARE @CollaborativeContentCreatorIds varchar(max);
	                                ----Create cursor for loop throught Course table
	                                DECLARE cursorClassRunInternal CURSOR FOR SELECT id, FacilitatorIds, CoFacilitatorIds FROM dbo.ClassRun
	                                OPEN cursorClassRunInternal
	                                FETCH NEXT FROM cursorClassRunInternal INTO @id, @ClassRunFacilitatorIds, @ClassRunCoFacilitatorIds
	                                WHILE @@FETCH_STATUS = 0
	                                BEGIN
		                                EXEC sp_insertClassRunInternalValues @id, 'FacilitatorIds', @ClassRunFacilitatorIds;
		                                EXEC sp_insertClassRunInternalValues @id, 'CoFacilitatorIds', @ClassRunCoFacilitatorIds;

		                                FETCH NEXT FROM cursorClassRunInternal INTO @id, @ClassRunFacilitatorIds, @ClassRunCoFacilitatorIds
	                                END
	                                CLOSE cursorClassRunInternal;
	                                DEALLOCATE cursorClassRunInternal");

            migrationBuilder.Sql(@"DROP PROC IF EXISTS sp_insertClassRunInternalValues");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
