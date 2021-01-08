import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ILearningPathModel, LearningPathModel } from './../models/learning-path.model';

import { Constant } from '@opal20/authentication';
import { ISaveLearningPathRequest } from './../dtos/save-learning-path-request';
import { Injectable } from '@angular/core';
import { SearchLearningPathResult } from './../dtos/search-learning-path-result';
import { map } from 'rxjs/internal/operators/map';
@Injectable()
export class LearningPathApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getLearningPath(id: string, showSpinner?: boolean): Promise<LearningPathModel> {
    return this.get<LearningPathModel>(`/learningpaths/${id}`, null, showSpinner)
      .pipe(map(_ => new LearningPathModel(_)))
      .toPromise();
  }

  public getLearningPathByIds(ids: string[], showSpinner?: boolean): Promise<LearningPathModel[]> {
    return this.post<object, LearningPathModel[]>(`/learningpaths/getbyids`, ids, showSpinner)
      .pipe(map(_ => _.map(p => new LearningPathModel(p))))
      .toPromise();
  }

  public searchLearningPaths(
    searchText: string = '',
    skipCount: number | null = 0,
    maxResultCount: number | null = 10,
    showSpinner?: boolean
  ): Promise<SearchLearningPathResult> {
    return this.get<SearchLearningPathResult>(
      '/learningpaths/search',
      {
        searchText,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(map(_ => new SearchLearningPathResult(_)))
      .toPromise();
  }

  public saveLearningPath(request: ISaveLearningPathRequest, showSpinner?: boolean): Promise<LearningPathModel> {
    return this.post<ISaveLearningPathRequest, ILearningPathModel>('/learningpaths/save', request, showSpinner)
      .pipe(map(data => new LearningPathModel(data)))
      .toPromise();
  }
}
