import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { ECertificateLayoutModel, IECertificateLayoutModel } from '../models/ecertificate-layout.model';
import { ECertificateTemplateModel, IECertificateTemplateModel } from './../models/ecertificate-template.model';
import { IPreviewECertificateTemplateModel, PreviewECertificateTemplateModel } from '../models/preview-ecertificate-template.model';

import { Constant } from '@opal20/authentication';
import { IGetPreviewECertificateTemplateRequest } from '../dtos/get-preview-ecertificate-template-request';
import { ISaveECertificateTemplateRequest } from '../dtos/save-ecertificate-template-request';
import { Injectable } from '@angular/core';
import { SearchECertificateResult } from './../dtos/search-ecertificate-result';
import { SearchECertificateType } from '../models/search-ecertificate-type.model';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class ECertificateApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public searchECertificateTemplates(
    searchText: string = '',
    searchType: SearchECertificateType = SearchECertificateType.CustomECertificateTemplateManagement,
    skipCount: number = 0,
    maxResultCount: number = 10,
    showSpinner?: boolean
  ): Promise<SearchECertificateResult> {
    return this.get<SearchECertificateResult>(
      `/ecertificate/template/search`,
      {
        searchText,
        searchType,
        skipCount,
        maxResultCount: maxResultCount == null ? Constant.MAX_ITEMS_PER_REQUEST : maxResultCount
      },
      showSpinner
    )
      .pipe(map(result => new SearchECertificateResult(result)))
      .toPromise();
  }

  public getECertificateTemplateById(id: string, showSpinner?: boolean): Promise<ECertificateTemplateModel> {
    return this.get<IECertificateTemplateModel>(`/ecertificate/template/${id}`, null, showSpinner)
      .pipe(map(data => new ECertificateTemplateModel(data)))
      .toPromise();
  }

  public saveECertificateTemplate(request: ISaveECertificateTemplateRequest): Promise<ECertificateTemplateModel> {
    return this.post<ISaveECertificateTemplateRequest, IECertificateTemplateModel>(`/ecertificate/template/save`, request)
      .pipe(map(data => new ECertificateTemplateModel(data)))
      .toPromise();
  }

  public getECertificateLayouts(showSpinner?: boolean): Promise<ECertificateLayoutModel[]> {
    return this.get<IECertificateLayoutModel[]>(`/ecertificate/layout/all`, null, showSpinner)
      .pipe(map(result => result.map(p => new ECertificateLayoutModel(p))))
      .toPromise();
  }

  public getECertificateLayoutById(id: string, showSpinner?: boolean): Promise<ECertificateLayoutModel> {
    return this.get<IECertificateLayoutModel>(`/ecertificate/layout/${id}`, null, showSpinner)
      .pipe(map(result => new ECertificateLayoutModel(result)))
      .toPromise();
  }

  public getPreviewECertificateTemplate(
    request: IGetPreviewECertificateTemplateRequest,
    showSpinner?: boolean
  ): Promise<IPreviewECertificateTemplateModel> {
    return this.post<IGetPreviewECertificateTemplateRequest, IPreviewECertificateTemplateModel>(
      `/ecertificate/template/preview`,
      request,
      showSpinner
    )
      .pipe(map(data => new PreviewECertificateTemplateModel(data)))
      .toPromise();
  }

  public deleteECertificateTemplate(id: string): Promise<void> {
    return this.delete<void>(`/ecertificate/${id}`).toPromise();
  }
}
