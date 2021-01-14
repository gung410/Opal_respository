import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { SurveyUtils } from 'app-utilities/survey-utils';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import {
  UserEntityStatusConst,
  UserEntityStatusEnum
} from 'app/user-accounts/user-accounts.model';

@Component({
  selector: 'cell-user-info',
  templateUrl: './cell-user-info.component.html',
  styleUrls: ['./cell-user-info.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellUserInfoComponent
  extends BaseComponent
  implements ICellRendererAngularComp {
  employee: UserManagement;
  params: any;

  agInit(params: any): void {
    this.params = params;
    if (!params.isFromBroadcastMessageList) {
      this.employee = params.data;

      return;
    }

    this.employee = params.broadcastMessageOwners.find(
      (broadcastMessageOwner) =>
        broadcastMessageOwner.identity.extId.toLowerCase() ===
        params.data.ownerId.toLowerCase()
    );
  }

  refresh(params?: any): boolean {
    return true;
  }

  getUserAccountTypeDescription(employee: UserManagement): string {
    return UserEntityStatusConst.find(
      (status: any) =>
        status.key ===
        (employee.entityStatus.externallyMastered
          ? UserEntityStatusEnum.SynchronizedUserAccount
          : UserEntityStatusEnum.ManualUserAccount)
    ).description;
  }

  onEditUserClicked($event: any): void {
    this.params.context.componentParent.editUser.emit($event);
  }
}
