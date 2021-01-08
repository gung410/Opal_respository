import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';

@Component({
  selector: 'code-cell',
  template: `<span class="cell-text-highlight"> {{ name }}</span>`,
  styles: [
    `
      .cell-text-highlight {
        color: #4285f4;
        cursor: pointer;
        font-size: 14px;
      }
      .cell-text-highlight:hover {
        text-decoration: none;
      }
    `,
  ],
})
export class CodeRendererComponent implements ICellRendererAngularComp {
  public params: any;
  public name: any;
  constructor() {}

  // called on init
  agInit(params: any): void {
    this.params = params;
    this.name = this.params.value;
  }

  // called when the cell is refreshed
  refresh(params: any): boolean {
    this.params = params;
    this.name = this.params.value.display;

    return true;
  }
}
