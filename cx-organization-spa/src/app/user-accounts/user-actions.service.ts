import { Injectable } from '@angular/core';
import { ICON_CONST } from 'app/shared/constants/icon.const';
import { SAM_PERMISSIONS } from 'app/shared/constants/sam-permission.constant';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { User } from '../shared/models/auth.model';
import { IUserAction } from './models/user-action.model';

@Injectable()
export class UserActionsService {
  constructor() {}

  getActions(tabLabel: string, currentUser: User): IUserAction[] {
    switch (tabLabel) {
      case 'userList':
        return this.actionsOnUserListTab(currentUser).filter(
          (action) => action.hasPermission
        );
      default:
        return [];
    }
  }

  private actionsOnUserListTab(currentUser: User): IUserAction[] {
    const isAllowedToManageBasicUserAccountsAction = currentUser.hasPermission(
      SAM_PERMISSIONS.BasicUserAccountsManagement
    );
    const isAllowedToManageAdvancedUserAccounts = currentUser.hasPermission(
      SAM_PERMISSIONS.AdvancedUserAccountsManagement
    );
    const actions: IUserAction[] = [
      {
        targetAction: StatusActionTypeEnum.Active,
        targetIcon: ICON_CONST.ACTIVE,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.Change_User_Status_Warning',
        currentStatus: [StatusTypeEnum.Inactive.code],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.Suspend,
        targetIcon: ICON_CONST.PAUSE,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.Change_User_Status_Warning',
        currentStatus: [StatusTypeEnum.New.code, StatusTypeEnum.Active.code],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.ResetPassword,
        targetIcon: ICON_CONST.REVERT,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.Reset_Password_Warning',
        currentStatus: [StatusTypeEnum.New.code, StatusTypeEnum.Active.code],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.Edit,
        targetIcon: ICON_CONST.EDIT,
        isSimpleAction: false,
        allowActionSingle: true,
        message: '',
        currentStatus: [
          StatusTypeEnum.New.code,
          StatusTypeEnum.Active.code,
          StatusTypeEnum.Inactive.code,
          StatusTypeEnum.IdentityServerLocked.code,
          StatusTypeEnum.PendingApproval1st.code,
          StatusTypeEnum.PendingApproval2nd.code,
          StatusTypeEnum.PendingApproval3rd.code,
          StatusTypeEnum.Archived.code
        ],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.Unlock,
        targetIcon: ICON_CONST.UNLOCK,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.Unlock_User_Warning',
        currentStatus: [StatusTypeEnum.IdentityServerLocked.code],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.SetApprovingOfficers,
        targetIcon: '',
        isSimpleAction: false,
        allowActionSingle: false,
        message: '',
        currentStatus: [],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.SetExpirationDate,
        targetIcon: ICON_CONST.SET_DATE,
        isSimpleAction: false,
        allowActionSingle: false,
        message: 'User_Account_Page.User_List.Set_Expiration_Date_Warning',
        currentStatus: [
          StatusTypeEnum.New.code,
          StatusTypeEnum.Active.code,
          StatusTypeEnum.Inactive.code,
          StatusTypeEnum.IdentityServerLocked.code
        ],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.AddToGroup,
        targetIcon: '',
        isSimpleAction: false,
        allowActionSingle: false,
        message: '',
        currentStatus: [],
        hasPermission: isAllowedToManageBasicUserAccountsAction
      },
      {
        targetAction: StatusActionTypeEnum.Unarchive,
        targetIcon: ICON_CONST.UNARCHIVE,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.UnArchive_User_Warning',
        currentStatus: [StatusTypeEnum.Archived.code],
        hasPermission: isAllowedToManageAdvancedUserAccounts
      },
      {
        targetAction: StatusActionTypeEnum.Archive,
        targetIcon: ICON_CONST.ARCHIVE,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.Archive_User_Warning',
        currentStatus: [
          StatusTypeEnum.New.code,
          StatusTypeEnum.Active.code,
          StatusTypeEnum.Inactive.code,
          StatusTypeEnum.IdentityServerLocked.code
        ],
        hasPermission: isAllowedToManageAdvancedUserAccounts
      },
      {
        targetAction: StatusActionTypeEnum.Delete,
        targetIcon: ICON_CONST.DELETE,
        isSimpleAction: false,
        allowActionSingle: true,
        message: 'User_Account_Page.User_List.Remove_User_Warning',
        currentStatus: [
          StatusTypeEnum.New.code,
          StatusTypeEnum.Active.code,
          StatusTypeEnum.Inactive.code,
          StatusTypeEnum.IdentityServerLocked.code,
          StatusTypeEnum.Archived.code
        ],
        hasPermission: isAllowedToManageAdvancedUserAccounts
      },
      {
        targetAction: StatusActionTypeEnum.ChangeUserPlaceOfWork,
        targetIcon: ICON_CONST.CHANGE_PLACE_OF_WORK,
        isSimpleAction: true,
        allowActionSingle: false,
        message: '',
        currentStatus: [
          StatusTypeEnum.New.code,
          StatusTypeEnum.Active.code,
          StatusTypeEnum.IdentityServerLocked.code,
          StatusTypeEnum.PendingApproval1st.code,
          StatusTypeEnum.PendingApproval2nd.code,
          StatusTypeEnum.PendingApproval3rd.code
        ],
        hasPermission: isAllowedToManageAdvancedUserAccounts
      }
    ];

    return actions;
  }
}
