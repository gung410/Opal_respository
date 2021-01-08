import { BaseRepository, IFilter, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { BlockoutDateModel } from '../models/blockout-date.model';
import { BlockoutDateService } from '../services/blockout-date-api.service';
import { CourseRepositoryContext } from '../course-repository-context';
import { GetBlockoutDateDependenciesModel } from '../models/get-blockout-date-dependencies-model';
import { IConfirmBlockoutDateRequest } from './../dtos/confirm-blockout-date-request';
import { IGetBlockoutDateDependenciesRequest } from '../dtos/get-blockout-date-dependencies-request';
import { ISaveBlockoutDateRequest } from '../dtos/save-blockout-date-request';
import { Injectable } from '@angular/core';
import { SearchBlockoutDateResult } from '../dtos/search-blockout-date-result';

@Injectable()
export class BlockoutDateRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: BlockoutDateService) {
    super(context);
  }

  public loadBlockoutDate(id: string): Observable<BlockoutDateModel> {
    return this.processUpsertData(
      this.context.blockoutDateSubject,
      implicitLoad => from(this.apiSvc.getBlockoutDateById(id, !implicitLoad)),
      'loadBlockoutDateById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true
    );
  }

  public loadBlockoutDateDependencies(request: IGetBlockoutDateDependenciesRequest): Observable<GetBlockoutDateDependenciesModel> {
    return this.processUpsertData(
      this.context.blockoutDateSubject,
      implicitLoad => from(this.apiSvc.getBlockoutDateDependencies(request, !implicitLoad)),
      'loadBlockoutDateDependencies',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.matchedBlockoutDates = apiResult.matchedBlockoutDates.map(p => repoData[p.id]).filter(p => p != null);
        apiResult.matchedFromDateBlockoutDates = apiResult.matchedFromDateBlockoutDates.map(p => repoData[p.id]).filter(p => p != null);
        apiResult.matchedToDateBlockoutDates = apiResult.matchedToDateBlockoutDates.map(p => repoData[p.id]).filter(p => p != null);
        return apiResult;
      },
      apiResult => apiResult.matchedBlockoutDates,
      x => x.id,
      true
    );
  }

  public searchBlockoutDates(
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 10,
    coursePlanningCycleId: string
  ): Observable<SearchBlockoutDateResult> {
    return this.processUpsertData(
      this.context.blockoutDateSubject,
      implicitLoad =>
        from(this.apiSvc.searchBlockoutDates(searchText, filter, skipCount, maxResultCount, coursePlanningCycleId, !implicitLoad)),
      'searchBlockoutDates',
      [searchText, filter, skipCount, maxResultCount, coursePlanningCycleId],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true
    );
  }

  public saveBlockoutDate(request: ISaveBlockoutDateRequest): Observable<BlockoutDateModel> {
    return from(
      this.apiSvc.saveBlockoutDate(request).then(blockoutDate => {
        this.upsertData(this.context.blockoutDateSubject, [Utils.cloneDeep(blockoutDate)], item => item.id, true);
        this.processRefreshData('searchBlockoutDates');
        return blockoutDate;
      })
    );
  }

  public confirmBlockoutDate(request: IConfirmBlockoutDateRequest): Observable<void> {
    return from(
      this.apiSvc.confirmBlockoutDate(request).then(_ => {
        this.processRefreshData('searchBlockoutDates');
        this.processRefreshData('loadBlockoutDateById');
        this.processRefreshData('loadCoursePlanningCycleById', [request.CoursePlanningCycleId]);
        return _;
      })
    );
  }

  public deleteBlockoutDate(blockoutDateId: string): Promise<void> {
    return this.apiSvc.deleteBlockoutDate(blockoutDateId).then(_ => {
      this.processClearRefreshDataRequest('loadBlockoutDateById', [blockoutDateId]);
      this.processRefreshData('searchBlockoutDates');
    });
  }
}
