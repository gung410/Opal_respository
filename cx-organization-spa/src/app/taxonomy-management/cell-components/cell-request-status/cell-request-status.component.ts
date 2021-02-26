import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { TaxonomyRequestStatus } from 'app/taxonomy-management/constant/taxonomy-request-status.enum';

@Component({
  selector: 'cell-request-status',
  templateUrl: './cell-request-status.component.html',
  styleUrls: ['./cell-request-status.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellRequestStatusComponent implements ICellRendererAngularComp {
  requestStatus: string;
  statusClass: string;
  statusReason: string;

  agInit(params: any): void {
    this.requestStatus = params.value;
    this.getStatusReason();
    this.getStatusClass();
  }

  refresh(params?: any): boolean {
    return true;
  }

  getStatusReason(): void {
    switch (this.requestStatus) {
      case TaxonomyRequestStatus.PendingLevel1:
        this.statusReason = 'Pending 1st Level Approval';
        break;
      case TaxonomyRequestStatus.PendingLevel2:
        this.statusReason = 'Pending 2nd Level Approval';
        break;
      case TaxonomyRequestStatus.RejectLevel1:
        this.statusReason = 'Rejected by level 1';
        break;
      case TaxonomyRequestStatus.RejectLevel2:
        this.statusReason = 'Rejected by level 2';
        break;
      case TaxonomyRequestStatus.Approved:
        this.statusReason = 'Approved';
        break;
      case TaxonomyRequestStatus.Completed:
        this.statusReason = 'Completed';
        break;
      default:
        this.statusReason = '';
        break;
    }
  }

  getStatusClass(): void {
    switch (this.requestStatus) {
      case TaxonomyRequestStatus.PendingLevel1:
      case TaxonomyRequestStatus.PendingLevel2:
        this.statusClass = 'warning-status';
        break;
      case TaxonomyRequestStatus.RejectLevel1:
      case TaxonomyRequestStatus.RejectLevel2:
        this.statusClass = 'danger-status';
        break;
      case TaxonomyRequestStatus.Approved:
      case TaxonomyRequestStatus.Completed:
        this.statusClass = 'success-status';
        break;
      default:
        this.statusReason = '';
        break;
    }
  }
}
