import { YearlyUserStatistic } from './../models/yearly-user-statistic.model';

export interface ISearchTopBadgeUserStatisticResult {
  items: YearlyUserStatistic[];
  totalCount: number;
}

export class SearchTopBadgeUserStatisticResult implements ISearchTopBadgeUserStatisticResult {
  public items: YearlyUserStatistic[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchTopBadgeUserStatisticResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new YearlyUserStatistic(item));
    this.totalCount = data.totalCount;
  }
}
