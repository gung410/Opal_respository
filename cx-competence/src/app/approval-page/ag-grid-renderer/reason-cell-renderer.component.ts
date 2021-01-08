import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';

@Component({
  selector: 'withdrawal-reason-cell',
  template: `
    <div class="withdrawal-reason-cell" [title]="reason ? reason : 'N/A'">
      {{ reason ? reason : 'N/A' }}
    </div>
  `,
  styleUrls: ['../approval-page.component.scss'],
})
export class ReasonCellRendererComponent implements ICellRendererAngularComp {
  reason: string;

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

  private processParam(param: any): void {
    if (!param || !param.value) {
      return;
    }
    this.reason = param.value;
  }
}
