import { BaseBackendService, CommonFacadeService, Utils } from '@opal20/infrastructure';
import {
  DepartmentInfoModel,
  IDepartmentInfoModel,
  IDepartmentInfoResult,
  OrganizationUnitLevelEnum
} from '../models/department-info.model';
import { DepartmentLevelModel, IDepartmentLevelModel } from '../models/department-level.model';
import { catchError, map } from 'rxjs/operators';
import { combineLatest, of } from 'rxjs';

import { DepartmentIdEnum } from '../models/department-id-enum';
import { Injectable } from '@angular/core';

@Injectable()
export class OrganizationApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.authConfig.organizationUrl + '';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getOrganizationalUnits(
    searchText: string = '',
    parentDepartmentExtId?: string,
    departmentTypeExtIds?: OrganizationUnitLevelEnum[],
    externallyMastered?: boolean,
    parentDepartmentIds?: number[],
    includeDepartmentType?: boolean,
    isByPassFilter: boolean = false,
    pageIndex: number | null = 0,
    pageSize: number | null = 10,
    showSpinner?: boolean
  ): Promise<DepartmentInfoModel[]> {
    return this.get<IDepartmentInfoModel[]>(
      '/organizationalunits',
      {
        searchText,
        parentDepartmentIds,
        parentDepartmentExtId,
        departmentTypeExtIds,
        includeDepartmentType,
        pageIndex,
        pageSize,
        externallyMastered,
        isByPassFilter
      },
      showSpinner
    )
      .pipe(map(result => Utils.orderBy(result.map(_ => new DepartmentInfoModel(_, externallyMastered)), x => x.departmentName)))
      .toPromise();
  }

  public getOrganizationalUnitsByIds(
    organizationalUnitIds: number[],
    getIsPartnering: boolean,
    showSpinner: boolean = true
  ): Promise<DepartmentInfoModel[]> {
    if (organizationalUnitIds.length === 0) {
      return Promise.resolve([]);
    }

    const getDepartmentByIdsFn = () =>
      this.get<IDepartmentInfoModel[]>(`/organizationalunits`, { organizationalUnitIds }, showSpinner).pipe(
        map(result => (result ? Utils.orderBy(result.map(_ => new DepartmentInfoModel(_)), x => x.departmentName) : []))
      );
    const getPartneringDepartmentByIdsFn = () =>
      this.get<IDepartmentInfoResult>(
        `/departments/${DepartmentIdEnum.PartnerDepartmentId}/hierarchydepartmentidentifiers/v2`,
        {
          includeChildren: true,
          departmentEntityStatuses: ['Active'],
          pageSize: 0,
          departmentIds: organizationalUnitIds
        },
        showSpinner
      );
    if (!getIsPartnering) {
      return getDepartmentByIdsFn().toPromise();
    }
    return combineLatest(getDepartmentByIdsFn(), getPartneringDepartmentByIdsFn())
      .pipe(
        map(([allDepartments, partnering]) => {
          const isPartneringDepartmentsDic = Utils.toDictionary(partnering.items.map(_ => new DepartmentInfoModel(_, true)), p => p.id);
          allDepartments.forEach(p => (p.isPartnering = isPartneringDepartmentsDic && isPartneringDepartmentsDic[p.id] != null));
          return allDepartments;
        })
      )
      .toPromise();
  }

  public getListOrganizationalLevels(showSpinner: boolean = true): Promise<DepartmentLevelModel[]> {
    // TODO: We are doing this because of the bug can not get data. Will remove it when this bug is fixed
    const dataObs = of(<IDepartmentLevelModel[]>getListOrganizationalLevelsData);
    return dataObs
      .pipe(
        map(
          (result: IDepartmentLevelModel[]) =>
            <DepartmentLevelModel[]>(result ? Utils.orderBy(result.map(_ => new DepartmentLevelModel(_)), x => x.departmentLevelName) : [])
        )
      )
      .toPromise();

    return this.get<IDepartmentLevelModel[]>(`/departmenttypes`, { archetypeEnums: 'OrganizationalUnitType' }, showSpinner)
      .pipe(
        catchError(err => dataObs),
        map(
          (result: IDepartmentLevelModel[]) =>
            <DepartmentLevelModel[]>(result ? Utils.orderBy(result.map(_ => new DepartmentLevelModel(_)), x => x.departmentLevelName) : [])
        )
      )
      .toPromise();
  }
}

const getListOrganizationalLevelsData = [
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'Data Owner' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'dataowner', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 10 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'Wing' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'wing', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 11 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'Division' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'division', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 12 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'Branch/Zone' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'branch', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 13 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'Cluster' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'cluster', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 14 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'School' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'school', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 15 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      { id: 2, languageCode: 'en-US', fields: [{ name: 'Name', localizedText: 'Ministry' }, { name: 'Description', localizedText: '' }] }
    ],
    identity: { extId: 'ministry', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 30 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  },
  {
    localizedData: [
      {
        id: 2,
        languageCode: 'en-US',
        fields: [{ name: 'Name', localizedText: 'Organization Unit' }, { name: 'Description', localizedText: '' }]
      }
    ],
    identity: { extId: 'OrganizationUnit', ownerId: 3001, customerId: 0, archetype: 'OrganizationalUnitType', id: 49 },
    entityStatus: {
      externallyMastered: false,
      lastUpdated: '0001-01-01T00:00:00Z',
      lastUpdatedBy: 0,
      statusId: 'Active',
      statusReasonId: 'Unknown',
      deleted: false
    }
  }
];
