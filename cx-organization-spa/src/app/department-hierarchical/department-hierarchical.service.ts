import 'rxjs/add/operator/map';

import { Injectable } from '@angular/core';
import { DepartmentType } from 'app-models/department-type.model';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs/Observable';

import { Utils } from 'app-utilities/utils';
import { Department, OrganisationUnitType } from './models/department.model';
import { DepartmentQueryModel } from './models/filter-params.model';
import { OrganizationUnitLevelEnum } from './models/organization-unit-type.enum';

@Injectable()
export class DepartmentHierarchicalService {
  constructor(private httpHelper: HttpHelpers) {}

  getHierarchy(
    currentDepartmentId: number,
    departmentQueryModel: DepartmentQueryModel
  ): Observable<Department[]> {
    return this.httpHelper.get<Department[]>(
      `${AppConstant.api.organization}/departments/${currentDepartmentId}/hierarchydepartmentidentifiers`,
      departmentQueryModel
    );
  }

  getDepartmentTypes(archetype: string): Observable<DepartmentType[]> {
    return this.httpHelper.get<DepartmentType[]>(
      `${AppConstant.api.organization}/departmenttypes`,
      {
        archetypeEnums: archetype
      }
    );
  }

  getDepartmentInfo(department: Department): Observable<Department> {
    return this.httpHelper.get<Department>(
      `${AppConstant.api.organization}/organizationalunits/${department.identity.id}`
    );
  }

  getTypeOfOrganisation(): Observable<OrganisationUnitType[]> {
    return this.httpHelper.get<OrganisationUnitType[]>(
      `${AppConstant.api.learningCatalog}/catalogentries/explorer/OU-TYPES`
    );
  }

  updateOrganizationUnit(department: Department): Observable<Department> {
    return this.httpHelper
      .put<Department>(
        `${AppConstant.api.organization}/organizationalunits/${department.identity.id}`,
        department
      )
      .pipe()
      .map((response) => new Department(response));
  }

  addNewOrganizationUnit(department: Department): Observable<Department> {
    const url = `${AppConstant.api.organization}/organizationalunits`;

    return this.httpHelper.post<Department>(url, department, null, {
      avoidIntercepterCatchError: true
    });
  }

  getOrganizationalUnitsByIds(
    searchText: string = '',
    organizationalUnitIds?: number[],
    organizationalUnitExtIds?: string[],
    parentDepartmentExtId?: string,
    departmentTypeExtIds?: OrganizationUnitLevelEnum[],
    externallyMastered?: boolean,
    parentDepartmentIds?: number[],
    includeDepartmentType?: boolean,
    isByPassFilter: boolean = false,
    pageIndex: number | null = 0,
    pageSize: number | null = 50
  ): Observable<Department[]> {
    const url = `${AppConstant.api.organization}/organizationalunits`;

    return this.httpHelper.get<Department[]>(url, {
      searchText,
      organizationalUnitIds,
      organizationalUnitExtIds,
      parentDepartmentIds,
      parentDepartmentExtId,
      departmentTypeExtIds,
      includeDepartmentType,
      pageIndex,
      pageSize,
      externallyMastered,
      isByPassFilter
    });
  }

  deleteHierarchy(department: any): Observable<any> {
    const departmentIsDeactived = {
      identities: [department.object.identity],
      deactivateIfContainingUser: true,
      deactivateIfContainingChildDepartment: true
    };

    return this.httpHelper.put<any>(
      `${AppConstant.api.organization}/deactivate_departments`,
      departmentIsDeactived
    );
  }
}
