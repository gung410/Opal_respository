import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { ICellRendererParams } from '@ag-grid-community/core';
import { Component } from '@angular/core';

@Component({
  selector: 'text-overflow-truncated',
  templateUrl: './text-overflow-truncated.component.html',
  styleUrls: ['./text-overflow-truncated.component.scss'],
})
export class TextOverflowTruncatedComponent
  implements ICellRendererAngularComp {
  params: any;
  textContent: string;

  constructor() {}

  agInit(params: ICellRendererParams): void {
    this.processParam(params);
  }

  refresh(params: any): boolean {
    this.processParam(params);

    return this.textContent && this.textContent.length > 0;
  }

  processParam(params: any): void {
    this.params = params;
    this.textContent = params.value;
  }
}
