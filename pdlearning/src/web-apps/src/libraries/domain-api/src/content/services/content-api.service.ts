import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { DigitalContent, IDigitalContent } from '../models/digital-content';
import { DigitalContentExpiryInfoModel, IDigitalContentExpiryInfoModel } from '../models/digital-content-expiry-info-model';
import { DigitalContentSearchResult, IDigitalContentSearchResult } from '../dtos/digital-content-search-result';
import { IDigitalContentSearchRequest, IGetPendingApprovalDigitalContentRequest } from '../dtos/digital-content-search-request';

import { IArchiveRequest } from '../../share/dtos/archive-form-request';
import { IDigitalContentChangeApprovalStatusRequest } from '../dtos/digital-content-change-approval-status-request';
import { IDigitalContentRenameRequest } from '../dtos/digital-content-rename-request';
import { IDigitalContentRequest } from '../dtos/digital-content-request';
import { ITransferOwnershipRequest } from '../../share/dtos/transfer-ownership-request';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class ContentApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.contentApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public createDigitalContent(request: IDigitalContentRequest, showSpinner: boolean = true): Promise<DigitalContent> {
    return this.post<IDigitalContentRequest, IDigitalContent>('/contents/create', request, showSpinner)
      .pipe(map(p => new DigitalContent(p)))
      .toPromise();
  }

  public transferOwnerShip(request: ITransferOwnershipRequest, showSpinner: boolean = true): Promise<void> {
    return this.put<ITransferOwnershipRequest, void>('/contents/transfer', request, showSpinner).toPromise();
  }

  public updateDigitalContent(request: IDigitalContentRequest, showSpinner?: boolean): Promise<DigitalContent> {
    return this.put<IDigitalContentRequest, IDigitalContent>('/contents/update', request, showSpinner)
      .pipe(map(p => new DigitalContent(p)))
      .toPromise();
  }

  public deleteDigitalContent(id: string): Promise<void> {
    return this.delete<void>(`/contents/${id}`).toPromise();
  }

  public renameDigitalContent(request: IDigitalContentRenameRequest): Promise<void> {
    return this.put<IDigitalContentRenameRequest, void>('/contents/rename', request).toPromise();
  }

  public duplicateDigitalContent(id: string): Promise<DigitalContent> {
    return this.put<Object, IDigitalContent>(`/contents/${id}/clone`, {})
      .pipe(map(p => new DigitalContent(p)))
      .toPromise();
  }

  public changeApprovalStatus(request: IDigitalContentChangeApprovalStatusRequest): Promise<void> {
    return this.put<IDigitalContentChangeApprovalStatusRequest, void>(`/contents/changeApprovalStatus`, request).toPromise();
  }

  public archiveContent(request: IArchiveRequest): Observable<void> {
    return this.put<IArchiveRequest, void>(`/contents/archive`, request);
  }

  public getExpiryInfoOfDigitalContents(
    digitalContentIds: string[],
    showSpinner: boolean = true
  ): Promise<DigitalContentExpiryInfoModel[]> {
    return this.post<string[], IDigitalContentExpiryInfoModel[]>('/contents/getExpiryInfoOfDigitalContents', digitalContentIds, showSpinner)
      .pipe(map(_ => _.map(__ => new DigitalContentExpiryInfoModel(__))))
      .toPromise();
  }

  public searchDigitalContent(request: IDigitalContentSearchRequest, showSpinner?: boolean): Promise<DigitalContentSearchResult> {
    return this.post<IDigitalContentSearchRequest, IDigitalContentSearchResult>('/contents/search', request, showSpinner)
      .pipe(map(_ => new DigitalContentSearchResult(_)))
      .toPromise();
  }

  public getPendingApprovalDigitalContents(
    request: IGetPendingApprovalDigitalContentRequest,
    showSpinner?: boolean
  ): Promise<DigitalContentSearchResult> {
    return this.post<IGetPendingApprovalDigitalContentRequest, IDigitalContentSearchResult>(
      '/contents/getPendingApprovalDigitalContents',
      request,
      showSpinner
    )
      .pipe(map(_ => new DigitalContentSearchResult(_)))
      .toPromise();
  }

  public getDigitalContent(id: string, showSpinner?: boolean): Promise<DigitalContent> {
    return this.get<IDigitalContent>(`/contents/${id}`, null, showSpinner)
      .pipe(
        map(response => {
          response.expiredDate = response.expiredDate ? new Date(response.expiredDate) : null;
          response.startDate = response.startDate ? new Date(response.startDate) : null;
          return new DigitalContent(response);
        })
      )
      .toPromise();
  }

  public getDigitalContentByIds(ids: string[], showSpinner: boolean = true): Promise<DigitalContent[]> {
    return this.post<string[], IDigitalContent[]>('/contents/getByIds', ids, showSpinner)
      .pipe(map(response => response.map(_ => new DigitalContent(_))))
      .toPromise();
  }

  public getDigitalContentByVersionTrackingId(id: string): Promise<DigitalContent> {
    return this.get<IDigitalContent>(`/contents/byVersionTrackingId/${id}`)
      .pipe(map(_ => new DigitalContent(_)))
      .toPromise();
  }
}
