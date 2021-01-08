import { BasePageComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import {
  CommunitySearchLearningType,
  CourseSearchLearningType,
  FilterStatisticResult,
  LearningPathSearchLearningType,
  MyLearningSearchFilterResult,
  SearchFilterModel,
  SearchLearningType
} from '../../models/my-learning-search-filter-model';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { ILearningItemModel } from '../../models/learning-item.model';
import { LearningCardListComponent } from '../learning-card-list.component';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'learner-search-filter-result',
  templateUrl: './learner-search-filter-result.component.html'
})
export class LearnerSearchFilterResultComponent extends BasePageComponent {
  @Input()
  public getPagedSearchFilterCallBack: (searchModel: SearchFilterModel) => Observable<MyLearningSearchFilterResult<ILearningItemModel>>;

  @Input()
  public placeholderText: string;

  @Input()
  public showLongCard: boolean = false;

  @Input()
  public set searchText(value: string) {
    if (!Utils.isDifferent(value, this._searchText)) {
      return;
    }
    this._searchText = value;
    this.triggerSearch();
  }

  public get searchText(): string {
    return this._searchText;
  }

  @Input()
  public filterByType: SearchFilterModel;

  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();

  @ViewChild('searchFilterResultList', { static: false })
  public searchFilterResultListComponent: LearningCardListComponent;

  public total: number = 0;

  public currenSearchModel: SearchFilterModel;
  public statisticResult: FilterStatisticResult[] = [];

  private _searchText: string;
  constructor(protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.currenSearchModel = new SearchFilterModel({ ...this.filterByType });
  }

  public searchOnType(type: SearchLearningType): void {
    this.currenSearchModel.learningTypeFilter = type;
    this.searchFilterResultListComponent.triggerDataChange(true);
  }

  public triggerSearch(): void {
    if (this.searchFilterResultListComponent !== undefined) {
      this.searchFilterResultListComponent.triggerDataChange(true);
    }
  }

  public getPagedItemsCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    this.currenSearchModel.skipCount = skipCount;
    this.currenSearchModel.maxResultCount = maxResultCount;
    this.currenSearchModel.includeStatistic = this.currenSearchModel.searchText !== this.searchText;
    this.currenSearchModel.searchText = this.searchText;
    return this.getPagedSearchFilterCallBack(this.currenSearchModel).pipe(
      this.untilDestroy(),
      map(response => {
        if (!response) {
          return {
            total: 0,
            items: []
          };
        }

        if (this.currenSearchModel.includeStatistic) {
          this.statisticResult = response.statistics;
          this.total = response.statistics.reduce((total, statistic) => (total += statistic.totalCount), 0);
        }
        return {
          total: response.totalCount,
          items: response.items
        };
      })
    );
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.learningCardClick.emit(event);
  }

  public getFilterNames(learningType: SearchLearningType): string {
    return FILTER_TYPES_NAMES.get(learningType);
  }
}

const FILTER_TYPES_NAMES: Map<SearchLearningType, string> = new Map<SearchLearningType, string>([
  [LearningPathSearchLearningType.Owned, 'My own'],
  [LearningPathSearchLearningType.Shared, 'Shared from users'],
  [LearningPathSearchLearningType.Recommended, 'Recommendations'],

  [CourseSearchLearningType.Registered, 'Registered'],
  [CourseSearchLearningType.Upcoming, 'Upcoming'],
  [CourseSearchLearningType.InProgress, 'InProgress'],
  [CourseSearchLearningType.Completed, 'Completed'],

  [CommunitySearchLearningType.Joined, 'My communities'],
  [CommunitySearchLearningType.Owned, 'My own communities']
]);
