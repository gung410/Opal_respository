import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { BaseComponent } from 'app/shared/components/component.abstract';
import { UserManagement } from 'app/user-accounts/models/user-management.model';

@Component({
  selector: 'taxonomy-cell-user-info',
  templateUrl: './taxonomy-cell-user-info.component.html',
  styleUrls: ['./taxonomy-cell-user-info.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaxonomyCellUserInfoComponent
  extends BaseComponent
  implements ICellRendererAngularComp {
  employee: UserManagement;
  params: any;
  currentTabLabel: string;

  agInit(params: any): void {
    this.setupCellUserInfo(params);
  }

  refresh(params?: any): boolean {
    return true;
  }

  onEditUserClicked($event: any): void {
    this.params.context.componentParent.editUser.emit($event);
  }

  private setupCellUserInfo(params: any): void {
    this.params = params;
    if (!params.metadataRequestedByUserInfo) {
      return;
    }
    this.employee = params.metadataRequestedByUserInfo.find(
      (user: UserManagement) =>
        user.identity.extId.toUpperCase() ===
        params.data.createdBy.toUpperCase()
    );
    this.currentTabLabel = params.context.componentParent.tabLabel;
  }
}
