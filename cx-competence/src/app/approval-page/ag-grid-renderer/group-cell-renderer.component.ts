import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { ApprovalGroupModel } from '../models/class-registration.model';

@Component({
  selector: 'group-cell',
  template: `
    <div *ngIf="groupModel" class="group-cell">
      <div class="group-cell__avatar">
        <div
          *ngFor="let item of [1, 1, 1]"
          class="group-cell-avatar"
          [style.background-image]="'url(' + defaultAvatar + ')'"
        ></div>
      </div>
      <div
        class="group-cell__infos group-cell-infos"
        title="{{ groupModel.name }}"
      >
        <div class="group-cell-infos__title">{{ groupModel.name }}</div>
        <div
          *ngIf="groupModel.totalPendingLv1 > 0"
          class="group-cell-infos__sub-title"
        >
          {{ groupModel.totalPendingLv1 }}
          {{ groupModel.totalPendingLv1 > 1 ? 'learners are' : 'learner is' }}
          pending approval
        </div>
        <div
          *ngIf="groupModel.totalPendingLv1 === 0"
          class="group-cell-infos__sub-title"
        >
          None learner pending approval
        </div>
      </div>
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class GroupCellRendererComponent implements ICellRendererAngularComp {
  groupModel: ApprovalGroupModel;
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
    this.groupModel = param.value;
  }
}
