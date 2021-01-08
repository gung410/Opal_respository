import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { User } from 'app-models/auth.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { SAM_PERMISSIONS } from 'app/shared/constants/sam-permission.constant';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { UserAccountsHelper } from 'app/user-accounts/user-accounts.helper';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';
import { PendingActionIndex } from 'app/user-accounts/user-pending-list/constant/pending-actions-index-const.enum';

@Component({
  selector: 'cell-dropdown-action',
  templateUrl: './cell-dropdown-pending-actions.component.html',
  styleUrls: ['./cell-dropdown-pending-actions.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CellDropdownPendingActionsComponent
  implements ICellRendererAngularComp {
  params: any;
  actionList: ActionsModel[];
  item: any;
  currentUser: User;
  private translateContext: string = 'User_Account_Page.User_Context_Menu.';

  agInit(params: any): void {
    this.params = params;
    this.item = this.params.data;
    this.currentUser = this.params.context.componentParent.currentUser;

    const allowedUserActionMapping = UserAccountsHelper.getAccessibleUserActionMapping(
      this.params.context.componentParent.currentUserRoles
    );

    this.actionList = allowedUserActionMapping
      .filter((item: any) => {
        return (
          item.currentStatus.findIndex(
            (stt: string) => stt === this.item.entityStatus.statusId
          ) > findIndexCommon.notFound && item.allowActionSingle === true
        );
      })
      .filter((statusMapped: any) => {
        if (!this.params.context.componentParent.validateActionsInPendingTabs) {
          return true;
        }
        // [canApprove, canEndorse, canReject, canEdit]
        const canActions = this.params.context.componentParent.validateActionsInPendingTabs();
        const isUserHasRightsToAccessRequestSpecialApproval = this.currentUser
          ? this.currentUser.hasPermission(
              SAM_PERMISSIONS.RequestSpecialApprovalPending2nd
            )
          : false;
        switch (statusMapped.targetAction) {
          case StatusActionTypeEnum.Accept:
            return (
              canActions[PendingActionIndex.Approve] ||
              canActions[PendingActionIndex.Endorse]
            );
          case StatusActionTypeEnum.Reject:
            return canActions[PendingActionIndex.Reject];
          case StatusActionTypeEnum.Edit:
            return canActions[PendingActionIndex.Edit];
          case StatusActionTypeEnum.RequestSpecialApproval:
            return isUserHasRightsToAccessRequestSpecialApproval;
          default:
            return false;
        }
      })
      .map((statusMapped: any) => {
        const action = new ActionsModel({
          actionType: statusMapped.targetAction,
          icon: statusMapped.targetIcon,
          message: statusMapped.message
        });
        if (
          statusMapped.targetAction === StatusActionTypeEnum.Accept &&
          this.params.context.componentParent
        ) {
          action.text = `${this.translateContext}${
            this.params.context.componentParent.isCurrentUserDivAdmin
              ? 'Endorse'
              : 'Approve'
          }`;
        } else {
          action.text = `${this.translateContext}${statusMapped.targetAction}`;
        }

        return action;
      });
  }

  refresh(params: any): boolean {
    throw new Error('Method not implemented.');
  }

  onClick($event: ActionsModel): void {
    this.params.onClick({ action: $event, item: this.item });
  }

  isCurrentUserRecord(userId: string, itemUserId: string): boolean {
    return userId === itemUserId;
  }

  onDropdownOpened(): void {
    if (
      !this.item ||
      !this.item.identity ||
      !this.params.context.componentParent.currentUser
    ) {
      return;
    }

    // If this record owned by current user. Then remove user action Suspend and Archive in user action list
  }
}
