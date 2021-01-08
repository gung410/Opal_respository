import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { FormParticipant, IFormParticipant } from '../models/form-participant';
import { FormParticipantForm, IFormParticipantForm } from '../models/form-participant-form-model';
import { FormParticipantSearchResult, IFormParticipantSearchResult } from '../dtos/form-participant-search-result';

import { FormParticipantType } from '../models/form-participant-type.enum';
import { IAssignFormParticipantsRequest } from '../dtos/assign-form-participants-request';
import { IDeleteFormParticipantsRequest } from '../dtos/delete-form-participants-request';
import { IFormParticipantByFormIdsRequest } from '../dtos/form-participant-by-form-ids-request';
import { IFormParticipantSearchRequest } from '../dtos/form-participant-search-request';
import { IRemindFormParticipantsRequest } from '../dtos/remind-form-participants-request';
import { IUpdateFormParticipantStatusRequest } from '../dtos/update-form-participant-status-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class FormParticipantApiService extends BaseBackendService {
  protected serviceType: FormParticipantType = FormParticipantType.Form;

  protected get apiUrl(): string {
    switch (this.serviceType) {
      case FormParticipantType.Form:
        return AppGlobal.environment.formApiUrl;
      case FormParticipantType.LnaForm:
        return AppGlobal.environment.lnaFormApiUrl;
      default:
        return AppGlobal.environment.apiUrl;
    }
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public initApiService(serviceType: FormParticipantType): void {
    this.serviceType = serviceType;
  }

  public assignFormParticipants(request: IAssignFormParticipantsRequest, showSpinner?: boolean): Promise<IFormParticipantSearchResult> {
    return this.post<IAssignFormParticipantsRequest, IFormParticipantSearchResult>(
      '/form-participant/assign-participants',
      request,
      showSpinner
    )
      .pipe(map(items => new FormParticipantSearchResult(items)))
      .toPromise();
  }

  public getFormParticipantsByFormId(id: string, showSpinner?: boolean): Promise<FormParticipant[]> {
    return this.get<IFormParticipant[]>(`/form-participant/form-id/${id}`, null, showSpinner)
      .pipe(map(items => items.map(item => new FormParticipant(item))))
      .toPromise();
  }

  public getFormParticipantsByFormIds(request: IFormParticipantByFormIdsRequest, showSpinner?: boolean): Promise<FormParticipantForm[]> {
    return this.post<IFormParticipantByFormIdsRequest, IFormParticipantForm[]>(`/form-participant/form-ids`, request, showSpinner)
      .pipe(map(items => items.map(item => new FormParticipantForm(item))))
      .toPromise();
  }

  public deleteFormParticipants(request: IDeleteFormParticipantsRequest, showSpinner?: boolean): Promise<void> {
    return this.put<IDeleteFormParticipantsRequest, void>('/form-participant/delete', request, showSpinner).toPromise();
  }

  public searchFormParticipants(request: IFormParticipantSearchRequest, showSpinner?: boolean): Promise<FormParticipantSearchResult> {
    return this.post<IFormParticipantSearchRequest, IFormParticipantSearchResult>('/form-participant/form-id', request, showSpinner)
      .pipe(map(_ => new FormParticipantSearchResult(_)))
      .toPromise();
  }

  public remindFormParticipants(request: IRemindFormParticipantsRequest, showSpinner?: boolean): Promise<void> {
    return this.post<IRemindFormParticipantsRequest, void>('/form-participant/remind', request, showSpinner).toPromise();
  }

  public updateFormParticipantStatus(request: IUpdateFormParticipantStatusRequest, showSpinner?: boolean): Promise<void> {
    return this.post<IUpdateFormParticipantStatusRequest, void>(
      '/form-participant/update-participant-status',
      request,
      showSpinner
    ).toPromise();
  }

  public getMyParticipantData(formId: string, showSpinner?: boolean): Promise<FormParticipant> {
    return this.get<FormParticipant>(`/form-participant/my-participant-data/${formId}`, null, showSpinner)
      .pipe(map(_ => new FormParticipant(_)))
      .toPromise();
  }
}
