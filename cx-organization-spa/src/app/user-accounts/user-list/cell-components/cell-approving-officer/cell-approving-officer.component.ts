import {
  ChangeDetectionStrategy,
  Component,
  ViewEncapsulation
} from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ApprovalGroupTypeEnum } from 'app/user-accounts/constants/approval-group.enum';

@Component({
  selector: 'cell-approving-officer',
  templateUrl: './cell-approving-officer.component.html',
  styleUrls: ['./cell-approving-officer.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CellApprovingOfficerComponent implements ICellRendererAngularComp {
  approvingOfficer: string;
  alternativeApprovingOfficer: string;
  params: any;

  agInit(params: any): void {
    this.params = params.value;
    this.approvingOfficer = this.getApprovingOfficer(
      ApprovalGroupTypeEnum.PrimaryApprovalGroup
    );
    this.alternativeApprovingOfficer = this.getApprovingOfficer(
      ApprovalGroupTypeEnum.AlternativeApprovalGroup
    );
  }

  refresh(params?: any): boolean {
    return true;
  }

  getApprovingOfficer(approvingOfficerType: ApprovalGroupTypeEnum): string {
    const groupApproval =
      this.params &&
      this.params.find((group: any) => group.type === approvingOfficerType);

    return groupApproval ? groupApproval.name : '';
  }
}
