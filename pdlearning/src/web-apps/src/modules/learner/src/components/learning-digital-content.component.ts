import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { IMyDigitalContentSearchRequest, MyDigitalContentStatus } from '@opal20/domain-api';
import { MyLearningSearchFilterResult, SearchFilterModel } from '../models/my-learning-search-filter-model';

import { DigitalContentDataService } from '../services/digital-content.service';
import { DigitalContentItemModel } from '../models/digital-content-item.model';
import { ILearningItemModel } from '../models/learning-item.model';
import { LearningCardListComponent } from './learning-card-list.component';
import { MyLearningSearchDataService } from '../services/my-learning-search.service';
import { MyLearningTab } from '@opal20/domain-components';
import { Observable } from 'rxjs';
import { SEARCH_FILTER_TYPE_INITIALIZE } from '../constants/my-learning-tab.enum';
import { map } from 'rxjs/operators';

@Component({
  selector: 'learning-digital-content',
  templateUrl: './learning-digital-content.component.html'
})
export class LearningDigitalContentComponent extends BaseComponent {
  @Input()
  public searchingText: string;

  @Input()
  public isSearchingWithText: boolean;
  public filterByType: SearchFilterModel;

  @Output()
  public learningCardClick: EventEmitter<ILearningItemModel> = new EventEmitter<ILearningItemModel>();
  @ViewChild('myDigitalContentList', { static: false })
  public myDigitalContentsComponent: LearningCardListComponent;

  public allStatusFilters: { value: MyDigitalContentStatus; displayText: string }[] = [
    {
      value: MyDigitalContentStatus.InProgress,
      displayText: this.moduleFacadeService.globalTranslator.translate(MyDigitalContentStatus.InProgress)
    },
    {
      value: MyDigitalContentStatus.Completed,
      displayText: this.moduleFacadeService.globalTranslator.translate(MyDigitalContentStatus.Completed)
    }
  ];

  public selectedFilterStatus: MyDigitalContentStatus = MyDigitalContentStatus.InProgress;
  protected currentActiveTab: MyLearningTab = MyLearningTab.DigitalContent;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private digitalContentDataService: DigitalContentDataService,
    private myLearningSearchDataService: MyLearningSearchDataService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.filterByType = this.initSearchFilterType(this.currentActiveTab);
  }

  public triggerDataChange(fromPage1: boolean = false): void {
    this.myDigitalContentsComponent.triggerDataChange(fromPage1);
  }

  // TODO: updated
  public getMyDigitalContentsCallBack(
    maxResultCount: number,
    skipCount: number
  ): Observable<{
    total: number;
    items: ILearningItemModel[];
  }> {
    const request: IMyDigitalContentSearchRequest = {
      statusFilter: this.selectedFilterStatus,
      maxResultCount: maxResultCount,
      skipCount: skipCount,
      orderBy: 'CreatedDate desc'
    };
    return this.digitalContentDataService.getMyDigitalContents(request).pipe(
      map(response => {
        const items = response.items.map(DigitalContentItemModel.createDigitalContentItemModel);
        return {
          items,
          total: response.totalCount
        };
      })
    );
  }

  public onQuickStatusFilterChanged(selectedValue: MyDigitalContentStatus): void {
    this.selectedFilterStatus = selectedValue;
    this.triggerDataChange();
  }

  public onLearningCardClick(item: DigitalContentItemModel): void {
    this.learningCardClick.emit(item);
  }

  public onSubmitSearch(): void {
    return;
  }

  public getPagedSearchFilterCallBack(searchModel: SearchFilterModel): Observable<MyLearningSearchFilterResult<DigitalContentItemModel>> {
    return this.myLearningSearchDataService.searchDigitalContents(searchModel);
  }

  private initSearchFilterType(tab: MyLearningTab): SearchFilterModel {
    return SEARCH_FILTER_TYPE_INITIALIZE.get(tab);
  }
}
