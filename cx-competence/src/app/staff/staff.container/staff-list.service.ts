import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { AppConstant } from 'app/shared/app.constant';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { DepartmentQueryModel } from './staff-list/models/filter-param.model';

@Injectable()
export class StaffListService {
  searchValueSubject = new Subject<string>();
  searchingBehaviorSubject = new BehaviorSubject<string>('');
  resetSearchValueSubject = new Subject<boolean>();
  constructor(private httpHelpers: HttpHelpers) {}

  public getHierarchicalDepartments(
    departmentId: number,
    includeParent: boolean,
    includeChildren: boolean,
    departmentName?: string
  ): Observable<any> {
    return this.httpHelpers.get(
      `${AppConstant.api.organization}/departments/${departmentId}/hierarchydepartmentidentifiers`,
      {
        includeParent: includeParent.toString(),
        includeChildren: includeChildren.toString(),
        departmentName: departmentName ? departmentName : '',
      }
    );
  }

  public getHierarchicalDepartmentsByQuery(
    departmentId: number,
    departmentQuery: DepartmentQueryModel
  ): any {
    if (departmentQuery.searchText !== undefined) {
      departmentQuery.maxChildrenLevel = undefined;

      return this.httpHelpers.get(
        `${AppConstant.api.organization}/departments/${departmentId}/hierarchydepartmentidentifiers`,
        departmentQuery
      );
    }

    return this.httpHelpers.get(
      `${AppConstant.api.organization}/departments/${departmentId}/hierarchydepartmentidentifiers`,
      departmentQuery
    );
  }
}
