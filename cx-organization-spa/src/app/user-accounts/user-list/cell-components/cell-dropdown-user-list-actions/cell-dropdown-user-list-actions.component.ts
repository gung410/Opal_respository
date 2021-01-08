import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { User } from 'app-models/auth.model';
import { EntityStatus } from 'app-models/entity-status.model';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { UserActionsService } from 'app/user-accounts/user-actions.service';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';

@Component({
  selector: 'cell-dropdown-action',
  templateUrl: './cell-dropdown-user-list-actions.component.html',
  styleUrls: ['./cell-dropdown-user-list-actions.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CellDropdownUserListActionsComponent
  implements ICellRendererAngularComp {
  params: any;
  actionList: ActionsModel[];
  item: any;
  currentUser: User;
  private translateContext: string = 'User_Account_Page.User_Context_Menu.';

  constructor(private userActionsService: UserActionsService) {}

  agInit(params: any): void {
    this.buildData(params);
    this.buildActionsForDropdown();
  }

  buildActionsForDropdown(): void {
    const allowedUserActionMapping = this.userActionsService.getActions(
      'userList',
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

    const isChangePlaceOfWorkTab = this.params.context.componentParent
      .isOtherPlaceOfWorkTab;
    if (isChangePlaceOfWorkTab) {
      this.actionList = this.actionList.filter(
        (action: ActionsModel) =>
          action.actionType !== StatusActionTypeEnum.Accept
      );
    }
  }

  buildData(params: any): void {
    this.params = params;
    this.item = this.params.data;
    this.currentUser = this.params.context.componentParent.currentUser;
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
    if (
      this.isCurrentUserRecord(
        this.item.identity.extId,
        this.params.context.componentParent.currentUser.id
      )
    ) {
      this.actionList = this.actionList.filter(
        (x) =>
          x.actionType !== StatusActionTypeEnum.Suspend &&
          x.actionType !== StatusActionTypeEnum.Archive &&
          x.actionType !== StatusActionTypeEnum.Delete &&
          x.actionType !== StatusActionTypeEnum.Unarchive
      );

      return;
    }
    if (!this.isArchiveButtonEnable([this.item.entityStatus])) {
      this.actionList = this.actionList.filter(
        (x) => x.actionType !== StatusActionTypeEnum.Unarchive
      );
    }

    if (this.isMOEUser) {
      this.actionList = this.actionList.filter(
        (x) => x.actionType !== StatusActionTypeEnum.Delete
      );
    }
  }

  private get isMOEUser(): boolean {
    return this.item.entityStatus.externallyMastered === true;
  }

  private isArchiveButtonEnable(entityStatuses: EntityStatus[]): boolean {
    return entityStatuses.some(
      (entityStatus: EntityStatus) =>
        entityStatus.statusId === StatusTypeEnum.Archived.code &&
        (!entityStatus.expirationDate ||
          (entityStatus.expirationDate &&
            DateTimeUtil.compareDate(
              new Date(entityStatus.expirationDate),
              new Date(),
              false
            ) > 0))
    );
  }
}
