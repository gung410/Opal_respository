import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import {
  departmentsData,
  departmentsData1,
  departmentsData2
} from './mock-data/container.data';
import { employeesData, employeesData1 } from './mock-data/item.data';
import { Md5 } from 'md5-typescript';
import { CxItemTableHeaderModel, CxTableContainersComponent, CxTableComponent, CxColumnSortType } from 'projects/cx-angular-common/src';
@Component({
  selector: 'cx-table-containers-doc',
  templateUrl: './cx-table-containers-doc.component.html',
  styleUrls: ['./cx-table-containers-doc.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class CxTableContainersDocComponent<T> implements OnInit {
  public currentSortType: CxColumnSortType.ASCENDING;
  public currentFieldSort: 'DepartmentName;FirstName';
  public headers: CxItemTableHeaderModel[] = [
    {
      text: 'Name',
      fieldSort: 'DepartmentName;FirstName',
      sortType: CxColumnSortType.UNSORTED
    },
    {
      text: 'Roles',
      sortType: CxColumnSortType.UNSORTED
    },
    {
      text: 'Status',
      fieldSort: '',
      sortType: CxColumnSortType.UNSORTED
    }
  ];
  public employeesRoleMap = {};
  public departments = departmentsData;
  public itemsData = [];
  public employeesData = employeesData;
  showActionsButtonInLastRow = false;
  @ViewChild(CxTableContainersComponent)
  employeesDepartmentTable: CxTableContainersComponent<any, any>;
  @ViewChild(CxTableComponent) employeesTable: CxTableComponent<any>;
  public itemActionPlacement = (index: number) => {
    console.log('calculate placement');
    return this.itemsData.length - 1 === index ? 'left-bottom': 'left-top';
  }
  constructor() { }

  ngOnInit() {
    this.itemsData = employeesData1.items;
  }

  onMoveEmployee(item) { }

  onRemoveEmployeesInTable(employees: any[]) {
    setTimeout(() => {
      this.employeesDepartmentTable.executeRemoveItems(employees);
    }, 1000);
  }

  onCustomActionClicked(clickedEmployee) {
    setTimeout(() => {
      this.employeesRoleMap[clickedEmployee.employeeIdentity.id] = 'Test value';
    }, 1000);
  }

  public getAvatarFromEmail(
    email: string,
    imageSize: number = 80,
    gavatarTypeD: string = 'mm'
  ): string {
    let imageURL = '';
    const gravataBaseUrl = 'http://www.gravatar.com';
    const hash = Md5.init(email.toLowerCase()).toLowerCase();
    imageURL = `${gravataBaseUrl}/avatar/${hash}.jpg?s=${imageSize}&d=${gavatarTypeD}`;
    return imageURL;
  }

  public changeData() {
    this.departments = departmentsData1;
  }

  public changeAnotherData() {
    this.departments = departmentsData2;
    this.itemsData = employeesData1.items;
  }

  public removeDataContainer() {
    this.departments = [];
  }

  public removeDataItem() {
    this.itemsData = [];
  }

  public resetData() {
    this.departments = departmentsData1;
    this.itemsData = employeesData1.items;
  }

  public onSortTypeChange($event: { fieldSort: string; sortType: CxColumnSortType }) {
    console.log($event);
  }

  clearSelected() {
    this.employeesDepartmentTable.clearSelectedItemsMap();
  }
}
