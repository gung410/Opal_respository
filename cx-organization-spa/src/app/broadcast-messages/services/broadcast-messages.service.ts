import { Injectable } from '@angular/core';
import { PagingResponseModel } from 'app/user-accounts/models/user-management.model';
import { Observable, of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { BroadcastMessagesFilterParams } from '../models/broadcast-messages.model';
import { BroadcastMessageViewModel } from '../models/broadcast-messages.view.model';
import { BroadcastMessagesApiService } from './broadcast-messages-api.service';

@Injectable()
export class BroadcastMessagesService {
  constructor(private broadcastDataSvc: BroadcastMessagesApiService) {}

  getBroadcastMessages(
    filterParamModel: BroadcastMessagesFilterParams
  ): Observable<PagingResponseModel<BroadcastMessageViewModel>> {
    return this.broadcastDataSvc.getBroadcastMessages(filterParamModel).pipe(
      switchMap((broadcastMessages) => {
        if (broadcastMessages.totalItems === 0) {
          return of(null);
        }

        return of(broadcastMessages);
      })
    );
  }
}
