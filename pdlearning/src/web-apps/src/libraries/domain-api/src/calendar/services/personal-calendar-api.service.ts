import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { EventDetailsModel, IEventDetailsModel } from '../models/event-details-model';
import { IPersonalEventModel, PersonalEventModel } from '../models/personal-event-model';

import { GetAllEventsRequest } from '../dtos/get-all-events-request';
import { GetPersonalEventByRangeRequest } from '../dtos/get-personal-event-by-range-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SavePersonalEventRequest } from '../dtos/save-personal-event-request';
import { map } from 'rxjs/operators';

@Injectable()
export class PersonalCalendarApiService extends BaseBackendService {
  private readonly personalCalendarRoute: string = '/calendars/personal';

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.calendarApiUrl;
  }

  public countEventsByRange(request: GetPersonalEventByRangeRequest, showSpinner: boolean = true): Observable<number> {
    return this.get<number>(`${this.personalCalendarRoute}/myEvents/range/count`, request, showSpinner);
  }

  public getEventsByRange(request: GetPersonalEventByRangeRequest, showSpinner: boolean = true): Observable<IPersonalEventModel[]> {
    return this.get<IPersonalEventModel[]>(`${this.personalCalendarRoute}/myEvents/range`, request, showSpinner).pipe(
      map(_ => _.map(e => new PersonalEventModel(e)))
    );
  }

  public getAllEvents(request: GetAllEventsRequest, showSpinner: boolean = true): Observable<IPersonalEventModel[]> {
    return this.get<IPersonalEventModel[]>(`${this.personalCalendarRoute}/myEvents`, request, showSpinner).pipe(
      map(_ => _.map(e => new PersonalEventModel(e)))
    );
  }

  public getEventDetailsById(eventId: string, showSpinner: boolean = true): Observable<IEventDetailsModel> {
    return this.get<IEventDetailsModel>(`${this.personalCalendarRoute}/events/${eventId}`, null, showSpinner).pipe(
      map(_ => new EventDetailsModel(_))
    );
  }

  public createEvent(request: SavePersonalEventRequest, showSpinner: boolean = true): Observable<EventDetailsModel> {
    return this.post<SavePersonalEventRequest, IEventDetailsModel>(`${this.personalCalendarRoute}/events`, request, showSpinner).pipe(
      map(_ => new EventDetailsModel(_))
    );
  }

  public updateEvent(request: SavePersonalEventRequest, showSpinner: boolean = true): Observable<EventDetailsModel> {
    return this.put<SavePersonalEventRequest, IEventDetailsModel>(`${this.personalCalendarRoute}/events`, request, showSpinner).pipe(
      map(_ => new EventDetailsModel(_))
    );
  }

  public deleteEvent(eventId: string, showSpinner: boolean = true): Observable<void> {
    return this.delete(`${this.personalCalendarRoute}/events/${eventId}`, showSpinner);
  }
}
