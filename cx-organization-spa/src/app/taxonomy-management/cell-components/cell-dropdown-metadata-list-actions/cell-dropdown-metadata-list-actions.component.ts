import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { TaxonomyRequestStatus } from 'app/taxonomy-management/constant/taxonomy-request-status.enum';
import { TaxonomyRequestStatusType } from 'app/taxonomy-management/models/taxonomy-request-status-type';

@Component({
  selector: 'cell-dropdown-metadata-list-actions',
  templateUrl: './cell-dropdown-metadata-list-actions.component.html',
  styleUrls: ['./cell-dropdown-metadata-list-actions.component.scss']
})
export class CellDropdownMetadataListActionsComponent
  implements ICellRendererAngularComp {
  taxonomyRequestStatus: TaxonomyRequestStatusType = {
    pendingLevel1: TaxonomyRequestStatus.PendingLevel1,
    pendingLevel2: TaxonomyRequestStatus.PendingLevel2,
    rejectLevel1: TaxonomyRequestStatus.RejectLevel1,
    rejectLevel2: TaxonomyRequestStatus.RejectLevel2,
    approved: TaxonomyRequestStatus.Approved,
    completed: TaxonomyRequestStatus.Completed
  };

  requestStatus: TaxonomyRequestStatus;

  hasPermissionToEdit: boolean;
  hasPermissionToApprove: boolean;
  hasPermissionToReject: boolean;
  hasPermissionToComplete: boolean;

  params: any;

  agInit(params: any): void {
    this.params = params;
    this.hasPermissionToEdit = params.hasPermissionToEdit;
    this.hasPermissionToApprove = params.hasPermissionToApprove;
    this.hasPermissionToReject = params.hasPermissionToReject;
    this.hasPermissionToComplete = params.hasPermissionToComplete;

    this.requestStatus = params.data.status;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onClick(action: string): void {
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        action,
        data: this.params.node.data
        // ...something
      };
      this.params.onClick(params);
    }
  }
}
