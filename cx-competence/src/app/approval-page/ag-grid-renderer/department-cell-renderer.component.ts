import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { ApprovalDepartmentModel } from '../models/class-registration.model';

@Component({
  selector: 'department-cell',
  template: `
    <div *ngIf="departmentModel" class="department-cell">
      <div class="department-cell__avatar">
        <div
          *ngFor="let item of [1, 1, 1, 1]"
          class="department-cell-avatar"
          [style.background-image]="'url(' + defaultAvatar + ')'"
        ></div>
      </div>
      <div
        class="department-cell__infos department-cell-infos"
        title="{{ departmentModel.name }}"
      >
        <div class="department-cell-infos__title">
          {{ departmentModel.name }}
        </div>
        <div
          *ngIf="departmentModel.totalPendingLv1 > 0"
          class="department-cell-infos__sub-title"
        >
          {{ departmentModel.totalPendingLv1 }}
          {{
            departmentModel.totalPendingLv1 > 1 ? 'learners are' : 'learner is'
          }}
          pending approval
        </div>
        <div
          *ngIf="departmentModel.totalPendingLv1 === 0"
          class="department-cell-infos__sub-title"
        >
          None learner pending approval
        </div>
      </div>
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class DepartmentCellRendererComponent
  implements ICellRendererAngularComp {
  departmentModel: ApprovalDepartmentModel;
  defaultAvatar: string = AppConstant.defaultAvatar;
  constructor() {}

  // called on init
  agInit(param: any): boolean {
    this.processParam(param);

    return true;
  }

  // called when the cell is refreshed
  refresh(param: any): boolean {
    this.processParam(param);

    return true;
  }

  processParam(param: any): void {
    if (!param || !param.value) {
      return;
    }
    this.departmentModel = param.value;
  }
}
