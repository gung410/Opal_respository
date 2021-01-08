import { Badge } from './models/badge.model';
import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { YearlyUserStatistic } from './models/yearly-user-statistic.model';

@Injectable()
export class BadgeRepositoryContext extends BaseRepositoryContext {
  public yearlyUserStatisticsSubject: BehaviorSubject<Dictionary<YearlyUserStatistic>> = new BehaviorSubject({});
  public badgeSubject: BehaviorSubject<Dictionary<Badge>> = new BehaviorSubject({});
}
