import { AfterContentInit, Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { BaseComponent, DateUtils, ModuleFacadeService } from '@opal20/infrastructure';
import {
  GetAllEventsRequest,
  GetPersonalEventByRangeRequest,
  IPersonalEventModel,
  PersonalCalendarApiService,
  eventResources,
  personalCalendarModel
} from '@opal20/domain-api';

import { APP_BASE_HREF } from '@angular/common';
@Component({
  selector: 'personal-calendar-widget',
  templateUrl: './personal-calendar-widget.component.html'
})
export class PersonalCalendarWidgetComponent extends BaseComponent implements AfterContentInit {
  @Input() public calendarHeight: string = '100%';
  @Input() public showWorkHours: boolean = false;
  @Output() public openFullCalendarClicked: EventEmitter<unknown> = new EventEmitter();
  public resources = eventResources;
  public today: Date = new Date();
  public workDayStart: string = '08:00';
  public workDayEnd: string = '18:00';
  public events: Array<IPersonalEventModel> = [];
  public selectedViewIndex: number = 0;
  public personalCalendarModel = personalCalendarModel;
  public defaultFetchEventsRequest: GetAllEventsRequest;
  public dayEventCount: number = 0;
  public weekEventCount: number = 0;
  public monthEventCount: number = 0;
  public router: Router = AppGlobal.router;

  constructor(
    @Inject(APP_BASE_HREF) private baseHref: string,
    public moduleFacadeService: ModuleFacadeService,
    private personalCalendarApiService: PersonalCalendarApiService
  ) {
    super(moduleFacadeService);
  }

  public ngAfterContentInit(): void {
    this.fetchEvents();
    this.getEventCount();
  }

  public fetchEvents(request: GetAllEventsRequest = this.defaultFetchEventsRequest): void {
    this.subscribe(this.personalCalendarApiService.getEventsByRange(this.dayRange), events => {
      this.events = events;
      this.dayEventCount = events.length;
    });
  }

  public getEventCount(): void {
    this.subscribe(this.personalCalendarApiService.countEventsByRange(this.weekRange), count => {
      this.weekEventCount = count;
    });
    this.subscribe(this.personalCalendarApiService.countEventsByRange(this.monthRange), count => {
      this.monthEventCount = count;
    });
  }

  public get dayRange(): GetPersonalEventByRangeRequest {
    const request: GetPersonalEventByRangeRequest = {
      startAt: DateUtils.setTimeToStartOfTheDay(new Date()).toISOString(),
      endAt: DateUtils.setTimeToEndInDay(new Date()).toISOString()
    };
    return request;
  }

  public get weekRange(): GetPersonalEventByRangeRequest {
    const today = new Date();
    const firstDay = today.getDate() - today.getDay();
    const lastDay = firstDay + 6;
    const firstDateOfWeek = new Date(today.setDate(firstDay));
    const lastDateOfWeek = new Date(today.setDate(lastDay));
    const request: GetPersonalEventByRangeRequest = {
      startAt: DateUtils.setTimeToStartOfTheDay(firstDateOfWeek).toISOString(),
      endAt: DateUtils.setTimeToEndInDay(lastDateOfWeek).toISOString()
    };
    return request;
  }

  public get monthRange(): GetPersonalEventByRangeRequest {
    const today = new Date();
    const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDayOfMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    const request: GetPersonalEventByRangeRequest = {
      startAt: DateUtils.setTimeToStartOfTheDay(firstDayOfMonth).toISOString(),
      endAt: DateUtils.setTimeToEndInDay(lastDayOfMonth).toISOString()
    };
    return request;
  }

  public openFullCalendar(): void {
    this.openFullCalendarClicked.emit();
  }
}
