import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { AppConstant } from 'app/shared/app.constant';
import { ApprovalMassNominationModel } from '../models/class-registration.model';

@Component({
  selector: 'mass-nomination-cell',
  template: `
    <div *ngIf="massNominationModel" class="mass-nomination-cell">
      <div class="mass-nomination-cell__avatar">
        <div
          *ngFor="let item of [1, 1, 1]"
          class="mass-nomination-cell-avatar"
          [style.background-image]="'url(' + defaultAvatar + ')'"
        ></div>
      </div>
      <div
        class="mass-nomination-cell__infos mass-nomination-cell-infos"
        title="{{ massNominationModel.name }}"
      >
        <div class="mass-nomination-cell-infos__title">
          {{ massNominationModel.name }}
        </div>
        <div
          *ngIf="massNominationModel.totalPendingLv1 > 0"
          class="mass-nomination-cell-infos__sub-title"
        >
          {{ massNominationModel.totalPendingLv1 }}
          {{
            massNominationModel.totalPendingLv1 > 1
              ? 'learners are'
              : 'learner is'
          }}
          pending approval
        </div>
        <div
          *ngIf="massNominationModel.totalPendingLv1 === 0"
          class="mass-nomination-cell-infos__sub-title"
        >
          None learner pending approval
        </div>
      </div>
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class MassNominationCellRendererComponent
  implements ICellRendererAngularComp {
  massNominationModel: ApprovalMassNominationModel;
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
    this.massNominationModel = param.value;
  }
}
