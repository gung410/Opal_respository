import { BadgeId, UserInfoModel } from '@opal20/domain-api';
import { BaseGridComponent, ModuleFacadeService, Utils } from '@opal20/infrastructure';
import { CellClickEvent, RowClassArgs } from '@progress/kendo-angular-grid';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable, Subscription } from 'rxjs';

import { ContextMenuEmit } from '../../models/context-menu-emit.model';
import { ContextMenuSelectEvent } from '@progress/kendo-angular-menu';
import { IRowCallbackModel } from '../../models/row-callback.model';
import { ListBadgeLearnerGridComponentService } from '../../services/list-badge-learner-grid-component.service';
import { YearlyUserStatisticViewModel } from '../../models/yearly-user-statistic-view.model';

@Component({
  selector: 'list-badge-learner-grid',
  templateUrl: './list-badge-learner-grid.component.html'
})
export class ListBadgeLearnerGridComponent extends BaseGridComponent<YearlyUserStatisticViewModel> {
  @Input() public set displayColumns(displayColumns: ListBadgeLearnerGridDisplayColumns[]) {
    this._displayColumns = displayColumns;
    if (this.initiated) {
      this.updateDisplayColumnsDict();
    }
  }

  @Input() public badgeId: BadgeId;

  @Output() public selectedContextMenu: EventEmitter<ContextMenuEmit<YearlyUserStatisticViewModel>> = new EventEmitter();
  public dicDisplayColumns: Dictionary<boolean> = {};

  @Output('viewDigitalLearner') public viewDigitalLearnerEvent: EventEmitter<YearlyUserStatisticViewModel> = new EventEmitter<
    YearlyUserStatisticViewModel
  >();

  public query: Observable<unknown>;
  public loading: boolean;

  private _loadDataSub: Subscription = new Subscription();
  private currentUser = UserInfoModel.getMyUserInfo();
  private _displayColumns: ListBadgeLearnerGridDisplayColumns[] = [];

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public listBadgeLearnerGridComponentService: ListBadgeLearnerGridComponentService
  ) {
    super(moduleFacadeService);
  }

  public rowCallback(context: RowClassArgs): IRowCallbackModel {
    return {
      selected: context.dataItem.selected
    };
  }

  public onSelectedContextMenu(event: ContextMenuSelectEvent, dataItem: YearlyUserStatisticViewModel, rowIndex: number): void {
    this.selectedContextMenu.emit(new ContextMenuEmit(event, dataItem, rowIndex));
  }

  public loadData(): void {
    this._loadDataSub.unsubscribe();
    this._loadDataSub = this.listBadgeLearnerGridComponentService
      .loadTopBadgeLearnerStatistics(this.badgeId, '', null, this.state.skip, this.state.take, this.checkAll, () => this.selecteds)
      .pipe(this.untilDestroy())
      .subscribe(data => {
        this.gridData = data;
        this.updateSelectedsAndGridData();
      });
  }

  public onGridCellClick(event: CellClickEvent): void {
    // columnIndex isn't action column
    if (event.dataItem instanceof YearlyUserStatisticViewModel && !this.indexActionColumns.includes(event.columnIndex)) {
      this.viewDigitalLearnerEvent.emit(event.dataItem);
    }
  }

  public displayCompletedMLUColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.completedMLU];
  }

  public displayCompletedDigitalResourcesColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.completedDigitalResources];
  }

  public displayCompletedElearningColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.completeElearning];
  }

  public displayReflectionColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.reflection];
  }

  public displaySharedReflectionColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.sharedReflection];
  }

  public displayAwardedCollaborativeLearnersBadgeColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.awardedCollaborativeLearnersBadge];
  }

  public displayAwardedCommunityBuilderBadgeColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.awardedCommunityBuilderBadge];
  }

  public displayAwardedDigitalLearnersBadgeColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.awardedDigitalLearnersBadge];
  }

  public displayAwardedReflectiveLearnersBadgeColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.awardedReflectiveLearnersBadge];
  }

  public displayCreatedLearningPathColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.createdLearningPath];
  }

  public displaySharedLearningPathColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.sharedLearningPath];
  }

  public displayBookmarkedLearningPathColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.bookmarkedLearningPath];
  }

  public displayCreatedMLUColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.createdMLU];
  }

  public displayAwardedActiveContributorsBadgeColumn(): boolean {
    return this.dicDisplayColumns[ListBadgeLearnerGridDisplayColumns.awardedActiveContributorsBadge];
  }

  protected onInit(): void {
    super.onInit();
    this.updateDisplayColumnsDict();
  }

  private updateDisplayColumnsDict(): void {
    this.dicDisplayColumns = Utils.toDictionarySelect(this._displayColumns, p => p, p => true);
  }
}

export enum ListBadgeLearnerGridDisplayColumns {
  selected = 'selected',
  name = 'name',
  awarded = 'awarded',
  completedMLU = 'completedMLU',
  completedDigitalResources = 'completedDigitalResources',
  completeElearning = 'completeElearning',
  reflection = 'reflection',
  sharedReflection = 'sharedReflection',
  awardedCollaborativeLearnersBadge = 'awardedCollaborativeLearnersBadge',
  awardedCommunityBuilderBadge = 'awardedCommunityBuilderBadge',
  awardedDigitalLearnersBadge = 'awardedDigitalLearnersBadge',
  awardedReflectiveLearnersBadge = 'awardedReflectiveLearnersBadge',
  createdLearningPath = 'createdLearningPath',
  sharedLearningPath = 'sharedLearningPath',
  bookmarkedLearningPath = 'bookmarkedLearningPath',
  createdMLU = 'createdMLU',
  awardedActiveContributorsBadge = 'awardedActiveContributorsBadge'
}
