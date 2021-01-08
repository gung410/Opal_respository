import { Observable, from } from 'rxjs';

import { BaseRepository } from '@opal20/infrastructure';
import { BookmarkInfoModel } from '../models/bookmark-info.model';
import { IGetMyBookmarkRequest } from '../dtos/my-bookmark-request-dto';
import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { Injectable } from '@angular/core';
import { LearnerLearningPath } from '../models/my-learning-path.model';
import { LearnerRepositoryContext } from '../learner-repository-context';
import { MyBookmarkApiService } from '../services/my-bookmark-backend.service';
import { MyDigitalContentSearchResult } from '../dtos/my-digital-content-search-result.dto';

@Injectable()
export class MyBookmarkRepository extends BaseRepository<LearnerRepositoryContext> {
  constructor(context: LearnerRepositoryContext, private myBookmarkApiService: MyBookmarkApiService) {
    super(context);
  }

  public loadUserBookmarkByType(request: IGetMyBookmarkRequest): Observable<IPagedResultDto<BookmarkInfoModel>> {
    return this.processUpsertData(
      this.context.bookmarkInfoSubject,
      implicitLoad => from(this.myBookmarkApiService.getUserBookmarkByType(request, !implicitLoad)),
      'loadUserBookmarkByType',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.id]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id
    );
  }

  public loadUserBookmarkByItemIds(request: IGetMyBookmarkRequest): Observable<BookmarkInfoModel[]> {
    return this.processUpsertData(
      this.context.bookmarkInfoSubject,
      implicitLoad => from(this.myBookmarkApiService.getUserBookmarkByItemIds(request, !implicitLoad)),
      'loadUserBookmarkByItemIds',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = apiResult.map(a => repoData[a.id]);
        return apiResult;
      },
      apiResult => apiResult,
      x => x.id
    );
  }

  public loadBookmarkDigitalContent(maxResultCount?: number, skipCount?: number): Observable<MyDigitalContentSearchResult> {
    return this.processUpsertData(
      this.context.myDigitalContentSubject,
      implicitLoad => from(this.myBookmarkApiService.getBookmarkDigitalContent(maxResultCount, skipCount, !implicitLoad)),
      'loadBookmarkDigitalContent',
      [maxResultCount, skipCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.digitalContentId]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.digitalContentId
    );
  }

  public loadBookmarkLearningPath(request: IGetMyBookmarkRequest): Observable<IPagedResultDto<LearnerLearningPath>> {
    return this.processUpsertData(
      this.context.learnerLearningPathSubject,
      implicitLoad => from(this.myBookmarkApiService.getBookmarkLearningPath(request, !implicitLoad)),
      'loadBookmarkLearningPath',
      [request],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(a => repoData[a.id]);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id
    );
  }
}
