import { BasePageComponent, ComponentType, IGridFilter, ModuleFacadeService } from '@opal20/infrastructure';
import { ButtonAction, DialogAction, OpalDialogService } from '@opal20/common-components';
import {
  CommentDialogComponent,
  ContextMenuAction,
  CourseDetailMode,
  CourseFilterComponent,
  CourseFilterModel,
  CourseViewModel,
  ICourseFilterSetting,
  IDialogActionEvent,
  LMMRoutePaths,
  LMMTabConfiguration,
  ListCourseGridComponentService,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import { Component, HostBinding } from '@angular/core';
import {
  ContentStatus,
  Course,
  CourseStatus,
  LMM_PERMISSIONS,
  LearningContentRepository,
  SearchCourseType,
  SystemRoleEnum,
  UserInfoModel
} from '@opal20/domain-api';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CourseManagementPageInput } from '../models/course-management-page-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { NAVIGATORS } from '../lmm.config';
import { SelectEvent } from '@progress/kendo-angular-layout';

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
  public courseManagementPageInput: RouterPageInput<CourseManagementPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.CourseManagementPage
  ] as RouterPageInput<CourseManagementPageInput, LMMTabConfiguration, unknown>;
  public SearchCourseType: typeof SearchCourseType = SearchCourseType;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
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
  public pendingCourseSelectedItems: CourseViewModel[] = [];

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<CourseViewModel>[] = [
    {
      id: 'publish',
      text: this.translateCommon('Publish'),
      conditionFn: dataItem => dataItem.canPublishContent(this.currentUser),
      actionFn: dataItems => this.handleMassAction(CourseMassAction.PublishCourseContent, dataItems),
      hiddenFn: () => this.selectedTab !== LMMTabConfiguration.CoursesTab
    },
    {
      id: 'unpublish',
      text: this.translateCommon('Unpublish'),
      conditionFn: dataItem => dataItem.canUnpublishContent(this.currentUser),
      actionFn: dataItems => this.handleMassAction(CourseMassAction.UnpublishCourseContent, dataItems),
      hiddenFn: () => this.selectedTab !== LMMTabConfiguration.CoursesTab
    }
  ];

  public courseFilterSettings: ICourseFilterSetting = {
    statusSelectItems: [
      { value: CourseStatus.Approved, label: 'Approved' },
      { value: CourseStatus.PlanningCycleVerified, label: 'Verified' },
      { value: CourseStatus.PlanningCycleCompleted, label: 'Completed Planning' },
      { value: CourseStatus.Published, label: 'Published' },
      { value: CourseStatus.Unpublished, label: 'Unpublished' },
      { value: CourseStatus.Completed, label: 'Completed' }
    ],
    contentStatusSelectItems: [
      { value: ContentStatus.Draft, label: 'Draft' },
      { value: ContentStatus.PendingApproval, label: 'Pending Approval' },
      { value: ContentStatus.Approved, label: 'Approved' },
      { value: ContentStatus.Rejected, label: 'Rejected' },
      { value: ContentStatus.Published, label: 'Published' },
      { value: ContentStatus.Unpublished, label: 'Unpublished' },
      { value: ContentStatus.Expired, label: 'Expired' }
    ]
  };

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public get selectedTab(): LMMTabConfiguration {
    return this.courseManagementPageInput.activeTab != null ? this.courseManagementPageInput.activeTab : LMMTabConfiguration.CoursesTab;
  }

  public static hasViewPermissions(currentUser: UserInfoModel): boolean {
    return (
      currentUser.hasPermissionPrefix(LMM_PERMISSIONS.LearningManagement) ||
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
    private listCourseGridComponentService: ListCourseGridComponentService,
    private opalDialogService: OpalDialogService,
    private learningContentRepository: LearningContentRepository
  ) {
    super(moduleFacadeService);
  }

  public handleMassAction(massAction: CourseMassAction, dataItems: CourseViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case CourseMassAction.PublishCourseContent:
        massActionPromise = this.changeCourseContentStatus(dataItems, ContentStatus.Published, '', false).toPromise();
        break;
      case CourseMassAction.UnpublishCourseContent:
        massActionPromise = this.changeCourseContentStatus(dataItems, ContentStatus.Unpublished, '', false).toPromise();
        break;
      case CourseMassAction.ApproveCourseContent:
        massActionPromise = this.showApprovalDialog(
          dataItems,
          {
            title: `${this.translate('Approve Content Course')}`,
            requiredCommentField: false
          },
          ContentStatus.Approved,
          false
        );
        break;
      case CourseMassAction.RejectCourseContent:
        massActionPromise = this.showApprovalDialog(
          dataItems,
          {
            title: `${this.translate('Reject Content Course')}`
          },
          ContentStatus.Rejected,
          false
        );
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.courseManagementPageInput.activeTab = courseManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.courseManagementPageInput);
  }

  public get getSelectedItemsForMassAction(): CourseViewModel[] {
    if (this.selectedTab === LMMTabConfiguration.CoursesTab) {
      return this.allCourseSelectedItems;
    } else if (this.selectedTab === LMMTabConfiguration.PendingApprovalTab) {
      return this.pendingCourseSelectedItems;
    }
    return [];
  }

  public resetSelectedItems(): void {
    this.allCourseSelectedItems = [];
    this.pendingCourseSelectedItems = [];
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onApplyFilter(data: CourseFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public onViewCourse(data: CourseViewModel): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.CourseDetailPage,
      {
        activeTab: LMMTabConfiguration.CourseInfoTab,
        subActiveTab: LMMTabConfiguration.AllClassRunsTab,
        data: {
          mode: CourseDetailMode.EditContent,
          id: data.id
        }
      },
      this.courseManagementPageInput
    );
  }

  public canViewPendingApproval(): boolean {
    return this.currentUser && Course.haveApproveCoursePermission(this.currentUser);
  }

  public checkSearchCourseHasDataFnCreator(searchCourseType: SearchCourseType): () => Observable<boolean> {
    return () => {
      return this.listCourseGridComponentService.loadCourses(null, null, searchCourseType, null, 0, 0).pipe(map(data => data.total > 0));
    };
  }

  public onViewCourseApproval(data: CourseViewModel): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.CourseDetailPage,
      {
        activeTab: LMMTabConfiguration.CourseInfoTab,
        subActiveTab: LMMTabConfiguration.AllClassRunsTab,
        data: {
          mode: CourseDetailMode.ForApprover,
          id: data.id
        }
      },
      this.courseManagementPageInput
    );
  }

  protected onInit(): void {
    this.getNavigatePageData();
  }

  private showApprovalDialog(
    courses: CourseViewModel[],
    input: unknown,
    contentStatus: ContentStatus,
    showNotification: boolean = true
  ): Promise<boolean> {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CommentDialogComponent, input);
    return dialogRef.result
      .pipe(
        switchMap((data: IDialogActionEvent) => {
          if (data.action === DialogAction.OK) {
            return this.changeCourseContentStatus(courses, contentStatus, data.comment, showNotification);
          }
          return of(false);
        })
      )
      .toPromise();
  }

  private changeCourseContentStatus(
    courses: CourseViewModel[],
    contentStatus: ContentStatus,
    comment: string = '',
    showNotification: boolean = true
  ): Observable<boolean> {
    return this.learningContentRepository
      .changeCourseContentStatus({ ids: courses.map(p => p.id), contentStatus: contentStatus, comment: comment })
      .pipe(
        this.untilDestroy(),
        map(_ => {
          if (showNotification) {
            this.showNotification();
          }
          courses.forEach(course => (course.contentStatus = contentStatus));
          this.resetFilter();
          return true;
        })
      );
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<CourseManagementPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.courseManagementPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }
}

export enum CourseMassAction {
  PublishCourseContent = 'publishCourseContent',
  UnpublishCourseContent = 'unpublishCourseContent',
  ApproveCourseContent = 'approveCourseContent',
  RejectCourseContent = 'rejectCourseContent'
}

export const courseManagementPageTabIndexMap = {
  0: LMMTabConfiguration.CoursesTab,
  1: LMMTabConfiguration.PendingApprovalTab
};
