import {
  BookmarkType,
  CSLCommunityResults,
  CatalogueRepository,
  CommunityResultModel,
  CslRepository,
  ICatalogSearchRequest,
  ICatalogSearchResult,
  ICommunityRequest,
  IGetMyBookmarkRequest,
  MyBookmarkRepository,
  ResourceStatistics
} from '@opal20/domain-api';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { CommunityItemModel } from '../models/community-item.model';
import { Injectable } from '@angular/core';

@Injectable()
export class CslDataService {
  constructor(
    private catalogueRepository: CatalogueRepository,
    private cslRepository: CslRepository,
    private myBookmarkRepository: MyBookmarkRepository
  ) {}

  public getItemsCommunity(userId: string, request: ICommunityRequest): Observable<CSLCommunityResults> {
    return this.cslRepository.loadAllItemsCommunity(userId, request).pipe(
      switchMap(communityResponse => {
        const communityIds = communityResponse.items.map(p => p.guid);
        if (communityIds && communityIds.length) {
          const bookmarkRequest: IGetMyBookmarkRequest = {
            itemType: BookmarkType.Community,
            itemIds: communityIds
          };

          return this.myBookmarkRepository.loadUserBookmarkByItemIds(bookmarkRequest).pipe(
            map(bookmarkResponse => {
              if (bookmarkResponse && bookmarkResponse.length) {
                bookmarkResponse.forEach(item => {
                  const existCommunityIndex = communityResponse.items.findIndex(p => p.guid === item.itemId);
                  if (existCommunityIndex > -1) {
                    communityResponse.items[existCommunityIndex].isBookmark = true;
                  }
                });
              }
              return communityResponse;
            })
          );
        } else {
          return of(communityResponse);
        }
      })
    );
  }

  public getCommunities(
    communityRequest: ICatalogSearchRequest
  ): Observable<{ total: number; items: CommunityResultModel[]; resourceStatistics: ResourceStatistics }> {
    if (communityRequest.searchText) {
      communityRequest.searchText = this.rewriteSearchText(communityRequest.searchText.trim());
    }

    return this.getLearningItemsFromTheirsRepository(this.catalogueRepository.search(communityRequest));
  }

  public getLearningItemsFromTheirsRepository(
    searchResult: Observable<ICatalogSearchResult>
  ): Observable<{
    total: number;
    items: CommunityResultModel[];
    resourceStatistics: ResourceStatistics;
  }> {
    return searchResult.pipe(
      switchMap(_ => {
        const communityIds = _.resources.map(r => r.id);
        if (communityIds.length === 0) {
          return of({
            total: 0,
            items: [],
            resourceStatistics: _.resourceStatistics.length === 0 ? [] : _.resourceStatistics
          });
        }

        const bookmarkRequest: IGetMyBookmarkRequest = {
          itemType: BookmarkType.Community,
          itemIds: communityIds
        };

        return combineLatest([
          this.cslRepository.loadCommunityByIds(communityIds),
          this.myBookmarkRepository.loadUserBookmarkByItemIds(bookmarkRequest)
        ]).pipe(
          map(([communityResponse, bookmarkResponse]) => {
            if (bookmarkResponse && bookmarkResponse.length) {
              bookmarkResponse.forEach(item => {
                const existCommunityIndex = communityResponse.items.findIndex(p => p.guid === item.itemId);
                if (existCommunityIndex > -1) {
                  communityResponse.items[existCommunityIndex].isBookmark = true;
                }
              });
            }
            return {
              total: _.total,
              items: communityResponse.items,
              resourceStatistics: _.resourceStatistics
            };
          })
        );
      })
    );
  }

  public getCommunityItems(communityIds: string[]): Observable<CommunityItemModel[]> {
    const bookmarkRequest: IGetMyBookmarkRequest = {
      itemType: BookmarkType.Community,
      itemIds: communityIds
    };
    return combineLatest(
      this.cslRepository.loadCommunityByIds(communityIds),
      this.myBookmarkRepository.loadUserBookmarkByItemIds(bookmarkRequest)
    ).pipe(
      map(([communityResponse, bookmarkResponse]) => {
        bookmarkResponse.forEach(item => {
          const existCommunityIndex = communityResponse.items.findIndex(p => p.guid === item.itemId);
          if (existCommunityIndex > -1) {
            communityResponse.items[existCommunityIndex].isBookmark = true;
          }
        });
        return communityResponse.items.map(CommunityItemModel.createCommunityItemModel);
      })
    );
  }

  public getMyCommunityBookmarks(request: IGetMyBookmarkRequest): Observable<CSLCommunityResults> {
    return this.myBookmarkRepository.loadUserBookmarkByType(request).pipe(
      switchMap(bookmarkResponse => {
        if (bookmarkResponse && bookmarkResponse.items) {
          const bookmarkItemIds = bookmarkResponse.items.map(item => item.itemId);
          if (bookmarkItemIds.length === 0) {
            return of(new CSLCommunityResults());
          }
          return this.cslRepository.loadCommunityByIds(bookmarkItemIds).pipe(
            map(communityResult => {
              communityResult.items.forEach(element => {
                element.isBookmark = true;
              });
              communityResult.totalCount = bookmarkResponse.totalCount;

              return communityResult;
            })
          );
        }
      })
    );
  }

  private rewriteSearchText(searchText: string): string {
    if (searchText && searchText.length > 1) {
      if (searchText[0] === '"' && searchText[searchText.length - 1] === '"') {
        return searchText;
      } else {
        return searchText.replace(/[.*+\-?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
      }
    } else {
      return searchText;
    }
  }
}
