import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { UserManagement } from 'app/user-accounts/models/user-management.model';
import {
  UserEntityStatusConst,
  UserEntityStatusEnum
} from 'app/user-accounts/user-accounts.model';
import { PendingActionIndex } from '../../constant/pending-actions-index-const.enum';

@Component({
  selector: 'cell-user-pending-info',
  templateUrl: './cell-user-pending-info.component.html',
  styleUrls: ['./cell-user-pending-info.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellUserPendingInfoComponent
  extends BaseComponent
  implements ICellRendererAngularComp {
  employee: UserManagement;
  params: any;
  currentPendingTabLabel: string;

  agInit(params: any): void {
    this.setupUserData(params);
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
    if (!this.params.context.componentParent.validateActionsInPendingTabs) {
      return;
    }

    this.params.context.componentParent.editUser.emit($event);
  }

  private setupUserData(params: any): void {
    this.params = params;
    this.employee = params.data;
    this.currentPendingTabLabel = params.context.componentParent.tabLabel;
  }
}
