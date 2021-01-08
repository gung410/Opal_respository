import {
  ChangeDetectorRef,
  Component,
  Input,
  ViewEncapsulation
} from '@angular/core';
import { AuthService } from 'app-auth/auth.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { BaseScreenComponent } from 'app/shared/components/component.abstract';
import {
  StatusTypeEnum,
  StatusTypeNumberEnum
} from 'app/shared/constants/user-status-type.enum';

import { DatahubEventActionType } from '../constants/user-field-mapping.constant';
import { UserManagement } from '../models/user-management.model';

@Component({
  selector: 'status-historical-row',
  templateUrl: './status-historical-row.component.html',
  styleUrls: ['./status-historical-row.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class StatusHistoricalRowComponent extends BaseScreenComponent {
  @Input() user: UserManagement;
  @Input() historicalStatusData: any;
  @Input() get modifyingUser(): UserManagement {
    return this._modifyingUser;
  }
  set modifyingUser(modifyingUser: UserManagement) {
    if (
      this.historicalStatusData.routing.action ===
      DatahubEventActionType.UserCreated
    ) {
      this.setCreatedLog(modifyingUser);

      return;
    } else if (
      this.historicalStatusData.routing.action ===
        DatahubEventActionType.EntityStatusChanged &&
      this.historicalStatusData.payload.body.userData.toEntityStatusId ===
        StatusTypeNumberEnum.IdentityServerLocked
    ) {
      this.statusChangeLog = `${this.translateAdapterService.getValueImmediately(
        'User_Account_Page.User_List.User_Historical_Status.Locked_User_Action'
      )}`;
      this.originalStatus = `${
        StatusTypeEnum[
          StatusTypeNumberEnum[
            this.historicalStatusData.payload.body.userData.fromEntityStatusId
          ]
        ].text
      } `;
      this.newStatus = ` ${
        StatusTypeEnum[
          StatusTypeNumberEnum[
            this.historicalStatusData.payload.body.userData.toEntityStatusId
          ]
        ].text
      } `;

      return;
    } else {
      this._modifyingUser = modifyingUser;
      this.currentLanguage = this.translateAdapterService.getCurrentLanguage();
      if (modifyingUser) {
        this.modifyingUserRole = this._modifyingUser.systemRoles
          ? `(${SurveyUtils.getPropLocalizedData(
              this._modifyingUser.systemRoles[0].localizedData,
              'Name',
              this.currentLanguage
            )})`
          : '';
      }
      this.getStatus();
      this.getStatusChangeProcess();
    }
  }

  statusType: typeof StatusTypeNumberEnum = StatusTypeNumberEnum;
  modifyingUserRole: any = '';
  statusChangeLog: string = '';
  originalStatus: string = '';
  newStatus: string = '';

  private _modifyingUser: UserManagement;
  private currentLanguage: string;

  constructor(
    protected authService: AuthService,
    changeDetectorRef: ChangeDetectorRef,
    private translateAdapterService: TranslateAdapterService
  ) {
    super(changeDetectorRef, authService);
  }

  private getStatus(): void {
    switch (this.historicalStatusData.payload.body.userData.toEntityStatusId) {
      case StatusTypeNumberEnum.New:
        this.statusChangeLog =
          this.historicalStatusData.payload.body.userData.fromEntityStatusId ===
          this.statusType.PendingApproval2nd
            ? `User_Account_Page.User_List.User_Historical_Status.Approved_2nd_Level_Action`
            : 'User_Account_Page.User_List.User_Historical_Status.Approved_Special_Level_Action';
        break;
      case StatusTypeNumberEnum.PendingApproval1st:
        this.statusChangeLog =
          'User_Account_Page.User_List.User_Historical_Status.Newly_Created_Action';
        break;
      case StatusTypeNumberEnum.PendingApproval2nd:
        this.statusChangeLog =
          this.historicalStatusData.payload.body.userData.fromEntityStatusId ===
          this.statusType.PendingApproval1st
            ? `User_Account_Page.User_List.User_Historical_Status.Approved_1st_Level_Action`
            : 'User_Account_Page.User_List.User_Historical_Status.Newly_Created_Action';
        break;
      case StatusTypeNumberEnum.PendingApproval3rd:
        this.statusChangeLog =
          'User_Account_Page.User_List.User_Historical_Status.Requested_Special_Approval_Action';
        break;
      case StatusTypeNumberEnum.Rejected:
        switch (
          this.historicalStatusData.payload.body.userData.fromEntityStatusId
        ) {
          case this.statusType.PendingApproval1st:
            this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Rejected_1st_Level_Action`;
            break;
          case this.statusType.PendingApproval2nd:
            this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Rejected_2nd_Level_Action`;
            break;
          case this.statusType.PendingApproval3rd:
            this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Rejected_3rd_Level_Action`;
            break;
          default:
            break;
        }
        break;
      case StatusTypeNumberEnum.Active:
        if (
          this.historicalStatusData.payload.body.userData.fromEntityStatusId ===
          this.statusType.New
        ) {
          this.statusChangeLog =
            'User_Account_Page.User_List.User_Historical_Status.Newly_Activated_User_Text';
        } else if (
          this.historicalStatusData.payload.body.userData.fromEntityStatusId ===
          this.statusType.IdentityServerLocked
        ) {
          this.statusChangeLog =
            'User_Account_Page.User_List.User_Historical_Status.Unlocked_User_Action';
        } else {
          this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Activated_User_Action`;
        }
        break;
      case StatusTypeNumberEnum.Inactive:
        this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Suspended_User_Action`;
        break;
      case StatusTypeNumberEnum.IdentityServerLocked:
        this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Locked_User_Action`;
        break;
      case StatusTypeNumberEnum.Deactive:
        this.statusChangeLog = `User_Account_Page.User_List.User_Historical_Status.Deactivated_User_Action`;
        break;
      default:
        return;
    }
  }

  private getStatusChangeProcess(): void {
    this.originalStatus = `${
      StatusTypeEnum[
        StatusTypeNumberEnum[
          this.historicalStatusData.payload.body.userData.fromEntityStatusId
        ]
      ].text
    } `;
    this.newStatus = ` ${
      StatusTypeEnum[
        StatusTypeNumberEnum[
          this.historicalStatusData.payload.body.userData.toEntityStatusId
        ]
      ].text
    } `;
  }

  private setCreatedLog(modifyingUser: UserManagement): void {
    if (!modifyingUser) {
      this.statusChangeLog = `${this.translateAdapterService.getValueImmediately(
        'User_Account_Page.Audit_History.Field_Change_Title.User_Registered'
      )}`;
      this.originalStatus = StatusTypeEnum.PendingApproval1st.text;
    } else {
      this.statusChangeLog = `${
        modifyingUser.firstName
      } ${this.translateAdapterService.getValueImmediately(
        'User_Account_Page.Audit_History.Field_Change_Title.User_Created'
      )}`;
      this.originalStatus = StatusTypeEnum.PendingApproval2nd.text;
    }
  }
}
