import { Component, ViewEncapsulation } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { IAfterGuiAttachedParams } from 'ag-grid-community';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { BroadcastMessageViewModel } from 'app/broadcast-messages/models/broadcast-messages.view.model';
import { BROADCAST_MESSAGE_STATUS } from 'app/shared/constants/broadcast-message-status.constant';
import { ICON_CONST } from 'app/shared/constants/icon.const';
import { RecurrenceType } from 'app/shared/constants/recurrence-type.enum';
import { StatusActionTypeEnum } from 'app/shared/constants/status-action-type.enum';
import { ActionsModel } from 'app/user-accounts/user-actions/models/actions.model';

@Component({
  selector: 'cell-dropdown-action',
  templateUrl: './cell-broadcast-dropdown-menu.component.html',
  styleUrls: ['./cell-broadcast-dropdown-menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CellBroadcastDropdownMenuComponent
  implements ICellRendererAngularComp {
  params: any;
  isShowIcon: boolean = true;
  actionList: ActionsModel[] = [];
  broadcastMessageItem: BroadcastMessageViewModel;

  private _editAction: ActionsModel = {
    text: 'User_Account_Page.User_Context_Menu.Edit',
    actionType: StatusActionTypeEnum.Edit,
    icon: ICON_CONST.EDIT,
    message: 'broadcast message replaced later',
    allowActionSingle: true
  };

  private _deactivateAction: ActionsModel = {
    text: 'User_Account_Page.User_Context_Menu.Deactivate',
    actionType: StatusActionTypeEnum.Deactivate,
    icon: ICON_CONST.REJECT,
    message: 'broadcast message replaced later',
    allowActionSingle: true
  };

  private _deleteAction: ActionsModel = {
    text: 'User_Account_Page.User_Context_Menu.Delete',
    actionType: StatusActionTypeEnum.Delete,
    icon: ICON_CONST.DELETE,
    message: 'broadcast message replaced later',
    allowActionSingle: true
  };

  agInit(params: any): void {
    this.params = params;
    this.broadcastMessageItem = this.params.data;

    if (
      this.broadcastMessageItem.status === BROADCAST_MESSAGE_STATUS.NONE &&
      !this.isBroadcastMessageExpired(this.broadcastMessageItem)
    ) {
      this.actionList.push(this._editAction, this._deactivateAction);
    }
    if (
      this.broadcastMessageItem.status ===
        BROADCAST_MESSAGE_STATUS.DEACTIVATE ||
      this.isBroadcastMessageExpired(this.broadcastMessageItem)
    ) {
      this.actionList.push(this._deleteAction);
    }
  }

  onClick($event: ActionsModel): void {
    this.params.onClick({ action: $event, item: this.broadcastMessageItem });
  }

  refresh(): boolean {
    throw new Error('Method not implemented.');
  }
  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
    throw new Error('Method not implemented.');
  }

  private isBroadcastMessageExpired(
    broadcastMessageVM: BroadcastMessageViewModel
  ): boolean {
    return (
      DateTimeUtil.compareDate(
        broadcastMessageVM.validToDate,
        new Date(),
        true
      ) < 0
    );
  }
}
