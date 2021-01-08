import { EventEmitter, Output, ViewChild } from '@angular/core';
import { Component, OnInit, ViewEncapsulation, Input } from '@angular/core';
import { CxTreeButtonCondition } from '../cx-tree/models/cx-tree-button-condition.model';
import { CxObjectRoute } from '../cx-tree/models/cx-object-route.model';
import { DepartmentHierarchiesModel } from '../../models/department-hierarchies.model';
import { CxTreeDropdownComponent } from '../cx-tree/cx-tree-dropdown/cx-tree-dropdown.component';
import {
  isEmpty,
  last
} from 'lodash';
@Component({
  selector: 'cx-universal-toolbar',
  templateUrl: './cx-universal-toolbar.component.html',
  styleUrls: ['./cx-universal-toolbar.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CxUniversalToolbarComponent implements OnInit {

  @Input() departmentModel: DepartmentHierarchiesModel;
  @Input() breadCrumbNavigation: any[];
  @Input() hideDepartmentHierarchyFilter: boolean;
  @Input() buttonCondition: CxTreeButtonCondition<any> = new CxTreeButtonCondition();
  @Output() expandChildDepartment = new EventEmitter<any>();
  @Output() searchDepartment = new EventEmitter<any>();
  @Output() selectDepartmentTree = new EventEmitter<any>();
  @Output() searchResultClick = new EventEmitter<any>();
  @Output() breadcrumbNavigationClick = new EventEmitter<any>();

  @ViewChild(CxTreeDropdownComponent)
  private cxTreeDropdown: CxTreeDropdownComponent<any>;
  constructor() { }

  ngOnInit() {
  }

  onExpandChildDepartment(departmentSelected: any): void {
     this.expandChildDepartment.emit(departmentSelected);
  }

  onSearchDepartment(searchKey: string): void {
    this.searchDepartment.emit(searchKey);
  }

  onSelectDepartmentTree(objectRoute: CxObjectRoute<any>): void {
    this.selectDepartmentTree.emit(objectRoute);
  }

  onClickSearchResult(department: DepartmentHierarchiesModel): void {
    this.searchResultClick.emit(department);
  }

  onClickBreadcrumbNavigation($event: {name: string, identity: {}}): void {
    this.breadcrumbNavigationClick.emit($event);
  }

  toggleDepartmentSelectorDropdown() {
    if (!this.cxTreeDropdown || this.hasSingleDepartment) {
      return;
    }

    this.cxTreeDropdown.toggleDropdown();
  }

  get currentDepartmentName(): string {
    if (isEmpty(this.breadCrumbNavigation)) {
      return;
    }
    return last(this.breadCrumbNavigation).name || 'N/A';
  }

  get hasSingleDepartment(): boolean {
    return !this.departmentModel || this.departmentModel.departments.length < 2;
  }

  get showDepartmentSelectorDropdown(): boolean {
    if (!this.cxTreeDropdown) {
      return;
    }

    return this.cxTreeDropdown.isShowDropdown();
  }
}
