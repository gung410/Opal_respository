import { ICellRendererAngularComp } from '@ag-grid-community/angular';
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'due-date-cell',
  template: `<span>{{ dueDate | date: 'dd/MM/yyyy' }}</span>`,
})
export class SLDueDateRendererComponent implements ICellRendererAngularComp {
  public dueDate: any;
  private params: any;
  constructor(private router: Router) {}

  // called on init
  public agInit(params: any): void {
    this.params = params;
    this.dueDate = this.params.value;
  }

  // called when the cell is refreshed
  public refresh(params: any): boolean {
    this.params = params;
    this.dueDate = this.params.value;
    return true;
  }
}
