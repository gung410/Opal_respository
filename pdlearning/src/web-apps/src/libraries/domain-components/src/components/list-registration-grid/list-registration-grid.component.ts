import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { COURSE_REGISTRATION_STATUS_PREFIX_MAP, COURSE_REGISTRATION_TYPE_MAP } from '../../models/course-registration-type.model';
import { CellClickEvent, GridDataResult, RowClassArgs } from '@progress/kendo-angular-grid';
import {
  ClassRun,
  ClassRunChangeStatus,
  ClassRunRepository,
  Course,
  CourseRepository,
  Registration,
  RegistrationLearningStatus,
  RegistrationStatus,
  SearchRegistrationsType,
  UserInfoModel,
  WithdrawalStatus
} from '@opal20/domain-api';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ContextMenuItem, OpalDialogService } from '@opal20/common-components';
import { Observable, Subscription, combineLatest, of } from 'rxjs';
import {
  REGISTRATION_CHANGE_CLASSRUN_STATUS_COLOR_MAP,
  REGISTRATION_LEARNING_STATUS_COLOR_MAP,
  REGISTRATION_STATUS_ADDING_PARTICIPANTS_COLOR_MAP,
  REGISTRATION_STATUS_COLOR_MAP,
  WITHDRAWAL_STATUS_COLOR_MAP
} from '../../models/course-registration-status-color-map.model';
import { map, switchMap } from 'rxjs/operators';

import { ContextMenuAction } from '../../models/context-menu-action.model';
import { ContextMenuEmit } from '../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { CourseCriteriaRegistrationViolationDialogComponent } from '../course-criteria-registration-violation-dialog/course-criteria-registration-violation-dialog.component';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListRegistrationGridComponentService } from '../../services/list-registration-component.service';
import { RegistrationViewModel } from '../../models/registration-view.model';

@Component({
  selector: 'list-registration-grid',
  templateUrl: './list-registration-grid.component.html'
})
export class ListRegistrationGridComponent extends BaseGridComponent<RegistrationViewModel> {
  public get courseId(): string | undefined {
    return this._courseId;
  }

  @Input()
  public set courseId(v: string | undefined) {
    if (Utils.isDifferent(this._courseId, v)) {
      this._courseId = v;
    }
  }

  public get classRunId(): string | undefined {
    return this._classRunId;
  }

  @Input()
  public set classRunId(v: string | undefined) {
    if (Utils.isDifferent(this._classRunId, v)) {
      this._classRunId = v;
      if (this.initiated) {
        this.refreshData();
      }
    }
  }

  @Input() public searchType: SearchRegistrationsType = SearchRegistrationsType.ClassRunRegistration;
  @Input() public set displayColumns(displayColumns: ListRegistrationGridDisplayColumns[]) {
    this._displayColumns = displayColumns;
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, p => true);
  }

  @Input() public displayLearningStatus: boolean = false;
  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<RegistrationViewModel>> = new EventEmitter();
  @Output('viewRegistration') public viewRegistrationEvent: EventEmitter<RegistrationViewModel> = new EventEmitter<RegistrationViewModel>();
  public contextMenuItemsForRegistration: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Confirm,
      text: this.translateCommon('Confirm'),
      icon: 'track-changes-accept'
    },
    {
      id: ContextMenuAction.Reject,
      text: this.translateCommon('Reject'),
      icon: 'track-changes-reject'
    },
    {
      id: ContextMenuAction.MoveToWaitlist,
      text: this.translateCommon('Move to Waitlist'),
      icon: 'arrow-right'
    },
    {
      id: ContextMenuAction.WithdrawManuallyByCA,
      text: this.translateCommon('Withdraw'),
      icon: 'arrow-right'
    },
    {
      id: ContextMenuAction.ChangeRegistrationClass,
      text: this.translateCommon('Change Class'),
      icon: 'arrow-right'
    }
  ];

  public contextMenuItemsForWaitlist: ContextMenuItem[] = [
    {
      id: ContextMenuAction.SendOffer,
      text: this.translateCommon('Send Offer'),
      icon: 'table-align-top-left'
    },
    {
      id: ContextMenuAction.WithdrawManuallyByCA,
      text: this.translateCommon('Withdraw'),
      icon: 'arrow-right'
    },
    {
      id: ContextMenuAction.ChangeRegistrationClass,
      text: this.translateCommon('Change Class'),
      icon: 'arrow-right'
    }
  ];

  public contextMenuItemsForWithdraw: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Confirm,
      text: this.translateCommon('Confirm'),
      icon: 'track-changes-accept'
    },
    {
      id: ContextMenuAction.Reject,
      text: this.translateCommon('Reject'),
      icon: 'track-changes-reject'
    }
  ];

  public contextMenuItemsForChangeClassRun: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Confirm,
      text: this.translateCommon('Confirm'),
      icon: 'track-changes-accept'
    },
    {
      id: ContextMenuAction.Reject,
      text: this.translateCommon('Reject'),
      icon: 'track-changes-reject'
    }
  ];

  public contextMenuItemsForParticipants: ContextMenuItem[] = [
    {
      id: ContextMenuAction.Completed,
      text: this.translateCommon('Complete'),
      icon: 'track-changes-accept'
    },
    {
      id: ContextMenuAction.Incomplete,
      text: this.translateCommon('Incomplete'),
      icon: 'track-changes-reject'
    },
    {
      id: ContextMenuAction.WithdrawManuallyByCA,
      text: this.translateCommon('Withdraw'),
      icon: 'arrow-right'
    },
    {
      id: ContextMenuAction.ChangeRegistrationClass,
      text: this.translateCommon('Change Class'),
      icon: 'arrow-right'
    }
  ];

  public classRun: ClassRun = new ClassRun();
  public course: Course = new Course();
  public registration: Registration = new Registration();
  public SearchRegistrationsType: typeof SearchRegistrationsType = SearchRegistrationsType;
  public RegistrationStatus: typeof RegistrationStatus = RegistrationStatus;
  public query: Observable<unknown>;
  public loading: boolean;
  public registrationTypeMap = COURSE_REGISTRATION_TYPE_MAP;
  public dicDisplayColumns: Dictionary<boolean> = {};
  public classRunsDict: Dictionary<ClassRun> = {};

  private _loadDataSub: Subscription = new Subscription();
  private _courseId: string | undefined;
  private _classRunId: string | undefined;
  private _displayColumns: ListRegistrationGridDisplayColumns[] = [
    ListRegistrationGridDisplayColumns.name,
    ListRegistrationGridDisplayColumns.organisation,
    ListRegistrationGridDisplayColumns.signedUp,
    ListRegistrationGridDisplayColumns.registrationType,
    ListRegistrationGridDisplayColumns.status
  ];
  private _currentUser = UserInfoModel.getMyUserInfo();

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public opalDialogService: OpalDialogService,
    public listRegistrationGridComponentService: ListRegistrationGridComponentService,
    public classRunRepository: ClassRunRepository,
    public courseRepository: CourseRepository
  ) {
    super(moduleFacadeService);
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, p => true);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: RegistrationViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public showContextMenu(dataItem: RegistrationViewModel): boolean {
    return (
      ((this.searchType === SearchRegistrationsType.ClassRunRegistration &&
        dataItem.canActionOnRegistrationList(this.classRun, this.course) &&
        Registration.hasActionOnRegistrationListPermission(this._currentUser, this.course)) ||
        (this.searchType === SearchRegistrationsType.Participant &&
          ((!this.displayLearningStatus &&
            Registration.canActionOnParticipantList(this.classRun, this.course) &&
            Registration.hasActionOnParticipantListPermission(this._currentUser, this.course)) ||
            (this.displayLearningStatus &&
              Registration.canDoLearningActionOnParticipantList(this.course) &&
              Registration.hasDoLearningActionOnParticipantListPermission(this._currentUser, this.course)))) ||
        (this.searchType === SearchRegistrationsType.Withdrawal &&
          dataItem.canActionOnWithdrawalRequestList(this.classRun, this.course) &&
          Registration.hasActionOnWithdrawalRequestListPermission(this._currentUser, this.course)) ||
        (this.searchType === SearchRegistrationsType.Waitlist &&
          Registration.canActionOnWaitlist(this.classRun, this.course) &&
          Registration.hasActionOnWaitlistPermission(this._currentUser, this.course) &&
          (dataItem.status === RegistrationStatus.WaitlistConfirmed ||
            dataItem.status === RegistrationStatus.WaitlistPendingApprovalByLearner ||
            dataItem.status === RegistrationStatus.OfferPendingApprovalByLearner)) ||
        (this.searchType === SearchRegistrationsType.ChangeClassRun &&
          dataItem.canActionOnChangeClassRequestList(this.classRun, this.course) &&
          Registration.hasActionOnChangeClassRequestListPermission(this._currentUser, this.course))) &&
      this.getContextMenuByRegistration(dataItem).length !== 0
    );
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = combineLatest(
      this.listRegistrationGridComponentService.loadRegistration(
        this.courseId,
        this.classRunId,
        this.searchType,
        this.filter.search,
        this.dicDisplayColumns[ListRegistrationGridDisplayColumns.courseJoined],
        this.filter.filter,
        null,
        this.state.skip,
        this.state.take,
        this.checkAll,
        () => this.selecteds,
        this.dicDisplayColumns[ListRegistrationGridDisplayColumns.noOfAssignmentDone],
        this.dicDisplayColumns[ListRegistrationGridDisplayColumns.attendanceRatioOfPresent]
      ),
      this.courseId != null ? this.courseRepository.loadCourse(this.courseId) : of(null)
    )
      .pipe(
        switchMap(([gridData, course]) => {
          const allRegistrationClassRunIds = Utils.distinct(
            [this.classRunId]
              .concat(Utils.flatTwoDimensionsArray(gridData.data.map(p => [p.classRunChangeId, p.classRunId])))
              .filter(p => p != null)
          );
          return this.classRunRepository
            .loadClassRunsByIds(allRegistrationClassRunIds)
            .pipe(map(classruns => [gridData, classruns, course]));
        }),
        this.untilDestroy()
      )
      .pipe(this.untilDestroy())
      .subscribe(([gridData, classruns, course]: [OpalGridDataResult<RegistrationViewModel>, ClassRun[], Course]) => {
        this.gridData = gridData;
        this.classRunsDict = Utils.toDictionary(classruns, p => p.id);
        this.classRun = this.classRunId != null ? this.classRunsDict[this.classRunId] : new ClassRun();
        this.course = course != null ? course : new Course();
        this.updateSelectedsAndGridData();
      });
  }

  public getValidatedGridData(): GridDataResult {
    return this.classRun.isNotCancelled() ? this.gridData : null;
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (event.dataItem instanceof RegistrationViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewRegistrationEvent.emit(event.dataItem);
    }
  }

  public canChangeClassByCA(registration: Registration): boolean {
    return registration.canChangeClassByCA(this.course) && Registration.hasChangeClassByCAPermission(this.course, this._currentUser);
  }

  public getContextMenuByRegistration(dataItem: RegistrationViewModel): ContextMenuItem[] {
    if (this.course.isArchived()) {
      return [];
    }
    if (this.searchType === SearchRegistrationsType.ClassRunRegistration) {
      return this.contextMenuItemsForRegistration.filter(p => {
        if (
          this.displayLearningStatus ||
          (p.id === ContextMenuAction.WithdrawManuallyByCA && !this.canWithdrawRegistrationByCA(dataItem)) ||
          (p.id === ContextMenuAction.ChangeRegistrationClass && !this.canChangeClassByCA(dataItem))
        ) {
          return false;
        }
        return true;
      });
    } else if (this.searchType === SearchRegistrationsType.Waitlist) {
      return this.contextMenuItemsForWaitlist.filter(p => {
        if (
          this.displayLearningStatus ||
          (p.id === ContextMenuAction.WithdrawManuallyByCA && !this.canWithdrawRegistrationByCA(dataItem)) ||
          (p.id === ContextMenuAction.ChangeRegistrationClass && !this.canChangeClassByCA(dataItem)) ||
          (p.id === ContextMenuAction.SendOffer &&
            !dataItem.canSendOffer(this._currentUser, this.classRun, this.course) &&
            Registration.hasSendOfferPermission(this._currentUser, this.course))
        ) {
          return false;
        }
        return true;
      });
    } else if (this.searchType === SearchRegistrationsType.Withdrawal) {
      return this.contextMenuItemsForWithdraw;
    } else if (this.searchType === SearchRegistrationsType.ChangeClassRun) {
      return this.contextMenuItemsForChangeClassRun;
    } else if (this.searchType === SearchRegistrationsType.Participant) {
      return this.contextMenuItemsForParticipants.filter(p => {
        if (!this.displayLearningStatus) {
          if (
            (p.id === ContextMenuAction.WithdrawManuallyByCA && this.canWithdrawRegistrationByCA(dataItem)) ||
            (p.id === ContextMenuAction.ChangeRegistrationClass && this.canChangeClassByCA(dataItem))
          ) {
            return true;
          }
          return false;
        } else {
          if (p.id === ContextMenuAction.Completed && !(dataItem.learningStatus === RegistrationLearningStatus.Failed)) {
            return false;
          }
          if (p.id === ContextMenuAction.Incomplete && !(dataItem.learningStatus === RegistrationLearningStatus.Completed)) {
            return false;
          }
          if (p.id === ContextMenuAction.Completed || p.id === ContextMenuAction.Incomplete) {
            return (
              Registration.canManageRegistrations(this.course) &&
              Registration.hasManageRegistrationsPermission(this._currentUser, this.course, this.classRun)
            );
          }
          return false;
        }
      });
    } else {
      return [];
    }
  }

  public canWithdrawRegistrationByCA(registrationVm: RegistrationViewModel): boolean {
    return registrationVm.canWithdrawByCA(this.course) && Registration.hasWithdrawByCAPermission(this.course, this._currentUser);
  }

  public displayStatus(
    dataItem: RegistrationViewModel
  ): RegistrationStatus | WithdrawalStatus | ClassRunChangeStatus | RegistrationLearningStatus {
    if (dataItem.isExpiredOrNeedToSetExpired(this.course, this.classRun)) {
      return RegistrationStatus.Expired;
    } else if (this.searchType === SearchRegistrationsType.Withdrawal) {
      return dataItem.withdrawalStatus;
    } else if (this.displayLearningStatus) {
      return dataItem.learningStatus;
    } else if (this.searchType === SearchRegistrationsType.ChangeClassRun) {
      return dataItem.classRunChangeStatus;
    }

    return dataItem.status;
  }

  public displayRequestDate(dataItem: RegistrationViewModel): Date {
    if (this.searchType === SearchRegistrationsType.Withdrawal) {
      return dataItem.withdrawalRequestDate;
    } else if (this.searchType === SearchRegistrationsType.ChangeClassRun) {
      return dataItem.classRunChangeRequestedDate;
    }
  }

  public displayPrefixStatus(dataItem: RegistrationViewModel): string {
    if (
      this.searchType === SearchRegistrationsType.Withdrawal ||
      this.searchType === SearchRegistrationsType.AddedByCA ||
      this.searchType === SearchRegistrationsType.ChangeClassRun ||
      this.displayLearningStatus
    ) {
      return '';
    } else if (
      dataItem.isExpiredOrNeedToSetExpired(this.course, this.classRun) ||
      (dataItem.status !== RegistrationStatus.WaitlistPendingApprovalByLearner &&
        dataItem.status !== RegistrationStatus.WaitlistConfirmed &&
        dataItem.status !== RegistrationStatus.OfferPendingApprovalByLearner &&
        dataItem.status !== RegistrationStatus.AddedByCAClassfull)
    ) {
      return COURSE_REGISTRATION_STATUS_PREFIX_MAP[dataItem.registrationType];
    }
  }

  public displayLearningContentProgressColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.learningContentProgress];
  }
  public displayNoOfAssignmentDoneColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.noOfAssignmentDone];
  }
  public displayAttendanceRatioOfPresentColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.attendanceRatioOfPresent];
  }

  public displayServiceSchemeColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.serviceScheme];
  }

  public displayDevelopmentalRole(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.developmentalRole];
  }

  public displayCheckboxColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.selected];
  }

  public displayNameColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.name];
  }

  public displayOrganisationColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.organisation];
  }

  public displayAccountTypeColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.accountType];
  }

  public displaySignedUpColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.signedUp];
  }

  public displayDesignationColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.designation];
  }

  public displayTeachingLevelColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.teachingLevel];
  }

  public displayTeachingSubjectJobFamilyColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.teachingSubjectJobFamily];
  }

  public displayRegistrationTypeColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.registrationType];
  }

  public displayCourseCriteriaColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.courseCriteria];
  }

  public displayRequestDateColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.requestDate];
  }

  public displayReasonColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.reason];
  }

  public displayAddedToClassColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.addedToClassRun];
  }

  public displayAddedDateColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.addedDate];
  }

  public displayStatusColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.status];
  }

  public displayChangeToColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.changeTo];
  }

  public displayPostCourseSurveyCompleted(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.postCourseEvaluationFormCompleted];
  }

  public displayActionsColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.actions];
  }

  public displayCourseJoinedColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.courseJoined];
  }

  public displayECertificateIssuanceColumn(): boolean {
    return this.dicDisplayColumns[ListRegistrationGridDisplayColumns.eCertificateIssuanceDate];
  }

  public showCourseCriteriaRegistrationViolationDetailDialog(e: Event, item: RegistrationViewModel): void {
    e.preventDefault();
    e.stopPropagation();
    this.opalDialogService.openDialogRef(CourseCriteriaRegistrationViolationDialogComponent, {
      registrationVM: item
    });
  }

  public statusColorMap(dataItem: Registration): unknown {
    if (dataItem.isExpiredOrNeedToSetExpired(this.course, this.classRun)) {
      return REGISTRATION_STATUS_COLOR_MAP;
    } else if (this.searchType === SearchRegistrationsType.Withdrawal) {
      return WITHDRAWAL_STATUS_COLOR_MAP;
    } else if (this.searchType === SearchRegistrationsType.AddedByCA) {
      return REGISTRATION_STATUS_ADDING_PARTICIPANTS_COLOR_MAP;
    } else if (this.displayLearningStatus) {
      return REGISTRATION_LEARNING_STATUS_COLOR_MAP;
    } else if (this.searchType === SearchRegistrationsType.ChangeClassRun) {
      return REGISTRATION_CHANGE_CLASSRUN_STATUS_COLOR_MAP;
    }
    return REGISTRATION_STATUS_COLOR_MAP;
  }

  protected onInit(): void {
    super.onInit();
  }
}

export enum ListRegistrationGridDisplayColumns {
  selected = 'selected',
  name = 'name',
  organisation = 'organisation',
  accountType = 'accountType',
  signedUp = 'signedUp',
  designation = 'designation',
  teachingSubjectJobFamily = 'teachingSubjectJobFamily',
  teachingLevel = 'teachingLevel',
  registrationType = 'registrationType',
  courseCriteria = 'courseCriteria',
  requestDate = 'requestDate',
  reason = 'reason',
  status = 'status',
  learningContentProgress = 'learningContentProgress',
  noOfAssignmentDone = 'noOfAssignmentDone',
  attendanceRatioOfPresent = 'attendanceRatioOfPresent',
  actions = 'actions',
  changeTo = 'changeTo',
  addedToClassRun = 'addedToClassRun',
  addedDate = 'addedDate',
  serviceScheme = 'serviceScheme',
  developmentalRole = 'developmentalRole',
  postCourseEvaluationFormCompleted = 'postCourseEvaluationFormCompleted',
  courseJoined = 'courseJoined',
  eCertificateIssuanceDate = 'eCertificateIssuanceDate'
}
