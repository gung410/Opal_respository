import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { EntityStatus } from 'app-models/entity-status.model';
import { findIndexCommon } from 'app/shared/constants/common.const';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';
import { UserAccountsHelper } from 'app/user-accounts/user-accounts.helper';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';
import { DateTimeUtil } from '../../../shared/utilities/date-time-utils';
@Component({
  selector: 'cell-dropdown-action',
  templateUrl: './cell-dropdown-action.component.html',
  styleUrls: ['./cell-dropdown-action.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CellDropdownActionComponent implements ICellRendererAngularComp {
  params: any;
  actionList: ActionsModel[];
  item: any;
  private translateContext: string = 'User_Account_Page.User_Context_Menu.';

  agInit(params: any): void {
    this.params = params;
    this.item = this.params.data;

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
