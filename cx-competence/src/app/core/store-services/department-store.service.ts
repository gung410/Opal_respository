import { Injectable } from '@angular/core';
import { Department } from 'app-models/department-model';
import { DepartmentType } from 'app-models/department-type.model';
import { MyTopHierarchyDepartment } from 'app-models/my-top-hierarchy-department-model';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable, of } from 'rxjs';
import 'rxjs/add/operator/map';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DepartmentStoreService {
  private baseUrl: string = `${AppConstant.api.organization}`;
  private responseCache: Map<string, any> = new Map();

  constructor(private httpHelper: HttpHelpers) {}

  getDepartmentTypesByDepartmentId(
    departmentId: number
  ): Observable<DepartmentType[]> {
    const key = `getDepartmentTypesByDepartmentId:${departmentId}`;
    const departmentTypesFromCache = this.responseCache.get(key);
    if (departmentTypesFromCache) {
      return of(departmentTypesFromCache);
    }

    return this.httpHelper
      .get<DepartmentType[]>(`${this.baseUrl}/departmenttypes`, {
        departmentids: [departmentId],
      })
      .pipe(
        map((response) => {
          this.responseCache.set(key, response);

          return response;
        })
      );
  }

  getDepartmentById(departmentId: number): Observable<Department> {
    const key = `getDepartmentById:${departmentId}`;
    const departmentFromCache = this.responseCache.get(key);
    if (departmentFromCache) {
      return of(departmentFromCache);
    }

    return this.httpHelper
      .get<Department[]>(
        `${this.baseUrl}/departments/${departmentId}/hierarchydepartmentidentifiers`,
        {
          includeParent: false,
        }
      )
      .pipe(
        map((response) => {
          this.responseCache.set(key, response[0]);

          return response[0];
        })
      );
  }

  /**
   * Gets the top accessible department of the current logged-in user.
   */
  getMyTopDepartment(): Observable<MyTopHierarchyDepartment> {
    const key = `getMyTopDepartment:`;
    if (this.responseCache.has(key)) {
      return this.responseCache.get(key);
    }

    return this.httpHelper
      .get<MyTopHierarchyDepartment>(`${this.baseUrl}/mytophierarchydepartment`)
      .pipe(
        map((response) => {
          this.responseCache.set(key, response);

          return response;
        })
      );
  }

  /**
   * Get all organizational unit types in the system. e.g: DataOwner, Wing, Division, Branch, School, etc.
   */
  getAllOrganizationalUnitTypes(): Observable<DepartmentType[]> {
    const key = `getAllOrganizationalUnitTypes`;
    const departmentTypesFromCache = this.responseCache.get(key);
    if (departmentTypesFromCache) {
      return of(departmentTypesFromCache);
    }

    return this.httpHelper
      .get<DepartmentType[]>(`${this.baseUrl}/departmenttypes`, {
        archetypeEnums: ['OrganizationalUnitType'],
      })
      .pipe(
        map((response) => {
          this.responseCache.set(key, response);

          return response;
        })
      );
  }

  /**
   * Get ancestor departments.
   * @param departmentId The department identifier of the current node.
   * @param includeDepartmentType True if you want to include the department types in the response.
   */
  getAncestorDepartmentsOfDepartment(
    departmentId: number,
    includeDepartmentType?: boolean
  ): Observable<Department[]> {
    const key = `getAncestorDepartmentsOfDepartment:${departmentId}:${includeDepartmentType}`;
    const departmentsFromCache = this.responseCache.get(key);
    if (departmentsFromCache) {
      return of(departmentsFromCache);
    }

    return this.httpHelper
      .get<Department[]>(
        `${this.baseUrl}/departments/${departmentId}/hierarchydepartmentidentifiers`,
        {
          includeParent: true,
          includeChildren: false,
          includeDepartmentType,
        }
      )
      .pipe(
        map((response) => {
          this.responseCache.set(key, response);

          return response;
        })
      );
  }
}
