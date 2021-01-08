import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';

import { IPOCSingleForm } from '../models/poc-single-form';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class POCSingleFormService extends BaseBackendService {
  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getData(): Observable<IPOCSingleForm> {
    return this.get<IPOCSingleForm>('/SingleForm/GetData');
  }

  public updateData(data: IPOCSingleForm): Observable<boolean> {
    return this.post<IPOCSingleForm, boolean>('/SingleForm/UpdateData', data);
  }
}
