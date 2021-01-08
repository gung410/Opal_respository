import { CommunityStatisticType, LearningPathStatisticType, MyLearningStatisticType } from '../models/search-filter-statistic-type.dto';
import { ILearnerLearningPath, LearnerLearningPath } from '../models/my-learning-path.model';
import { IMyCourseResultModel, MyCourseResultModel } from '../models/my-course-result.model';

export type StatisticType = MyLearningStatisticType | LearningPathStatisticType | CommunityStatisticType;

export interface IStatistic {
  type: StatisticType;
  totalCount: number;
}

export class Statistic {
  public type: StatisticType;
  public totalCount: number;
  constructor(data?: IStatistic) {
    if (data == null) {
      return;
    }
    this.type = data.type;
    this.totalCount = data.totalCount ? data.totalCount : 0;
  }
}

export interface ISearchFilterResultModel<T> {
  statistics: IStatistic[];
  totalCount: number;
  items: T[];
}

export class MyCourseSearchFilterResultModel implements ISearchFilterResultModel<IMyCourseResultModel> {
  public statistics: Statistic[] = [];
  public totalCount: number;
  public items: MyCourseResultModel[] = [];

  constructor(data?: ISearchFilterResultModel<IMyCourseResultModel>) {
    if (data == null) {
      return;
    }
    this.statistics = data.statistics ? data.statistics.map(_ => new Statistic(_)) : [];
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items ? data.items.map(_ => new MyCourseResultModel(_)) : [];
  }
}

export class LearningPathSearchFilterResultModel implements ISearchFilterResultModel<ILearnerLearningPath> {
  public statistics: Statistic[] = [];
  public totalCount: number;
  public items: LearnerLearningPath[] = [];

  constructor(data?: ISearchFilterResultModel<ILearnerLearningPath>) {
    if (data == null) {
      return;
    }
    this.statistics = data.statistics ? data.statistics.map(_ => new Statistic(_)) : [];
    this.totalCount = data.totalCount ? data.totalCount : 0;
    this.items = data.items ? data.items.map(_ => new LearnerLearningPath(_)) : [];
  }
}
