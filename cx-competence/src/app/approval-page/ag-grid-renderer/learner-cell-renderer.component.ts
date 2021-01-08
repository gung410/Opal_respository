import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { ApprovalLearnerModel } from '../models/class-registration.model';

@Component({
  selector: 'learner-cell',
  template: `
    <div *ngIf="learnerModel" class="learner-cell">
      <div
        class="learner-cell__avatar"
        [style.background-image]="'url(' + userAvatar + ')'"
      ></div>
      <div
        class="learner-cell__infos learner-cell-infos"
        title="{{ learnerModel.name }}"
      >
        <div class="learner-cell-infos__name">{{ learnerModel.name }}</div>
        <div class="learner-cell-infos__email">{{ learnerModel.email }}</div>
      </div>
      <a class="learner-cell__hiddenLink" routerLink="{{ detailLink }}"></a>
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class LearnerCellRendererComponent implements ICellRendererAngularComp {
  learnerModel: ApprovalLearnerModel;
  detailLink: string;
  userAvatar: string = AppConstant.defaultAvatar;
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
    this.learnerModel = param.value;
    this.detailLink = `/employee/detail/${this.learnerModel.id}`;
    if (this.learnerModel && this.learnerModel.avatar) {
      this.userAvatar = this.learnerModel.avatar;
    }
  }
}
