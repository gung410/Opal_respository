import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { UserManagement } from 'app/user-accounts/models/user-management.model';

@Component({
  selector: 'cell-user-status',
  templateUrl: './cell-user-status.component.html',
  styleUrls: ['./cell-user-status.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellUserStatusComponent implements ICellRendererAngularComp {
  employee: UserManagement;

  agInit(params: any): void {
    this.employee = params.data;
  }

  refresh(params?: any): boolean {
    return true;
  }
}
