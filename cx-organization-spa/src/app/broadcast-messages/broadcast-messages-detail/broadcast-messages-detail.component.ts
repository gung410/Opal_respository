import {
  ChangeDetectorRef,
  Component,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'app-auth/auth.service';
import { User } from 'app-models/auth.model';
import { DateTimeUtil, DAY_OF_WEEK } from 'app-utilities/date-time-utils';
import { StringUtil } from 'app-utilities/string-utils';
import { Utils } from 'app-utilities/utils';
import { SystemRole } from 'app/core/models/system-role';
import { DepartmentHierarchicalService } from 'app/department-hierarchical/department-hierarchical.service';
import { Department } from 'app/department-hierarchical/models/department.model';
import { DepartmentQueryModel } from 'app/department-hierarchical/models/filter-params.model';
import { AppConstant } from 'app/shared/app.constant';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import {
  ConfirmDialogComponent,
  ConfirmDialogModel
} from 'app/shared/components/confirm-dialog/confirm-dialog.component';
import { NUMBER_OF_REMOVED_ITEM_DEFAULT } from 'app/shared/constants/common.const';
import { HTTP_STATUS_CODE } from 'app/shared/constants/http-status-code';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { TargetUserType } from 'app/shared/constants/target-user-type.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { ifValidator } from 'app/shared/validators/if-validator';
import { noContentWhiteSpaceValidator } from 'app/shared/validators/no-content-white-space-validator';
import {
  UserManagement,
  UserManagementQueryModel
} from 'app/user-accounts/models/user-management.model';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { UserGroupsDataService } from 'app/user-groups/user-groups-data.service';
import {
  UserGroupDto,
  UserGroupFilterParams
} from 'app/user-groups/user-groups.model';
import * as _ from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { DayRepetition } from '../constant/day-repetition.enum';
import { MonthRepetition } from '../constant/month-repetition.enum';
import { DepartmentsDialogComponent } from '../departments-dialog/departments-dialog.component';
import { BroadcastMessageDetailViewModel } from '../models/broadcast-message-detail.view.model';
import { BroadcastMessagesDto } from '../models/broadcast-messages.model';
import { DepartmentDialogResult } from '../models/department-dialog-result.model';
import { RecurringInfoViewModel } from '../models/recurring-info.view.model';
import { RecurringModel } from '../models/recurring.model';
import { SimpleRoleViewModel } from '../models/simple-roles.view.model';
import { RolesRequest } from '../requests-dto/roles-request';
import { BroadcastMessagesApiService } from '../services/broadcast-messages-api.service';
import { UserAccountsHelper } from './../../user-accounts/user-accounts.helper';
import { BroadcastMessageRecurringDialogComponent } from './../broadcast-messages-recurring/broadcast-messages-recurring.component';
@Component({
  selector: 'broadcast-messages-detail',
  templateUrl: './broadcast-messages-detail.component.html',
  styleUrls: ['./broadcast-messages-detail.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BroadcastMessagesDetailComponent
  extends BaseScreenComponent
  implements OnInit {
  get isEditMode(): boolean {
    return this._isEditMode;
  }

  set isEditMode(isEditMode: boolean) {
    if (!isEditMode) {
      return;
    }

    this._isEditMode = isEditMode;
  }

  get broadcastMessage(): BroadcastMessageDetailViewModel {
    return this._broadcastMessage;
  }

  set broadcastMessage(
    broadcastMessageDetailViewModel: BroadcastMessageDetailViewModel
  ) {
    if (!broadcastMessageDetailViewModel) {
      return;
    }

    this._broadcastMessage = broadcastMessageDetailViewModel;
  }

  get isRecurring(): boolean {
    return this._isRecurring;
  }

  set isRecurring(isRecurring: boolean) {
    this._isRecurring = isRecurring;
    this.updateDateRangeValidators();
  }

  get opalTextAreaStyle(): unknown {
    return {
      border: '1x solid #d8dce6',
      boxSizing: 'border-box',
      borderRadius: '5px',
      width: '100%',
      padding: '10px'
    };
  }

  get isValidTargetAudienceInput(): boolean {
    if (this.isSpecificMode) {
      return this.broadcastMessage.hasTargetAudience();
    }

    return true;
  }

  get broadcastMessageTitle() {
    return this.broadcastMessageForm.get('title');
  }

  get broadcastMessageContent() {
    return this.broadcastMessageForm.get('content');
  }

  get broadcastMessageValidToDate() {
    return this.broadcastMessageForm.get('validToDate');
  }

  get broadcastMessageValidFromDate() {
    return this.broadcastMessageForm.get('validFromDate');
  }

  get broadcastMessageValidFromTime() {
    return this.broadcastMessageForm.get('validFromTime');
  }

  get broadcastMessageValidToTime() {
    return this.broadcastMessageForm.get('validToTime');
  }

  readonly SYSTEM_ROLE_CODE: number = 43;
  readonly MESSAGE_TITLE_MAX_LENGTH: number = 100;
  readonly MESSAGE_CONTENT_MAX_LENGTH: number = 1000;
  readonly BROADCAST_MESSAGE_ID_ROUTING: string = 'broadcastMessageId';
  flatDepartmentsArray: Department[] = [];
  rolesArray: SimpleRoleViewModel[] = [];
  broadcastMessageId: string;
  userDepartmentId: number;
  minFromDate: Date = new Date();
  maxFromDate: Date;
  isInvalidTargetAudience: boolean = false;
  recurrenceInfo: RecurringInfoViewModel = new RecurringInfoViewModel();
  isInvalidTimeRangeWarning: boolean = false;
  isInPastWarning: boolean = false;
  minToDate: Date = new Date();
  maxToDate: Date;
  recurringModel: RecurringModel = new RecurringModel();

  fetchUsersFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserManagement[]> = null;

  fetchGroupsFn: (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserGroupDto[]> = null;

  departmentQueryObject: DepartmentQueryModel = new DepartmentQueryModel({
    maxChildrenLevel: undefined,
    includeChildren: true,
    includeParent: false,
    countChildren: true,
    includeDepartmentType: true
  });

  isExistedTargetAudience: boolean = false;
  isSpecificMode: boolean = false;
  broadcastMessageForm: FormGroup;
  targetDate: Date;

  sendModeItems: unknown = [
    {
      sendMode: 'Banner',
      sendModeLabel: 'Banner'
    },

    {
      sendMode: 'Email',
      sendModeLabel: 'Email'
    },

    {
      sendMode: 'EmailAndBanner',
      sendModeLabel: 'Email and banner'
    }
  ];

  targetUserTypeItems: unknown = [
    {
      targetUserType: 'AllUser',
      targetUserTypeLabel: 'All users'
    },

    {
      targetUserType: 'ExternalUser',
      targetUserTypeLabel: 'External users'
    },

    {
      targetUserType: 'HRMSUser',
      targetUserTypeLabel: 'HRMS users'
    },

    {
      targetUserType: 'SpecificTargetUser',
      targetUserTypeLabel: 'Specific target audience'
    }
  ];
  private _broadcastMessage: BroadcastMessageDetailViewModel = new BroadcastMessageDetailViewModel(
    null
  );
  private _isEditMode: boolean = false;
  private _isRecurring: boolean = false;

  constructor(
    protected changeDetectorRef: ChangeDetectorRef,
    protected authService: AuthService,
    private ngbModal: NgbModal,
    private userAccountsDataService: UserAccountsDataService,
    private userGroupsDataSvc: UserGroupsDataService,
    private fb: FormBuilder,
    private toastrService: ToastrService,
    private router: Router,
    private departmentHierarchicalService: DepartmentHierarchicalService,
    private translateService: TranslateService,
    private cxGlobalLoaderService: CxGlobalLoaderService,
    private broadcastDataSvc: BroadcastMessagesApiService,
    private activatedRoute: ActivatedRoute,
    private dialog: MatDialog
  ) {
    super(changeDetectorRef, authService);
  }

  ngOnInit(): void {
    this.cxGlobalLoaderService.showLoader();
    this.getBroadcastMessageIdFromURL();
    this.createFormBuilderDefinition();
    // Target Audience Functions
    this.fetchUsersFn = this.createFetchUsersFn();
    this.fetchGroupsFn = this.createFetchGroupsFn();
    this.getRoles();

    this.initBroadcastMessageViewModel();
  }

  onSelectTargetUserTypeChange(targetUserType: TargetUserType): void {
    if (targetUserType === TargetUserType.SpecificTargetUser) {
      this.isSpecificMode = true;

      return;
    }

    this.isSpecificMode = false;
  }

  checkRecurring(): void {
    this.isRecurring = !this.isRecurring;
  }

  openDepartmentsDialog(): void {
    if (
      this.isDisabledTargetAudienceSelectBox('department') ||
      this.isEditMode
    ) {
      return;
    }

    const dialogRef = this.dialog.open(DepartmentsDialogComponent, {
      data: this.flatDepartmentsArray,
      width: '950px',
      maxHeight: '800px'
    });

    dialogRef
      .afterClosed()
      .subscribe((departmentDialogResult: DepartmentDialogResult) => {
        if (
          !(
            departmentDialogResult && departmentDialogResult.selectedDepartments
          )
        ) {
          return;
        }

        const selectedDepartmentCloned: Department[] = Utils.cloneDeep(
          departmentDialogResult.selectedDepartments
        );
        this.flatDepartmentsArray = selectedDepartmentCloned;
        this.broadcastMessage.departments = selectedDepartmentCloned.map(
          (department) => department.identity.id
        );
      });
  }

  onBackButtonClicked(): void {
    if (this.broadcastMessage.dataHasChanged()) {
      this.openConfirmDialog();
    } else {
      this.goBackToList();
    }
  }

  validFromDateChange(newDate: Date): void {
    if (!newDate) {
      return;
    }

    this.targetDate = newDate;
  }

  openConfirmDialog(): void {
    const content: string = this.translateService.instant(
      'Common.Unsaved_Changes_Warning'
    ) as string;
    const title: string = this.translateService.instant(
      'Common.Label.Warning'
    ) as string;

    const dialogData = new ConfirmDialogModel();
    dialogData.title = title;
    dialogData.content = content;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '500px',
      data: dialogData,
      hasBackdrop: true
    });

    this.subscription.add(
      dialogRef.afterClosed().subscribe((isConfirmedYes: boolean) => {
        if (!Utils.isDefined(isConfirmedYes)) {
          return;
        }

        if (isConfirmedYes) {
          if (!this.isEditMode) {
            this.onSave((broadcastId) => this.goToDetailView(broadcastId));

            return;
          }

          this.onSave(() => {
            this.broadcastMessage.originalData = Utils.cloneDeep(
              this.broadcastMessage.data
            );
          });

          return;
        }

        this.goBackToList();
      })
    );
  }

  onSave(onSaveDone?: (broadcastMessageId?: string) => void): void {
    // this.clearValidatorsForValidDate();

    if (this.broadcastMessageForm.invalid) {
      this.toastrService.error('Please input all required fields');

      return;
    }

    this.isInPastWarning = DateTimeUtil.IsDateInPast(
      this.broadcastMessage.validToDate
    );
    this.isInvalidTimeRangeWarning = !this.isValidDateRange();

    if (this.isInPastWarning || this.isInvalidTimeRangeWarning) {
      return;
    }

    if (this.isInvalidInputField() || !this.isValidTargetAudienceInput) {
      this.isInvalidTargetAudience = !this.isValidTargetAudienceInput;

      return;
    }

    if (this.isRecurring) {
      const content: string = this.generateRecurringInformation();

      this.showRecurringConfirmationDialog(() => {
        this.processSavingBroadcastMessage(onSaveDone);
      }, content);

      return;
    }

    this.processSavingBroadcastMessage(onSaveDone);
  }

  onDateChange(
    type: 'validFrom' | 'validTo',
    event: MatDatepickerInputEvent<Date>
  ): void {
    switch (type) {
      case 'validFrom':
        this.minToDate = event.value;
        break;
      case 'validTo':
        this.maxFromDate = event.value;
        break;
      default:
        return;
    }
  }

  isDisabledTargetAudienceSelectBox(
    targetAudience: 'department' | 'user' | 'group' | 'role'
  ): boolean {
    switch (targetAudience) {
      case 'department':
        return (
          this.broadcastMessage.users.length > 0 ||
          this.broadcastMessage.groups.length > 0 ||
          this.broadcastMessage.roles.length > 0
        );
      case 'user':
        return (
          this.broadcastMessage.departments.length > 0 ||
          this.broadcastMessage.groups.length > 0 ||
          this.broadcastMessage.roles.length > 0
        );
      case 'group':
        return (
          this.broadcastMessage.users.length > 0 ||
          this.broadcastMessage.departments.length > 0 ||
          this.broadcastMessage.roles.length > 0
        );
      case 'role':
        return (
          this.broadcastMessage.users.length > 0 ||
          this.broadcastMessage.groups.length > 0 ||
          this.broadcastMessage.departments.length > 0
        );
      default:
        return false;
    }
  }

  getDateTimeFromNow(date: Date): string {
    return DateTimeUtil.getDateTimeFromNow(date);
  }

  getDateTimeFormat(date: Date): string {
    return DateTimeUtil.convertToLocaleFormat(date, 'lll');
  }

  private processSavingBroadcastMessage(
    onSaveDone?: (broadcastMessageId?: string) => void
  ): void {
    this.cxGlobalLoaderService.showLoader();

    this.saveBroadcastMessagePromise()
      .then((broadcastMessage) => {
        if (broadcastMessage) {
          this.toastrService.success('Saved successfully');
        } else {
          this.toastrService.error('This broadcast message can not be edited');
        }

        this.broadcastMessageId = broadcastMessage.broadcastMessageId;
      })
      .finally(() => {
        this.cxGlobalLoaderService.hideLoader();

        if (onSaveDone) {
          onSaveDone(this.broadcastMessageId);

          return;
        }

        this.goBackToList();
      });
  }

  private generateRecurringInformation(): string {
    const startDate = DateTimeUtil.toDateString(
      this.broadcastMessage.validFromDate,
      AppConstant.backendDateFormat
    );
    const endDate = DateTimeUtil.toDateString(
      this.broadcastMessage.validToDate,
      AppConstant.backendDateFormat
    );
    const convertedStartTime = this.broadcastMessage.validFromTime;
    const convertedEndTime = this.broadcastMessage.validToTime;

    let repeatOnContent: string;

    switch (this.broadcastMessage.recurringType) {
      case RecurrenceType.Week:
        const weekPeriod = StringUtil.numericGrammaticalize(
          this.broadcastMessage.numberOfRecurrence,
          'week'
        );
        const { lastDay, sortedDays } = this.getWeekRepeatOnContent();

        if (sortedDays.length) {
          repeatOnContent = `every ${weekPeriod} on <b>${sortedDays.join(
            ', '
          )} and ${lastDay.toString()}</b> `;

          break;
        }
        repeatOnContent = `every ${weekPeriod} on <b>${lastDay}</b> `;
        break;
      case RecurrenceType.Month:
        const monthPeriod = StringUtil.numericGrammaticalize(
          this.broadcastMessage.numberOfRecurrence,
          'month'
        );
        let monthRepetition: string;
        if (this.broadcastMessage.monthRepetition === MonthRepetition.InOrder) {
          monthRepetition = `<b>on the ${DateTimeUtil.getOrderOfDayInMonthText(
            this.broadcastMessage.validFromDate
          )} ${DateTimeUtil.getWeekDay(
            this.broadcastMessage.validFromDate
          )}</b> `;
        } else if (
          this.broadcastMessage.monthRepetition === MonthRepetition.OnDay
        ) {
          monthRepetition = `<b>on day ${this.broadcastMessage.validFromDate.getDate()}</b>`;
        }
        repeatOnContent = `every ${monthPeriod} ${monthRepetition}`;
        break;
      default:
        break;
    }

    const info: string =
      '<br/> You are about the broadcast this message. <br/> <br/>' +
      `The message will be sent on <b>${startDate} ${convertedStartTime} </b> and will recur ${repeatOnContent} ` +
      `until <b>${endDate} ${convertedEndTime}</b>.`;

    return info;
  }

  private getWeekRepeatOnContent(): {
    lastDay: DayRepetition;
    sortedDays: DayRepetition[];
  } {
    const sortedDays = DateTimeUtil.sortDay(
      Utils.cloneDeep(this.broadcastMessage.dayRepetitions)
    );
    const lastDay = sortedDays[this.broadcastMessage.dayRepetitions.length - 1];

    sortedDays.splice(sortedDays.length - 1, NUMBER_OF_REMOVED_ITEM_DEFAULT);

    return { lastDay, sortedDays };
  }

  private buildDefaultDataForBroadcastMessage(): void {
    const dateAfterAWeek = new Date();
    dateAfterAWeek.setDate(dateAfterAWeek.getDate() + DAY_OF_WEEK.length);
    this.broadcastMessage.validFromTime = '12:00 AM';
    this.broadcastMessage.validToDate = dateAfterAWeek;
    this.broadcastMessage.validToTime = '11:59 PM';
    this.broadcastMessage.dayRepetitions = [this.getCurrentDay];
    this.broadcastMessage.monthRepetition = MonthRepetition.OnDay;
    this.maxFromDate = dateAfterAWeek;
  }

  private clearSpecificSendingTarget(): void {
    this.broadcastMessage.users = [];
    this.broadcastMessage.departments = [];
    this.broadcastMessage.groups = [];
    this.broadcastMessage.roles = [];
  }

  private clearRecurringData(): void {
    this.broadcastMessage.data.numberOfRecurrence = 0;
    this.broadcastMessage.data.recurrenceType = RecurrenceType.None;
    this.broadcastMessage.data.dayRepetitions = [];
    this.broadcastMessage.data.monthRepetition = null;
  }

  private clearValidatorsForValidDate(): void {
    if (
      this.broadcastMessageValidToDate.errors !== null &&
      !this.broadcastMessageValidToDate.errors.required
    ) {
      this.broadcastMessageValidToDate.setErrors(null);
    }

    if (
      this.broadcastMessageValidFromDate.errors !== null &&
      !this.broadcastMessageValidFromDate.errors.required
    ) {
      this.broadcastMessageValidFromDate.setErrors(null);
    }
  }

  private isValidDateRange(): boolean {
    return (
      DateTimeUtil.compareDate(
        this.broadcastMessage.validToDate,
        this.broadcastMessage.validFromDate,
        true,
        true
      ) > 0
    );
  }

  private initBroadcastMessageViewModel(): void {
    if (Utils.isNullOrEmpty(this.broadcastMessageId)) {
      this.cxGlobalLoaderService.hideLoader();
      this.buildDefaultDataForBroadcastMessage();
    } else {
      this.isEditMode = true;
      this.buildBroadcastMessageViewModel(this.broadcastMessageId);
    }
  }

  private getRoles(): void {
    const sub = this.broadcastDataSvc
      .getRoles(
        new RolesRequest({
          archetypeIds: [this.SYSTEM_ROLE_CODE],
          includeLocalizedData: true
        })
      )
      .subscribe((rolesResponse) => {
        this.rebuildRoleItems(rolesResponse);
      });

    this.subscription.add(sub);
  }

  private rebuildRoleItems(roles: SystemRole[]): void {
    roles.forEach((role) => {
      this.rolesArray.push(
        new SimpleRoleViewModel({
          id: role.identity.id,
          roleName: role.localizedData[0].fields[0].localizedText
        })
      );
    });
  }

  private buildBroadcastMessageViewModel(broadcastMessageId: string): void {
    const broadcastSub = this.broadcastDataSvc
      .getBroadcastMessageById(broadcastMessageId)
      .pipe(
        switchMap((broadcastMessage) => {
          return BroadcastMessageDetailViewModel.create(
            (ids) =>
              this.userAccountsDataService
                .getUsers(
                  new UserManagementQueryModel({
                    extIds: ids,
                    orderBy: 'firstName asc',
                    userEntityStatuses: [StatusTypeEnum.Active.code],
                    pageIndex: 0,
                    pageSize: 0
                  })
                )
                .pipe(map((res) => res.items)),
            this.userGroupsDataSvc
              .getUserGroups(
                new UserGroupFilterParams({
                  countActiveMembers: true,
                  orderBy: 'name',
                  departmentIds: [this.currentUser.departmentId],
                  pageIndex: 0,
                  pageSize: 0
                })
              )
              .pipe(
                map((userGroupsPaging) => {
                  return userGroupsPaging.items;
                })
              ),
            broadcastMessage
          );
        })
      )
      .subscribe(
        (broadcastMessageVM) => {
          this.broadcastMessage = broadcastMessageVM;

          if (this.broadcastMessage.data.ownerId) {
            const lastUpdatedByUserId = this.broadcastMessage.data.lastUpdatedBy
              ? this.broadcastMessage.data.lastUpdatedBy
              : null;
            this.getBasicUsersInfo(
              this.broadcastMessage.data.ownerId,
              lastUpdatedByUserId
            );
          }

          if (
            this.broadcastMessage.originalData.targetUserType ===
            TargetUserType.SpecificTargetUser
          ) {
            this.isSpecificMode = true;
          }

          if (
            this.broadcastMessage.originalData.recurrenceType !==
            RecurrenceType.None
          ) {
            this.isRecurring = true;
            this.updateDateRangeValidators();
            // this.buildRecurrenceLabel();
          }

          this.getDepartments(
            UserAccountsHelper.getDefaultRootDepartment(this.currentUser)
          );
        },

        null,
        () => {
          this.cxGlobalLoaderService.hideLoader();
        }
      );

    this.subscription.add(broadcastSub);
  }

  private isInvalidInputField(): boolean {
    return this.isOutOfMaxLength() || this.isOnlyWhiteSpaceInput();
  }

  private isOutOfMaxLength(): boolean {
    return (
      (this.broadcastMessage.title &&
        this.broadcastMessage.title.length > this.MESSAGE_TITLE_MAX_LENGTH) ||
      (this.broadcastMessage.content &&
        this.broadcastMessage.content.length > this.MESSAGE_CONTENT_MAX_LENGTH)
    );
  }

  private isOnlyWhiteSpaceInput(): boolean {
    return (
      (this.broadcastMessage.title &&
        !this.broadcastMessage.title.trim().length) ||
      (this.broadcastMessage.title &&
        !this.broadcastMessage.content.trim().length)
    );
  }

  private goBackToList(): void {
    this.router.navigate(['/broadcast-messages']);
  }

  private goToDetailView(broadcastMessageId: string): void {
    this.router.navigate(['/broadcast-messages/detail/', broadcastMessageId]);
  }

  private getBroadcastMessageIdFromURL(): void {
    this.broadcastMessageId = this.activatedRoute.snapshot.paramMap.get(
      this.BROADCAST_MESSAGE_ID_ROUTING
    );
  }

  private saveBroadcastMessagePromise(): Promise<BroadcastMessagesDto> {
    return new Promise((resolve, reject) => {
      this.broadcastMessage.data.title = this.broadcastMessage.data.title.trimLeft();

      if (this.isEditMode) {
        const data = Utils.clone(
          this.broadcastMessage.data,
          (clonedMessage: BroadcastMessagesDto) => {
            clonedMessage.recipients.userIds = this.broadcastMessage.originalUserIds;
            clonedMessage.lastUpdatedBy = this.currentUser.identity.extId;
          }
        );

        this.broadcastDataSvc.editBroadcastMessage(data).subscribe((result) => {
          if (result.status === HTTP_STATUS_CODE.STATUS_200_SUCCESS) {
            this.broadcastDataSvc.updateBroadcastMessage(data).subscribe(
              (broadcastMessage) => {
                resolve(broadcastMessage);
              },

              reject
            );
          } else {
            resolve(null);
          }
        });
      } else {
        if (!this.isSpecificMode) {
          this.clearSpecificSendingTarget();
        }

        this.verifyRecurringData();

        const data = Utils.clone(
          this.broadcastMessage.data,
          (broadcastMessageCloned) => {
            broadcastMessageCloned.ownerId = this.currentUser.identity.extId;
          }
        );

        this.broadcastDataSvc.saveBroadcastMessage(data).subscribe(
          (broadcastMessage) => {
            resolve(broadcastMessage);
          },

          reject
        );
      }
    });
  }

  private verifyRecurringData(): void {
    if (!this.isRecurring) {
      this.clearRecurringData();

      return;
    }
    if (this.broadcastMessage.recurringType === RecurrenceType.Week) {
      this.broadcastMessage.data.monthRepetition = null;
    } else if (this.broadcastMessage.recurringType === RecurrenceType.Month) {
      this.broadcastMessage.data.dayRepetitions = [];
    }
  }

  private createFormBuilderDefinition(): void {
    this.broadcastMessageForm = this.fb.group({
      title: [
        '',
        [
          Validators.required,
          noContentWhiteSpaceValidator,
          Validators.maxLength(this.MESSAGE_TITLE_MAX_LENGTH)
        ]
      ],
      content: [
        '',
        [
          Validators.required,
          noContentWhiteSpaceValidator,
          Validators.maxLength(this.MESSAGE_CONTENT_MAX_LENGTH)
        ]
      ],
      validFromDate: [
        '',
        [
          ifValidator(
            (control) => !this.isRecurring,
            () => Validators.required
          )
        ]
      ],
      validFromTime: [
        '',
        [
          ifValidator(
            (control) => !this.isRecurring,
            () => Validators.required
          )
        ]
      ],
      validToDate: [
        '',
        [
          ifValidator(
            (control) => !this.isRecurring,
            () => Validators.required
          )
        ]
      ],
      validToTime: [
        '',
        [
          ifValidator(
            (control) => !this.isRecurring,
            () => Validators.required
          )
        ]
      ],
      sendMode: ['', [Validators.required]],
      targetUserType: ['', [Validators.required]],
      recipients: this.fb.group({
        departments: [''],
        users: [''],
        groups: [''],
        roles: ['']
      })
    });
  }

  private getDepartments(fromDepartmentId: number): void {
    this.departmentHierarchicalService
      .getOrganizationalUnitsByIds(
        null,
        this.broadcastMessage.selectedDepartments
      )
      .subscribe((departmentsResult) => {
        if (!departmentsResult) {
          return;
        }

        const convertedDepartments = departmentsResult.map((department) => {
          return new Department(department);
        });
        this.flatDepartmentsArray = _.uniqBy(
          this.flatDepartmentsArray.concat(convertedDepartments),
          'identity.id'
        );

        this.changeDetectorRef.detectChanges();
      });
  }

  private createFetchUsersFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserManagement[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.userAccountsDataService
        .getUsers(
          new UserManagementQueryModel({
            searchKey: searchText,
            orderBy: 'firstName asc',
            parentDepartmentId: [this.userDepartmentId],
            userEntityStatuses: [StatusTypeEnum.Active.code],
            pageIndex:
              maxResultCount === 0 ? 1 : skipCount / maxResultCount + 1,
            pageSize: maxResultCount,
            filterOnSubDepartment: true
          })
        )
        .pipe(
          map((usersPaging) => {
            return usersPaging.items;
          })
        );
  }

  private updateDateRangeValidators(): void {
    this.broadcastMessageValidFromDate.updateValueAndValidity();
    this.broadcastMessageValidFromTime.updateValueAndValidity();
    this.broadcastMessageValidToDate.updateValueAndValidity();
    this.broadcastMessageValidToTime.updateValueAndValidity();
  }

  private showRecurringConfirmationDialog(
    onConfirm: () => void,
    content: string
  ): void {
    const dialogData = new ConfirmDialogModel();
    dialogData.title = 'Broadcast message';
    dialogData.content = content;
    dialogData.confirmButtonText = 'Confirm';
    dialogData.cancelButtonText = 'Cancel';

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '500px',
      data: dialogData,
      hasBackdrop: true
    });

    this.subscription.add(
      dialogRef.afterClosed().subscribe((isConfirmedYes: boolean) => {
        if (!Utils.isDefined(isConfirmedYes)) {
          return;
        }

        if (isConfirmedYes) {
          onConfirm();
        }
      })
    );
  }

  private createFetchGroupsFn(): (
    searchText: string,
    skipCount: number,
    maxResultCount: number
  ) => Observable<UserGroupDto[]> {
    return (searchText, skipCount, maxResultCount) =>
      this.userGroupsDataSvc
        .getUserGroups(
          new UserGroupFilterParams({
            countActiveMembers: true,
            orderBy: 'name',
            departmentIds: [this.currentUser.departmentId],
            pageIndex: 0,
            pageSize: 0
          })
        )
        .pipe(
          map((userGroupsPaging) => {
            return userGroupsPaging.items;
          })
        );
  }

  private get getCurrentDay(): DayRepetition {
    const currentDay = DateTimeUtil.getWeekDay(new Date());

    return DayRepetition[currentDay];
  }

  private getBasicUsersInfo(ownerId: string, lastUpdateId: string = ''): void {
    this.userAccountsDataService
      .getUsersBasicInfo({ extIds: [ownerId, lastUpdateId] })
      .subscribe((userBasicInfos) => {
        if (userBasicInfos.items.length) {
          const ownerInfo = userBasicInfos.items.find(
            (owner) => owner.userCxId.toLowerCase() === ownerId.toLowerCase()
          );
          this.broadcastMessage.ownerInfo = ownerInfo;

          const lastUpdatedByInfo = userBasicInfos.items.find(
            (lastUpdated) =>
              lastUpdated.userCxId.toLowerCase() === lastUpdateId.toLowerCase()
          );

          if (lastUpdatedByInfo) {
            this.broadcastMessage.lastUpdatedUserInfo = lastUpdatedByInfo;
          }
        }
      });
  }
}

enum OPTION {
  ALL_USER = 1,
  TARGET_AUDIENCE = 2
}
