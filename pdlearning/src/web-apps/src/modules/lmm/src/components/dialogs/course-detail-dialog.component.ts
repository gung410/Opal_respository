import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, ElementRef, HostListener, Input } from '@angular/core';
import {
  Course,
  CoursePlanningCycleRepository,
  CourseRepository,
  ECertificateRepository,
  FormRepository,
  FormStatus,
  FormType,
  OrganizationRepository,
  TaggingRepository,
  UserRepository
} from '@opal20/domain-api';
import { CourseDetailMode, CourseDetailViewModel, LMMTabConfiguration } from '@opal20/domain-components';
import { DialogAction, RootElementScrollableService, SPACING_CONTENT } from '@opal20/common-components';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Constant } from '@opal20/authentication';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'course-detail-dialog',
  templateUrl: './course-detail-dialog.component.html'
})
export class CourseDetailDialogComponent extends BaseFormComponent {
  public get course(): CourseDetailViewModel | null {
    return this._course;
  }
  public set course(v: CourseDetailViewModel) {
    this._course = v;
  }

  public get title(): string {
    return this.course.courseName;
  }

  public get subTitle(): string {
    return this.course.courseCode;
  }

  public set selectedTab(v: LMMTabConfiguration) {
    this._selectedTab = v;
  }

  public get selectedTab(): LMMTabConfiguration {
    return this._selectedTab;
  }
  @Input() public courseId: string = '';
  public loadingCourseVmData: boolean = false;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public stickySpacing: number = SPACING_CONTENT;
  public mode: CourseDetailMode = CourseDetailMode.View;
  private _loadCourseInfoSub: Subscription = new Subscription();
  private _course: CourseDetailViewModel | null = new CourseDetailViewModel(new Course(), {}, [], [], {}, []);
  private _selectedTab: LMMTabConfiguration = LMMTabConfiguration.CourseInfoTab;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private courseRepository: CourseRepository,
    private taggingRepository: TaggingRepository,
    private formRepository: FormRepository,
    private ecertificateRepository: ECertificateRepository,
    private userRepository: UserRepository,
    private organizationRepository: OrganizationRepository,
    private coursePlanningCycleRepository: CoursePlanningCycleRepository,
    private elementRef: ElementRef,
    private rootElementScrollableService: RootElementScrollableService
  ) {
    super(moduleFacadeService);
  }

  public loadCourseInfo(): void {
    this.loadCourse();
  }

  @HostListener('scroll')
  public onScroll(): void {
    const htmlElement = this.elementRef.nativeElement as HTMLElement;
    if (htmlElement != null) {
      this.rootElementScrollableService.emitScroll(htmlElement);
    }
  }

  public loadCourse(): void {
    this._loadCourseInfoSub.unsubscribe();
    const courseObs: Observable<Course | null> = this.courseId != null ? this.courseRepository.loadCourse(this.courseId) : of(null);
    const taggingObs = this.taggingRepository.loadAllMetaDataTags();
    const formObs = this.formRepository.searchForm(
      [FormStatus.Published],
      FormType.Survey,
      CourseDetailViewModel.formsSurveyTypes,
      0,
      Constant.MAX_ITEMS_PER_REQUEST,
      null,
      true
    );

    this.loadingCourseVmData = true;
    this._loadCourseInfoSub = combineLatest(courseObs, taggingObs, formObs)
      .pipe(
        switchMap(([course, metadatas, formResult]) => {
          return CourseDetailViewModel.create(
            ids =>
              this.userRepository.loadUserInfoList(
                {
                  parentDepartmentId: [1],
                  userIds: ids,
                  pageIndex: 0,
                  pageSize: 0,
                  filterOnSubDepartment: true
                },
                null,
                ['All']
              ),
            ids => this.courseRepository.loadCourses(ids),
            ids => this.organizationRepository.loadOrganizationalUnitsByIds(ids, true),
            coursePlanningCycleId => this.coursePlanningCycleRepository.loadCoursePlanningCycleById(coursePlanningCycleId),
            ecertificateTemplateId => this.ecertificateRepository.getECertificateTemplateById(ecertificateTemplateId),
            course,
            metadatas,
            formResult.items
          ).pipe(map(courseVm => <[CourseDetailViewModel, Course]>[courseVm, course]));
        })
      )
      .pipe(this.untilDestroy())
      .subscribe(
        ([courseVm, course]) => {
          this.course = courseVm;
          this.loadingCourseVmData = false;
        },
        () => {
          this.loadingCourseVmData = false;
        }
      );
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  public onTabSelected(event: SelectEvent): void {
    this.selectedTab = courseDetailDialogTabIndexMap[event.index];
  }

  public canViewContent(): boolean {
    return this.course.courseData.canViewContent();
  }

  protected onInit(): void {
    this.loadCourseInfo();
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      // Overview Info
      formName: 'overview-info',
      controls: {
        thumbnailUrl: {
          defaultValue: null,
          validators: null
        },
        courseName: {
          defaultValue: null,
          validators: null
        },
        durationHours: {
          defaultValue: null,
          validators: null
        },
        durationMinutes: {
          defaultValue: null,
          validators: null
        },
        pdActivityType: {
          defaultValue: null,
          validators: null
        },
        categoryIds: {
          defaultValue: null,
          validators: null
        },
        learningMode: {
          defaultValue: null,
          validators: null
        },
        courseCode: {
          defaultValue: null,
          validators: null
        },
        externalCode: {
          defaultValue: null,
          validators: null
        },
        courseLevel: {
          defaultValue: null,
          validators: null
        },
        courseOutlineStructure: {
          defaultValue: null,
          validators: null
        },
        courseObjective: {
          defaultValue: null,
          validators: null
        },
        description: {
          defaultValue: null,
          validators: null
        },
        // Provider Info,
        trainingAgency: {
          defaultValue: null,
          validators: null
        },
        otherTrainingAgencyReason: {
          defaultValue: null,
          validators: null
        },
        nieAcademicGroups: {
          defaultValue: null,
          validators: null
        },
        ownerDivisionIds: {
          defaultValue: null,
          validators: null
        },
        ownerBranchIds: {
          defaultValue: null,
          validators: null
        },
        partnerOrganisationIds: {
          defaultValue: null,
          validators: null
        },
        moeOfficerId: {
          defaultValue: null,
          validators: null
        },
        moeOfficerPhoneNumber: {
          defaultValue: null,
          validators: null
        },
        moeOfficerEmail: {
          defaultValue: null,
          validators: null
        },
        notionalCost: {
          defaultValue: null,
          validators: null
        },
        courseFee: {
          defaultValue: null,
          validators: null
        },

        // Metadata
        serviceSchemeIds: {
          defaultValue: null,
          validators: null
        },
        learningFrameworkIds: {
          defaultValue: null,
          validators: null
        },
        learningDimensionIds: {
          defaultValue: null,
          validators: null
        },
        learningAreaIds: {
          defaultValue: null,
          validators: null
        },
        learningSubAreaIds: {
          defaultValue: null,
          validators: null
        },
        subjectAreaIds: {
          defaultValue: null,
          validators: null
        },
        pdAreaThemeId: {
          defaultValue: null,
          validators: null
        },
        teacherOutcomeIds: {
          defaultValue: null,
          validators: null
        },
        // Copyright
        allowPersonalDownload: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerInMoeReuseWithoutModification: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerReuseWithoutModification: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerInMOEReuseWithModification: {
          defaultValue: null,
          validators: null
        },
        allowNonCommerReuseWithModification: {
          defaultValue: null,
          validators: null
        },
        copyrightOwner: {
          defaultValue: null,
          validators: null
        },
        acknowledgementAndCredit: {
          defaultValue: null,
          validators: null
        },
        remarks: {
          defaultValue: null,
          validators: null
        },
        // Target Audience
        trackIds: {
          defaultValue: null,
          validators: null
        },
        developmentalRoleIds: {
          defaultValue: null,
          validators: null
        },
        teachingLevels: {
          defaultValue: null,
          validators: null
        },
        teachingCourseStudyIds: {
          defaultValue: null,
          validators: null
        },
        placeOfWork: {
          defaultValue: null,
          validators: null
        },
        applicableDivisionIds: {
          defaultValue: null,
          validators: null
        },
        applicableBranchIds: {
          defaultValue: null,
          validators: null
        },
        applicableZoneIds: {
          defaultValue: null,
          validators: null
        },
        applicableClusterIds: {
          defaultValue: null,
          validators: null
        },
        applicableSchoolIds: {
          defaultValue: null,
          validators: null
        },
        registrationMethod: {
          defaultValue: null,
          validators: null
        },
        maximumPlacesPerSchool: {
          defaultValue: null,
          validators: null
        },
        prerequisiteCourseIds: {
          defaultValue: null,
          validators: null
        },
        numOfSchoolLeader: {
          defaultValue: null,
          validators: null
        },
        numOfSeniorOrLeadTeacher: {
          defaultValue: null,
          validators: null
        },
        numOfMiddleManagement: {
          defaultValue: null,
          validators: null
        },
        numOfBeginningTeacher: {
          defaultValue: null,
          validators: null
        },
        teachingSubjectIds: {
          defaultValue: null,
          validators: null
        },
        jobFamily: {
          defaultValue: null,
          validators: null
        },
        cocurricularActivityIds: {
          defaultValue: null,
          validators: null
        },
        easSubstantiveGradeBandingIds: {
          defaultValue: null,
          validators: null
        },
        // Course Planning
        natureOfCourse: {
          defaultValue: null,
          validators: null
        },
        numOfPlannedClass: {
          defaultValue: null,
          validators: null
        },
        numOfSessionPerClass: {
          defaultValue: null,
          validators: null
        },
        numOfHoursPerSession: {
          defaultValue: null,
          validators: null
        },
        totalHoursAttendWithinYear: {
          defaultValue: null,
          validators: null
        },
        planningPublishDate: {
          defaultValue: null,
          validators: null
        },
        startDate: {
          defaultValue: null,
          validators: null
        },
        expiredDate: {
          defaultValue: null,
          validators: null
        },
        planningArchiveDate: {
          defaultValue: null,
          validators: null
        },
        pdActivityPeriods: {
          defaultValue: null,
          validators: null
        },
        minParticipantPerClass: {
          defaultValue: null,
          validators: null
        },
        maxParticipantPerClass: {
          defaultValue: null,
          validators: null
        },
        maxReLearningTimes: {
          defaultValue: null,
          validators: null
        },
        // Evaluation And ECertificate
        preCourseEvaluationFormId: {
          defaultValue: null,
          validators: null
        },
        postCourseEvaluationFormId: {
          defaultValue: null,
          validators: null
        },
        eCertificateTemplateId: {
          defaultValue: null,
          validators: null
        },
        eCertificatePrerequisite: {
          defaultValue: null,
          validators: null
        },
        // Administration
        firstAdministratorId: {
          defaultValue: null,
          validators: null
        },
        secondAdministratorId: {
          defaultValue: null,
          validators: null
        },
        primaryApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        alternativeApprovingOfficerId: {
          defaultValue: null,
          validators: null
        },
        courseFacilitatorId: {
          defaultValue: null,
          validators: null
        },
        courseCoFacilitatorId: {
          defaultValue: null,
          validators: null
        },
        collaborativeContentCreatorIds: {
          defaultValue: null,
          validators: null
        }
      }
    };
  }
}

export const courseDetailDialogTabIndexMap = {
  0: LMMTabConfiguration.CourseInfoTab,
  1: LMMTabConfiguration.CourseContentTab
};
