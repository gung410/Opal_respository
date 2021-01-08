import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { AppConstant } from 'app/shared/app.constant';
import { ApprovalGroupTypeEnum } from 'app/shared/constants/approval-group.enum';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { FilterParamModel } from '../models/filter-param.model';
import { ObjectData } from '../models/filter.model';

@Injectable({
  providedIn: 'root',
})
export class StaffFilterService {
  private baseUrl: string = `${AppConstant.api.competence}`;
  private getFilterOptionsUrl: string = `${this.baseUrl}/idp/employeelist`;
  private responseCache: Map<string, any> = new Map();

  constructor(private httpHelper: HttpHelpers) {}

  getApprovingOfficerFilterOptions(): Observable<ObjectData[]> {
    const key = `getApprovingOfficerFilterOptions`;
    const approvingOfficerFilterOptionsFromCache = this.responseCache.get(key);
    if (approvingOfficerFilterOptionsFromCache) {
      return of(approvingOfficerFilterOptionsFromCache);
    }

    const filterParams: FilterParamModel = new FilterParamModel({
      pageSize: 1,
      pageIndex: 1,
      includeFilterOptions: {
        approvalGroupInfo: true,
      },
    });

    return this.httpHelper
      .post<ObjectData[]>(this.getFilterOptionsUrl, filterParams)
      .pipe(
        map((response: any) => {
          if (
            response &&
            response.filterOptions &&
            response.filterOptions.approvalGroupInfos
          ) {
            const primaryApprovalGroupType = ApprovalGroupTypeEnum.PrimaryApprovalGroup.toString();
            const primaryApprovingOfficerSuffix = 'Primary';
            const alternateApprovingOfficerSuffix = 'Alternate';
            const approvalGroupFilterOptions = response.filterOptions.approvalGroupInfos.map(
              (item) => {
                return {
                  value: item.identity.id,
                  text: `${item.name} (${
                    item.type === primaryApprovalGroupType
                      ? primaryApprovingOfficerSuffix
                      : alternateApprovingOfficerSuffix
                  })`,
                };
              }
            );
            this.responseCache.set(key, approvalGroupFilterOptions);

            return approvalGroupFilterOptions;
          } else {
            this.responseCache.set(key, []);

            return of([]);
          }
        })
      );
  }

  getUserGroupFilterOptions(): Observable<ObjectData[]> {
    const key = `getUserGroupFilterOptions`;
    const userGroupFilterOptionsFromCache = this.responseCache.get(key);
    if (userGroupFilterOptionsFromCache) {
      return of(userGroupFilterOptionsFromCache);
    }

    const filterParams: FilterParamModel = new FilterParamModel({
      pageSize: 1,
      pageIndex: 1,
      includeFilterOptions: {
        userGroupInfo: true,
      },
    });

    return this.httpHelper
      .post<ObjectData[]>(this.getFilterOptionsUrl, filterParams)
      .pipe(
        map((response: any) => {
          if (
            response &&
            response.filterOptions &&
            response.filterOptions.userGroupInfos
          ) {
            const userGroupFilterOptions = response.filterOptions.userGroupInfos.map(
              (item) => {
                return {
                  value: item.identity.id,
                  text: item.name,
                };
              }
            );
            this.responseCache.set(key, userGroupFilterOptions);

            return userGroupFilterOptions;
          } else {
            this.responseCache.set(key, []);

            return of([]);
          }
        })
      );
  }
}
