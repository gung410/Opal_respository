import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-cell-dropdown-menu',
  templateUrl: './cell-dropdown-menu.component.html'
})
export class CellDropdownMenuComponent implements ICellRendererAngularComp {
  params: any;
  label: string;

  agInit(params: any): void {
    this.params = params;
    this.label = this.params.label || null;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onClick(action: string): void {
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        action,
        data: this.params.node.data
        // ...something
      };
      this.params.onClick(params);
    }
  }
}
