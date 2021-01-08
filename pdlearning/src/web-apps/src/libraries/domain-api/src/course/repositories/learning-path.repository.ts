import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { CourseRepositoryContext } from '../course-repository-context';
import { ISaveLearningPathRequest } from './../dtos/save-learning-path-request';
import { Injectable } from '@angular/core';
import { LearningPathApiService } from '../services/learning-path-api.service';
import { LearningPathModel } from '../models/learning-path.model';
import { SearchLearningPathResult } from '../dtos/search-learning-path-result';

@Injectable()
export class LearningPathRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: LearningPathApiService) {
    super(context);
  }

  public loadLearningPath(id: string): Observable<LearningPathModel> {
    return this.processUpsertData(
      this.context.learningPathsSubject,
      implicitLoad => from(this.apiSvc.getLearningPath(id, !implicitLoad)),
      'loadLearningPath',
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

  public loadLearningPathByIds(ids: string[]): Observable<LearningPathModel[]> {
    return this.processUpsertData(
      this.context.learningPathsSubject,
      implicitLoad => from(this.apiSvc.getLearningPathByIds(ids, !implicitLoad)),
      'loadLearningPathByIds',
      ids,
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(p => repoData[p.id]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id
    );
  }

  public searchLearningPaths(
    searchText: string = '',
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Observable<SearchLearningPathResult> {
    return this.processUpsertData(
      this.context.learningPathsSubject,
      implicitLoad => from(this.apiSvc.searchLearningPaths(searchText, skipCount, maxResultCount, !implicitLoad)),
      'searchLearningPaths',
      [searchText, skipCount, maxResultCount],
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

  public saveLearningPath(request: ISaveLearningPathRequest): Observable<LearningPathModel> {
    return from(
      this.apiSvc.saveLearningPath(request).then(learningPath => {
        this.upsertData(this.context.learningPathsSubject, [Utils.cloneDeep(learningPath)], item => item.id, true);
        this.processRefreshData('searchLearningPaths');
        return learningPath;
      })
    );
  }
}
