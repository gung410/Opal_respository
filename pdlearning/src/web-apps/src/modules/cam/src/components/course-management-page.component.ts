import {
  BasePageComponent,
  ComponentType,
  IGridFilter,
  ModuleFacadeService,
  NotificationType,
  TranslationMessage
} from '@opal20/infrastructure';
import { ButtonAction, ContextMenuItem, OpalDialogService } from '@opal20/common-components';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  ContextMenuAction,
  ContextMenuEmit,
  CourseCriteriaDetailMode,
  CourseDetailMode,
  CourseFilterComponent,
  CourseFilterModel,
  CourseViewModel,
  ISelectUserDialogResult,
  ListCourseGridComponentService,
  ListCourseGridDisplayColumns,
  NavigationPageService,
  RouterPageInput,
  SelectCourseDialogComponent,
  SelectCourseModel,
  SelectUserDialogComponent
} from '@opal20/domain-components';
import {
  CAM_PERMISSIONS,
  Course,
  CourseRepository,
  CourseStatus,
  ITransferOwnerRequest,
  Registration,
  SearchCourseType,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { Component, HostBinding } from '@angular/core';

import { CourseManagementPageInput } from './../models/course-management-page-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { NAVIGATORS } from '../cam.config';
import { Observable } from 'rxjs';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { map } from 'rxjs/operators';

@Component({
  selector: 'course-management-page',
  templateUrl: './course-management-page.component.html'
})
export class CourseManagementPageComponent extends BasePageComponent {
  public filterPopupContent: ComponentType<CourseFilterComponent> = CourseFilterComponent;

  public searchText: string = '';
  public filterData: CourseFilterModel = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public courseManagementPageInput: RouterPageInput<CourseManagementPageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.CourseManagementPage
  ] as RouterPageInput<CourseManagementPageInput, CAMTabConfiguration, unknown>;
  public SearchCourseType: typeof SearchCourseType = SearchCourseType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;

  public ListCourseGridDisplayColumns: typeof ListCourseGridDisplayColumns = ListCourseGridDisplayColumns;
  public createCourseModes: Array<unknown> = [
    {
      id: CourseDetailMode.NewCourse,
      text: this.translate('New Course')
    },
    {
      id: CourseDetailMode.Recurring,
      text: this.translate('Recurring')
    },
    {
      id: CourseDetailMode.Edit,
      text: this.translate('Duplicate')
    }
  ];

  public contextMenuItemsForCourses: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Publish,
      text: this.translateCommon('Publish'),
      icon: 'check'
    },
    {
      id: ContextMenuAction.Unpublish,
      text: this.translateCommon('Unpublish'),
      icon: 'cancel'
    },
    {
      id: ContextMenuAction.Delete,
      text: this.translateCommon('Delete'),
      icon: 'delete'
    },
    {
      id: ContextMenuAction.TransferOwnership,
      text: this.translateCommon('Transfer Ownership'),
      icon: 'user'
    },
    {
      id: ContextMenuAction.Archive,
      text: this.translateCommon('Archive'),
      icon: 'aggregate-fields'
    }
  ];

  public contextMenuItemsForPendingApproval = [
    {
      id: ContextMenuAction.Approve,
      text: this.translateCommon('Approve'),
      icon: 'track-changes-accept'
    },
    {
      id: ContextMenuAction.Reject,
      text: this.translateCommon('Reject'),
      icon: 'track-changes-reject'
    }
  ];
  public allCourseSelectedItems: CourseViewModel[] = [];

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<CourseViewModel>[] = [
    {
      id: 'publish',
      text: this.translateCommon('Publish'),
      conditionFn: dataItem => dataItem.canPublishCourse() && dataItem.hasPublishCoursePermission(this.currentUser),
      actionFn: dataItems => this.handleMassAction(CourseMassAction.PublishCourse, dataItems),
      hiddenFn: () => this.selectedTab !== CAMTabConfiguration.CoursesTab
    },
    {
      id: 'unpublish',
      text: this.translateCommon('Unpublish'),
      conditionFn: dataItem => dataItem.canUnpublishCourse(this.currentUser),
      actionFn: dataItems => this.handleMassAction(CourseMassAction.UnpublishCourse, dataItems),
      hiddenFn: () => this.selectedTab !== CAMTabConfiguration.CoursesTab
    },
    {
      id: 'approve',
      text: this.translateCommon('Approve')
    },
    {
      id: 'reject',
      text: this.translateCommon('Reject')
    }
  ];
  public get selectedTab(): CAMTabConfiguration {
    return this.courseManagementPageInput.activeTab != null ? this.courseManagementPageInput.activeTab : CAMTabConfiguration.CoursesTab;
  }

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  public static hasViewPermissions(currentUser: UserInfoModel): boolean {
    return (
      currentUser.hasPermissionPrefix(CAM_PERMISSIONS.CourseAdministration) ||
      currentUser.hasAdministratorRoles() ||
      currentUser.hasRole(SystemRoleEnum.CourseAdministrator) ||
      currentUser.hasRole(SystemRoleEnum.CourseContentCreator) ||
      currentUser.hasRole(SystemRoleEnum.SchoolContentApprovingOfficer) ||
      currentUser.hasRole(SystemRoleEnum.CourseApprovingOfficer) ||
      currentUser.hasRole(SystemRoleEnum.MOEHQContentApprovingOfficer)
    );
  }

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public navigationPageService: NavigationPageService,
    private opalDialogService: OpalDialogService,
    private courseRepository: CourseRepository,
    private listCourseGridComponentService: ListCourseGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public onApplyFilter(data: CourseFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public handleMassAction(massAction: CourseMassAction, dataItems: CourseViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case CourseMassAction.PublishCourse:
        massActionPromise = this.changeCourseStatus(dataItems, CourseStatus.Published, '', false);
        break;
      case CourseMassAction.UnpublishCourse:
        massActionPromise = this.changeCourseStatus(dataItems, CourseStatus.Unpublished, '', false);
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  public get getSelectedItemsForMassAction(): CourseViewModel[] {
    if (this.selectedTab === CAMTabConfiguration.CoursesTab) {
      return this.allCourseSelectedItems;
    }
    return [];
  }

  public resetSelectedItems(): void {
    this.allCourseSelectedItems = [];
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onCreateCourse(mode: string): void {
    // TODO: Switch case for mode
    if (mode === CourseDetailMode.Recurring) {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
        SelectCourseDialogComponent,
        SelectCourseDialogComponent.selectRecurringCoursesConfig()
      );
      this.subscribe(dialogRef.result, (data: SelectCourseModel) => {
        if (data.id) {
          this.navigationPageService.navigateTo(
            CAMRoutePaths.CourseDetailPage,
            {
              activeTab: CAMTabConfiguration.CourseInfoTab,
              subActiveTab: CAMTabConfiguration.AllClassRunsTab,
              data: {
                mode: CourseDetailMode.Recurring,
                courseCriteriaMode: CourseCriteriaDetailMode.View,
                id: data.id
              }
            },
            this.courseManagementPageInput
          );
        }
      });
    } else if (mode === CourseDetailMode.Edit) {
      const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
        SelectCourseDialogComponent,
        SelectCourseDialogComponent.selectToCloneCoursesConfig()
      );
      this.subscribe(dialogRef.result, (data: SelectCourseModel) => {
        if (data.id) {
          this.courseRepository
            .cloneCourse({ id: data.id, fromCoursePlanning: false })
            .pipe(this.untilDestroy())
            .subscribe(course => {
              this.navigationPageService.navigateTo(
                CAMRoutePaths.CourseDetailPage,
                {
                  activeTab: CAMTabConfiguration.CourseInfoTab,
                  data: {
                    mode: CourseDetailMode.Edit,
                    courseCriteriaMode: CourseCriteriaDetailMode.View,
                    id: course.id
                  }
                },
                this.courseManagementPageInput
              );
            });
        }
      });
    } else {
      this.navigationPageService.navigateTo(
        CAMRoutePaths.CourseDetailPage,
        {
          activeTab: CAMTabConfiguration.CourseInfoTab,
          subActiveTab: CAMTabConfiguration.AllClassRunsTab,
          data: {
            mode: CourseDetailMode.NewCourse
          }
        },
        this.courseManagementPageInput
      );
    }
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onViewCourse(data: CourseViewModel): void {
    if (this.canViewCourse(data)) {
      this.navigationPageService.navigateTo(
        CAMRoutePaths.CourseDetailPage,
        {
          activeTab: CAMTabConfiguration.CourseInfoTab,
          subActiveTab: CAMTabConfiguration.AllClassRunsTab,
          data: {
            mode: CourseDetailMode.View,
            courseCriteriaMode: CourseCriteriaDetailMode.View,
            id: data.id
          }
        },
        this.courseManagementPageInput
      );
    }
  }

  public onViewArchivedCourse(data: CourseViewModel): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.CourseDetailPage,
      {
        activeTab: CAMTabConfiguration.CourseInfoTab,
        subActiveTab: CAMTabConfiguration.AllClassRunsTab,
        data: {
          mode: CourseDetailMode.View,
          courseCriteriaMode: CourseCriteriaDetailMode.View,
          id: data.id
        }
      },
      this.courseManagementPageInput
    );
  }

  public canViewCourse(course: CourseViewModel): boolean {
    return course.hasViewCourseDetailPermission(this.currentUser);
  }

  public canCreateCourse(): boolean {
    return Course.haveCreateCoursePermission(this.currentUser) && this.selectedTab === CAMTabConfiguration.CoursesTab;
  }

  public onTabSelected(event: SelectEvent): void {
    this.courseManagementPageInput.activeTab = courseManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.courseManagementPageInput);
  }

  public canViewPendingApproval(): boolean {
    return Course.haveApproveCoursePermission(this.currentUser);
  }

  public canViewCourses(): boolean {
    return Course.haveViewCoursesPermission(this.currentUser);
  }

  public canViewPendingRegistrationsApproval(): boolean {
    return Registration.canApproveRegistration(this.currentUser);
  }

  public canViewArchivalCourses(): boolean {
    return Course.canViewArchivalCourses(this.currentUser);
  }

  public onViewCourseApproval(data: CourseViewModel, searchType: SearchCourseType): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.CourseDetailPage,
      {
        activeTab:
          searchType === SearchCourseType.ClassApprover || searchType === SearchCourseType.RegistrationApprover
            ? CAMTabConfiguration.ClassRunsTab
            : CAMTabConfiguration.CourseInfoTab,
        subActiveTab:
          searchType === SearchCourseType.RegistrationApprover
            ? CAMTabConfiguration.HasPendingRegistrationApprovalClassrunTab
            : searchType === SearchCourseType.ClassApprover
            ? CAMTabConfiguration.CancellationRequestsTab
            : CAMTabConfiguration.AllClassRunsTab,
        data: {
          mode: CourseDetailMode.ForApprover,
          id: data.id
        }
      },
      this.courseManagementPageInput
    );
  }

  public selectedContextMenuCourseList(contextMenuEmit: ContextMenuEmit<Course>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Publish:
        this.changeCourseStatus([contextMenuEmit.dataItem], CourseStatus.Published);
        break;
      case ContextMenuAction.Unpublish:
        this.changeCourseStatus([contextMenuEmit.dataItem], CourseStatus.Unpublished);
        break;
      case ContextMenuAction.Delete:
        this.deleteCourse(contextMenuEmit.dataItem);
        break;
      case ContextMenuAction.TransferOwnership:
        this.transferOwnerCourse(contextMenuEmit.dataItem);
        break;
      case ContextMenuAction.Archive:
        this.archiveCourse(contextMenuEmit.dataItem);
        break;
      default:
        break;
    }
  }

  public checkSearchCourseHasDataFnCreator(searchCourseType: SearchCourseType): () => Observable<boolean> {
    return () => {
      return this.listCourseGridComponentService.loadCourses(null, null, searchCourseType, null, 0, 0).pipe(map(data => data.total > 0));
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  private deleteCourse(course: Course): void {
    this.modalService.showConfirmMessage(
      new TranslationMessage(this.moduleFacadeService.globalTranslator, "You're about to delete this course. Do you want to proceed?"),
      () => {
        this.courseRepository.deleteCourse(course.id).then(() => {
          this.showNotification(`${course.courseName} is successfully deleted`, NotificationType.Success);
        });
      }
    );
  }

  private transferOwnerCourse(course: Course): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
      SelectUserDialogComponent,
      SelectUserDialogComponent.selectTransferOwnerCoursesConfig()
    );
    this.subscribe(dialogRef.result, data => {
      if ((<ISelectUserDialogResult>data).id) {
        this.courseRepository
          .transferOwnerCourse(<ITransferOwnerRequest>{
            courseId: course.id,
            newOwnerId: (<ISelectUserDialogResult>data).id
          })
          .then(() => {
            this.showNotification(this.translate('Ownership transferred successfully'), NotificationType.Success);
          });
      }
    });
  }

  private archiveCourse(course: Course): Promise<void> {
    return this.courseRepository.archiveCourse({ ids: [course.id] }).then(_ => {
      this.showNotification();
    });
  }

  private changeCourseStatus(
    courses: Course[],
    status: CourseStatus,
    comment: string = '',
    showNotification: boolean = true
  ): Promise<boolean> {
    return this.courseRepository
      .changeCourseStatus({ ids: courses.map(course => course.id), status: status, comment: comment })
      .pipe(
        map(_ => {
          if (showNotification) {
            this.showNotification();
          }
          courses.forEach(course => (course.status = status));
          this.resetFilter();
          return true;
        }),
        this.untilDestroy()
      )
      .toPromise();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<CourseManagementPageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.courseManagementPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }
}

export enum CourseMassAction {
  PublishCourse = 'publishCourse',
  UnpublishCourse = 'unpublishCourse'
}

export const courseManagementPageTabIndexMap = {
  0: CAMTabConfiguration.CoursesTab,
  1: CAMTabConfiguration.HasCoursePendingApprovalTab,
  2: CAMTabConfiguration.HasPendingClassApprovalCourseTab,
  3: CAMTabConfiguration.HasPendingRegistrationApprovalCourseTab,
  4: CAMTabConfiguration.ArchivalTab
};
