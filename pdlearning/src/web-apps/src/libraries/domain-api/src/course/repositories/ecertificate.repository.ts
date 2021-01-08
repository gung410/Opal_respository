import { BaseRepository, Utils } from '@opal20/infrastructure';
import { Observable, from } from 'rxjs';

import { CourseRepositoryContext } from '../course-repository-context';
import { ECertificateApiService } from '../services/ecertificate-api.service';
import { ECertificateLayoutModel } from '../models/ecertificate-layout.model';
import { ECertificateTemplateModel } from '../models/ecertificate-template.model';
import { ISaveECertificateTemplateRequest } from '../dtos/save-ecertificate-template-request';
import { Injectable } from '@angular/core';
import { SearchECertificateResult } from '../dtos/search-ecertificate-result';
import { SearchECertificateType } from '../models/search-ecertificate-type.model';

@Injectable()
export class ECertificateRepository extends BaseRepository<CourseRepositoryContext> {
  constructor(context: CourseRepositoryContext, private apiSvc: ECertificateApiService) {
    super(context);
  }
  public searchECertificateTemplates(
    searchText: string = '',
    searchType: SearchECertificateType = SearchECertificateType.CustomECertificateTemplateManagement,
    skipCount: number = 0,
    maxResultCount: number = 10
  ): Observable<SearchECertificateResult> {
    return this.processUpsertData(
      this.context.eCertificateTemplateSubject,
      implicitLoad => from(this.apiSvc.searchECertificateTemplates(searchText, searchType, skipCount, maxResultCount, !implicitLoad)),
      'searchECertificateTemplates',
      [searchText, searchType, skipCount, maxResultCount],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult.items = apiResult.items.map(item => repoData[item.id]).filter(_ => _ != null);
        return apiResult;
      },
      apiResult => apiResult.items,
      x => x.id,
      true,
      null,
      null,
      ECertificateTemplateModel.optionalProps
    );
  }

  public getECertificateTemplateById(id: string): Observable<ECertificateTemplateModel> {
    return this.processUpsertData(
      this.context.eCertificateTemplateSubject,
      implicitLoad => from(this.apiSvc.getECertificateTemplateById(id, !implicitLoad)),
      'getECertificateTemplateById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        apiResult = repoData[apiResult.id];
        return apiResult;
      },
      apiResult => [apiResult],
      x => x.id,
      true,
      null,
      null,
      ECertificateTemplateModel.optionalProps
    );
  }

  public getECertificateLayouts(): Observable<ECertificateLayoutModel[]> {
    return this.processUpsertData(
      this.context.eCertificateLayoutSubject,
      implicitLoad => from(this.apiSvc.getECertificateLayouts(!implicitLoad)),
      'getECertificateLayouts',
      [],
      'implicitReload',
      (repoData, apiResult) => {
        return apiResult.map(_ => repoData[_.id]).filter(p => p != null);
      },
      apiResult => apiResult,
      x => x.id,
      true
    );
  }

  public getECertificateLayoutById(id: string): Observable<ECertificateLayoutModel> {
    return this.processUpsertData(
      this.context.eCertificateLayoutSubject,
      implicitLoad => from(this.apiSvc.getECertificateLayoutById(id, !implicitLoad)),
      'getECertificateLayoutById',
      [id],
      'implicitReload',
      (repoData, apiResult) => {
        return repoData[apiResult.id];
      },
      apiResult => [apiResult],
      x => x.id,
      true
    );
  }

  public saveECertificateTemplate(request: ISaveECertificateTemplateRequest): Observable<ECertificateTemplateModel> {
    return from(
      this.apiSvc.saveECertificateTemplate(request).then(eCertificateTemplate => {
        this.upsertData(
          this.context.eCertificateTemplateSubject,
          [new ECertificateTemplateModel(Utils.cloneDeep(request.data))],
          item => item.id,
          true,
          null,
          ECertificateTemplateModel.optionalProps
        );
        return eCertificateTemplate;
      })
    );
  }

  public deleteECertificateTemplate(eCertificateTemplateId: string): Promise<void> {
    return this.apiSvc.deleteECertificateTemplate(eCertificateTemplateId).then(_ => {
      this.processRefreshData('searchECertificateTemplates');
    });
  }
}
