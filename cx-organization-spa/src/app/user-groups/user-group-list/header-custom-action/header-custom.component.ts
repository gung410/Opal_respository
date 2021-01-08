import { Component } from '@angular/core';
import { boxAlternativeIcon } from 'app/shared/constants/check-box-icon-ag-grid.enum';

@Component({
  selector: 'header-custom',
  templateUrl: 'header-custom.component.html',
  styleUrls: ['header-custom.component.scss']
})
export class HeaderCustomComponent {
  checkBoxIcon: string = 'box-icon';
  isShowArrowUpIcon: boolean = false;
  isShowArrowDownIcon: boolean = true;
  isShowCloseIcon: boolean = false;
  params: any;
  agInit(params: any): void {
    this.params = params;

    // params.column.addEventListener('sortChanged', this.onSortChanged.bind(this));
    //this.onSortChanged();
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

  // onSortChanged(): void {
  //   const abc = this.params.column.isSortAscending();
  //   const cbd = this.params.column.isSortDescending();

  //   if (this.params.column.isSortAscending()) {
  //     this.ascSort = true;
  //   } else if (this.params.column.isSortDescending()) {
  //     this.descSort = true;
  //   } else {
  //     this.noSort = true;
  //   }
  // }

  onSortRequested(order: any, event: any): any {
    this.params.setSort(order, event.shiftKey);
    if (order === 'asc') {
      this.isShowArrowUpIcon = false;
      this.isShowArrowDownIcon = true;
    } else if (order === 'desc') {
      this.isShowArrowDownIcon = false;
      this.isShowCloseIcon = true;
    } else {
      this.isShowCloseIcon = false;
      this.isShowArrowUpIcon = true;
    }
  }
}
