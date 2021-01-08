import { IPagedResultRequestDto } from '@opal20/domain-api';

export interface ISearchFilterModel extends IPagedResultRequestDto {
  type: SearchType;
  includeStatistic: boolean;
  learningTypes?: SearchLearningType[];
  learningTypeFilter: SearchLearningType;
  searchText: string;
  navigateBack?: string;
}

export class SearchFilterModel implements ISearchFilterModel, IPagedResultRequestDto {
  public type: SearchType;
  public includeStatistic: boolean;
  public learningTypes?: SearchLearningType[];
  public learningTypeFilter: SearchLearningType;
  public searchText: string;
  public skipCount?: number;
  public maxResultCount?: number;
  public navigateBack?: string;

  constructor(data?: Partial<ISearchFilterModel>) {
    if (data == null) {
      return;
    }
    this.type = data.type;
    this.learningTypes = data.learningTypes;
    this.includeStatistic = data.includeStatistic;
    this.learningTypeFilter = data.learningTypeFilter;
    this.searchText = data.searchText;
    this.skipCount = data.skipCount ? data.skipCount : 0;
    this.maxResultCount = data.maxResultCount ? data.maxResultCount : 12;
    this.navigateBack = data.navigateBack;
  }
}

export interface IMyLearningSearchFilterResult<T> {
  statistics: FilterStatisticResult[];
  totalCount: number;
  items: T[];
}

export class MyLearningSearchFilterResult<T> implements IMyLearningSearchFilterResult<T> {
  public statistics: FilterStatisticResult[];
  public totalCount: number;
  public items: T[];

  constructor(data?: Partial<IMyLearningSearchFilterResult<T>>) {
    if (data == null) {
      return;
    }
    this.statistics = data.statistics;
    this.totalCount = data.totalCount;
    this.items = data.items;
  }
}

export interface IFilterStatisticResult {
  type: SearchLearningType;
  totalCount: number;
}

export class FilterStatisticResult {
  public type: SearchLearningType;
  public totalCount: number;
  constructor(data?: IFilterStatisticResult) {
    if (!data) {
      return;
    }
    this.type = data.type;
    this.totalCount = data.totalCount;
  }
}

export type SearchLearningType = CourseSearchLearningType | LearningPathSearchLearningType | CommunitySearchLearningType;

export type SearchType = 'FaceToFace' | 'Microlearning' | 'DigitalContent' | 'Community' | 'LearningPath';

export enum CourseSearchLearningType {
  Registered = 'Registered',
  Upcoming = 'Upcoming',
  InProgress = 'InProgress',
  Completed = 'Completed'
}

export enum LearningPathSearchLearningType {
  Owned = 'Owner',
  Shared = 'Shared',
  Recommended = 'Recommended'
}

export enum CommunitySearchLearningType {
  All = 'All',
  Joined = 'Joined',
  Owned = 'Owned'
}
