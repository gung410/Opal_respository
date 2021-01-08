import { AfterContentInit, Component } from '@angular/core';
import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CalendarIntergrationService,
  CommunityCalendarApiService,
  GetEventsByCommunityIdRequest,
  ICommunityEventModel,
  communityCalendarModel,
  eventResources
} from '@opal20/domain-api';
import { DateChangeEvent, EventClickEvent } from '@progress/kendo-angular-scheduler';

import { CommunityEventDetailFormComponent } from '../community-event-detail-form/community-event-detail-form.component';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'specific-community-calendar',
  templateUrl: './specific-community-calendar.component.html'
})
export class SpecificCommunityCalendarComponent extends BasePageComponent implements AfterContentInit {
  public communityId: string;
  public communityCalendarModel = communityCalendarModel;
  public resources = eventResources;
  public selectedDate: Date = new Date();
  public workDayStart: string = '09:00';
  public workDayEnd: string = '18:00';

  public events: Array<ICommunityEventModel> = [];

  public monthOffset: number = 7;
  public monthOffsetPoint: number = 6;
  public dayOffsetPoint: number = 1;
  public yearOffsetPoint: number = new Date().getFullYear();
  public defaultFetchEventsRequest: GetEventsByCommunityIdRequest;

  private createEventDialogRef: DialogRef;
  private window: Window = window.parent ? window.parent : window;
  constructor(
    private communityCalendarApiService: CommunityCalendarApiService,
    private calendarIntergrationService: CalendarIntergrationService,
    protected moduleFacadeService: ModuleFacadeService
  ) {
    super(moduleFacadeService);

    const params = this.calendarIntergrationService.GetParams();
    this.communityId = params.communityId as string;

    this.defaultFetchEventsRequest = {
      communityId: this.communityId,
      numberMonthOffset: this.monthOffset,
      offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
    };
  }

  public ngAfterContentInit(): void {
    this.fetchEvents();

    AppGlobal.calendarIntergrations.reloadCommunityCalendar = () => {
      this.fetchEvents({
        communityId: this.communityId,
        numberMonthOffset: this.monthOffset,
        offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
      });
    };
  }

  public createEvent(): void {
    this.createEventDialogRef = this.moduleFacadeService.dialogService.open({
      content: CommunityEventDetailFormComponent
    });
    const popupCreateEvent = this.createEventDialogRef.content.instance as CommunityEventDetailFormComponent;
    popupCreateEvent.communityId = this.communityId;
    this.subscribe(this.createEventDialogRef.result, success => {
      if (success === true) {
        this.fetchEvents();
      }
    });
  }

  public fetchEvents(request: GetEventsByCommunityIdRequest = this.defaultFetchEventsRequest): void {
    this.subscribe(this.communityCalendarApiService.getEventsByCommunityId(request), events => {
      this.events = events;
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
    const eventId = eventClickEvent.event.dataItem.eventId;
    this.window.postMessage(
      {
        key: 'Calendar_Events_OpenDetailEvent',
        eventId: eventId
      },
      '*'
    );
  }
}
