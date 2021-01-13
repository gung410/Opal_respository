import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { User } from 'app-models/auth.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { SAM_PERMISSIONS } from 'app/shared/constants/sam-permission.constant';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { UserActionsService } from 'app/user-accounts/user-actions.service';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';

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
  tabLabel: string;
  isAuthorizedAdmins: boolean = false;
  private translateContext: string = 'User_Account_Page.User_Context_Menu.';

  constructor(private userActionsService: UserActionsService) {}

  agInit(params: any): void {
    this.buildData(params);
    const allowedUserActionMapping = this.userActionsService.getActions(
      this.tabLabel,
      this.currentUser
    );

    this.actionList = allowedUserActionMapping
      .filter((item: any) => {
        return (
          item.currentStatus.findIndex(
            (stt: string) => stt === this.item.entityStatus.statusId
          ) > findIndexCommon.notFound && item.allowActionSingle === true
        );
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
          action.text = `${this.translateContext}${'Approve'}`;
          if (
            !this.isAuthorizedAdmins &&
            this.tabLabel === 'pending1stLevel' &&
            this.currentUser.hasPermission(SAM_PERMISSIONS.EndorsePending1st)
          ) {
            action.text = `${this.translateContext}${'Endorse'}`;
          }

          if (
            this.isAuthorizedAdmins &&
            this.tabLabel === 'pending1stLevel' &&
            this.currentUser.hasPermission(SAM_PERMISSIONS.EndorsePending1st) &&
            !this.currentUser.hasPermission(SAM_PERMISSIONS.ApprovePending1st)
          ) {
            action.text = `${this.translateContext}${'Endorse'}`;
          }
        } else {
          action.text = `${this.translateContext}${statusMapped.targetAction}`;
        }

        return action;
      });
  }

  buildData(params: any): void {
    this.params = params;
    this.item = this.params.data;
    this.currentUser = this.params.context.componentParent.currentUser;
    this.tabLabel = this.params.context.componentParent.tabLabel;
    this.isAuthorizedAdmins =
      this.currentUser.hasUserAccountAdministrator() ||
      this.currentUser.hasOverallSystemAdministrator();
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
