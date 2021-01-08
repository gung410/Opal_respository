import { BaseBackendService, CommonFacadeService, IGetParams } from '@opal20/infrastructure';
import { BookmarkInfoModel, BookmarkType, IBookmarkInfoModel } from '../models/bookmark-info.model';

import { GetCountUserBookmarkedResult } from '../dtos/get-count-user-bookmarked-result.dto';
import { IGetMyBookmarkRequest } from '../dtos/my-bookmark-request-dto';
import { IPagedResultDto } from '../../share/dtos/paged-result.dto';
import { IUserBookmarkRequest } from '../dtos/my-bookmark-backend-service.dto';
import { Injectable } from '@angular/core';
import { LearnerLearningPath } from '../models/my-learning-path.model';
import { MyDigitalContentSearchResult } from '../dtos/my-digital-content-search-result.dto';
import { UserBookMarkedModel } from '../models/user-bookmarked.model';
import { map } from 'rxjs/operators';

@Injectable()
export class MyBookmarkApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.learnerApiUrl + '/me/bookmarks';
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getUserBookmarkByType(request: IGetMyBookmarkRequest, showSpinner: boolean = true): Promise<IPagedResultDto<BookmarkInfoModel>> {
    const queryParams: IGetParams = {
      itemType: request.itemType,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };
    return this.get<IPagedResultDto<BookmarkInfoModel>>('', queryParams, showSpinner).toPromise();
  }

  public getUserBookmarkByItemIds(request: IGetMyBookmarkRequest, showSpinner: boolean = true): Promise<BookmarkInfoModel[]> {
    const queryParams: IGetParams = {
      itemType: request.itemType,
      itemIds: request.itemIds
    };
    return this.get<BookmarkInfoModel[]>('/ids', queryParams, showSpinner).toPromise();
  }

  public getBookmarkDigitalContent(
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Promise<MyDigitalContentSearchResult> {
    const queryParams: IGetParams = {
      maxResultCount: maxResultCount,
      skipCount: skipCount
    };
    return this.get<MyDigitalContentSearchResult>(`/digitalcontent`, queryParams, showSpinner)
      .pipe(map(result => new MyDigitalContentSearchResult(result)))
      .toPromise();
  }

  public getBookmarkLearningPath(
    request: IGetMyBookmarkRequest,
    showSpinner: boolean = true
  ): Promise<IPagedResultDto<LearnerLearningPath>> {
    const queryParams: IGetParams = {
      itemType: request.itemType,
      maxResultCount: request.maxResultCount,
      skipCount: request.skipCount
    };
    return this.get<IPagedResultDto<LearnerLearningPath>>(`/learningpath`, queryParams, showSpinner).toPromise();
  }

  public createBookmark(request: IUserBookmarkRequest): Promise<BookmarkInfoModel> {
    return this.post<IUserBookmarkRequest, IBookmarkInfoModel>('/create', request)
      .pipe(map(result => new BookmarkInfoModel(result)))
      .toPromise();
  }

  public unbookmark(id: string): Promise<void> {
    return this.delete<void>(`/unbookmark/${id}`).toPromise();
  }

  public unbookmarkItem(request: IUserBookmarkRequest): Promise<void> {
    return this.delete<void>(`/unbookmarkItem/${request.itemId}`).toPromise();
  }

  public getCountUserBookmarked(itemType: BookmarkType, itemIds: string[]): Promise<GetCountUserBookmarkedResult> {
    const request = {
      itemType: itemType,
      itemIds: itemIds
    };
    return this.get<UserBookMarkedModel[]>(`/countuserbookmark`, request)
      .pipe(
        map(result => {
          return new GetCountUserBookmarkedResult(result);
        })
      )
      .toPromise();
  }
}
