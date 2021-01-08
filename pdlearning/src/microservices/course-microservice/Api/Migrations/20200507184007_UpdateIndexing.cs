using Microsoft.EntityFrameworkCore.Migrations;

namespace Microservice.Course.Migrations
{
    public partial class UpdateIndexing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExtID",
                table: "DepartmentTypes",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassRunCode",
                table: "ClassRun",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserID",
                table: "Users",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Session_ClassRunId",
                table: "Session",
                column: "ClassRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Session_CreatedBy",
                table: "Session",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Session_CreatedDate",
                table: "Session",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Session_IsDeleted",
                table: "Session",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Section_ClassRunId",
                table: "Section",
                column: "ClassRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Section_CourseId",
                table: "Section",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Section_CreatedBy",
                table: "Section",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Section_CreatedDate",
                table: "Section",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Section_IsDeleted",
                table: "Section",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_ClassRunChangeId",
                table: "Registration",
                column: "ClassRunChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_ClassRunChangeStatus",
                table: "Registration",
                column: "ClassRunChangeStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_ClassRunId",
                table: "Registration",
                column: "ClassRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_CourseId",
                table: "Registration",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_CreatedBy",
                table: "Registration",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_CreatedDate",
                table: "Registration",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_IsDeleted",
                table: "Registration",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_Status",
                table: "Registration",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_UserId",
                table: "Registration",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_WithdrawalStatus",
                table: "Registration",
                column: "WithdrawalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_LectureContent_CreatedBy",
                table: "LectureContent",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LectureContent_CreatedDate",
                table: "LectureContent",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_LectureContent_IsDeleted",
                table: "LectureContent",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LectureContent_LectureId",
                table: "LectureContent",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_ClassRunId",
                table: "Lecture",
                column: "ClassRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_CourseId",
                table: "Lecture",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_CreatedBy",
                table: "Lecture",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_CreatedDate",
                table: "Lecture",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_IsDeleted",
                table: "Lecture",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Lecture_SectionId",
                table: "Lecture",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourse_CourseId",
                table: "LearningPathCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourse_CreatedDate",
                table: "LearningPathCourse",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourse_IsDeleted",
                table: "LearningPathCourse",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourse_LearningPathId",
                table: "LearningPathCourse",
                column: "LearningPathId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPathCourse_Order",
                table: "LearningPathCourse",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPath_CreatedBy",
                table: "LearningPath",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPath_CreatedDate",
                table: "LearningPath",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPath_IsDeleted",
                table: "LearningPath",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPath_Status",
                table: "LearningPath",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_HDID",
                table: "HierarchyDepartments",
                column: "HDID");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_ParentID",
                table: "HierarchyDepartments",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_HierarchyDepartments_Path",
                table: "HierarchyDepartments",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_CreatedDate",
                table: "ECertificateTemplate",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ECertificateTemplate_IsDeleted",
                table: "ECertificateTemplate",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypes_DepartmentTypeID",
                table: "DepartmentTypes",
                column: "DepartmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypes_ExtID",
                table: "DepartmentTypes",
                column: "ExtID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentID",
                table: "DepartmentTypeDepartments",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentTypeID",
                table: "DepartmentTypeDepartments",
                column: "DepartmentTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentID",
                table: "Departments",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_ContentStatus",
                table: "Course",
                column: "ContentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CourseCode",
                table: "Course",
                column: "CourseCode");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedBy",
                table: "Course",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedDate",
                table: "Course",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Course_DepartmentId",
                table: "Course",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted",
                table: "Course",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Course_Status",
                table: "Course",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CancellationStatus",
                table: "ClassRun",
                column: "CancellationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ClassRunCode",
                table: "ClassRun",
                column: "ClassRunCode");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ClassRunVenueId",
                table: "ClassRun",
                column: "ClassRunVenueId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_ContentStatus",
                table: "ClassRun",
                column: "ContentStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CourseId",
                table: "ClassRun",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CreatedBy",
                table: "ClassRun",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_CreatedDate",
                table: "ClassRun",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_EndDateTime",
                table: "ClassRun",
                column: "EndDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_IsDeleted",
                table: "ClassRun",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_RescheduleStatus",
                table: "ClassRun",
                column: "RescheduleStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_StartDateTime",
                table: "ClassRun",
                column: "StartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_ClassRun_Status",
                table: "ClassRun",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTracking_CreatedDate",
                table: "AttendanceTracking",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTracking_IsDeleted",
                table: "AttendanceTracking",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTracking_RegistrationId",
                table: "AttendanceTracking",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTracking_SessionId",
                table: "AttendanceTracking",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTracking_Status",
                table: "AttendanceTracking",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceTracking_Userid",
                table: "AttendanceTracking",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_ClassRunId",
                table: "Assignment",
                column: "ClassRunId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CourseId",
                table: "Assignment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CreatedBy",
                table: "Assignment",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CreatedDate",
                table: "Assignment",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_EndDate",
                table: "Assignment",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_IsDeleted",
                table: "Assignment",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_StartDate",
                table: "Assignment",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Type",
                table: "Assignment",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Session_ClassRunId",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_CreatedBy",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_CreatedDate",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_IsDeleted",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Section_ClassRunId",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Section_CourseId",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Section_CreatedBy",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Section_CreatedDate",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Section_IsDeleted",
                table: "Section");

            migrationBuilder.DropIndex(
                name: "IX_Registration_ClassRunChangeId",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_ClassRunChangeStatus",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_ClassRunId",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_CourseId",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_CreatedBy",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_CreatedDate",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_IsDeleted",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_Status",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_UserId",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_WithdrawalStatus",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_LectureContent_CreatedBy",
                table: "LectureContent");

            migrationBuilder.DropIndex(
                name: "IX_LectureContent_CreatedDate",
                table: "LectureContent");

            migrationBuilder.DropIndex(
                name: "IX_LectureContent_IsDeleted",
                table: "LectureContent");

            migrationBuilder.DropIndex(
                name: "IX_LectureContent_LectureId",
                table: "LectureContent");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_ClassRunId",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_CourseId",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_CreatedBy",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_CreatedDate",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_IsDeleted",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_Lecture_SectionId",
                table: "Lecture");

            migrationBuilder.DropIndex(
                name: "IX_LearningPathCourse_CourseId",
                table: "LearningPathCourse");

            migrationBuilder.DropIndex(
                name: "IX_LearningPathCourse_CreatedDate",
                table: "LearningPathCourse");

            migrationBuilder.DropIndex(
                name: "IX_LearningPathCourse_IsDeleted",
                table: "LearningPathCourse");

            migrationBuilder.DropIndex(
                name: "IX_LearningPathCourse_LearningPathId",
                table: "LearningPathCourse");

            migrationBuilder.DropIndex(
                name: "IX_LearningPathCourse_Order",
                table: "LearningPathCourse");

            migrationBuilder.DropIndex(
                name: "IX_LearningPath_CreatedBy",
                table: "LearningPath");

            migrationBuilder.DropIndex(
                name: "IX_LearningPath_CreatedDate",
                table: "LearningPath");

            migrationBuilder.DropIndex(
                name: "IX_LearningPath_IsDeleted",
                table: "LearningPath");

            migrationBuilder.DropIndex(
                name: "IX_LearningPath_Status",
                table: "LearningPath");

            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_HDID",
                table: "HierarchyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_ParentID",
                table: "HierarchyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_HierarchyDepartments_Path",
                table: "HierarchyDepartments");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_CreatedDate",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_ECertificateTemplate_IsDeleted",
                table: "ECertificateTemplate");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypes_DepartmentTypeID",
                table: "DepartmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypes_ExtID",
                table: "DepartmentTypes");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentID",
                table: "DepartmentTypeDepartments");

            migrationBuilder.DropIndex(
                name: "IX_DepartmentTypeDepartments_DepartmentTypeID",
                table: "DepartmentTypeDepartments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentID",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Course_ContentStatus",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CourseCode",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedBy",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_CreatedDate",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_DepartmentId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_IsDeleted",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_Status",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CancellationStatus",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ClassRunCode",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ClassRunVenueId",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_ContentStatus",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CourseId",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CreatedBy",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_CreatedDate",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_EndDateTime",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_IsDeleted",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_RescheduleStatus",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_StartDateTime",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_ClassRun_Status",
                table: "ClassRun");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTracking_CreatedDate",
                table: "AttendanceTracking");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTracking_IsDeleted",
                table: "AttendanceTracking");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTracking_RegistrationId",
                table: "AttendanceTracking");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTracking_SessionId",
                table: "AttendanceTracking");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTracking_Status",
                table: "AttendanceTracking");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceTracking_Userid",
                table: "AttendanceTracking");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_ClassRunId",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_CourseId",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_CreatedBy",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_CreatedDate",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_EndDate",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_IsDeleted",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_StartDate",
                table: "Assignment");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_Type",
                table: "Assignment");

            migrationBuilder.AlterColumn<string>(
                name: "ExtID",
                table: "DepartmentTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassRunCode",
                table: "ClassRun",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
