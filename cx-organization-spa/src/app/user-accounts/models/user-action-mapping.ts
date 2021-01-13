import {
  ActionToolbarModel,
  CxTreeIcon,
  CxTreeText,
  DepartmentHierarchiesModel
} from '@conexus/cx-angular-common';
import { environment } from 'app-environments/environment';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import { ICON_CONST } from 'app/shared/constants/icon.const';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { UserRoleEnum } from 'app/shared/constants/user-roles.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';

// TODO: Refactor this constant:
//  1. Create a new class for the item in the array since currently if someone looking into this array he could not know how to configure.
//  2. Figure out a better way to configure this list so that the other places doesn't need to handle so much logic to show/hide an action.
export const USER_ACTION_MAPPING_CONST = [
  {
    targetAction: StatusActionTypeEnum.Active,
    targetIcon: ICON_CONST.ACTIVE,
    isSimpleAction: false,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Change_User_Status_Warning',
    currentStatus: [StatusTypeEnum.Inactive.code],
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator
    ]
  },
  {
    targetAction: StatusActionTypeEnum.Suspend,
    targetIcon: ICON_CONST.PAUSE,
    isSimpleAction: false,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Change_User_Status_Warning',
    currentStatus: [StatusTypeEnum.New.code, StatusTypeEnum.Active.code],
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator,
      UserRoleEnum.DivisionAdmin,
      UserRoleEnum.BranchAdmin,
      UserRoleEnum.SchoolAdmin,
      UserRoleEnum.DivisionalLearningCoordinator,
      UserRoleEnum.SchoolStaffDeveloper
    ]
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
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator
    ]
  },
  {
    targetAction: StatusActionTypeEnum.Unarchive,
    targetIcon: ICON_CONST.UNARCHIVE,
    isSimpleAction: false,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.UnArchive_User_Warning',
    currentStatus: [StatusTypeEnum.Archived.code],
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator
    ]
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
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator
    ]
  },
  {
    targetAction: StatusActionTypeEnum.Unlock,
    targetIcon: ICON_CONST.UNLOCK,
    isSimpleAction: false,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Unlock_User_Warning',
    currentStatus: [StatusTypeEnum.IdentityServerLocked.code],
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator,
      UserRoleEnum.DivisionAdmin,
      UserRoleEnum.BranchAdmin,
      UserRoleEnum.SchoolAdmin,
      UserRoleEnum.DivisionalLearningCoordinator,
      UserRoleEnum.SchoolStaffDeveloper
    ]
  },
  {
    targetAction: StatusActionTypeEnum.ResetPassword,
    targetIcon: ICON_CONST.REVERT,
    isSimpleAction: false,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Reset_Password_Warning',
    currentStatus: [StatusTypeEnum.New.code, StatusTypeEnum.Active.code]
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
    ]
  },
  {
    targetAction: StatusActionTypeEnum.Accept,
    targetIcon: ICON_CONST.ACCEPT,
    isSimpleAction: true,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Accept_Warning',
    currentStatus: [
      StatusTypeEnum.PendingApproval1st.code,
      StatusTypeEnum.PendingApproval2nd.code,
      StatusTypeEnum.PendingApproval3rd.code
    ]
  },
  {
    targetAction: StatusActionTypeEnum.Reject,
    targetIcon: ICON_CONST.REJECT,
    isSimpleAction: true,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Reject_Warning',
    currentStatus: [
      StatusTypeEnum.PendingApproval1st.code,
      StatusTypeEnum.PendingApproval2nd.code,
      StatusTypeEnum.PendingApproval3rd.code
    ]
  },
  {
    targetAction: StatusActionTypeEnum.RequestSpecialApproval,
    targetIcon: ICON_CONST.REQUEST,
    isSimpleAction: true,
    allowActionSingle: true,
    message: 'User_Account_Page.User_List.Request_Special_Approval_Warning',
    currentStatus: [StatusTypeEnum.PendingApproval2nd.code]
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
    ]
  },
  {
    targetAction: StatusActionTypeEnum.CreateAccount,
    targetIcon: '',
    isSimpleAction: false,
    allowActionSingle: false,
    message: '',
    currentStatus: []
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
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator
    ]
  },
  {
    targetAction: StatusActionTypeEnum.CreateNewOrgUnit,
    targetIcon: ICON_CONST.CHANGE_PLACE_OF_WORK,
    isSimpleAction: true,
    allowActionSingle: false,
    message: '',
    currentStatus: [],
    allowedUserRoles: [
      UserRoleEnum.OverallSystemAdministrator,
      UserRoleEnum.UserAccountAdministrator
    ]
  }
];

export function initUserActions(
  translateAdapterService: TranslateAdapterService,
  isMoreAction: boolean = false,
  hasRightToAccess: boolean = false,
  hasRightToAccessBasicUserAccountsManagement: boolean = false,
  hasRightToAccessExportUsers: boolean = false
): ActionToolbarModel {
  const essentialActions: ActionsModel[] = [];
  const specifyActions: ActionsModel[] = [];

  //These user actions follow this order: Add to Group - Export - Set Approving Officers.
  if (hasRightToAccessBasicUserAccountsManagement) {
    essentialActions.push(
      new ActionsModel({
        text: translateAdapterService.getValueImmediately(
          `Common.Button.Add_To_Group`
        ),
        actionType: StatusActionTypeEnum.AddToGroup,
        allowActionSingle: false,
        icon: null,
        message: '',
        disable: true
      })
    );
  }

  if (hasRightToAccessExportUsers) {
    essentialActions.push(
      new ActionsModel({
        text: translateAdapterService.getValueImmediately(
          `Common.Button.Export`
        ),
        actionType: StatusActionTypeEnum.Export,
        allowActionSingle: false,
        icon: null,
        message: '',
        disable: true
      })
    );
  }

  if (hasRightToAccessBasicUserAccountsManagement) {
    essentialActions.push(
      new ActionsModel({
        text: translateAdapterService.getValueImmediately(
          `Common.Button.Set_Approving_Officers`
        ),
        actionType: StatusActionTypeEnum.SetApprovingOfficers,
        allowActionSingle: false,
        icon: null,
        message: '',
        disable: true
      })
    );
  }

  if (isMoreAction) {
    if (hasRightToAccess) {
      // Temporarily hide away Generate_Account_Review_Report item in menu
      // specifyActions.push(
      //   new ActionsModel({
      //     text: translateAdapterService.getValueImmediately(
      //       `Common.Button.Generate_Account_Review_Report`
      //     ),
      //     actionType: StatusActionTypeEnum.GenerateAccountReviewReport,
      //     allowActionSingle: false,
      //     icon: null,
      //     message: ''
      //   })
      // );
    }
  }

  return new ActionToolbarModel({
    listEssentialActions: essentialActions,
    listSpecifyActions: specifyActions
  });
}
export function initUserActionsForExportButton(
  translateAdapterService: TranslateAdapterService,
  isMoreAction: boolean = false
): ActionsModel[] {
  const userActions: ActionsModel[] = [];
  if (isMoreAction) {
    userActions.push(
      new ActionsModel({
        text: translateAdapterService.getValueImmediately(
          `Common.Button.Export_All`
        ),
        actionType: StatusActionTypeEnum.ExportAll,
        allowActionSingle: false,
        icon: null,
        message: ''
      })
    );
  }

  return userActions;
}

export function initUserActionsForCreateAccButton(
  translateAdapterService: TranslateAdapterService,
  isAllowToSingleCreateUserAccountRequest: boolean = true,
  isAllowToMassCreateUserAccountRequest: boolean = true
): ActionsModel[] {
  const userActions: ActionsModel[] = [];
  if (
    environment.userAccounts.enableCreateUserAccountRequest &&
    isAllowToSingleCreateUserAccountRequest
  ) {
    userActions.push(
      new ActionsModel({
        text: translateAdapterService.getValueImmediately(
          `Common.Button.Create_User_Account`
        ),
        actionType: StatusActionTypeEnum.CreateAccount,
        allowActionSingle: false,
        icon: null,
        message: ''
      })
    );
  }

  if (
    environment.userAccounts.enableMassCreateUserAccountRequest &&
    isAllowToMassCreateUserAccountRequest
  ) {
    userActions.push(
      new ActionsModel({
        text: translateAdapterService.getValueImmediately(
          `Common.Button.Mass_Create_User_Account_Request`
        ),
        actionType: StatusActionTypeEnum.MassCreateAccount,
        allowActionSingle: false,
        icon: null,
        message: ''
      })
    );
  }

  return userActions;
}

export function initPendingUserActions(
  translateAdapterService: TranslateAdapterService,
  isShowEndorseText: boolean,
  canApprove: boolean,
  canEndorse: boolean,
  canReject: boolean
): ActionsModel[] {
  const userActions: ActionsModel[] = [];

  if (canEndorse || canApprove) {
    const acceptAction = new ActionsModel({
      text: translateAdapterService.getValueImmediately(
        'User_Account_Page.User_Context_Menu.Approve'
      ),
      actionType: StatusActionTypeEnum.Accept,
      allowActionSingle: false,
      icon: null,
      message: '',
      disable: true
    });

    if (isShowEndorseText) {
      acceptAction.text = translateAdapterService.getValueImmediately(
        'User_Account_Page.User_Context_Menu.Endorse'
      );
    }

    userActions.push(acceptAction);
  }

  if (canReject) {
    userActions.push(
      new ActionsModel({
        text: StatusActionTypeEnum.Reject,
        actionType: StatusActionTypeEnum.Reject,
        allowActionSingle: false,
        icon: null,
        message: '',
        disable: true
      })
    );
  }

  return userActions;
}

export function initUniversalToolbar(
  translateAdapterService: TranslateAdapterService
): DepartmentHierarchiesModel {
  const departmentModel = new DepartmentHierarchiesModel({
    idFieldRoute: 'identity.id',
    parentIdFieldRoute: 'parentDepartmentId',
    icon: new CxTreeIcon(),
    text: new CxTreeText(),
    isViewMode: true,
    enableSearch: true,
    treeHeader: translateAdapterService.getValueImmediately(
      'User_Account_Page.Table_Header.OrganisationUnit'
    ),
    havingExtensiveArea: false,
    noResultFoundMessage: translateAdapterService.getValueImmediately(
      'User_Account_Page.Search_Result_Dialog.No_Result_Message'
    ),
    isDisplayOrganisationNavigation: true,
    departments: []
  });

  return departmentModel;
}
