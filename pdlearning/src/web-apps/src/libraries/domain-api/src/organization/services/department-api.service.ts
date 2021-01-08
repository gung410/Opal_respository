import { BaseBackendService, CommonFacadeService, Utils } from '@opal20/infrastructure';
import { DepartmentInfoModel, IDepartmentInfoResult } from '../models/department-info.model';
import { map, switchMap } from 'rxjs/operators';

import { DepartmentIdEnum } from '../models/department-id-enum';
import { IDepartmentInfoRequest } from '../dtos/department-info-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class DepartmentApiService extends BaseBackendService {
  private isPartneringDepartmentsDic?: Dictionary<DepartmentInfoModel>;

  protected get apiUrl(): string {
    return AppGlobal.environment.authConfig.organizationUrl + '/departments';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getListDepartmentInfo(request: IDepartmentInfoRequest, showSpinner: boolean = true): Observable<DepartmentInfoModel[]> {
    const url = `/${request.departmentId}/hierarchydepartmentidentifiers/v2`;
    const loadDataFn = () => {
      return this.get<IDepartmentInfoResult>(
        url,
        {
          ...request,
          departmentEntityStatuses: ['Active'],
          pageSize: request.pageSize == null ? 0 : request.pageSize
        },
        showSpinner
      ).pipe(
        map(result =>
          Utils.orderBy(
            result.items.map(
              _ => new DepartmentInfoModel(_, this.isPartneringDepartmentsDic && this.isPartneringDepartmentsDic[_.identity.id] != null)
            ),
            x => x.departmentName
          )
        )
      );
    };

    if (this.isPartneringDepartmentsDic == null) {
      return this.get<IDepartmentInfoResult>(
        `/${DepartmentIdEnum.PartnerDepartmentId}/hierarchydepartmentidentifiers/v2`,
        {
          includeChildren: true,
          departmentEntityStatuses: ['Active'],
          pageSize: 0
        },
        showSpinner
      ).pipe(
        map(result => {
          this.isPartneringDepartmentsDic = Utils.toDictionary(result.items.map(_ => new DepartmentInfoModel(_, true)), p => p.id);
          return this.isPartneringDepartmentsDic;
        }),
        switchMap(p => loadDataFn())
      );
    } else {
      return loadDataFn();
    }
  }
}
