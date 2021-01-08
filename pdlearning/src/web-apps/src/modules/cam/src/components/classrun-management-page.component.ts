import { BasePageComponent, IFilter, IGridFilter, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { ButtonAction, DialogAction, OpalDialogService } from '@opal20/common-components';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  ClassRunDetailMode,
  ClassRunViewModel,
  ContextMenuAction,
  ContextMenuEmit,
  CourseDetailMode,
  ListClassRunGridComponentService,
  ListClassrunGridDisplayColumns,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import {
  ClassRun,
  ClassRunCancellationStatus,
  ClassRunRepository,
  ClassRunRescheduleStatus,
  ClassRunStatus,
  Course,
  IClassRunCancellationStatusRequest,
  IClassRunRescheduleStatusRequest,
  SearchClassRunType,
  UserInfoModel
} from '@opal20/domain-api';
import { Component, HostBinding, Input } from '@angular/core';

import { CancellationRequestDialogComponent } from './dialogs/cancellation-request-dialog.component';
import { ClassRunCancellationInput } from '../models/classrun-cancellation-request-input.model';
import { ClassRunRescheduleInput } from '../models/classrun-reschedule-request-input.model';
import { CourseDetailPageInput } from '../models/course-detail-page-input.model';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { Observable } from 'rxjs';
import { RescheduleRequestDialogComponent } from './dialogs/reschedule-request-dialog.component';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { map } from 'rxjs/operators';

@Component({
  selector: 'classrun-management-page',
  templateUrl: './classrun-management-page.component.html'
})
export class ClassRunManagementPageComponent extends BasePageComponent {
  @Input() public stickyDependElement: HTMLElement;

  public searchText: string = '';
  public filterData: unknown = null;
  public gridData: GridDataResult;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };

  public get course(): Course {
    return this._course;
  }

  @Input()
  public set course(v: Course) {
    if (Utils.isDifferent(this._course, v)) {
      this._course = v;
    }
  }

  @Input() public courseDetailMode: CourseDetailMode;

  public get courseDetailPageInput(): RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined {
    return this._courseDetailPageInput;
  }

  @Input() public set courseDetailPageInput(
    courseDetailPageInput: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined
  ) {
    this._courseDetailPageInput = courseDetailPageInput;
  }

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<ClassRunViewModel>[] = [
    {
      id: 'publish',
      text: this.translateCommon('Publish'),
      conditionFn: dataItem =>
        dataItem.status !== ClassRunStatus.Cancelled &&
        dataItem.canPublish(this.course) &&
        ClassRun.hasPublishUnPublishPermission(this.course, this.currentUser),
      actionFn: dataItems => this.handleMassAction(ClassRunMassAction.PublishClassRun, dataItems),
      hiddenFn: () => this.selectedTab !== CAMTabConfiguration.AllClassRunsTab
    },
    {
      id: 'unpublish',
      text: this.translateCommon('Unpublish'),
      conditionFn: dataItem =>
        dataItem.status !== ClassRunStatus.Cancelled &&
        dataItem.canUnpublish(this.course) &&
        ClassRun.hasPublishUnPublishPermission(this.course, this.currentUser),
      actionFn: dataItems => this.handleMassAction(ClassRunMassAction.UnpublishClassRun, dataItems),
      hiddenFn: () => this.selectedTab !== CAMTabConfiguration.AllClassRunsTab
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
  public SearchClassRunType: typeof SearchClassRunType = SearchClassRunType;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;
  public allClassRunSelectedItems: ClassRunViewModel[] = [];
  public allClassRunDisplayColumns: ListClassrunGridDisplayColumns[] = [
    ListClassrunGridDisplayColumns.selected,
    ListClassrunGridDisplayColumns.title,
    ListClassrunGridDisplayColumns.learningPeriod,
    ListClassrunGridDisplayColumns.applicationPeriod,
    ListClassrunGridDisplayColumns.owner,
    ListClassrunGridDisplayColumns.status,
    ListClassrunGridDisplayColumns.actions
  ];
  public hasCoursePlanningAsParentPage: boolean;
  public get selectedTab(): CAMTabConfiguration {
    return this.courseDetailPageInput.subActiveTab != null ? this.courseDetailPageInput.subActiveTab : CAMTabConfiguration.AllClassRunsTab;
  }
  private _course: Course;
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  private _courseDetailPageInput: RouterPageInput<CourseDetailPageInput, CAMTabConfiguration, CAMTabConfiguration> | undefined;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private navigationPageService: NavigationPageService,
    private classRunRepository: ClassRunRepository,
    private opalDialogService: OpalDialogService,
    private listClassRunGridComponentService: ListClassRunGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public onTabSelected(event: SelectEvent): void {
    this.courseDetailPageInput.subActiveTab = classRunManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.courseDetailPageInput);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public get getSelectedItemsForMassAction(): ClassRunViewModel[] {
    if (this.selectedTab === CAMTabConfiguration.AllClassRunsTab) {
      return this.allClassRunSelectedItems;
    }
    return [];
  }

  public resetSelectedItems(): void {
    this.allClassRunSelectedItems = [];
  }

  public handleMassAction(massAction: ClassRunMassAction, dataItems: ClassRunViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case ClassRunMassAction.PublishClassRun:
        massActionPromise = this.onClassRunGridPublishContextMenuSelected(dataItems, false);
        break;
      case ClassRunMassAction.UnpublishClassRun:
        massActionPromise = this.changeClassRunStatus(dataItems, ClassRunStatus.Unpublished, false);
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  public onViewClassRun(data: ClassRunViewModel): void {
    if (this.canViewClassRun()) {
      this.navigationPageService.navigateTo(
        CAMRoutePaths.ClassRunDetailPage,
        {
          activeTab: CAMTabConfiguration.ClassRunInfoTab,
          data: {
            mode: ClassRunDetailMode.View,
            id: data.id,
            courseId: this.course.id
          }
        },
        this.courseDetailPageInput
      );
    }
  }

  public onViewClassRunRequest(data: ClassRunViewModel, searchType: SearchClassRunType): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.ClassRunDetailPage,
      {
        activeTab: CAMTabConfiguration.ClassRunInfoTab,
        data: {
          mode: ClassRunDetailMode.View,
          searchType: searchType,
          id: data.id,
          courseId: this.course.id
        }
      },
      this.courseDetailPageInput
    );
  }

  public onClassRunGridContextMenuSelected(contextMenuEmit: ContextMenuEmit<ClassRun>): void {
    switch (contextMenuEmit.event.item.id) {
      case ContextMenuAction.Publish:
        this.onClassRunGridPublishContextMenuSelected([contextMenuEmit.dataItem]);
        break;
      case ContextMenuAction.Unpublish:
        this.changeClassRunStatus([contextMenuEmit.dataItem], ClassRunStatus.Unpublished);
        break;
      case ContextMenuAction.CancellationRequest:
        this.showCancellationRequestDialog(
          {
            title: `${this.translate('Cancel Request')}: ${contextMenuEmit.dataItem.classTitle}`,
            classRunId: contextMenuEmit.dataItem.id
          },
          contextMenuEmit.dataItem.id,
          ClassRunCancellationStatus.PendingApproval
        );
        break;
      case ContextMenuAction.RescheduleRequest:
        this.showRescheduleRequestDialog(
          {
            title: `${this.translate('Reschedule Request')}: ${contextMenuEmit.dataItem.classTitle}`,
            classRunId: contextMenuEmit.dataItem.id,
            courseServiceSchemeIds: this.course.serviceSchemeIds
          },
          contextMenuEmit.dataItem.id,
          ClassRunRescheduleStatus.PendingApproval
        );
        break;
    }
  }

  public onClassRunGridPublishContextMenuSelected(classRuns: ClassRun[], showNotification: boolean = true): Promise<boolean> {
    if (classRuns.some(classRun => classRun.planningStartTime == null || classRun.planningEndTime == null)) {
      return this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg: 'You need to provide both Class Start Time and Class End Time before you are able to publish the classrun.',
          yesBtnText: 'OK',
          hideNoBtn: true
        })
        .pipe(map(_ => false))
        .toPromise();
    } else {
      return this.changeClassRunStatus(classRuns, ClassRunStatus.Published, showNotification);
    }
  }

  public onCreateClassRun(): void {
    if (this.gridData && this.course.numOfPlannedClass <= this.gridData.total) {
      this.opalDialogService
        .openConfirmDialog({
          confirmTitle: 'Warning',
          confirmMsg:
            'You are about to create a class run that exceeds the planned number of class runs for this course. Do you want to proceed?',
          yesBtnText: 'Ok',
          noBtnText: 'Cancel'
        })
        .subscribe(action => {
          if (action === DialogAction.OK) {
            this.navigationCreateClassRun();
          }
        });
    } else {
      this.navigationCreateClassRun();
    }
  }

  public canCreateClassRun(): boolean {
    return (
      this.course.hasCreateClassRunPermission(this.currentUser) &&
      this.course.canCreateClassRun() &&
      this.selectedTab === CAMTabConfiguration.AllClassRunsTab
    );
  }

  public canViewClassRuns(): boolean {
    return this.course.hasViewClassRunsPermission(this.currentUser);
  }

  public canViewRescheduleClassRequests(): boolean {
    return this.hasApprovalRescheduleClassRunRequestPermission() && !this.hasCoursePlanningAsParentPage;
  }

  public canViewCancellationClassRequests(): boolean {
    return this.hasApprovalCancellationClassRunRequestPermission();
  }

  public showCancellationRequestDialog(input: unknown, classRunId: string, status: ClassRunCancellationStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(CancellationRequestDialogComponent, input);

    this.subscribe(dialogRef.result, (data: ClassRunCancellationInput) => {
      if (data.comment) {
        const cancellationRequest: IClassRunCancellationStatusRequest = {
          ids: [classRunId],
          comment: data.comment,
          cancellationStatus: status
        };
        this.changeCancellationStatus(cancellationRequest);
      }
    });
  }

  public showRescheduleRequestDialog(input: unknown, classRunId: string, status: ClassRunRescheduleStatus): void {
    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(RescheduleRequestDialogComponent, input);

    this.subscribe(dialogRef.result, (data: ClassRunRescheduleInput) => {
      if (data.comment && data.startDateTime && data.endDateTime) {
        const rescheduleRequest: IClassRunRescheduleStatusRequest = {
          ids: [classRunId],
          comment: data.comment,
          startDateTime: data.startDateTime,
          endDateTime: data.endDateTime,
          rescheduleSessions: data.rescheduleSessions,
          rescheduleStatus: status
        };
        this.changeRescheduleStatus(rescheduleRequest);
      }
    });
  }

  public canViewPendingRegistrationsAndWithdrawals(): boolean {
    return (
      this.courseDetailMode === CourseDetailMode.ForApprover &&
      this.course.hasAdministrationPermission(this.currentUser) &&
      !this.hasCoursePlanningAsParentPage
    );
  }

  public loadedDataGrid(gridData: GridDataResult): void {
    this.gridData = gridData;
  }

  public checkSearchClassHasDataFnCreator(searchType: SearchClassRunType): () => Observable<boolean> {
    const filterClassRun: IFilter = {
      containFilters: [
        {
          field: 'Status',
          values: [ClassRunStatus.Published, ClassRunStatus.Unpublished],
          notContain: false
        }
      ],
      fromToFilters: []
    };
    return () => {
      return this.listClassRunGridComponentService
        .loadClassRunsByCourseId(this.courseDetailPageInput.data.id, searchType, '', filterClassRun, false, false, 0, 0)
        .pipe(
          map(result => {
            return result.total > 0;
          })
        );
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public hasApprovalCancellationClassRunRequestPermission(): boolean {
    return ClassRun.hasApprovalCancellationClassRunRequestPermission(this.course, this.currentUser);
  }

  public hasApprovalRescheduleClassRunRequestPermission(): boolean {
    return ClassRun.hasApprovalRescheduleClassRunRequestPermission(this.course, this.currentUser);
  }

  protected onInit(): void {
    this.hasCoursePlanningAsParentPage = this.navigationPageService.findParentOfCurrentRouter(CAMRoutePaths.CoursePlanningPage) != null;
  }

  protected currentUserPermissionDic(): IPermissionDictionary {
    return this.currentUser.permissionDic;
  }

  private canViewClassRun(): boolean {
    return this.course.hasViewClassRunsPermission(this.currentUser);
  }

  private changeClassRunStatus(classRuns: ClassRun[], status: ClassRunStatus, showNotification: boolean = true): Promise<boolean> {
    return this.classRunRepository
      .changeClassRunStatus({ ids: classRuns.map(p => p.id), status: status })
      .pipe(
        this.untilDestroy(),
        map(() => {
          if (showNotification) {
            this.showNotification();
          }
          classRuns.forEach(x => (x.status = status));
          this.resetFilter();
          return true;
        })
      )
      .toPromise();
  }

  private changeCancellationStatus(request: IClassRunCancellationStatusRequest): void {
    this.subscribe(this.classRunRepository.changeClassRunCancellationStatus(request), () => {
      this.showNotification();
    });
  }

  private changeRescheduleStatus(request: IClassRunRescheduleStatusRequest): void {
    this.subscribe(this.classRunRepository.changeClassRunRescheduleStatus(request, this.course.id), () => {
      this.showNotification();
    });
  }

  private navigationCreateClassRun(): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.ClassRunDetailPage,
      {
        activeTab: CAMTabConfiguration.ClassRunInfoTab,
        data: {
          mode: ClassRunDetailMode.NewClassRun,
          courseId: this.course.id
        }
      },
      this.courseDetailPageInput
    );
  }
}

export enum ClassRunMassAction {
  PublishClassRun = 'publishClassRun',
  UnpublishClassRun = 'unpublishClassRun'
}

export const classRunManagementPageTabIndexMap = {
  0: CAMTabConfiguration.AllClassRunsTab,
  1: CAMTabConfiguration.CancellationRequestsTab,
  2: CAMTabConfiguration.RescheduleRequestsTab,
  3: CAMTabConfiguration.HasPendingRegistrationApprovalClassrunTab
};
