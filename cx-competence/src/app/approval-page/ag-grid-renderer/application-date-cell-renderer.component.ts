import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { DateTimeUtil } from 'app-utilities/date-time-utils';
import { AppConstant } from 'app/shared/app.constant';

@Component({
  selector: 'application-date-cell',
  template: `
    <div class="application-date-cell" [title]="getDateString(applicationDate)">
      {{ getDateString(applicationDate) }}
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class ApplicationDateCellRendererComponent
  implements ICellRendererAngularComp {
  applicationDate: string;

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

  getDateString(date: string): string {
    return DateTimeUtil.toDateString(date, AppConstant.backendDateTimeFormat);
  }

  private processParam(param: any): void {
    if (!param || !param.value) {
      return;
    }
    this.applicationDate = param.value;
  }
}
