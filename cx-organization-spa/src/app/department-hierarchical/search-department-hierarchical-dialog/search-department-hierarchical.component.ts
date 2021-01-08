import { Component, EventEmitter, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'search-department-hierarchical',
  templateUrl: './search-department-hierarchical.component.html',
  styleUrls: ['./search-department-hierarchical.component.scss']
})
export class SearchDepartmentHierarchicalComponent implements OnInit {
  searchDepartmentResult: any[];
  isSearchAction: boolean = true;
  dialogHeaderText: string;
  departmentPathMap: any = {};
  isNoSearchResult: boolean;
  cancel: EventEmitter<any> = new EventEmitter<any>();
  clickItemResult: EventEmitter<any> = new EventEmitter<any>();
  resultMatch: string;

  constructor(private translateService: TranslateService) {}

  ngOnInit(): void {
    this.resultMatch = this.translateService.instant(
      this.isSearchAction
        ? `Department_Page.Department_Search.ResultMatch`
        : `Department_Page.Department_Filter.ResultMatch`,
      { resultLength: this.searchDepartmentResult.length }
    );
  }
  onCancel(): void {
    this.cancel.emit();
  }

  onClickItemResult(department: {}): void {
    this.clickItemResult.emit(department);
  }
}
