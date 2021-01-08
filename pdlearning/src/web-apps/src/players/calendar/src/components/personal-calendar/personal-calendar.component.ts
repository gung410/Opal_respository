import { AfterContentInit, Component, Input } from '@angular/core';
import { BasePageComponent, DateUtils, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CommunityCalendarApiService,
  EventType,
  GetAllEventsRequest,
  ICommunityEventDetailsModel,
  IEventDetailsModel,
  IPersonalEventModel,
  PersonalCalendarApiService,
  PersonalEventFilterModel,
  UserInfoModel,
  eventResources,
  personalCalendarModel
} from '@opal20/domain-api';
import { DateChangeEvent, EventClickEvent, SlotClickEvent } from '@progress/kendo-angular-scheduler';

import { CalendarDialogRefService } from '../../services/calendar-dialog-ref.service';
import { CheckableSettings } from '@progress/kendo-angular-treeview';
import { CommunityEventDetailFormComponent } from './../community-event-detail-form/community-event-detail-form.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Observable } from 'rxjs/internal/Observable';
import { PersonalEventDetailFormComponent } from '../personal-event-detail-form/personal-event-detail-form.component';
import { filter } from 'lodash-es';

@Component({
  selector: 'personal-calendar',
  templateUrl: './personal-calendar.component.html'
})
export class PersonalCalendarComponent extends BasePageComponent implements AfterContentInit {
  @Input() public calendarHeight: string = '100%';

  public get checkableSettings(): CheckableSettings {
    return {
      checkChildren: true,
      checkParents: true,
      enabled: true,
      mode: 'multiple',
      checkOnClick: true
    };
  }

  public personalCalendarModel = personalCalendarModel;
  public resources = eventResources;
  public selectedDate: Date = new Date();
  public workDayStart: string = '09:00';
  public workDayEnd: string = '18:00';

  public events: Array<IPersonalEventModel> = [];
  public filteredEvents: Array<IPersonalEventModel> = [];

  public eventFilterModel: PersonalEventFilterModel = new PersonalEventFilterModel();

  public monthOffset: number = 7;
  public monthOffsetPoint: number = 6;
  public dayOffsetPoint: number = 1;
  public yearOffsetPoint: number = new Date().getFullYear();
  public defaultFetchEventsRequest: GetAllEventsRequest;

  private createEventDialogRef: DialogRef;

  constructor(
    private personalCalendarApiService: PersonalCalendarApiService,
    private communityCalendarApiService: CommunityCalendarApiService,
    private calendarDialogRefService: CalendarDialogRefService,
    protected moduleFacadeService: ModuleFacadeService
  ) {
    super(moduleFacadeService);

    this.defaultFetchEventsRequest = {
      numberMonthOffset: this.monthOffset,
      offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
    };
  }

  public ngAfterContentInit(): void {
    this.fetchEvents();
  }

  public ngOnDestroy(): void {
    if (this.createEventDialogRef) {
      this.createEventDialogRef.close();
    }
  }

  public createEvent(): void {
    this.createEventDialogRef = this.moduleFacadeService.dialogService.open({
      content: PersonalEventDetailFormComponent
    });

    this.subscribe(this.createEventDialogRef.result, success => {
      if (success === true) {
        this.fetchEvents();
      }
    });
  }

  public onOpenFilterPanel(): void {
    document.body.classList.add('opened-calendar-selector');
  }

  public onCloseFilterPanel(): void {
    document.body.classList.remove('opened-calendar-selector');
  }

  public fetchEvents(request: GetAllEventsRequest = this.defaultFetchEventsRequest): void {
    this.subscribe(this.personalCalendarApiService.getAllEvents(request), events => {
      this.events = events;
      this.filteredEvents = events;
    });
  }

  // On Time Slot double click event handler is triggered when user double-clicks on a calendar's time slot.
  public onSlotDblClick(slotClickEvent: SlotClickEvent): void {
    // Add (60 * 15000 = 15 minutes) = 1 time slot to enable selecting the current time slot.
    const currentSlotDate = new Date(slotClickEvent.start.getTime() + 60 * 15000);
    if (DateUtils.compareDate(currentSlotDate, new Date()) < 0) {
      return;
    }
    this.createEventDialogRef = this.moduleFacadeService.dialogService.open({
      content: PersonalEventDetailFormComponent
    });
    const createEventPopup = this.createEventDialogRef.content.instance as PersonalEventDetailFormComponent;
    const now = new Date();

    // To make sure start time > now and end time > start time:
    // Set start time equals now + 1 min (for UX) if now passes time slot start, else use time slot start.
    const startTime = now > slotClickEvent.start ? DateUtils.addMinutes(now, 1) : slotClickEvent.start;
    // Set end time equals start time + 1 min (end time must be greater than start time atleast 1 min) if start time >= time slot end.
    const endTime = startTime >= slotClickEvent.end ? DateUtils.addMinutes(startTime, 1) : slotClickEvent.end;

    if (slotClickEvent.isAllDay) {
      createEventPopup.setAllDaySlot();
      createEventPopup.setTimeSlot(startTime, DateUtils.addMinutes(new Date(startTime), 30));
    } else {
      createEventPopup.setTimeSlot(startTime, endTime);
    }
    this.subscribe(this.createEventDialogRef.result, success => {
      if (success === true) {
        this.fetchEvents();
      }
    });
  }

  public onEventDblClick(eventClickEvent: EventClickEvent): void {
    const eventId = eventClickEvent.event.dataItem.eventId;
    let getEventDetailsService: Observable<IEventDetailsModel>;
    let getCommunityEventDetailsService: Observable<ICommunityEventDetailsModel>;
    const eventType: string = eventClickEvent.event.dataItem.type;
    switch (eventType) {
      case EventType.Personal:
        getEventDetailsService = this.personalCalendarApiService.getEventDetailsById(eventId);
        this.subscribe(getEventDetailsService, eventDetails => {
          this.createEventDialogRef = this.moduleFacadeService.dialogService.open({
            content: PersonalEventDetailFormComponent
          });

          const configurationPopup = this.createEventDialogRef.content.instance as PersonalEventDetailFormComponent;
          configurationPopup.selectedEvent = eventDetails;

          this.subscribe(this.createEventDialogRef.result, success => {
            if (success === true) {
              this.fetchEvents();
            }
          });
        });
        break;
      case EventType.Community:
        getCommunityEventDetailsService = this.communityCalendarApiService.getEventDetailsById(eventId);
        this.subscribe(getCommunityEventDetailsService, eventDetails => {
          this.createEventDialogRef = this.moduleFacadeService.dialogService.open({
            content: CommunityEventDetailFormComponent
          });

          const configurationPopup = this.createEventDialogRef.content.instance as CommunityEventDetailFormComponent;
          configurationPopup.selectedEventId = eventId;
          configurationPopup.communityId = eventDetails.communityId;
          configurationPopup.selectedEventSource = eventDetails.source;
          configurationPopup.communitySelectable = false;
          configurationPopup.initEvent(eventId);

          this.subscribe(this.calendarDialogRefService.communityEventDialogRef, result => {
            this.createEventDialogRef.close();
            if (result.action === 'update') {
              this.fetchEvents();
            }
          });
        });
        break;
      default:
        throw new Error(`Event type not found: ${eventType ? eventType : 'undefined'}`);
    }
  }

  public onDateChange(dateChangeEvent: DateChangeEvent): void {
    const newYear = dateChangeEvent.selectedDate.getFullYear();
    if (newYear !== this.yearOffsetPoint) {
      this.yearOffsetPoint = newYear;

      this.fetchEvents({
        numberMonthOffset: this.monthOffset,
        offsetPoint: new Date(this.yearOffsetPoint, this.monthOffsetPoint, this.dayOffsetPoint).toJSON()
      });
    }
  }

  public onFilterKeyChange(): void {
    if (this.eventFilterModel.isFilterSourcesEmpty()) {
      this.filteredEvents = this.events;
    } else {
      const filterSources = this.eventFilterModel.getFilterSources();
      const userId = UserInfoModel.getMyUserInfo().id;

      this.filteredEvents = filter(
        this.events,
        x =>
          (x.createdBy === userId && this.eventFilterModel.myEventFilter.some(source => source === x.source)) ||
          filterSources.some(source => source === x.source)
      );
    }
  }
}
