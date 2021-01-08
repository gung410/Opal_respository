DROP PROC IF EXISTS sp_insertCourseInternalValues
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
End
Go
Exec('
	DECLARE @id uniqueidentifier;

	DECLARE @OwnerDivisionIds varchar(max);
	DECLARE @OwnerBranchIds varchar(max);
	DECLARE @PartnerOrganisationIds varchar(max);
	DECLARE @TrainingAgency varchar(max);
	DECLARE @OtherTrainingAgencyReason varchar(max);
	DECLARE @NieAcademicGroups varchar(max);
                                    
	DECLARE @PrerequisiteCourseIds varchar(max);
	DECLARE @ApplicableDivisionIds varchar(max);
	DECLARE @ApplicableBranchIds varchar(max);
	DECLARE @ApplicableZoneIds varchar(max);
	DECLARE @ApplicableClusterIds varchar(max);
	DECLARE @ApplicableSchoolIds varchar(max);
	DECLARE @DevelopmentalRoleIds varchar(max);
	DECLARE @EasSubstantiveGradeBandingIds varchar(max);

	DECLARE @ServiceSchemeIds varchar(max);
	DECLARE @SubjectAreaIds varchar(max);
	DECLARE @LearningFrameworkIds varchar(max);
	DECLARE @LearningDimensionIds varchar(max);
	DECLARE @LearningAreaIds varchar(max);
	DECLARE @LearningSubAreaIds varchar(max);
	DECLARE @TeacherOutcomeIds varchar(max);
	DECLARE @TrackIds varchar(max);
	DECLARE @CategoryIds varchar(max);
	DECLARE @JobFamily varchar(max);
	DECLARE @TeachingCourseStudyIds varchar(max);
	DECLARE @TeachingLevels varchar(max);
	DECLARE @TeachingSubjectIds varchar(max);
	DECLARE @CocurricularActivityIds varchar(max);
	DECLARE @PdActivityPeriods varchar(max);

    DECLARE @CourseFacilitatorIds varchar(max);
	DECLARE @CourseCoFacilitatorIds varchar(max);
	DECLARE @CollaborativeContentCreatorIds varchar(max);

	----Create cursor for loop through Course table
	DECLARE cursorCourseInternal CURSOR FOR SELECT
		id,
		OwnerDivisionIds,
		OwnerBranchIds,
		PartnerOrganisationIds,
		TrainingAgency,
		OtherTrainingAgencyReason,
		NieAcademicGroups,

		PrerequisiteCourseIds,
		ApplicableDivisionIds,
		ApplicableBranchIds,
		ApplicableZoneIds,
		ApplicableClusterIds,
		ApplicableSchoolIds,
		DevelopmentalRoleIds,
		EasSubstantiveGradeBandingIds,

		ServiceSchemeIds,
		SubjectAreaIds,
		LearningFrameworkIds,
		LearningDimensionIds,
		LearningAreaIds,
		LearningSubAreaIds,
		TeacherOutcomeIds,
		TrackIds,
		CategoryIds,
		JobFamily,
		TeachingCourseStudyIds,
		TeachingLevels,
		TeachingSubjectIds,
		CocurricularActivityIds,
		PdActivityPeriods,

        CourseFacilitatorIds,
		CourseCoFacilitatorIds,
		CollaborativeContentCreatorIds
	FROM dbo.Course
	OPEN cursorCourseInternal
	FETCH NEXT FROM cursorCourseInternal INTO
		@id,
		@OwnerDivisionIds,
		@OwnerBranchIds,
		@PartnerOrganisationIds,
		@TrainingAgency,
		@OtherTrainingAgencyReason,
		@NieAcademicGroups,

		@PrerequisiteCourseIds,
		@ApplicableDivisionIds,
		@ApplicableBranchIds,
		@ApplicableZoneIds,
		@ApplicableClusterIds,
		@ApplicableSchoolIds,
		@DevelopmentalRoleIds,
		@EasSubstantiveGradeBandingIds,

		@ServiceSchemeIds,
		@SubjectAreaIds,
		@LearningFrameworkIds,
		@LearningDimensionIds,
		@LearningAreaIds,
		@LearningSubAreaIds,
		@TeacherOutcomeIds,
		@TrackIds,
		@CategoryIds,
		@JobFamily,
		@TeachingCourseStudyIds,
		@TeachingLevels,
		@TeachingSubjectIds,
		@CocurricularActivityIds,
		@PdActivityPeriods,

        @CourseFacilitatorIds,
		@CourseCoFacilitatorIds,
		@CollaborativeContentCreatorIds

	WHILE @@FETCH_STATUS = 0
	BEGIN
		EXEC sp_insertCourseInternalValues @id, ''OwnerDivisionIds'', @OwnerDivisionIds;
		EXEC sp_insertCourseInternalValues @id, ''OwnerBranchIds'', @OwnerBranchIds;
		EXEC sp_insertCourseInternalValues @id, ''PartnerOrganisationIds'', @PartnerOrganisationIds;
		EXEC sp_insertCourseInternalValues @id, ''TrainingAgency'', @TrainingAgency;
		EXEC sp_insertCourseInternalValues @id, ''OtherTrainingAgencyReason'', @OtherTrainingAgencyReason;
		EXEC sp_insertCourseInternalValues @id, ''NieAcademicGroups'', @NieAcademicGroups;

		EXEC sp_insertCourseInternalValues @id, ''PrerequisiteCourseIds'', @PrerequisiteCourseIds;
		EXEC sp_insertCourseInternalValues @id, ''ApplicableDivisionIds'', @ApplicableDivisionIds;
		EXEC sp_insertCourseInternalValues @id, ''ApplicableBranchIds'', @ApplicableBranchIds;
		EXEC sp_insertCourseInternalValues @id, ''ApplicableZoneIds'', @ApplicableZoneIds;
		EXEC sp_insertCourseInternalValues @id, ''ApplicableClusterIds'', @ApplicableClusterIds;
		EXEC sp_insertCourseInternalValues @id, ''ApplicableSchoolIds'', @ApplicableSchoolIds;
		EXEC sp_insertCourseInternalValues @id, ''DevelopmentalRoleIds'', @DevelopmentalRoleIds;
		EXEC sp_insertCourseInternalValues @id, ''EasSubstantiveGradeBandingIds'', @EasSubstantiveGradeBandingIds;

		EXEC sp_insertCourseInternalValues @id, ''ServiceSchemeIds'', @ServiceSchemeIds;
		EXEC sp_insertCourseInternalValues @id, ''SubjectAreaIds'', @SubjectAreaIds;
		EXEC sp_insertCourseInternalValues @id, ''LearningFrameworkIds'', @LearningFrameworkIds;
		EXEC sp_insertCourseInternalValues @id, ''LearningDimensionIds'', @LearningDimensionIds;
		EXEC sp_insertCourseInternalValues @id, ''LearningAreaIds'', @LearningAreaIds;
		EXEC sp_insertCourseInternalValues @id, ''LearningSubAreaIds'', @LearningSubAreaIds;
		EXEC sp_insertCourseInternalValues @id, ''TeacherOutcomeIds'', @TeacherOutcomeIds;
		EXEC sp_insertCourseInternalValues @id, ''TrackIds'', @TrackIds;
		EXEC sp_insertCourseInternalValues @id, ''CategoryIds'', @CategoryIds;
		EXEC sp_insertCourseInternalValues @id, ''JobFamily'', @JobFamily;
		EXEC sp_insertCourseInternalValues @id, ''TeachingCourseStudyIds'', @TeachingCourseStudyIds;
		EXEC sp_insertCourseInternalValues @id, ''TeachingLevels'', @TeachingLevels;
		EXEC sp_insertCourseInternalValues @id, ''TeachingSubjectIds'', @TeachingSubjectIds;
		EXEC sp_insertCourseInternalValues @id, ''CocurricularActivityIds'', @CocurricularActivityIds;
		EXEC sp_insertCourseInternalValues @id, ''PdActivityPeriods'', @PdActivityPeriods;

        EXEC sp_insertCourseInternalValues @id, ''CourseFacilitatorIds'', @CourseFacilitatorIds;
		EXEC sp_insertCourseInternalValues @id, ''CourseCoFacilitatorIds'', @CourseCoFacilitatorIds;
		EXEC sp_insertCourseInternalValues @id, ''CollaborativeContentCreatorIds'', @CollaborativeContentCreatorIds;

		FETCH NEXT FROM cursorCourseInternal INTO
			@id,
			@OwnerDivisionIds,
			@OwnerBranchIds,
			@PartnerOrganisationIds,
			@TrainingAgency,
			@OtherTrainingAgencyReason,
			@NieAcademicGroups,

			@PrerequisiteCourseIds,
			@ApplicableDivisionIds,
			@ApplicableBranchIds,
			@ApplicableZoneIds,
			@ApplicableClusterIds,
			@ApplicableSchoolIds,
			@DevelopmentalRoleIds,
			@EasSubstantiveGradeBandingIds,

			@ServiceSchemeIds,
			@SubjectAreaIds,
			@LearningFrameworkIds,
			@LearningDimensionIds,
			@LearningAreaIds,
			@LearningSubAreaIds,
			@TeacherOutcomeIds,
			@TrackIds,
			@CategoryIds,
			@JobFamily,
			@TeachingCourseStudyIds,
			@TeachingLevels,
			@TeachingSubjectIds,
			@CocurricularActivityIds,
			@PdActivityPeriods,

            @CourseFacilitatorIds,
			@CourseCoFacilitatorIds,
			@CollaborativeContentCreatorIds
	END
	CLOSE cursorCourseInternal;
	DEALLOCATE cursorCourseInternal
	')
