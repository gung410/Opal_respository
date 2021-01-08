import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { CommunitySearchLearningType, MyLearningSearchFilterResult, SearchFilterModel } from '../models/my-learning-search-filter-model';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';

import { CommunityItemModel } from '../models/community-item.model';
import { CslDataService } from '../services/csl-data.service';
import { FilterCommunityStatus } from '@opal20/domain-api';
import { ILearningItemModel } from '../models/learning-item.model';
import { LearningCardListComponent } from './learning-card-list.component';
import { MyLearningSearchDataService } from '../services/my-learning-search.service';
import { MyLearningTab } from '@opal20/domain-components';
import { Observable } from 'rxjs';
import { SEARCH_FILTER_TYPE_INITIALIZE } from '../constants/my-learning-tab.enum';
import { StatusFilterModel } from '../models/status-filter.model';
import { map } from 'rxjs/operators';

@Component({
  selector: 'learning-communities',
  templateUrl: './learning-communities.component.html'
})
export class LearningCommunitiesComponent extends BaseComponent {
  @Input() public searchingText: string;
  @Input() public isSearchingWithText: boolean = false;

  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();

  @ViewChild('comminityList', { static: false })
  public comminityItemComponent: LearningCardListComponent;

  public filterByType: SearchFilterModel;

  // Filter
  public currentStatusFilter = CommunitySearchLearningType.All;
  public allStatusFilters: {} = CommunitySearchLearningType;
  public statusFilter: StatusFilterModel[];

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected cslService: CslDataService,
    protected myLearningSearchDataService: MyLearningSearchDataService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.statusFilter = this.getMyCourseStatusFilter();
    this.filterByType = this.initSearchFilterType(MyLearningTab.Community);
  }

  public getComminityCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: CommunityItemModel[];
  }> {
    const communityRequest: SearchFilterModel = {
      searchText: this.searchingText,
      maxResultCount: maxResultCount,
      skipCount: skipCount,
      includeStatistic: false,
      type: 'Community',
      learningTypeFilter: this.currentStatusFilter,
      learningTypes: this.filterByType.learningTypes
    };
    return this.myLearningSearchDataService.searchCommunities(communityRequest).pipe(
      map(commData => {
        if (!commData) {
          return;
        }
        return {
          items: commData.items,
          total: commData.totalCount
        };
      })
    );
  }

  public getPagedSearchFilterCallBack(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<CommunityItemModel>> {
    return this.myLearningSearchDataService.searchCommunities(searchModel);
  }

  public onLearningCardClick(event: ILearningItemModel): void {
    this.learningCardClick.emit(event);
  }

  public onQuickStatusFilterChanged(status: string): void {
    this.currentStatusFilter = status as CommunitySearchLearningType;
    this.triggerDataChange(true);
  }

  public triggerDataChange(fromPage1: boolean = false): void {
    this.comminityItemComponent.triggerDataChange(fromPage1);
  }

  protected getMyCourseStatusFilter(): StatusFilterModel[] {
    return Object.keys(this.allStatusFilters).map(status => {
      return new StatusFilterModel({
        status: status,
        displayText: this.translate(FilterCommunityStatus[status])
      });
    });
  }

  private initSearchFilterType(tab: MyLearningTab): SearchFilterModel {
    return SEARCH_FILTER_TYPE_INITIALIZE.get(tab);
  }
}
