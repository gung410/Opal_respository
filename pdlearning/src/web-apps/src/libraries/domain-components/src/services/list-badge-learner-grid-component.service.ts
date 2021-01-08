import {
  BadgeId,
  BadgeRepository,
  SearchTopBadgeUserStatisticResult,
  UserRepository,
  YearlyUserStatisticRepository
} from '@opal20/domain-api';
import { IFilter, Utils } from '@opal20/infrastructure';
import { Observable, combineLatest, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Injectable } from '@angular/core';
import { YearlyUserStatisticViewModel } from '../models/yearly-user-statistic-view.model';

@Injectable()
export class ListBadgeLearnerGridComponentService {
  constructor(
    private userRepository: UserRepository,
    private yearlyUserStatisticRepository: YearlyUserStatisticRepository,
    private badgeRepository: BadgeRepository
  ) {}

  public loadTopBadgeLearnerStatistics(
    badgeId: BadgeId,
    searchText: string = '',
    filter: IFilter = null,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<YearlyUserStatisticViewModel>> {
    return this.progressDigitalLearner(
      this.yearlyUserStatisticRepository.searchTopBadgeUserStatistics(badgeId, searchText, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressDigitalLearner(
    yearlyUserStatisticObs: Observable<SearchTopBadgeUserStatisticResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<YearlyUserStatisticViewModel>> {
    return yearlyUserStatisticObs.pipe(
      switchMap((result: SearchTopBadgeUserStatisticResult) => {
        if (result.totalCount === 0) {
          return of(<OpalGridDataResult<YearlyUserStatisticViewModel>>{
            data: [],
            total: result.totalCount
          });
        }

        const userObs = this.userRepository.loadPublicUserInfoList({
          userIds: result.items.map(_ => _.userId)
        });

        const badgeObs = this.badgeRepository.getAllBadges();
        return combineLatest(userObs, badgeObs).pipe(
          map(([users, badges]) => {
            const userDic = Utils.toDictionary(users, p => p.id);
            return <OpalGridDataResult<YearlyUserStatisticViewModel>>{
              data: result.items.map(p =>
                YearlyUserStatisticViewModel.createFromModel(
                  p,
                  checkAll,
                  selectedsFn(),
                  userDic[p.userId],
                  true,
                  Utils.toDictionary(badges, x => x.id)
                )
              ),
              total: result.totalCount
            };
          })
        );
      })
    );
  }
}
