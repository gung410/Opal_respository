import {
  BookmarkType,
  GetCountUserBookmarkedResult,
  LearningPathRepository,
  MyBookmarkApiService,
  SearchLearningPathResult
} from '@opal20/domain-api';
import { Observable, from, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { LearningPathViewModel } from '../models/learning-path-view.model';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class ListLearningPathGridComponentService {
  constructor(private learningPathRepository: LearningPathRepository, private myBookmarkApiService: MyBookmarkApiService) {}

  public loadLearningPaths(
    searchText: string = '',
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<LearningPathViewModel>> {
    return this.progressLearningPaths(
      this.learningPathRepository.searchLearningPaths(searchText, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressLearningPaths(
    learningPathObs: Observable<SearchLearningPathResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<LearningPathViewModel>> {
    return learningPathObs.pipe(
      switchMap(searchLearningPathResult => {
        if (searchLearningPathResult.items.length === 0) {
          return of([new GetCountUserBookmarkedResult(), searchLearningPathResult]);
        }
        return from(
          this.myBookmarkApiService.getCountUserBookmarked(BookmarkType.LearningPathLMM, searchLearningPathResult.items.map(x => x.id))
        ).pipe(map(myUserBookmarkedResult => [myUserBookmarkedResult, searchLearningPathResult]));
      }),
      map(([myUserBookmarkedResult, searchLearningPathResult]: [GetCountUserBookmarkedResult, SearchLearningPathResult]) => {
        const userBookmarkDict = Utils.toDictionary(myUserBookmarkedResult.item, x => x.itemId);
        return <OpalGridDataResult<LearningPathViewModel>>{
          data: searchLearningPathResult.items.map(learningPathModel =>
            LearningPathViewModel.createFromModel(
              learningPathModel,
              userBookmarkDict[learningPathModel.id] ? userBookmarkDict[learningPathModel.id].countTotal : 0,
              checkAll,
              selectedsFn != null ? selectedsFn() : {}
            )
          ),
          total: searchLearningPathResult.totalCount
        };
      })
    );
  }
}
