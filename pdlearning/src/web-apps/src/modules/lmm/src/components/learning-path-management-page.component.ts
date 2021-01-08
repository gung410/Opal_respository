import { BasePageComponent, IGridFilter, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostBinding } from '@angular/core';
import {
  LMMRoutePaths,
  LMMTabConfiguration,
  LearningPathDetailMode,
  LearningPathViewModel,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import { LearningPathModel, UserInfoModel } from '@opal20/domain-api';

import { LearningPathManagementPageInput } from '../models/learning-path-management-page-input.model';
import { NAVIGATORS } from '../lmm.config';
import { SelectEvent } from '@progress/kendo-angular-layout';
@Component({
  selector: 'learning-path-management-page',
  templateUrl: './learning-path-management-page.component.html'
})
export class LearningPathManagementPageComponent extends BasePageComponent {
  public createMode: boolean;
  public LMMTabConfiguration: typeof LMMTabConfiguration = LMMTabConfiguration;
  public learningPathManagementPageInput: RouterPageInput<LearningPathManagementPageInput, LMMTabConfiguration, unknown> = NAVIGATORS[
    LMMRoutePaths.LearningPathManagementPage
  ] as RouterPageInput<LearningPathManagementPageInput, LMMTabConfiguration, unknown>;

  public get selectedTab(): LMMTabConfiguration {
    return this.learningPathManagementPageInput.activeTab != null
      ? this.learningPathManagementPageInput.activeTab
      : LMMTabConfiguration.LearningPathsTab;
  }
  public searchText: string = '';
  public filterData: unknown = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();

  constructor(public moduleFacadeService: ModuleFacadeService, public navigationPageService: NavigationPageService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public canCreateLearningPath(): boolean {
    return (
      this.currentUser &&
      LearningPathModel.hasCreateLearningPathPermission(this.currentUser) &&
      this.selectedTab === LMMTabConfiguration.LearningPathsTab
    );
  }

  public onViewLearningPath(dataItem: LearningPathViewModel): void {
    if (this.canViewLearningPath()) {
      this.navigationPageService.navigateTo(
        LMMRoutePaths.LearningPathDetailPage,
        {
          activeTab: LMMTabConfiguration.LearningPathInfoTab,
          data: {
            id: dataItem.id,
            mode: LearningPathDetailMode.View
          }
        },
        this.learningPathManagementPageInput
      );
    }
  }

  public onCreateNewLearningPath(): void {
    this.navigationPageService.navigateTo(
      LMMRoutePaths.LearningPathDetailPage,
      {
        activeTab: LMMTabConfiguration.LearningPathInfoTab,
        data: {
          id: null,
          mode: LearningPathDetailMode.NewLearningPath
        }
      },
      this.learningPathManagementPageInput
    );
  }

  public onTabSelected(event: SelectEvent): void {
    this.learningPathManagementPageInput.activeTab = learningPathManagementPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.learningPathManagementPageInput);
  }

  public resetFilter(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
  }

  protected onInit(): void {
    this.getNavigatePageData();
  }

  private getNavigatePageData(): void {
    const navigateData: RouterPageInput<LearningPathManagementPageInput, LMMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.learningPathManagementPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }

  private canViewLearningPath(): boolean {
    return LearningPathModel.hasViewLearningPathDetailPermission(this.currentUser);
  }
}

export const learningPathManagementPageTabIndexMap = {
  0: LMMTabConfiguration.LearningPathsTab
};
