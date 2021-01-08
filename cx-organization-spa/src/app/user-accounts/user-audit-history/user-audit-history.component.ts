import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  ViewEncapsulation
} from '@angular/core';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ObjectUtilities } from 'app-utilities/object-utils';
import { PDCatalogueStoreService } from 'app/core/store-services/pd-catalogue-store.service';
import { BaseSmartComponent } from 'app/shared/components/component.abstract';

import { CxGlobalLoaderService } from '@conexus/cx-angular-common';
import { ApprovalGroupTypeEnum } from '../constants/approval-group.enum';
import {
  AuditActionType,
  DatahubEventActionType,
  EntityStatusEnum,
  EntityStatusMapping,
  GenderEnum,
  ListOfMetadatas,
  METADATA_ID,
  TrackedUserFieldEnum,
  TrackedUserUpdatedProps,
  UserFieldNameConstant
} from '../constants/user-field-mapping.constant';
import {
  ActionUserInfo,
  AuditHistory,
  AuditLogExecutor,
  DatahubEventModel
} from '../models/audit-history.model';
import { PDCatalogueEnumerationDto } from '../models/pd-catalogue.model';
import { UserManagement } from '../models/user-management.model';
import { UserAuditHistoryHelper } from './user-audit-history.helper';
import { UserAuditHistoryService } from './user-audit-history.service';

const APPROVAL_GROUP_TO_AUDIT_TYPE = new Map<
  ApprovalGroupTypeEnum,
  AuditActionType
>([
  [
    ApprovalGroupTypeEnum.PrimaryApprovalGroup,
    AuditActionType.PrimaryApprovalGroupChanged
  ],
  [
    ApprovalGroupTypeEnum.AlternativeApprovalGroup,
    AuditActionType.AlternateApprovalGroupChanged
  ]
]);

@Component({
  selector: 'user-audit-history',
  templateUrl: './user-audit-history.component.html',
  styleUrls: ['./user-audit-history.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserAuditHistoryComponent
  extends BaseSmartComponent
  implements OnInit {
  @Input() user: UserManagement;
  userFieldNameConstant: any = UserFieldNameConstant;
  auditHistories: AuditHistory[];
  loading: boolean;
  readonly CURRENT_STATUS_INDEX: number = 2;

  constructor(
    changeDetectorRef: ChangeDetectorRef,
    private auditHistoryService: UserAuditHistoryService,
    private translateAdapterService: TranslateAdapterService,
    private pdCatalogueStoreService: PDCatalogueStoreService,
    private globalLoader: CxGlobalLoaderService
  ) {
    super(changeDetectorRef);
  }
  getDisplaySubTemplate = (key: any, value: any) => {
    if (
      value === null ||
      value === undefined ||
      (Array.isArray(value) && !value.length)
    ) {
      switch (key) {
        case TrackedUserFieldEnum.ReportingOfficer:
          return this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Audit_History.Sub_Field_Change_Title.Unassign'
          );
        default:
          return this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Audit_History.Sub_Field_Change_Title.None'
          );
      }
    }
    if (Array.isArray(value)) {
      return value.join(', ');
    }
    switch (key) {
      case TrackedUserFieldEnum.ReportingOfficer:
        return value.fullName;
      case TrackedUserFieldEnum.DateOfBirth:
      case TrackedUserFieldEnum.ActiveDate:
      case TrackedUserFieldEnum.ExpirationDate:
        return UserAuditHistoryHelper.getDateFormat(value);
      case TrackedUserFieldEnum.Gender:
        return GenderEnum[value];
      default:
        return value;
    }
  };
  getDisplayTemplate = (auditHistory: AuditHistory) => {
    switch (auditHistory.actionType) {
      case AuditActionType.AccountCreated:
        const isExternallyMastered = this.user.entityStatus.externallyMastered;
        const isRegistered =
          auditHistory.actionUserInfo.extId === this.user.identity.extId;
        if (isExternallyMastered) {
          return `<b>${this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Audit_History.Field_Change_Title.User_Imported'
          )}</b>`;
        }
        if (isRegistered) {
          return `<b>${this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Audit_History.Field_Change_Title.User_Registered'
          )}</b>`;
        }

        if (
          auditHistory.payload &&
          auditHistory.payload.body.isInsertedByImport
        ) {
          return `<b>${
            auditHistory.actionUserInfo.firstName
          }</b> ${this.translateAdapterService.getValueImmediately(
            'User_Account_Page.Audit_History.Field_Change_Title.Mass_Created'
          )}`;
        }

        return `<b>${
          auditHistory.actionUserInfo.firstName
        }</b> ${this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Audit_History.Field_Change_Title.User_Created'
        )}`;
      case AuditActionType.InfoUpdated:
        return `<b>${
          auditHistory.actionUserInfo.firstName
        }</b> ${this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Audit_History.Field_Change_Title.User_Updated'
        )}`;
      case AuditActionType.LoginFailed:
      case AuditActionType.EntityStatusChanged:
        return `${auditHistory.title}`;
      case AuditActionType.PrimaryApprovalGroupChanged:
        return `<b>${
          auditHistory.actionUserInfo.firstName
        }</b> ${this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Audit_History.Field_Change_Title.Primary_AO_Changed'
        )}`;
      case AuditActionType.AlternateApprovalGroupChanged:
        return `<b>${
          auditHistory.actionUserInfo.firstName
        }</b> ${this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Audit_History.Field_Change_Title.Alternate_AO_Changed'
        )}`;
      default:
        return `<b>${
          auditHistory.actionType
        }</b> ${this.translateAdapterService.getValueImmediately(
          'User_Account_Page.Audit_History.Field_Change_Title.Not_Found'
        )}`;
    }
  };

  ngOnInit(): void {
    this.globalLoader.showLoader();
    this.auditHistoryService
      .getUserHistoricalData(this.user.identity.extId)
      .subscribe(
        async (response) => {
          if (!response || !response.items || !response.items.length) {
            this.auditHistories = [];
            this.changeDetectorRef.detectChanges();
            this.globalLoader.hideLoader();

            return;
          }
          const logs = response.items;

          logs.sort(
            (log1, log2) => +new Date(log1.created) - +new Date(log2.created)
          );

          const createdLog = logs.find(
            (log) => log.routing.action === DatahubEventActionType.UserCreated
          );
          const updatedLogs = logs.filter(
            (log) =>
              log.routing.action === DatahubEventActionType.UserCreated ||
              log.routing.action === DatahubEventActionType.UserUpdated
          );
          const entityStatusLogs = logs.filter(
            (log) =>
              log.routing.action === DatahubEventActionType.EntityStatusChanged
          );
          const approvalGroupLogs = logs.filter(
            (log) =>
              log.routing.action ===
                DatahubEventActionType.ApprovalGroupCreated ||
              log.routing.action === DatahubEventActionType.ApprovalGroupDeleted
          );
          const loginFailedLogs = logs.filter(
            (log) => log.routing.action === DatahubEventActionType.LoginFailed
          );

          let allChanges = [];

          if (createdLog) {
            allChanges.push(this.getCreatedChange(createdLog));
          }

          allChanges = allChanges
            .concat(await this.getUpdatedChanges(updatedLogs))
            .concat(this.getEntityStatusChanges(entityStatusLogs))
            .concat(await this.getApprovalGroupChanges(approvalGroupLogs))
            .concat(this.getLoginFailedChanges(loginFailedLogs));

          allChanges = this.removeDuplicatedLockedEvent(allChanges);
          allChanges.sort(
            (log1, log2) => +new Date(log2.date) - +new Date(log1.date)
          );
          this.auditHistories = allChanges;
          this.globalLoader.hideLoader();
          this.changeDetectorRef.detectChanges();
        },
        (error) => {
          this.globalLoader.hideLoader();
          this.changeDetectorRef.detectChanges();
        }
      );
  }

  private removeDuplicatedLockedEvent(
    allChanges: AuditHistory[]
  ): AuditHistory[] {
    allChanges.sort(
      (log1, log2) => +new Date(log1.date) - +new Date(log2.date)
    );
    const histories = [];

    let hasLocked = false;
    allChanges.forEach((change) => {
      if (change.actionType === AuditActionType.EntityStatusChanged) {
        hasLocked = false;
      }
      if (change.actionType === AuditActionType.LoginFailed) {
        if (hasLocked === false) {
          hasLocked = true;
          histories.push(change);
        }

        return;
      }
      histories.push(change);
    });

    return histories;
  }

  private getLoginFailedChanges(logs: DatahubEventModel[]): AuditHistory[] {
    if (!logs || !logs.length) {
      return [];
    }

    const histories = [];
    logs.forEach((log) => {
      const loginFailedTitle = this.translateAdapterService.getValueImmediately(
        'User_Account_Page.Audit_History.EntityStatusChanged.Locked'
      );
      const loginFailedInfo = this.translateAdapterService.getValueImmediately(
        'User_Account_Page.Audit_History.FailedLogin',
        { ipAddress: log.payload.identity.sourceIp }
      );
      histories.push(
        new AuditHistory({
          id: log.id,
          actionUserInfo: this.convertExecutorToActionUserInfo(log.executor),
          date: log.created,
          actionType: AuditActionType.LoginFailed,
          title: loginFailedTitle,
          info: loginFailedInfo
        })
      );
    });

    return histories;
  }

  private async getApprovalGroupChanges(
    logs: DatahubEventModel[]
  ): Promise<AuditHistory[]> {
    if (!logs || !logs.length) {
      return [];
    }

    const histories = [];

    for (let i = 0; i < logs.length; i++) {
      const differ = {};
      differ[
        TrackedUserFieldEnum.ReportingOfficer
      ] = UserAuditHistoryHelper.getApprovalGroupDiff(logs[i], logs[i + 1]);
      const previousValue =
        differ[TrackedUserFieldEnum.ReportingOfficer].previous;
      const currentValue =
        differ[TrackedUserFieldEnum.ReportingOfficer].current;
      if (previousValue && currentValue) {
        i++;
      }
      let approvalGroupType: ApprovalGroupTypeEnum;
      if (previousValue && previousValue.type) {
        approvalGroupType = previousValue.type;
      } else if (currentValue && currentValue.type) {
        approvalGroupType = currentValue.type;
      }

      if (!approvalGroupType) {
        continue;
      }
      histories.push(
        new AuditHistory({
          id: logs[i].id,
          actionUserInfo: this.convertExecutorToActionUserInfo(
            logs[i].executor
          ),
          date: logs[i].created,
          actionType: APPROVAL_GROUP_TO_AUDIT_TYPE.get(approvalGroupType),
          payload: differ
        })
      );
    }

    return histories;
  }

  private getEntityStatusChanges(
    entityStatusLogs: DatahubEventModel[]
  ): AuditHistory[] {
    if (!entityStatusLogs || !entityStatusLogs.length) {
      return [];
    }

    const histories = [];
    for (const log of entityStatusLogs) {
      const userData = log.payload.body.userData;
      const statusMappingKey = `${userData.fromEntityStatusId}-${userData.toEntityStatusId}`;
      if (!EntityStatusMapping.has(statusMappingKey)) {
        continue;
      }
      const user = this.convertExecutorToActionUserInfo(log.executor);
      const auditTitle = this.generateAuditTitle(
        user.firstName,
        statusMappingKey,
        log.payload
      );
      if (!auditTitle) {
        continue;
      }

      histories.push(
        new AuditHistory({
          id: log.id,
          actionUserInfo: user,
          date: log.created,
          actionType: AuditActionType.EntityStatusChanged,
          title: auditTitle
        })
      );
    }

    return histories;
  }

  private generateAuditTitle(
    userName: string,
    statusMappingKey: string,
    payload?: any
  ): string {
    if (
      Number(statusMappingKey.slice(this.CURRENT_STATUS_INDEX)) ===
        EntityStatusEnum.Archived &&
      payload &&
      payload.body.isAutoArchived
    ) {
      return this.translateAdapterService.getValueImmediately(
        'User_Account_Page.Audit_History.EntityStatusChanged.Auto_Archive'
      );
    }

    return this.translateAdapterService.getValueImmediately(
      EntityStatusMapping.get(statusMappingKey),
      { user: userName }
    );
  }

  private getCreatedChange(log: DatahubEventModel): AuditHistory {
    return new AuditHistory({
      id: log.id,
      actionUserInfo: this.convertExecutorToActionUserInfo(log.executor),
      date: log.created,
      actionType: AuditActionType.AccountCreated,
      payload: log.payload
    });
  }

  private async getUpdatedChanges(
    logs: DatahubEventModel[]
  ): Promise<AuditHistory[]> {
    if (!logs || !logs.length) {
      return [];
    }
    const histories = [];
    logs[0] = UserAuditHistoryHelper.mapArrayObjectToSimpleArray(logs[0]);
    const metadatas: { [id: string]: PDCatalogueEnumerationDto[] } = {};

    for (const field of ListOfMetadatas) {
      const metadataId = METADATA_ID[field];
      metadatas[field] = await this.pdCatalogueStoreService.getPDCatalogueAsync(
        metadataId
      );
    }

    for (let i = 0; i < logs.length - 1; i++) {
      logs[i + 1] = UserAuditHistoryHelper.mapArrayObjectToSimpleArray(
        logs[i + 1]
      );
      const differ = UserAuditHistoryHelper.getDiffs(
        logs[i].payload.body.userData,
        logs[i + 1].payload.body.userData,
        TrackedUserUpdatedProps,
        metadatas
      );
      if (ObjectUtilities.isEmpty(differ)) {
        continue;
      }

      histories.push(
        new AuditHistory({
          id: logs[i + 1].id,
          actionUserInfo: this.convertExecutorToActionUserInfo(
            logs[i + 1].executor
          ),
          date: logs[i + 1].created,
          actionType: AuditActionType.InfoUpdated,
          showSubInfo: true,
          payload: differ
        })
      );
    }

    return histories;
  }

  private convertExecutorToActionUserInfo(
    executor: AuditLogExecutor
  ): ActionUserInfo {
    if (!executor) {
      return new ActionUserInfo({
        extId: this.user.identity.extId,
        firstName: this.user.firstName,
        avatarUrl: this.getUserImage(this.user)
      });
    }
    const actionUserInfo = new ActionUserInfo({
      extId: executor.extId,
      firstName: executor.fullName,
      avatarUrl: executor.avatarUrl
    });

    return actionUserInfo;
  }
}
