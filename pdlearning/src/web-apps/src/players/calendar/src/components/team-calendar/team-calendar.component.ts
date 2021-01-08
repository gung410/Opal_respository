import '@progress/kendo-ui/js/kendo.gantt';

import { BasePageComponent, DateUtils, ModuleFacadeService } from '@opal20/infrastructure';
import {
  GetTeamMemberEventOverviewsRequest,
  GetTeamMemberEventsRequest,
  IBaseTeamEventViewModel,
  ISharedTeamModel,
  ITeamCalendarApiResolverService,
  ITeamMemberModel,
  TeamCalendarApiResolverService,
  TeamCalendarContextService,
  TeamCalendarRepository,
  TeamCalendarSwitchViewService,
  TeamCalendarViewMode,
  TeamCalendarViewType,
  TeamMemberEventViewModel
} from '@opal20/domain-api';

import { Component } from '@angular/core';
@Component({
  selector: 'team-calendar',
  templateUrl: './team-calendar.component.html'
})
export class TeamCalendarComponent extends BasePageComponent {
  public isPersonalTeamCalendar: boolean = false;
  public sharedTeamList: ISharedTeamModel[] = [];
  public valueDropdownSharedTeam: ISharedTeamModel;

  public learnerList: ITeamMemberModel[] = [];
  public selectedUserIds: string[] = [];

  public dropDownYearData: Array<number> = [];
  public selectedViewType: TeamCalendarViewType;
  public selectedYear: number;
  public resourceTeamCalendar: TeamMemberEventViewModel[] = [];

  private intervalDropdownYear: number = 50;
  private defaultGetTeamMemberEventOverviewsRequest: GetTeamMemberEventOverviewsRequest;
  private baseTeamEventViewModel: IBaseTeamEventViewModel = { summary: false, expanded: false };
  private teamMemberOverviewsData: TeamMemberEventViewModel[] = [];
  private teamCalendarApiService: ITeamCalendarApiResolverService;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private teamCalendarSwitchViewService: TeamCalendarSwitchViewService,
    private teamCalendarContextService: TeamCalendarContextService,
    private teamCalendarRepository: TeamCalendarRepository,
    private teamCalendarApiResolverService: TeamCalendarApiResolverService
  ) {
    super(moduleFacadeService);

    this.subscribe(this.teamCalendarApiResolverService.teamCalendarApiService, service => {
      this.teamCalendarApiService = service;

      this.isPersonalTeamCalendar = this.teamCalendarApiService.getViewMode() === TeamCalendarViewMode.LearnerView;
      this.getMySharedTeams();

      this.registerTeamCalendarContextChange();
      this.initDropdownYears();
      this.getTeamMembers();
      this.setValueGetTeamMemberEventOverviewsRequest(this.selectedViewType);
      this.getTeamMemberEventOverviewsData();
    });
  }

  /**
   * When change view mode of the team calendar (switch current month, three months or year)
   * Keep value of the new view and get new data for calendar.
   */
  public onChangeViewType(viewType: TeamCalendarViewType): void {
    this.selectedViewType = viewType;
    this.setTeamCalendarContext();
    this.setValueGetTeamMemberEventOverviewsRequest(viewType);
    this.getTeamMemberEventOverviewsData();
  }

  /**
   * When expanded any learner on calendar
   * Get list event of this learner
   */
  public onExpandDetailLearner(learnerId: string): void {
    const getTeamMemberEventsRequest: GetTeamMemberEventsRequest = {
      memberId: learnerId,
      rangeStart: this.defaultGetTeamMemberEventOverviewsRequest.rangeStart,
      rangeEnd: this.defaultGetTeamMemberEventOverviewsRequest.rangeEnd
    };
    this.subscribe(this.teamCalendarApiService.getTeamMemberEvents(getTeamMemberEventsRequest, true), res => {
      const teamMemberEvents = res.map(item => {
        return new TeamMemberEventViewModel(
          {
            id: item.id,
            title: item.subTitle,
            start: new Date(item.startAt),
            end: new Date(item.endAt),
            parentId: learnerId
          },
          this.baseTeamEventViewModel
        );
      });
      this.addDataSourceTeamCalendar(teamMemberEvents);
    });
  }

  /**
   * When filter by learner
   * Set new data for calendar
   */
  public onSelectedLearnerChange(): void {
    this.setTeamCalendarContext();

    if (this.selectedUserIds.length <= 0) {
      this.setDataSourceOfGantt(this.teamMemberOverviewsData);
      return;
    }

    const result = this.teamMemberOverviewsData.filter(
      x => this.selectedUserIds.includes(x.id) || (x.parentId !== undefined && this.selectedUserIds.includes(x.parentId))
    );
    this.setDataSourceOfGantt(result);
  }

  /**
   * When change dropdown year
   * Get new data for team calendar
   */
  public onChangeYear(year: number): void {
    this.selectedYear = year;
    this.setTeamCalendarContext();
    this.setValueGetTeamMemberEventOverviewsRequest(this.selectedViewType, year - DateUtils.getNow().getFullYear());
    this.getTeamMemberEventOverviewsData();
  }

  /**
   * When change dropdown shared team
   * Get new data for team calendar
   */
  public dropdownSharedTeamChange(): void {
    if (this.isPersonalTeamCalendar) {
      this.teamCalendarApiService.initParams(this.valueDropdownSharedTeam.accessShareId);
      this.getTeamMembers();
      this.getTeamMemberEventOverviewsData();
      this.selectedUserIds = [];
    }
  }

  public onOpenFilterPanel(): void {
    document.body.classList.add('opened-calendar-selector');
  }

  public onCloseFilterPanel(): void {
    document.body.classList.remove('opened-calendar-selector');
  }

  private getMySharedTeams(): void {
    if (this.isPersonalTeamCalendar) {
      this.subscribe(this.teamCalendarRepository.getMySharedTeams(), res => {
        this.sharedTeamList = res;
        this.valueDropdownSharedTeam = res[0];
      });
    }
  }

  /**
   * Get learner list of AO
   */
  private getTeamMembers(): void {
    this.subscribe(this.teamCalendarRepository.getTeamMembers(), res => {
      this.learnerList = res;
    });
  }

  private registerTeamCalendarContextChange(): void {
    this.subscribe(this.teamCalendarContextService.teamCalendarContext, context => {
      if (context) {
        this.selectedYear = context.selectedYear;
        this.selectedViewType = context.selectedViewType;
        this.selectedUserIds = context.selectedUserIds;
      } else {
        this.selectedYear = DateUtils.getNow().getFullYear();
        this.selectedViewType = TeamCalendarViewType.ThreeMonths;
        this.selectedUserIds = [];
      }
    });
  }

  private setTeamCalendarContext(): void {
    this.teamCalendarContextService.changeContext({
      selectedUserIds: this.selectedUserIds,
      selectedViewType: this.selectedViewType,
      selectedYear: this.selectedYear
    });
  }

  private initDropdownYears(): void {
    const pastYear = DateUtils.addYear(new Date(), -this.intervalDropdownYear).getFullYear();
    const futureYear = DateUtils.addYear(new Date(), this.intervalDropdownYear).getFullYear();
    this.dropDownYearData = Array.from({ length: futureYear - pastYear + 1 }, (_, k) => k + pastYear);
  }

  private setValueGetTeamMemberEventOverviewsRequest(
    type: TeamCalendarViewType,
    addYear: number = this.selectedYear - DateUtils.getNow().getFullYear()
  ): void {
    const rangeDate = this.teamCalendarSwitchViewService.caculateRangeDateGantt(type, addYear);
    this.defaultGetTeamMemberEventOverviewsRequest = {
      rangeStart: rangeDate.start.toJSON(),
      rangeEnd: rangeDate.end.toJSON()
    };
  }

  private getTeamMemberEventOverviewsData(): void {
    this.subscribe(this.teamCalendarApiService.getTeamMemberEventOverviews(this.defaultGetTeamMemberEventOverviewsRequest, true), res => {
      const teamMemberEventOverviews = res.map(item => {
        return new TeamMemberEventViewModel({
          id: item.id,
          title: item.title,
          start: new Date(item.startAt),
          end: new Date(item.endAt)
        });
      });
      this.initDataSourceTeamCalendar(teamMemberEventOverviews);
    });
  }

  private initDataSourceTeamCalendar(newData: TeamMemberEventViewModel[]): void {
    this.teamMemberOverviewsData = newData;
    this.onSelectedLearnerChange();
  }

  private addDataSourceTeamCalendar(data: TeamMemberEventViewModel[]): void {
    this.teamMemberOverviewsData = this.teamMemberOverviewsData.concat(data);
    this.onSelectedLearnerChange();
  }

  private setDataSourceOfGantt(dataSourse: TeamMemberEventViewModel[]): void {
    this.resourceTeamCalendar = dataSourse;
  }
}
