import { AccessRightSearchResult, IAccessRightSearchResult } from '../dtos/access-right-search-result';
import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { IAccessRightGetAllCollaboratorsIdRequest, IAccessRightSearchRequest } from '../dtos/access-right-search-request';

import { AccessRightType } from '../models/access-right-type';
import { IAccessRightCreateRequest } from '../dtos/access-right-create-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class AccessRightApiService extends BaseBackendService {
  protected serviceType: AccessRightType = AccessRightType.DigitalContent;

  protected get apiUrl(): string {
    switch (this.serviceType) {
      case AccessRightType.DigitalContent:
        return AppGlobal.environment.contentApiUrl + '/content';
      case AccessRightType.Form:
        return AppGlobal.environment.formApiUrl + '/form';
      case AccessRightType.LnaForm:
        return AppGlobal.environment.lnaFormApiUrl + '/form';
      default:
        return AppGlobal.environment.apiUrl;
    }
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public initApiService(serviceType: AccessRightType): void {
    this.serviceType = serviceType;
  }

  public searchAccessRights(request: IAccessRightSearchRequest, showSpinner: boolean = true): Promise<AccessRightSearchResult> {
    return this.post<IAccessRightSearchRequest, IAccessRightSearchResult>('/collaboration/search', request, showSpinner)
      .pipe(map(_ => new AccessRightSearchResult(_)))
      .toPromise();
  }

  public getAllListAccessRightId(request: IAccessRightGetAllCollaboratorsIdRequest): Observable<string[]> {
    return this.post<IAccessRightGetAllCollaboratorsIdRequest, string[]>('/collaboration/getAllIds', request);
  }

  public addAccessRight(request: IAccessRightCreateRequest, showSpinner?: boolean): Promise<void> {
    return this.post<IAccessRightCreateRequest, void>('/collaboration/create', request, showSpinner).toPromise();
  }

  public deleteAccessRight(id: string, showSpinner?: boolean): Promise<void> {
    return this.delete<void>(`/collaboration/${id}`).toPromise();
  }
}
