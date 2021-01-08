import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';

@Component({
  selector: 'name-cell',
  template: `<span
    ><a
      [routerLink]="detailLink"
      [queryParams]="queryParams"
      style="font-size: 14px; text-decoration: none"
      >{{ name || 'N/A' }}</a
    ></span
  >`,
})
export class LPLNameRendererComponent implements ICellRendererAngularComp {
  public params: any;
  public name: any;
  public queryParams: any;
  public detailLink: string;
  constructor() {}

  // called on init
  public agInit(params: any): void {
    this.params = params;
    if (params && params.value) {
      this.name = this.params.value.display;
      this.detailLink = this.params.value.route;
      this.queryParams = this.params.value.params;
    }
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    this.params = params;
    this.name = this.params.value.display;
    this.detailLink = this.params.value.route;

    return true;
  }
}
