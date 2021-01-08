import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { User } from 'app-models/auth.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { UserAccountsHelper } from 'app/user-accounts/user-accounts.helper';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';
import { SAM_PERMISSIONS } from '../../../shared/constants/sam-permission.constant';

@Component({
  selector: 'cell-dropdown-action',
  templateUrl: './cell-dropdown-other-place-actions.component.html',
  styleUrls: ['./cell-dropdown-other-place-actions.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CellDropdownOtherPlaceOfWorkActionsComponent
  implements ICellRendererAngularComp {
  params: any;
  actionList: ActionsModel[];
  item: any;
  currentUser: User = null;
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
          ) > findIndexCommon.notFound &&
          item.allowActionSingle === true &&
          item.targetAction !== StatusActionTypeEnum.Accept
        );
      })
      .filter((statusMapped: any) => {
        if (!this.currentUser) {
          return true;
        }

        switch (statusMapped.targetAction) {
          case StatusActionTypeEnum.Reject:
            return this.currentUser.hasPermission(
              SAM_PERMISSIONS.RejectInOtherPlaceOfWork
            );
          case StatusActionTypeEnum.Edit:
            return this.currentUser.hasPermission(
              SAM_PERMISSIONS.EditOtherPlaceOfWork
            );
          default:
            return false;
        }
      })
      .map(
        (statusMapped: any) =>
          new ActionsModel({
            actionType: statusMapped.targetAction,
            icon: statusMapped.targetIcon,
            message: statusMapped.message,
            text: `${this.translateContext}${statusMapped.targetAction}`
          })
      );
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
  }
}
