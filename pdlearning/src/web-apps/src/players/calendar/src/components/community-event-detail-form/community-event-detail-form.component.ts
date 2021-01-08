import { BaseFormComponent, DateUtils, MODULE_INPUT_DATA, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CalendarCommunityEventPrivacy,
  CalendarIntergrationService,
  CommunityApiService,
  CommunityCalendarApiService,
  EventRepeatFrequency,
  EventSource,
  ICommunityEventDetailsModel,
  ICommunityModel,
  SaveCommunityEventRequest
} from '@opal20/domain-api';
import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';

import { CalendarDialogRefService } from '../../services/calendar-dialog-ref.service';
import { CommunityEventRegularTemplateComponent } from '../community-event-regular-template/community-event-regular-template.component';
import { CommunityEventWebinarTemplateComponent } from '../community-event-webinar-template/community-event-webinar-template.component';
import { DatePickerComponent } from '@progress/kendo-angular-dateinputs';

@Component({
  selector: 'community-event-detail-form',
  templateUrl: './community-event-detail-form.component.html'
})
export class CommunityEventDetailFormComponent extends BaseFormComponent {
  @ViewChild('dateStart', null) public dateStart: DatePickerComponent;
  @ViewChild('dateEnd', null) public dateEnd: DatePickerComponent;
  @ViewChild('timeStart', null) public timeStart: DatePickerComponent;
  @ViewChild('timeEnd', null) public timeEnd: DatePickerComponent;
  @ViewChild(CommunityEventRegularTemplateComponent, { static: false }) public regularFormComponent: CommunityEventRegularTemplateComponent;
  @ViewChild(CommunityEventWebinarTemplateComponent, { static: false }) public webinarFormComponent: CommunityEventWebinarTemplateComponent;

  public selectedEvent: ICommunityEventDetailsModel;
  public selectedEventId: string = null;
  public communityId: string = null;
  public communitySelectable: boolean = true;
  public eventTypeSelectable: boolean = true;

  public today: Date = new Date();
  public selectedEventSource: EventSource | string = EventSource.Community;
  public communityEventTypeMapping: { [key: string]: string } = {
    [EventSource.Community]: 'Regular',
    [EventSource.Webinar]: 'Webinar'
  };
  public eventSourceEnum: typeof EventSource = EventSource;
  public formValidation: boolean;
  public isAllDay: boolean = false;
  public title: string;
  public acceptButtonName: string = 'Create';
  public isInvalidForm: boolean = false;
  public communities: ICommunityModel[] = [];
  public isComponentMode: boolean = false;
  public canUpdateEvent: boolean = false;
  private window: Window = window.parent ? window.parent : window;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private communityCalendarApiService: CommunityCalendarApiService,
    private communityApiService: CommunityApiService,
    private cdr: ChangeDetectorRef,
    private calendarDialogRefService: CalendarDialogRefService,
    private calendarIntegrationService: CalendarIntergrationService
  ) {
    super(moduleFacadeService);
    this.isComponentMode = this.calendarIntegrationService.isInternalIntergration.getValue();
    const routingData: {
      communityId?: string;
      eventType?: string;
      eventId?: string;
      editEvent?: boolean;
      createEvent?: boolean;
    } = moduleFacadeService.moduleDataService.getData(MODULE_INPUT_DATA);

    this.getCommunities();
    if (!this.isComponentMode) {
      this.title = routingData.editEvent ? 'Update Event' : 'New Event';
      this.acceptButtonName = routingData.editEvent ? 'Update' : 'Create';

      if (routingData.createEvent) {
        if (routingData.eventType) {
          this.selectedEventSource =
            routingData.eventType.toLowerCase() === this.communityEventTypeMapping[EventSource.Webinar].toLowerCase() ||
            routingData.eventType.toLowerCase() === EventSource.Webinar.toLowerCase()
              ? EventSource.Webinar
              : EventSource.Community;
        }

        if (routingData.communityId) {
          this.communitySelectable = false;
          this.communityId = routingData.communityId;
        }
      }

      if (routingData.editEvent && routingData.eventId) {
        this.selectedEventId = routingData.eventId;
        this.initEvent(this.selectedEventId);
      }
      const params = this.calendarIntegrationService.GetParams();
      this.eventTypeSelectable = params.eventTypeSelectable === 'false' ? false : true;
    } else {
      this.title = 'Update Event';
      this.acceptButtonName = 'Update';
    }
  }

  public ngOnInit(): void {
    this.initFormData();
  }

  public ngAfterViewInit(): void {
    if (!this.selectedEventId) {
      this.subscribeFormValuesChange();
    }
    if (!this.isComponentMode) {
      this.postHeightOfContainerToIframe(document.getElementsByClassName('event-detail-form')[0]);
    }
  }

  public subscribeEventOwner(): void {
    if (this.selectedEvent) {
      const isOwnEvent = this.communities.find(x => x.id === this.selectedEvent.communityId) != null;
      this.canUpdateEvent =
        this.selectedEventSource === EventSource.Webinar
          ? DateUtils.compareDate(this.selectedEvent.startAt, new Date()) > 0 && isOwnEvent
          : isOwnEvent;
    }
  }

  public initEvent(eventId: string): void {
    this.subscribe(this.communityCalendarApiService.getEventDetailsById(eventId), eventDetails => {
      this.selectedEvent = eventDetails;
      this.selectedEventSource = this.selectedEvent.source;
      this.communitySelectable = false;
      setTimeout(() => {
        if (this.selectedEventSource === EventSource.Community) {
          this.regularFormComponent.initEventData(this.selectedEvent);
        } else {
          this.webinarFormComponent.initEventData(this.selectedEvent);
        }
        this.subscribeFormValuesChange();
        this.subscribeEventOwner();
      });
    });
  }

  public subscribeFormValuesChange(): void {
    setTimeout(() => {
      if (this.regularFormComponent) {
        this.isInvalidForm = this.regularFormComponent.form.invalid;
        this.subscribe(this.regularFormComponent.form.valueChanges, data => {
          this.isInvalidForm = this.regularFormComponent.form.invalid;
        });
      }
      if (this.webinarFormComponent) {
        this.isInvalidForm = this.webinarFormComponent.form.invalid;
        this.subscribe(this.webinarFormComponent.form.valueChanges, data => {
          this.isInvalidForm = this.webinarFormComponent.form.invalid;
        });
        this.cdr.detectChanges();
      }
    });
  }

  public onCreate(): void {
    const request = this.getDataEventRequest();

    // For webinar event
    if (this.selectedEventSource === EventSource.Webinar) {
      this.subscribe(this.communityCalendarApiService.createWebinarEvent(request), () => this.emitEventReloadCalendar());
    } else {
      // For regular event
      this.subscribe(this.communityCalendarApiService.createEvent(request), () => this.emitEventReloadCalendar());
    }
  }

  public onUpdate(): void {
    const request = this.getDataEventRequest();

    // For webinar event
    if (this.selectedEventSource === EventSource.Webinar) {
      this.subscribe(this.communityCalendarApiService.updateWebinarEvent(request), () => this.emitEventReloadCalendar());
    } else {
      // For regular event
      this.subscribe(this.communityCalendarApiService.updateEvent(request), () => this.emitEventReloadCalendar());
    }
  }

  public onCancel(): void {
    this.calendarDialogRefService.notifyDialogAction('cancel');
    // The CSL module will be listen this event to close popup detail.
    this.window.postMessage(
      {
        key: 'Calendar_Events_CancelPopupDetail',
        content: null
      },
      '*'
    );
  }

  public onDelete(): void {
    this.window.postMessage(
      {
        // The CSL module will be listen this event to open popup confirm for deleting the selected event.
        key: 'Calendar_Events_DeleteEventClicked',
        eventId: this.selectedEventId,
        eventType: this.communityEventTypeMapping[this.selectedEventSource]
      },
      '*'
    );
  }

  private emitEventReloadCalendar(): void {
    this.calendarDialogRefService.notifyDialogAction('update');
    this.window.postMessage(
      {
        // The CSL module will be listen this event to close popup detail and close iframe and reload calendar.
        key: 'Calendar_Events_ClosePopupAndReloadCalendar',
        eventId: this.selectedEventId,
        eventType: this.communityEventTypeMapping[this.selectedEventSource]
      },
      '*'
    );
  }

  private getCommunities(): void {
    this.communityApiService.getOwnCommunities().subscribe(items => {
      this.communities = items;
    });
  }

  private postHeightOfContainerToIframe(element: Element): void {
    if (element) {
      setInterval(() => {
        const message = {
          key: 'HeightOfContainer',
          params: {
            height: element.scrollHeight
          }
        };
        window.parent.postMessage(message, '*');
      }, 1000);
    }
  }

  private getDataEventRequest(): SaveCommunityEventRequest {
    // For webinar event
    if (this.selectedEventSource === EventSource.Webinar) {
      const webinarForm = this.webinarFormComponent;
      if (webinarForm.form.invalid) {
        return;
      }
      const isScheduled: boolean = webinarForm.scheduled === 'scheduled';
      const startDate: Date = isScheduled ? webinarForm.form.value.eventDate : new Date();
      const endDate: Date = new Date(startDate);
      const timeStartsAt: Date = webinarForm.form.value.timeStartsAt;
      if (isScheduled && timeStartsAt) {
        startDate.setHours(timeStartsAt.getHours(), timeStartsAt.getMinutes());
      }
      const timeEndsAt: Date = webinarForm.form.value.timeEndsAt;
      if (timeEndsAt) {
        endDate.setHours(timeEndsAt.getHours(), timeEndsAt.getMinutes());
      }
      const request: SaveCommunityEventRequest = {
        id: webinarForm.selectedEventId,
        title: webinarForm.form.value.title,
        startAt: startDate,
        endAt: endDate,
        isAllDay: webinarForm.form.value.isAllDay,
        communityId: this.communitySelectable ? webinarForm.form.value.communityId : this.communityId,
        calendarEventSource: this.selectedEventSource,
        communityEventPrivacy: CalendarCommunityEventPrivacy.Private
      };
      return request;
    } else {
      // For regular event
      const regularForm = this.regularFormComponent;
      if (regularForm.form.invalid) {
        return;
      }

      const startAt: Date = regularForm.form.get('dateStartsAt').value;
      const timeStartsAt: Date = regularForm.form.get('timeStartsAt').value;
      if (timeStartsAt) {
        startAt.setHours(timeStartsAt.getHours(), timeStartsAt.getMinutes());
      }
      const endAt: Date = regularForm.form.get('dateEndsAt').value;
      const timeEndsAt: Date = regularForm.form.get('timeEndsAt').value;
      if (timeEndsAt) {
        endAt.setHours(timeEndsAt.getHours(), timeEndsAt.getMinutes());
      }

      const request: SaveCommunityEventRequest = {
        id: regularForm.selectedEventId,
        title: regularForm.form.value.title,
        startAt: startAt,
        endAt: endAt,
        isAllDay: regularForm.form.value.isAllDay,
        description: regularForm.form.value.description,
        communityId: this.communitySelectable ? regularForm.form.value.communityId : this.communityId,
        calendarEventSource: this.selectedEventSource,
        communityEventPrivacy: null,
        repeatFrequency: EventRepeatFrequency.None
      };

      if (regularForm.isDailyRepeat) {
        request.repeatFrequency = EventRepeatFrequency.Daily;
        request.endAt.setDate(request.startAt.getDate());
        request.repeatUntil = regularForm.repeatUntil.value;
      }
      return request;
    }
  }
}
