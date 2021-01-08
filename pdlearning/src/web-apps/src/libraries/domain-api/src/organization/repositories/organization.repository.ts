import { DepartmentInfoModel, OrganizationUnitLevelEnum } from '../models/department-info.model';
import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { DepartmentApiService } from '../services/department-api.service';
import { DepartmentLevelModel } from '../models/department-level.model';
import { IDepartmentInfoRequest } from '../dtos/department-info-request';
import { Injectable } from '@angular/core';
import { OrganizationApiService } from '../services/organization-api.service';
import { OrganizationRepositoryContext } from '../organization-repository-context';

@Injectable()
export class OrganizationRepository extends BaseRepository<OrganizationRepositoryContext> {
  constructor(
    context: OrganizationRepositoryContext,
    private departmentApiSvc: DepartmentApiService,
    private organizationApiSvc: OrganizationApiService
  ) {
    super(context);
  }

  public loadDepartmentInfoList(request: IDepartmentInfoRequest, showSpinner: boolean = true): Observable<DepartmentInfoModel[]> {
    return this.processUpsertData(
      this.context.departmentsSubject,
      implicitLoad => this.departmentApiSvc.getListDepartmentInfo(request, !implicitLoad && showSpinner),
      'loadDepartmentInfoList',
      [
        request.departmentId,
        request.departmentTypeIds,
        request.includeChildren,
        request.includeDepartmentType,
        request.getParentDepartmentId,
        request.pageIndex,
        request.pageSize
      ],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id,
      null,
      null,
      null,
      DepartmentInfoModel.optionalProps
    );
  }

  public loadOrganizationalUnits(
    searchText: string = '',
    departmentTypeExtIds?: OrganizationUnitLevelEnum[],
    parentDepartmentIds?: number[],
    includeDepartmentType?: boolean,
    isPartnering?: boolean,
    parentDepartmentExtId?: string,
    isByPassFilter: boolean = false,
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner: boolean = true
  ): Observable<DepartmentInfoModel[]> {
    return this.processUpsertData(
      this.context.departmentsSubject,
      implicitLoad =>
        from(
          this.organizationApiSvc.getOrganizationalUnits(
            searchText,
            parentDepartmentExtId ? parentDepartmentExtId : null,
            departmentTypeExtIds ? departmentTypeExtIds : null,
            isPartnering ? isPartnering : null,
            parentDepartmentIds ? parentDepartmentIds : null,
            includeDepartmentType ? includeDepartmentType : null,
            isByPassFilter,
            skipCount,
            maxResultCount,
            !implicitLoad && showSpinner
          )
        ),
      'loadOrganizationalUnits',
      [
        departmentTypeExtIds,
        parentDepartmentIds,
        includeDepartmentType,
        isPartnering,
        parentDepartmentExtId,
        isByPassFilter,
        skipCount,
        maxResultCount
      ],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id,
      null,
      null,
      null,
      DepartmentInfoModel.optionalProps
    );
  }

  public loadOrganizationalUnitsByIds(
    organizationalUnitIds: number[],
    getIsPartnering: boolean = false,
    showSpinner: boolean = true
  ): Observable<DepartmentInfoModel[]> {
    return this.processUpsertData(
      this.context.departmentsSubject,
      implicitLoad =>
        from(this.organizationApiSvc.getOrganizationalUnitsByIds(organizationalUnitIds, getIsPartnering, !implicitLoad && showSpinner)),
      'loadOrganizationalUnitsByIds',
      [organizationalUnitIds, getIsPartnering],
      'implicitReload',
      (repoData, apiResult) => {
        return apiResult.map(_ => repoData[_.id]).filter(p => p != null);
      },
      apiResult => apiResult,
      x => x.id,
      true,
      null,
      null,
      DepartmentInfoModel.optionalProps
    );
  }

  public loadOrganizationalLevels(showSpinner: boolean = true): Observable<DepartmentLevelModel[]> {
    return this.processUpsertData(
      this.context.departmentLevelsSubject,
      implicitLoad => from(this.organizationApiSvc.getListOrganizationalLevels(!implicitLoad && showSpinner)),
      'loadOrganizationalLevels',
      null,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id,
      null,
      null,
      null
    );
  }
}
