import { Injectable } from '@angular/core';
import { ClassRunDTO, ClassRunModel } from 'app-models/classrun.model';
import {
  PDCatalogSearchDTO,
  PDCatalogSearchPayload,
  PDCatalogSearchResult,
} from 'app-models/pdcatalog/pdcatalog.dto';
import { PagingResponseModel } from 'app-models/user-management.model';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { HttpHelpers } from 'app-utilities/httpHelpers';
import { APIConstant } from 'app/shared/app.constant';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export enum SearchClassRunType {
  Owner = 'Owner',
  Learner = 'Learner',
  LearningManagement = 'LearningManagement',
  CancellationPending = 'CancellationPending',
  ReschedulePending = 'ReschedulePending',
  AllReschedule = 'AllReschedule',
  NotApprovedCancellation = 'NotApprovedCancellation',
  OrganisationDevelopment = 'OrganisationDevelopment',
  RegistrationApprover = 'RegistrationApprover',
}

@Injectable()
export class CourseFilterService {
  constructor(private httpHelpers: HttpHelpers) {}
  public filterCourses = (
    searchText?: string,
    pageIndex: number = 0,
    pageSize: number = 10
  ): Observable<CxSelectItemModel<PDCatalogSearchResult>[]> => {
    const filterPayload: PDCatalogSearchPayload = {
      page: pageIndex,
      limit: pageSize,
      searchText,
      searchFields: ['Title', 'Code'],
      useFuzzy: true,
      useSynonym: true,
      searchCriteria: {
        status: ['contains', 'published'],
        resourceType: ['contains', 'course'],
      },
    };

    return this.httpHelpers
      .post<PDCatalogSearchDTO>(
        APIConstant.PD_CATALOGUE_SEARCH,
        filterPayload,
        null,
        {
          avoidIntercepterCatchError: true,
        }
      )
      .pipe(
        map((response) => {
          if (response.total > 0) {
            return response.resources.map(
              (item) =>
                new CxSelectItemModel({
                  id: item.id,
                  dataObject: item,
                  primaryField: item.name,
                })
            );
          }

          return [];
        })
      );
  };

  public getClassRunsByCourse = (
    courseId?: string,
    pageIndex: number = 0,
    pageSize: number = 1000
  ): Observable<CxSelectItemModel<ClassRunModel>[]> => {
    const filterParam = {
      courseId,
      page: pageIndex,
      SkipCount: pageIndex * pageSize,
      searchType: SearchClassRunType.OrganisationDevelopment,
    };

    return this.httpHelpers
      .post<PagingResponseModel<ClassRunDTO>>(
        APIConstant.PD_CLASSRUN_GET_BY_COURSE_ID,
        filterParam,
        {
          avoidIntercepterCatchError: true,
        }
      )
      .pipe(map(AssignPDOHelper.mapPagedClassRunsToCxSelectItems));
  };
}
