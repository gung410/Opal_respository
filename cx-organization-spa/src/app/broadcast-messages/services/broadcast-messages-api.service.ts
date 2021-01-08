import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { TargetUserType } from 'app/shared/constants/target-user-type.enum';
import { PagingResponseModel } from 'app/user-accounts/models/user-management.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  BroadcastMessagesDto,
  BroadcastMessagesFilterParams
} from '../models/broadcast-messages.model';
import { BroadcastNotification } from '../models/broadcast-notification.model';
import { RolesRequest } from '../requests-dto/roles-request';

@Injectable()
export class BroadcastMessagesApiService {
  private broadcastMessagesApiUrl: string = `${AppConstant.api.organization}/broadcast-messages`;
  private getRoleUrl: string = `${AppConstant.api.organization}/roles`;
  private commonBroadcastMessageUrl: string = `${AppConstant.api.communication}/communication/notification`;
  private communicationClientId: string = `${AppConstant.communicationClientId}`;

  constructor(private httpHelper: HttpHelpers, private http: HttpClient) {}

  getBroadcastMessages(
    filterParamModel: BroadcastMessagesFilterParams
  ): Observable<PagingResponseModel<BroadcastMessagesDto>> {
    return this.httpHelper.get<PagingResponseModel<BroadcastMessagesDto>>(
      this.broadcastMessagesApiUrl,
      filterParamModel
    );
  }

  getBroadcastMessageById(id: string): Observable<BroadcastMessagesDto> {
    return this.httpHelper
      .get<BroadcastMessagesDto>(`${this.broadcastMessagesApiUrl}/${id}`)
      .pipe(map((_) => new BroadcastMessagesDto(_)));
  }

  updateBroadcastMessage(
    request: BroadcastMessagesDto
  ): Observable<BroadcastMessagesDto> {
    return this.httpHelper
      .put(
        `${this.broadcastMessagesApiUrl}/${request.broadcastMessageId}`,
        request
      )
      .pipe(map((_) => new BroadcastMessagesDto(_)));
  }

  saveBroadcastMessage(
    request: BroadcastMessagesDto
  ): Observable<BroadcastMessagesDto> {
    return this.httpHelper.post(this.broadcastMessagesApiUrl, request);
  }

  deleteBroadcastMessage(id: string): Observable<any> {
    return this.httpHelper.delete(`${this.broadcastMessagesApiUrl}/${id}`);
  }

  changeBroadcastMessageStatus(
    broadcastMessage: BroadcastMessagesDto
  ): Observable<BroadcastMessagesDto> {
    return this.httpHelper.put(
      `${this.broadcastMessagesApiUrl}/${broadcastMessage.broadcastMessageId}`,
      broadcastMessage
    );
  }

  getRoles(request: RolesRequest): Observable<any> {
    return this.httpHelper.get(this.getRoleUrl, request);
  }

  editBroadcastMessage(
    broadCastMessageDto: BroadcastMessagesDto
  ): Observable<any> {
    const requestUrl = `${this.commonBroadcastMessageUrl}/${this.communicationClientId}/${broadCastMessageDto.broadcastMessageId}`;
    const broadcastNotification = new BroadcastNotification({
      active: !(broadCastMessageDto.status === 'Deactivate'),
      defaultBody: broadCastMessageDto.broadcastContent,
      defaultSubject: broadCastMessageDto.title,
      startDateUtc: broadCastMessageDto.validFromDate,
      endDateUtc: broadCastMessageDto.validToDate,
      isGlobal: broadCastMessageDto.targetUserType === TargetUserType.AllUser
    });

    return this.http.put(requestUrl, broadcastNotification, {
      observe: 'response'
    });
  }
}
