import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { SendMode } from 'app/shared/constants/send-mode.enum';
import { TargetUserType } from 'app/shared/constants/target-user-type.enum';
import { UserBasicInfo } from 'app/user-accounts/models/user-basic-info.model';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import { UserGroupDto } from 'app/user-groups/user-groups.model';
import { combineLatest, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { Utils } from '../../shared/utilities/utils';
import { DayRepetition } from '../constant/day-repetition.enum';
import { MonthRepetition } from '../constant/month-repetition.enum';
import { BroadcastMessagesDto, Recipients } from './broadcast-messages.model';

export class BroadcastMessageDetailViewModel {
  get broadcastMessageId(): string {
    return this.data.broadcastMessageId;
  }
  set broadcastMessageId(broadcastMessageId: string) {
    this.data.broadcastMessageId = broadcastMessageId;
  }

  get title(): string {
    return this.data.title;
  }
  set title(title: string) {
    if (title == null) {
      return;
    }
    this.data.title = title;
  }

  get content(): string {
    return this.data.broadcastContent;
  }
  set content(content: string) {
    if (content == null) {
      return;
    }
    this.data.broadcastContent = content;
  }

  get createdDate(): Date {
    return this.data.createdDate;
  }
  set createdDate(createdDate: Date) {
    if (createdDate == null) {
      return;
    }
    this.data.createdDate = createdDate;
  }

  get lastUpdated(): Date {
    return this.data.lastUpdated;
  }
  set lastUpdated(lastUpdated: Date) {
    if (lastUpdated == null) {
      return;
    }
    this.data.lastUpdated = lastUpdated;
  }

  get ownerId(): string {
    return this.data.ownerId;
  }
  set ownerId(ownerId: string) {
    if (ownerId == null) {
      return;
    }
    this.data.ownerId = ownerId;
  }

  get ownerInfo(): UserBasicInfo {
    return this._ownerInfo;
  }
  set ownerInfo(ownerInfo: UserBasicInfo) {
    if (ownerInfo == null) {
      return;
    }
    this._ownerInfo = ownerInfo;
  }

  get lastUpdatedBy(): string {
    return this.data.lastUpdatedBy;
  }
  set lastUpdatedBy(lastUpdatedBy: string) {
    if (lastUpdatedBy == null) {
      return;
    }
    this.data.lastUpdatedBy = lastUpdatedBy;
  }

  get lastUpdatedUserInfo(): UserBasicInfo {
    return this._lastUpdatedUserInfo;
  }
  set lastUpdatedUserInfo(lastUpdatedInfo: UserBasicInfo) {
    if (lastUpdatedInfo == null) {
      return;
    }

    this._lastUpdatedUserInfo = lastUpdatedInfo;
  }

  get recipients(): Recipients {
    return this.data.recipients;
  }
  set recipients(recipients: Recipients) {
    if (recipients == null) {
      return;
    }
    this.data.recipients = recipients;
  }

  get validFromDate(): Date {
    return this.data.validFromDate;
  }
  set validFromDate(validFromDate: Date) {
    if (validFromDate == null) {
      return;
    }
    this.data.validFromDate = DateTimeUtil.updateDateWithoutTime(
      this.data.validFromDate ? this.data.validFromDate : validFromDate,
      validFromDate
    );
  }

  get validFromTime(): string {
    return DateTimeUtil.convertTimeFormat(this._validFromTime, '12h');
  }
  set validFromTime(validFromTime: string) {
    if (validFromTime == null) {
      return;
    }
    this._validFromTime = DateTimeUtil.convertTimeFormat(validFromTime, '24h');
    this.data.validFromDate = DateTimeUtil.buildDateTime(
      this.data.validFromDate ? this.data.validFromDate : new Date(),
      DateTimeUtil.createDefaultDateFromTime(this._validFromTime)
    );
  }

  get validToDate(): Date {
    return this.data.validToDate;
  }
  set validToDate(validToDate: Date) {
    if (validToDate == null) {
      return;
    }
    this.data.validToDate = DateTimeUtil.updateDateWithoutTime(
      this.data.validToDate ? this.data.validToDate : validToDate,
      validToDate
    );
  }

  get validToTime(): string {
    return DateTimeUtil.convertTimeFormat(this._validToTime, '12h');
  }
  set validToTime(validToTime: string) {
    if (validToTime == null) {
      return;
    }

    this._validToTime = DateTimeUtil.convertTimeFormat(validToTime, '24h');
    this.data.validToDate = DateTimeUtil.buildDateTime(
      this.data.validToDate ? this.data.validToDate : new Date(),
      DateTimeUtil.createDefaultDateFromTime(this._validToTime)
    );
  }

  get targetUserType(): TargetUserType {
    return this.data.targetUserType;
  }
  set targetUserType(targetUserType: TargetUserType) {
    if (!targetUserType) {
      return;
    }
    this.data.targetUserType = targetUserType;
  }

  get sendMode(): SendMode {
    return this.data.sendMode;
  }
  set sendMode(sendMode: SendMode) {
    if (!sendMode) {
      return;
    }
    this.data.sendMode = sendMode;
  }

  get numberOfRecurrence(): number {
    return this.data.numberOfRecurrence;
  }
  set numberOfRecurrence(numberOfRecurrence: number) {
    if (numberOfRecurrence == null) {
      return;
    }
    this.data.numberOfRecurrence = numberOfRecurrence;
  }

  get recurringType(): RecurrenceType {
    return this.data.recurrenceType;
  }
  set recurringType(recurringType: RecurrenceType) {
    if (!recurringType) {
      return;
    }
    this.data.recurrenceType = recurringType;
  }

  get dayRepetitions(): DayRepetition[] {
    return this.data.dayRepetitions;
  }
  set dayRepetitions(dayRepetitions: DayRepetition[]) {
    if (!dayRepetitions) {
      return;
    }

    this.data.dayRepetitions = dayRepetitions;
  }

  get monthRepetition(): MonthRepetition {
    return this.data.monthRepetition;
  }
  set monthRepetition(monthRepetition: MonthRepetition) {
    if (!monthRepetition) {
      return;
    }

    this.data.monthRepetition = monthRepetition;
  }

  // public get status(): BroadcastMessageStatus {
  //   return this.data.status;
  // }
  // public set status(status: BroadcastMessageStatus) {
  //   this.data.status = status;
  // }

  //#region Target Audience
  get departments(): number[] {
    return this.data.recipients.departmentIds;
  }
  set departments(departments: number[]) {
    if (departments == null) {
      return;
    }
    this.data.recipients.departmentIds = departments;
  }

  get users(): string[] {
    return this.data.recipients.userIds;
  }
  set users(users: string[]) {
    if (users == null) {
      return;
    }
    this.data.recipients.userIds = users;
  }

  get groups(): number[] {
    return this.data.recipients.groupIds;
  }
  set groups(groups: number[]) {
    if (groups == null) {
      return;
    }
    this.data.recipients.groupIds = groups;
  }

  get roles(): number[] {
    return this.data.recipients.roleIds;
  }
  set roles(roles: number[]) {
    if (roles == null) {
      return;
    }
    this.data.recipients.roleIds = roles;
  }

  // List of selected Target Audiences
  get selectedUsers(): UserManagement[] {
    return this._selectedUsers;
  }

  get selectedDepartments(): number[] {
    return this._selectedDepartmentIds;
  }

  get selectedGroups(): UserGroupDto[] {
    return this._selectedGroups;
  }

  get selectedRoles(): number[] {
    return this._selectedRoles;
  }

  get originalUserIds(): string[] {
    return this._originalUserIds;
  }

  //#endregion

  static create(
    getUsersByIdsFn: (userIds: string[]) => Observable<UserManagement[]>,
    getGroupsObs: Observable<UserGroupDto[]>,
    broadcastMessage: BroadcastMessagesDto
  ): Observable<BroadcastMessageDetailViewModel> {
    return combineLatest(
      broadcastMessage.recipients.userIds.length === 0
        ? of([])
        : getUsersByIdsFn(broadcastMessage.recipients.userIds),
      getGroupsObs
    ).pipe(
      map(([users, groups]) => {
        return new BroadcastMessageDetailViewModel(
          broadcastMessage,
          users,
          groups
        );
      })
    );
  }

  data: BroadcastMessagesDto = new BroadcastMessagesDto();
  originalData: BroadcastMessagesDto = new BroadcastMessagesDto();

  private _validFromTime: string;
  private _validToTime: string;
  private _selectedDepartmentIds: number[] = [];
  private _selectedUsers: UserManagement[] = [];
  private _selectedGroups: UserGroupDto[] = [];
  private _ownerInfo: UserBasicInfo;
  private _lastUpdatedUserInfo: UserBasicInfo;
  private _selectedRoles: number[] = [];
  private _originalUserIds: string[];
  constructor(
    broadcastMessagesDto?: BroadcastMessagesDto,
    users: UserManagement[] = [],
    groups: UserGroupDto[] = []
  ) {
    if (broadcastMessagesDto) {
      // Keep original userIds to send to COMMON
      this._originalUserIds = users.map((user) => user.identity.extId);

      this.updateBroadcastMessagesData(broadcastMessagesDto);
      this.targetAudiencesBuilder(users, groups);
    }
  }

  hasTargetAudience(): boolean {
    return (
      this.departments.length > 0 ||
      this.users.length > 0 ||
      this.groups.length > 0 ||
      this.roles.length > 0
    );
  }

  dataHasChanged(): boolean {
    const originalBroadcastMessage = Utils.cloneDeep(this.originalData);
    const currentBroadcastMessage = Utils.cloneDeep(this.data);

    // Standardized DateTime values
    originalBroadcastMessage.validFromDate = DateTimeUtil.removeSeconds(
      originalBroadcastMessage.validFromDate
    );
    originalBroadcastMessage.validToDate = DateTimeUtil.removeSeconds(
      originalBroadcastMessage.validToDate
    );

    currentBroadcastMessage.validFromDate = DateTimeUtil.removeSeconds(
      currentBroadcastMessage.validFromDate
    );
    currentBroadcastMessage.validToDate = DateTimeUtil.removeSeconds(
      currentBroadcastMessage.validToDate
    );

    return Utils.isDifferent(originalBroadcastMessage, currentBroadcastMessage);
  }

  private targetAudiencesBuilder(
    users: UserManagement[] = [],
    groups: UserGroupDto[] = []
  ): void {
    const groupDic = Utils.toDictionary(groups, (g) => g.identity.id);

    if (this.originalData.recipients.groupIds != null) {
      this.originalData.recipients.groupIds.forEach((id) => {
        this._selectedGroups.push(groupDic[id]);
      });
    }

    if (this.originalData.recipients.departmentIds != null) {
      this._selectedDepartmentIds = this.originalData.recipients.departmentIds;
    }

    if (this.originalData.recipients.roleIds != null) {
      this._selectedRoles = this.originalData.recipients.roleIds;
    }

    if (this.originalData.recipients.userIds != null) {
      this._selectedUsers = Utils.clone(users, (clonedUsers) => {
        clonedUsers.forEach((u) => {
          u.identity.extId = u.identity.extId.toLowerCase();
        });
      });
    }
  }

  private updateBroadcastMessagesData(
    broadcastMessagesDto: BroadcastMessagesDto
  ): void {
    this.data = Utils.cloneDeep(broadcastMessagesDto);
    this.originalData = Utils.cloneDeep(broadcastMessagesDto);
    this._validFromTime = DateTimeUtil.getTimeFromDate(this.data.validFromDate);
    this._validToTime = DateTimeUtil.getTimeFromDate(this.data.validToDate);
  }
}
