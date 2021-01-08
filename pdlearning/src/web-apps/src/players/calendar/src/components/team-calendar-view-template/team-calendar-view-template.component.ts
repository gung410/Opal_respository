import { AfterViewInit, Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import {
  QuarterlyGanttTemplate,
  TeamCalendarConfigModel,
  TeamCalendarSwitchViewService,
  TeamCalendarViewType,
  TeamMemberEventViewModel,
  ThreeMonthsGanttTemplate
} from '@opal20/domain-api';

import { DateUtils } from '@opal20/infrastructure';

// tslint:disable-next-line:no-any
declare var kendo: any;

@Component({
  selector: 'team-calendar-view-template',
  templateUrl: './team-calendar-view-template.component.html'
})
export class TeamCalendarViewTemplateComponent implements AfterViewInit {
  @ViewChild('gantt', { static: false }) public ganttElement: ElementRef;
  @Input() public dropDownYearData: Array<number>;
  @Input() public activeViewType: TeamCalendarViewType;
  @Input()
  public set activeYear(year: number) {
    this._activeYear = year;
    this.valueDropdownYear = year;
    this.reRenderGantt();
  }
  public get activeYear(): number {
    return this._activeYear;
  }

  @Input()
  public set teamMembersEvents(data: TeamMemberEventViewModel[]) {
    this._teamMembersEvents = data;
    this.setDataSourceOfGantt(data);
    this.reRenderGantt();
  }
  public get teamMembersEvents(): TeamMemberEventViewModel[] {
    return this._teamMembersEvents;
  }

  @Output() public viewTypeChanged = new EventEmitter<TeamCalendarViewType>();
  @Output() public detailLearnerExpanded = new EventEmitter<string>();
  @Output() public yearChanged = new EventEmitter<number>();
  // tslint:disable-next-line: no-any
  public gantt: any;
  public valueDropdownYear: number = DateUtils.getNow().getFullYear();

  private _activeYear: number;
  private _teamMembersEvents: TeamMemberEventViewModel[] = [];
  private adjustHeight: number = 2.85;
  private teamCalendarConfig: TeamCalendarConfigModel = new TeamCalendarConfigModel();
  constructor(private teamCalendarSwitchViewService: TeamCalendarSwitchViewService) {}

  public ngAfterViewInit(): void {
    this.configGanttTemplate(this.activeYear - DateUtils.getNow().getFullYear());

    const config = this.teamCalendarConfig.getConfig(this.adjustHeight);
    this.gantt = kendo
      .jQuery(this.ganttElement.nativeElement)
      .kendoGantt(config)
      .data('kendoGantt');

    this.handleEventsTeamCalendar();
    this.gantt.view(this.activeViewType);
  }

  public dropdownYearChange(): void {
    this.configGanttTemplate(this.valueDropdownYear - DateUtils.getNow().getFullYear());
    this.yearChanged.emit(this.valueDropdownYear);
  }

  private handleEventsTeamCalendar(): void {
    this.expandTeamMemberOverview();
    this.switchViewTeamCalendar();
  }

  private expandTeamMemberOverview(): void {
    /**The Kendo UI Gantt widget does not expose an expand event.
     * https://docs.telerik.com/kendo-ui/api/javascript/ui/gantt/events/databound
     * When a task is expanded, the dataBound events are triggered.
     * Foreach list task, get children's task of the task was expanded. */
    this.gantt.bind('dataBound', () => {
      const dataSource = this.gantt.dataSource.data();
      dataSource.forEach(element => {
        this.teamMembersEvents.find(x => x.id === element.id).expanded = element.expanded;

        if (element.expanded) {
          const expandedData = this.teamMembersEvents.find(x => x.parentId !== undefined && x.parentId === element.id);
          if (expandedData === undefined) {
            this.detailLearnerExpanded.emit(element.id);
          }
        }
      });
    });
  }

  private switchViewTeamCalendar(): void {
    /**https://docs.telerik.com/kendo-ui/api/javascript/ui/gantt/events/navigate
     * When user switch view mode, base on new view type for get data.*/
    this.gantt.bind('navigate', e => {
      if (e.view) {
        this.activeViewType = e.view as TeamCalendarViewType;
        this.viewTypeChanged.emit(this.activeViewType);
      }
    });
  }

  private setDataSourceOfGantt(dataSourse: TeamMemberEventViewModel[]): void {
    if (this.gantt !== undefined) {
      this.gantt.dataSource.data(dataSourse);
      this.gantt.refresh();
    }
  }

  private configGanttTemplate(addYear: number): void {
    const quarterlyRange = this.teamCalendarSwitchViewService.caculateRangeDateGantt(TeamCalendarViewType.Year, addYear);
    const threeMonthsRange = this.teamCalendarSwitchViewService.caculateRangeDateGantt(TeamCalendarViewType.ThreeMonths, addYear);
    const cunrrentMonthRange = this.teamCalendarSwitchViewService.caculateRangeDateGantt(TeamCalendarViewType.CurrentMonth, addYear);

    kendo.ui.QuarterlyGanttView = new QuarterlyGanttTemplate().create(quarterlyRange.start.getFullYear());
    kendo.ui.ThreeMonthsGanttView = new ThreeMonthsGanttTemplate().create(threeMonthsRange.start, threeMonthsRange.end);
    kendo.ui.CurrentMonthGanttView = new ThreeMonthsGanttTemplate().create(cunrrentMonthRange.start, cunrrentMonthRange.end);
  }

  private reRenderGantt(): void {
    /**The difference between methods: refresh() and view() in gantt
     * Refresh method:
        * Only renders all in dataSource: tasks and dependencies using the current data items
        * Does not re-render the template title and headers
     * View method:
        * Re-render all gantt chart with the selected view and new dataSource

     * In this case the selected duration was changed so we need re-render all gantt chart
     * https://docs.telerik.com/kendo-ui/api/javascript/ui/gantt/methods/refresh
     * https://docs.telerik.com/kendo-ui/api/javascript/ui/gantt/methods/view */
    if (this.gantt !== undefined) {
      this.gantt.view(this.activeViewType);
    }
  }
}
