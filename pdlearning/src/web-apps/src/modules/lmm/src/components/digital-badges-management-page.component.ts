import { BadgeId, IAwardBadgeRequest, UserInfoModel, YearlyUserStatisticRepository } from '@opal20/domain-api';
import { BasePageComponent, IGridFilter, ModuleFacadeService } from '@opal20/infrastructure';
import { ButtonAction, OpalDialogService } from '@opal20/common-components';
import { Component, HostBinding } from '@angular/core';
import {
  DigitalBadgesFilterModel,
  LMMRoutePaths,
  LMMTabConfiguration,
  ListBadgeLearnerGridDisplayColumns,
  NavigationPageService,
  RouterPageInput,
  YearlyUserStatisticViewModel
} from '@opal20/domain-components';

import { DigitalBadgesManagementPageInput } from '../models/digital-badges-management-page-input.model';
import { NAVIGATORS } from '../lmm.config';
import { SelectEvent } from '@progress/kendo-angular-layout';
import { SettingActiveContributorBadgeCriteriaDialogComponent } from './dialogs/setting-active-contributor-badge-criteria-dialog.component';

@Component({
  selector: 'digital-badges-management-page',
  templateUrl: './digital-badges-management-page.component.html'
})
export class DigitalBadgesManagementPageComponent extends BasePageComponent {
  public BadgeId: typeof BadgeId = BadgeId;
  public searchText: string = '';
  public filterData: DigitalBadgesFilterModel = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public digitalBadgesManagementPageInput: RouterPageInput<DigitalBadgesManagementPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.DigitalBadgesManagementPage
  ] as RouterPageInput<DigitalBadgesManagementPageInput, LMMTabConfiguration, unknown>;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;

  public allDigitalLearnersSelectedItems: YearlyUserStatisticViewModel[] = [];
  public allReflectiveLearnersSelectedItems: YearlyUserStatisticViewModel[] = [];
  public allActiveContributorsSelectedItems: YearlyUserStatisticViewModel[] = [];
  public allLifeLongSelectedItems: YearlyUserStatisticViewModel[] = [];

  public get hasActionBtnGroup(): boolean {
    return this.actionBtnGroup.findIndex(x => x.actionFn != null && (x.hiddenFn == null || !x.hiddenFn())) > -1;
  }

  public actionBtnGroup: ButtonAction<YearlyUserStatisticViewModel>[] = [
    {
      id: 'award',
      text: this.translateCommon('Award'),
      conditionFn: dataItem => !dataItem.awarded,
      actionFn: dataItems => this.handleMassAction(DigitalBadgesMassAction.Award, dataItems),
      hiddenFn: () => false
    }
  ];

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public get selectedTab(): LMMTabConfiguration {
    return this.digitalBadgesManagementPageInput.activeTab != null
      ? this.digitalBadgesManagementPageInput.activeTab
      : LMMTabConfiguration.DigitalLearnerTab;
  }
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public navigationPageService: NavigationPageService,
    private yearlyUserStatisticRepository: YearlyUserStatisticRepository,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public handleMassAction(massAction: DigitalBadgesMassAction, dataItems: YearlyUserStatisticViewModel[]): Promise<boolean> {
    let massActionPromise: Promise<boolean>;
    switch (massAction) {
      case DigitalBadgesMassAction.Award:
        massActionPromise = this.awardDigitalBadge(dataItems);
        break;
    }
    return massActionPromise.then(_ => {
      this.resetSelectedItems();
      return _;
    });
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.digitalBadgesManagementPageInput.activeTab = digitalBadgesManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.digitalBadgesManagementPageInput);
  }

  public get showSettingBadgeCriteria(): boolean {
    return this.selectedTab === LMMTabConfiguration.ActiveContributorTab;
  }

  public get getSelectedItemsForMassAction(): YearlyUserStatisticViewModel[] {
    if (this.selectedTab === LMMTabConfiguration.DigitalLearnerTab) {
      return this.allDigitalLearnersSelectedItems;
    } else if (this.selectedTab === LMMTabConfiguration.ActiveContributorTab) {
      return this.allActiveContributorsSelectedItems;
    } else if (this.selectedTab === LMMTabConfiguration.ReflectiveLearnerTab) {
      return this.allReflectiveLearnersSelectedItems;
    } else if (this.selectedTab === LMMTabConfiguration.LifeLongLearnerTab) {
      return this.allLifeLongSelectedItems;
    }
    return [];
  }

  public get getDigitalLearnerDisplayColumn(): ListBadgeLearnerGridDisplayColumns[] {
    return [
      ListBadgeLearnerGridDisplayColumns.completedMLU,
      ListBadgeLearnerGridDisplayColumns.completedDigitalResources,
      ListBadgeLearnerGridDisplayColumns.completeElearning
    ];
  }

  public get getReflectiveLearnerDisplayColumn(): ListBadgeLearnerGridDisplayColumns[] {
    return [ListBadgeLearnerGridDisplayColumns.reflection, ListBadgeLearnerGridDisplayColumns.sharedReflection];
  }

  public get getActiveContributorLearnerDisplayColumn(): ListBadgeLearnerGridDisplayColumns[] {
    return [
      ListBadgeLearnerGridDisplayColumns.awardedCollaborativeLearnersBadge,
      ListBadgeLearnerGridDisplayColumns.awardedCommunityBuilderBadge,
      ListBadgeLearnerGridDisplayColumns.awardedDigitalLearnersBadge,
      ListBadgeLearnerGridDisplayColumns.awardedReflectiveLearnersBadge,
      ListBadgeLearnerGridDisplayColumns.createdLearningPath,
      ListBadgeLearnerGridDisplayColumns.sharedLearningPath,
      ListBadgeLearnerGridDisplayColumns.bookmarkedLearningPath,
      ListBadgeLearnerGridDisplayColumns.createdMLU
    ];
  }

  public get getLifeLongDisplayColumn(): ListBadgeLearnerGridDisplayColumns[] {
    return [ListBadgeLearnerGridDisplayColumns.awardedActiveContributorsBadge];
  }

  public resetSelectedItems(): void {
    this.allDigitalLearnersSelectedItems = [];
    this.allActiveContributorsSelectedItems = [];
  }

  public onSettingBadgeCriteria(): void {
    switch (this.selectedTab) {
      case LMMTabConfiguration.ActiveContributorTab:
        this.opalDialogService.openDialogRef(SettingActiveContributorBadgeCriteriaDialogComponent, null, {
          maxWidth: '650px'
        });
        break;
      default:
        break;
    }
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  public onApplyFilter(data: DigitalBadgesFilterModel): void {
    this.filterData = data;

    this.filter = {
      ...this.filter,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText,
      filter: this.filterData ? this.filterData.convert() : null
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<DigitalBadgesManagementPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.digitalBadgesManagementPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private awardDigitalBadge(yearlyUserStatistics: YearlyUserStatisticViewModel[], showNotification: boolean = true): Promise<boolean> {
    return this.yearlyUserStatisticRepository
      .awardBadge(<IAwardBadgeRequest>{
        userIds: yearlyUserStatistics.map(p => p.userId)
      })
      .toPromise()
      .then(_ => {
        if (showNotification) {
          this.showNotification();
        }
        this.resetFilter();
        return true;
      });
  }
}

export enum DigitalBadgesMassAction {
  Award = 'award'
}

export const digitalBadgesManagementPageTabIndexMap = {
  0: LMMTabConfiguration.DigitalLearnerTab,
  1: LMMTabConfiguration.ReflectiveLearnerTab,
  2: LMMTabConfiguration.ActiveContributorTab,
  3: LMMTabConfiguration.LifeLongLearnerTab
};
