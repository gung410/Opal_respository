import { Component, OnInit } from '@angular/core';
import { departments } from '../../data/departments.data';
import { CxBreadCrumbItem } from 'projects/cx-angular-common/src/lib/components/cx-breadcrumb-simple/model/breadcrumb.model';
import { Identity } from 'projects/cx-angular-common/src/lib/models/identity.model';
import { ActionsModel } from 'projects/cx-angular-common/src';
@Component({
  selector: 'cx-breadcrumb-doc',
  templateUrl: './cx-breadcrumb-doc.component.html',
  styleUrls: ['./cx-breadcrumb-doc.component.scss']
})
export class CxBreadcrumbDocComponent implements OnInit {
  constructor() {}
  public currentDepartmentCrumb: any[] = [];
  public departments = departments;
  public separatorSymbol: string;
  public listBreadCrumbItem: CxBreadCrumbItem[] = [];
  public listBreadCrumbItemSimple: CxBreadCrumbItem[] = [];
  ngOnInit() {
    this.buildCurrentDepartmentCrumbSimple();
    this.currentDepartmentCrumb = this.buildCurrentDepartmentCrumb(
      this.currentDepartmentCrumb,
      this.departments,
      13734,
      14479
    );
    this.separatorSymbol = '>';
  }

  onCxBreadCrumbClick($event: ActionsModel) {
  }

  private buildCurrentDepartmentCrumb(
    currentDepartmentCrumb: any[],
    hierarchicalUserDepartments: any[],
    userDepartmentId: number,
    currentDepartmentId: number
  ): any[] {
    if (!currentDepartmentCrumb) {
      currentDepartmentCrumb = [];
    }
    for (const department of hierarchicalUserDepartments) {
      if (department.identity.id !== currentDepartmentId) {
        continue;
      }
      currentDepartmentCrumb.push(department);
      if (department.identity.id === userDepartmentId) {
        continue;
      }
      currentDepartmentId = department.parentDepartmentId;
      return this.buildCurrentDepartmentCrumb(
        currentDepartmentCrumb,
        hierarchicalUserDepartments,
        userDepartmentId,
        currentDepartmentId
      );
    }
    return currentDepartmentCrumb.reverse();
  }
  private buildCurrentDepartmentCrumbSimple() {
    this.listBreadCrumbItemSimple.push(
      new CxBreadCrumbItem( {name: 'Organisational Development', identity: new Identity()})
    );
    this.listBreadCrumbItemSimple.push(
      new CxBreadCrumbItem( {name: 'Learning Plans', identity: new Identity()})
    );
    this.listBreadCrumbItemSimple.push(
      new CxBreadCrumbItem( {name: 'Learning Plan 001Learning Plans', identity: new Identity()})
    );
  }
}
