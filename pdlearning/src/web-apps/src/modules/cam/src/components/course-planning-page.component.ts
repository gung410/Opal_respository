import { BasePageComponent, IGridFilter, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CAMRoutePaths,
  CAMTabConfiguration,
  CoursePlanningCycleDetailMode,
  CoursePlanningCycleViewModel,
  NavigationPageService,
  RouterPageInput
} from '@opal20/domain-components';
import { Component, HostBinding } from '@angular/core';
import { CoursePlanningCycle, UserInfoModel } from '@opal20/domain-api';

import { CoursePlanningPageInput } from '../models/course-planning-page-input.model';
import { NAVIGATORS } from '../cam.config';
import { SelectEvent } from '@progress/kendo-angular-layout';

@Component({
  selector: 'course-planning-page',
  templateUrl: './course-planning-page.component.html'
})
export class CoursePlanningPageComponent extends BasePageComponent {
  public searchText: string = '';
  public filterData: unknown = null;
  public filter: IGridFilter = {
    search: '',
    filter: null
  };
  public coursePlanningPageInput: RouterPageInput<CoursePlanningPageInput, CAMTabConfiguration, unknown> = NAVIGATORS[
    CAMRoutePaths.CoursePlanningPage
  ] as RouterPageInput<CoursePlanningPageInput, CAMTabConfiguration, unknown>;
  public CAMTabConfiguration: typeof CAMTabConfiguration = CAMTabConfiguration;

  private currentUser: UserInfoModel = UserInfoModel.getMyUserInfo();
  public get selectedTab(): CAMTabConfiguration {
    return this.coursePlanningPageInput.activeTab != null ? this.coursePlanningPageInput.activeTab : CAMTabConfiguration.PlanningCycleTab;
  }

  constructor(public moduleFacadeService: ModuleFacadeService, public navigationPageService: NavigationPageService) {
    super(moduleFacadeService);
  }

  @HostBinding('class.flex')
  public getFlexClass(): boolean {
    return true;
  }

  public onTabSelected(event: SelectEvent): void {
    this.coursePlanningPageInput.activeTab = coursePlanningPageTabIndexMap[event.index];
    this.navigationPageService.navigateByRouter(this.coursePlanningPageInput);
  }

  public onCreatePlanningCycle(): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.CoursePlanningCycleDetailPage,
      {
        activeTab: CAMTabConfiguration.CoursePlanningCycleInfoTab,
        subActiveTab: CAMTabConfiguration.AllCoursesOfPlanningCycleTab,
        data: {
          mode: CoursePlanningCycleDetailMode.NewPlanningCycle
        }
      },
      this.coursePlanningPageInput
    );
  }

  public onViewCoursePlanningCycle(data: CoursePlanningCycleViewModel): void {
    this.navigationPageService.navigateTo(
      CAMRoutePaths.CoursePlanningCycleDetailPage,
      {
        activeTab: CAMTabConfiguration.CoursePlanningCycleInfoTab,
        subActiveTab: CAMTabConfiguration.AllCoursesOfPlanningCycleTab,
        data: {
          mode: CoursePlanningCycleDetailMode.View,
          id: data.id
        }
      },
      this.coursePlanningPageInput
    );
  }

  public canCreateCoursePlanningCycle(): boolean {
    return (
      this.currentUser && CoursePlanningCycle.canVerifyCourse(this.currentUser) && this.selectedTab === CAMTabConfiguration.PlanningCycleTab
    );
  }

  public onSubmitSearch(): void {
    this.filter = {
      ...this.filter,
      search: this.searchText
    };
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
    const navigateData: RouterPageInput<CoursePlanningPageInput, CAMTabConfiguration, unknown> = this.getNavigateData();
    if (navigateData) {
      this.coursePlanningPageInput = navigateData;
    } else {
      this.navigationPageService.returnHome();
    }
  }
}

export const coursePlanningPageTabIndexMap = {
  0: CAMTabConfiguration.PlanningCycleTab
};
