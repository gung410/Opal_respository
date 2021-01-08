import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { ILearnerLearningPath, LearnerLearningPath } from '../models/my-learning-path.model';
import { ISearchFilterResultModel, LearningPathSearchFilterResultModel } from '../dtos/my-learning-search-result.dto';

import { IGetMyLearningPathRequest } from '../dtos/my-learning-path-request.dto';
import { IMyLearningSearchRequest } from '../dtos/my-learning-search-request.dto';
import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { ISaveMyLearningPath } from '../dtos/save-my-learning-path-request.dto';
import { ISearchUsersForLearningPathRequestDto } from '../dtos/search-users-for-learning-path-request.dto';
import { IUserModel } from '../models/user.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchUsersForLearningPathResultDto } from '../dtos/search-users-for-learning-path-result.dto';
import { map } from 'rxjs/operators';

@Injectable()
export class MyLearningPathApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/learningpaths';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public createMyLearningPath(request: ISaveMyLearningPath): Promise<LearnerLearningPath> {
    return this.post<ISaveMyLearningPath, ILearnerLearningPath>('', request)
      .pipe(map(result => new LearnerLearningPath(result)))
      .toPromise();
  }

  public updateMyLearningPath(request: ISaveMyLearningPath): Promise<LearnerLearningPath> {
    return this.put<ISaveMyLearningPath, ILearnerLearningPath>('', request)
      .pipe(map(result => new LearnerLearningPath(result)))
      .toPromise();
  }

  public getLearningPaths(request: IGetMyLearningPathRequest): Promise<IPagedResultDto<LearnerLearningPath>> {
    const queryParams: IGetParams = {
      searchText: request.searchText,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };

    return this.post<IGetParams, IPagedResultDto<LearnerLearningPath>>('/search', queryParams).toPromise();
  }

  public getLearningPathById(id: string): Promise<LearnerLearningPath> {
    return this.get<ILearnerLearningPath>(`/detail/${id}`)
      .pipe(map(_ => new LearnerLearningPath(_)))
      .toPromise();
  }

  public getLearningPathByIds(ids: string[]): Promise<LearnerLearningPath[]> {
    return this.post<string[], ILearnerLearningPath[]>(`/search/ids`, ids).toPromise();
  }

  public deleteLearningPathById(id: string): Promise<LearnerLearningPath> {
    return this.delete<ILearnerLearningPath>(`/${id}`).toPromise();
  }

  public searchUsers(request: ISearchUsersForLearningPathRequestDto): Observable<SearchUsersForLearningPathResultDto> {
    const queryParams: IGetParams = {
      searchText: request.searchText,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };
    return this.get<IPagedResultDto<IUserModel>>(`/searchUsers`, queryParams).pipe(map(_ => new SearchUsersForLearningPathResultDto(_)));
  }

  public publicLearningPath(id: string): Promise<void> {
    return this.post<object, void>(`/enablePublic/${id}`, {}, false).toPromise();
  }

  public searchLearnerLearningPaths(request: IMyLearningSearchRequest): Observable<LearningPathSearchFilterResultModel> {
    const params: IGetParams = {
      type: request.type,
      includeStatistic: request.includeStatistic,
      statisticsFilter: request.statisticsFilter,
      statusFilter: request.statusFilter,
      searchText: request.searchText
    };
    return this.post<IGetParams, ISearchFilterResultModel<ILearnerLearningPath>>(`/search`, params).pipe(
      map(response => {
        return new LearningPathSearchFilterResultModel({
          statistics: response.statistics,
          totalCount: response.totalCount,
          items: response.items
        });
      })
    );
  }
}
