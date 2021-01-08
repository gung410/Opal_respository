import {
  BookmarkInfoModel,
  BookmarkType,
  ICatalogSearchResult,
  IGetMyBookmarkRequest,
  LearningPathModel,
  LearningPathRepository,
  MyBookmarkRepository
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { LearnerLearningPathModel } from '../models/learning-path.model';

@Injectable()
export class LearnerLearningPathDataService {
  constructor(private learningPathRepository: LearningPathRepository, private myBookmarkRepository: MyBookmarkRepository) {}

  public getLearningPathFromCatalogResult(
    result: Observable<ICatalogSearchResult>
  ): Observable<{
    total: number;
    items: LearnerLearningPathModel[];
  }> {
    const response = result.pipe(
      switchMap(csr => {
        const learningPathIds = csr.resources.filter(r => r.resourcetype === 'learningpath').map(r => r.id);
        return this.getLearningPathItems(learningPathIds).pipe(
          map(learningPathsCombined => {
            return {
              total: csr.total,
              items: learningPathsCombined
            };
          })
        );
      })
    );
    return response;
  }

  public getLearningPathFromLMM(learningPathIds: string[]): Observable<LearningPathModel[]> {
    return this.learningPathRepository.loadLearningPathByIds(learningPathIds);
  }

  public getLearningPathItems(learningPathIds: string[]): Observable<LearnerLearningPathModel[]> {
    return combineLatest(this.getLearningPathFromLMM(learningPathIds), this.getBookmarkForLearningPathFromLMM(learningPathIds)).pipe(
      map(([learningPaths, bookmarks]) => {
        return learningPaths.map(p => {
          const bookmark = bookmarks.filter(bm => bm.itemId === p.id)[0];
          return LearnerLearningPathModel.convertLearningPathFromLMM(p, bookmark);
        });
      })
    );
  }

  private getBookmarkForLearningPathFromLMM(learningPathIds: string[]): Observable<BookmarkInfoModel[]> {
    if (!learningPathIds || !learningPathIds.length) {
      return of([]);
    }
    const request: IGetMyBookmarkRequest = {
      itemType: BookmarkType.LearningPath,
      itemIds: learningPathIds
    };
    return this.myBookmarkRepository.loadUserBookmarkByItemIds(request);
  }
}
