import { AfterContentInit, Component, OnDestroy, OnInit } from '@angular/core';
import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CommunityApiService,
  CommunityCalendarApiService,
  EventSource,
  GetAllEventsRequest,
  ICommunity,
  ICommunityEventModel,
  ICommunityTreeviewItem,
  communityCalendarModel
} from '@opal20/domain-api';
import { DateChangeEvent, EventClickEvent } from '@progress/kendo-angular-scheduler';
import { Observable, of } from 'rxjs';

import { CheckableSettings } from '@progress/kendo-angular-treeview';

@Component({
  selector: 'communities-calendar',
  templateUrl: './communities-calendar.component.html'
})
export class CommunitiesCalendarComponent extends BasePageComponent implements OnInit, AfterContentInit, OnDestroy {
  public communityHierarchy: ICommunityTreeviewItem[];
  public checkedKeys: string[] = [];

  public communityId: string;
  public communityCalendarModel = communityCalendarModel;
  public selectedDate: Date = new Date();
  public workDayStart: string = '08:00';

  public allEvents: Array<ICommunityEventModel> = [];
  public filteredEvents: Array<ICommunityEventModel> = [];
  public hierarchyFetched: boolean = false;

  public monthOffset: number = 7;
  public monthOffsetPoint: number = 6;
  public dayOffsetPoint: number = 1;
  public yearOffsetPoint: number = new Date().getFullYear();
  public defaultFetchEventsRequest: GetAllEventsRequest;
  public createOptions: string[] = ['Event', 'Webinar'];

  public get checkableSettings(): CheckableSettings {
    return {
      checkChildren: false,
      checkParents: false,
      checkOnClick: true,
      mode: 'multiple'
    };
  }

  private window: Window = window.parent ? window.parent : window;

  private readonly bodyClass = 'community-body';

  constructor(
    private communityService: CommunityApiService,
    private communityCalendarApiService: CommunityCalendarApiService,
    protected moduleFacadeService: ModuleFacadeService
  ) {
    super(moduleFacadeService);
  }

  public ngOnInit(): void {
    this.defaultFetchEventsRequest = {
      numberMonthOffset: this.monthOffset,
      offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
    };

    AppGlobal.calendarIntergrations.reloadCommunitiesCalendar = () => {
      this.fetchEvents({
        numberMonthOffset: this.monthOffset,
        offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
      });
    };

    this.fetchCommunityHierarchy();
    this.removeBodyClass(this.bodyClass);
  }

  public ngOnDestroy(): void {
    this.removeBodyClass(this.bodyClass);
  }

  public ngAfterContentInit(): void {
    this.fetchEvents();
  }

  public createEvent(): void {
    // The CSL module will be listen this event to close popup detail.
    this.window.postMessage(
      {
        key: 'Calendar_Events_CreateEventClicked'
      },
      '*'
    );
  }

  public fetchEvents(request: GetAllEventsRequest = this.defaultFetchEventsRequest): void {
    this.subscribe(this.communityCalendarApiService.getMyEvents(request, true), events => {
      this.allEvents = events;
      this.filteredEvents = this.checkedKeys.length > 0 ? this.filterEventsByCheckedKeys() : this.allEvents;
    });
  }

  public onDateChange(dateChangeEvent: DateChangeEvent): void {
    const newYear = dateChangeEvent.selectedDate.getFullYear();

    if (newYear !== this.yearOffsetPoint) {
      this.yearOffsetPoint = newYear;

      this.fetchEvents({
        communityId: this.communityId,
        numberMonthOffset: this.monthOffset,
        offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
      });
    }
  }

  public onEventDblClick(eventClickEvent: EventClickEvent): void {
    // The CSL module will be listen this event to open detail event iframe.
    this.window.postMessage(
      {
        key: 'Calendar_Events_OpenDetailEvent',
        eventId: eventClickEvent.event.dataItem.eventId
      },
      '*'
    );
  }

  public onCreateItemClick(eventType: string): void {
    // The CSL module will be listen this event to open create event iframe.
    this.window.postMessage(
      {
        key: 'Calendar_Events_CreateEventClicked',
        eventType: eventType === 'Event' ? EventSource.Community : EventSource.Webinar
      },
      '*'
    );
  }

  public children = (dataItem: ICommunityTreeviewItem): Observable<ICommunity[]> => of(dataItem.subCommunities);
  public hasChildren = (dataItem: ICommunityTreeviewItem): boolean => !!dataItem.subCommunities;

  public onCheckedKeysChange(): void {
    this.filteredEvents = this.filterEventsByCheckedKeys();
  }

  public onOpenFilterPanel(): void {
    document.body.classList.add('opened-calendar-selector');
  }

  public onCloseFilterPanel(): void {
    document.body.classList.remove('opened-calendar-selector');
  }

  private filterEventsByCheckedKeys(): ICommunityEventModel[] {
    const filteredEvents = this.allEvents.filter(e => this.checkedKeys.includes(e.communityId));
    return filteredEvents;
  }

  private fetchCommunityHierarchy(): void {
    this.subscribe(this.communityService.getCommunityHierarchyOfCurrentUser(true), hierarchy => {
      this.communityHierarchy = hierarchy;

      if (this.communityHierarchy.length !== 0 && this.checkedKeys.length === 0) {
        this.communityHierarchy.forEach(p => {
          this.checkedKeys.push(p.id);
          p.subCommunities.forEach(s => {
            this.checkedKeys.push(s.id);
          });
        });
      }
    });
  }

  private removeBodyClass(className: string): void {
    const body = document.getElementsByTagName('body')[0];
    body.classList.remove(className);
  }
}
