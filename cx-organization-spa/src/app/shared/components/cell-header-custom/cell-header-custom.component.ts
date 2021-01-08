import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { boxAlternativeIcon } from 'app/shared/constants/check-box-icon-ag-grid.enum';

@Component({
  selector: 'cell-header-custom',
  templateUrl: 'cell-header-custom.component.html',
  styleUrls: ['cell-header-custom.component.scss']
})
export class CellHeaderCustomComponent implements ICellRendererAngularComp {
  checkBoxIcon: string = 'box-icon';
  isShowArrowUpIcon: boolean = false;
  isShowArrowDownIcon: boolean = true;
  isShowCloseIcon: boolean = false;
  params: any;
  agInit(params: any): void {
    this.params = params;
  }

  onHeaderCheckBoxChange(): void {
    const displayedRowCount = this.params.api.getDisplayedRowCount();
    const selectedRowCount = this.params.api.getSelectedRows().length;
    if (selectedRowCount === 0) {
      this.params.api.selectAll();
      this.params.menuIcon = boxAlternativeIcon.CheckBoxIcon;
    } else if (displayedRowCount >= selectedRowCount) {
      this.params.api.deselectAll();
      this.params.menuIcon = boxAlternativeIcon.BoxIcon;
    }
  }

  refresh(params?: any): boolean {
    return true;
  }

  onSortRequested(order: any, event: any): any {
    this.params.setSort(order, event.shiftKey);
    switch (order) {
      case 'asc':
        this.isShowArrowUpIcon = false;
        this.isShowArrowDownIcon = true;
        break;
      case 'desc':
        this.isShowArrowDownIcon = false;
        this.isShowCloseIcon = true;
        break;
      default:
        this.isShowCloseIcon = false;
        this.isShowArrowUpIcon = true;
        break;
    }
  }
}
