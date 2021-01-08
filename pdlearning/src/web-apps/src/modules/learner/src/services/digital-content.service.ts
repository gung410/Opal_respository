import {
  ContentRepository,
  IMyDigitalContentSearchRequest,
  IMyLearningSearchRequest,
  ISearchFilterResultModel,
  MyBookmarkRepository,
  MyDigitalContentRepository
} from '@opal20/domain-api';
import { MyDigitalContentDetail, PagedMyDigitalContentDetailResult } from '../models/my-digital-content-detail.model';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { Injectable } from '@angular/core';
import { Utils } from '@opal20/infrastructure';

@Injectable()
export class DigitalContentDataService {
  constructor(
    private contentRepository: ContentRepository,
    private myDigitalContentRepository: MyDigitalContentRepository,
    private myBookmarkRepository: MyBookmarkRepository
  ) {}

  public getDigitalContentDetail(digitalContentId: string): Observable<MyDigitalContentDetail> {
    return this.contentRepository.loadDigitalContentById(digitalContentId).pipe(
      switchMap(dcResult => {
        return this.myDigitalContentRepository.loadDigitalContentDetails(dcResult.originalObjectId).pipe(
          map(myDcResult => {
            return MyDigitalContentDetail.createMyDigitalContentDetail(myDcResult, dcResult);
          })
        );
      })
    );
  }

  public getMyDigitalContentsBySearch(request: IMyLearningSearchRequest): Observable<ISearchFilterResultModel<MyDigitalContentDetail>> {
    return this.myDigitalContentRepository.loadMyDigitalContentsBySearch(request).pipe(
      switchMap((response: ISearchFilterResultModel<MyDigitalContentDetail>) => {
        const ids: string[] = response.items.map(myDc => myDc.digitalContentId);
        return this.contentRepository.loadDigitalContentByIds(ids).pipe(
          map(dcResult => {
            response.items.forEach(item => {
              item.digitalContent = dcResult.find(dc => dc.id === item.digitalContentId);
            });
            return response;
          })
        );
      })
    );
  }

  public getMyDigitalContents(request: IMyDigitalContentSearchRequest): Observable<PagedMyDigitalContentDetailResult> {
    return this.myDigitalContentRepository.loadMyDigitalContents(request).pipe(
      switchMap(myDcResult => {
        const ids: string[] = myDcResult.items.map(myDc => myDc.digitalContentId);
        return this.contentRepository.loadDigitalContentByIds(ids).pipe(
          map(dcResult => {
            const items = dcResult.map(dc => {
              const myDigitalContent = myDcResult.items.find(myDc => myDc.digitalContentId === dc.id);
              return MyDigitalContentDetail.createMyDigitalContentDetail(myDigitalContent, dc);
            });
            return new PagedMyDigitalContentDetailResult({
              totalCount: myDcResult.totalCount,
              items: items
            });
          })
        );
      })
    );
  }

  public getMyBookmarkedDigitalContents(
    maxResultCount: number = 10,
    skipCount: number = 0,
    showSpinner: boolean = true
  ): Observable<PagedMyDigitalContentDetailResult> {
    return this.myBookmarkRepository.loadBookmarkDigitalContent(maxResultCount, skipCount).pipe(
      switchMap(myDcResult => {
        const ids: string[] = myDcResult.items.map(myDc => myDc.digitalContentId);
        return this.contentRepository.loadDigitalContentByIds(ids).pipe(
          map(dcResult => {
            const items = dcResult.map(dc => {
              const myDigitalContent = myDcResult.items.find(myDc => myDc.digitalContentId === dc.id);
              return MyDigitalContentDetail.createMyDigitalContentDetail(myDigitalContent, dc);
            });
            return new PagedMyDigitalContentDetailResult({
              totalCount: myDcResult.totalCount,
              items: items
            });
          })
        );
      })
    );
  }

  public getMyDigitalContentDetaisByContentIds(contentIds: string[]): Observable<MyDigitalContentDetail[]> {
    const contents$ = this.contentRepository.loadDigitalContentByIds(contentIds);

    return contents$.pipe(
      switchMap(contents => {
        const originalIds = Utils.distinct(contents.map(c => c.originalObjectId));
        return this.myDigitalContentRepository.loadByDigitalContentIds(originalIds).pipe(
          map(myContents => {
            return contents.map(content => {
              const myContent = myContents.find(myC => myC.digitalContentId === content.originalObjectId);
              return MyDigitalContentDetail.createMyDigitalContentDetail(myContent, content);
            });
          })
        );
      })
    );
  }

  public getContentLearningItems(contentIds: string[]): Observable<DigitalContentItemModel[]> {
    if (contentIds.length > 0) {
      return this.getMyDigitalContentDetaisByContentIds(Utils.distinct(contentIds)).pipe(
        map(e => e.filter(c => c !== undefined).map(d => DigitalContentItemModel.createDigitalContentItemModel(d)))
      );
    }
    return of([]);
  }
}
