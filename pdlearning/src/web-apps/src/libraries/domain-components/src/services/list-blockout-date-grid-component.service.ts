import { BlockoutDateRepository, SearchBlockoutDateResult, TaggingRepository } from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { map, switchMap } from 'rxjs/operators';

import { BlockoutDateViewModel } from '../models/blockout-date-view.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class ListBlockoutDateGridComponentService {
  constructor(private blockoutDateRepository: BlockoutDateRepository, private taggingRepository: TaggingRepository) {}

  public loadBlockoutDates(
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    coursePlanningCycleId: string,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<BlockoutDateViewModel>> {
    return this.progressBlockoutDates(
      this.blockoutDateRepository.searchBlockoutDates(searchText, filter, skipCount, maxResultCount, coursePlanningCycleId),
      checkAll,
      selectedsFn
    );
  }

  private progressBlockoutDates(
    blockoutDateObs: Observable<SearchBlockoutDateResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<BlockoutDateViewModel>> {
    return blockoutDateObs.pipe(
      switchMap(searchBlockoutDateResult => {
        return this.taggingRepository.loadAllMetaDataTags().pipe(
          map(metadataTags => {
            const metadataTagDict = Utils.toDictionary(metadataTags, p => p.id);
            return <OpalGridDataResult<BlockoutDateViewModel>>{
              data: searchBlockoutDateResult.items.map(_ =>
                BlockoutDateViewModel.createFromModel(_, checkAll, selectedsFn != null ? selectedsFn() : {}, metadataTagDict)
              ),
              total: searchBlockoutDateResult.totalCount
            };
          })
        );
      })
    );
  }
}
