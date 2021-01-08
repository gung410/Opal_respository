import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CommunityEventDetailsModel, ICommunityEventDetailsModel } from '../models/community-event-details-model';
import { CommunityEventModel, ICommunityEventModel } from '../models/community-event-model';

import { GetAllEventsRequest } from '../dtos/get-all-events-request';
import { GetEventsByCommunityIdRequest } from '../dtos/get-events-community-by-id-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SaveCommunityEventRequest } from '../dtos/save-community-event-request';
import { map } from 'rxjs/operators';

@Injectable()
export class CommunityCalendarApiService extends BaseBackendService {
  private readonly communityCalendarRoute: string = '/calendars/communities';

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  protected get apiUrl(): string {
    return AppGlobal.environment.calendarApiUrl;
  }

  public getMyEvents(request: GetAllEventsRequest, showSpinner: boolean = true): Observable<ICommunityEventModel[]> {
    return this.get<ICommunityEventModel[]>(`${this.communityCalendarRoute}/myEvents`, request, showSpinner).pipe(
      map(_ => _.map(r => new CommunityEventModel(r)))
    );
  }

  public getEventDetailsById(eventId: string, showSpinner: boolean = true): Observable<ICommunityEventDetailsModel> {
    return this.get<ICommunityEventDetailsModel>(`${this.communityCalendarRoute}/events/${eventId}`, null, showSpinner).pipe(
      map(_ => new CommunityEventDetailsModel(_))
    );
  }
  public getEventsByCommunityId(request: GetEventsByCommunityIdRequest, showSpinner: boolean = true): Observable<ICommunityEventModel[]> {
    return this.get<ICommunityEventModel[]>(`${this.communityCalendarRoute}/${request.communityId}/events`, request, showSpinner).pipe(
      map(_ => _.map(e => new CommunityEventModel(e)))
    );
  }
  public createEvent(request: SaveCommunityEventRequest, showSpinner: boolean = true): Observable<CommunityEventDetailsModel> {
    return this.post<SaveCommunityEventRequest, ICommunityEventDetailsModel>(
      `${this.communityCalendarRoute}/events`,
      request,
      showSpinner
    ).pipe(map(_ => new CommunityEventDetailsModel(_)));
  }

  public updateEvent(request: SaveCommunityEventRequest, showSpinner: boolean = true): Observable<CommunityEventDetailsModel> {
    return this.put<SaveCommunityEventRequest, ICommunityEventDetailsModel>(
      `${this.communityCalendarRoute}/events`,
      request,
      showSpinner
    ).pipe(map(_ => new CommunityEventDetailsModel(_)));
  }

  public deleteEvent(eventId: string, showSpinner: boolean = true): Observable<void> {
    return this.delete(`${this.communityCalendarRoute}/events/${eventId}`, showSpinner);
  }

  // Webinar
  public createWebinarEvent(request: SaveCommunityEventRequest, showSpinner: boolean = true): Observable<CommunityEventDetailsModel> {
    return this.post<SaveCommunityEventRequest, ICommunityEventDetailsModel>(
      `${this.communityCalendarRoute}/events/webinar`,
      request,
      showSpinner
    ).pipe(map(_ => new CommunityEventDetailsModel(_)));
  }

  public updateWebinarEvent(request: SaveCommunityEventRequest, showSpinner: boolean = true): Observable<CommunityEventDetailsModel> {
    return this.put<SaveCommunityEventRequest, ICommunityEventDetailsModel>(
      `${this.communityCalendarRoute}/events/webinar`,
      request,
      showSpinner
    ).pipe(map(_ => new CommunityEventDetailsModel(_)));
  }

  public deleteWebinarEvent(eventId: string, showSpinner: boolean = true): Observable<void> {
    return this.delete(`${this.communityCalendarRoute}/events/${eventId}/webinar`, showSpinner);
  }
}
