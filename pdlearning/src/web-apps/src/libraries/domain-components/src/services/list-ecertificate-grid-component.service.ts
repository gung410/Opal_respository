import { ECertificateRepository, SearchECertificateResult, SearchECertificateType } from '@opal20/domain-api';

import { ECertificateViewModel } from '../models/ecertificate-view.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ListECertificateGridComponentService {
  constructor(private eCertificateRepository: ECertificateRepository) {}

  public loadECertificates(
    searchText: string = '',
    searchCourseType: SearchECertificateType = SearchECertificateType.CustomECertificateTemplateManagement,
    skipCount: number = 0,
    maxResultCount: number = 25,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<ECertificateViewModel>> {
    return this.progressECertificates(
      this.eCertificateRepository.searchECertificateTemplates(searchText, searchCourseType, skipCount, maxResultCount),
      checkAll,
      selectedsFn
    );
  }

  private progressECertificates(
    eCertificateObs: Observable<SearchECertificateResult>,
    checkAll: boolean = false,
    selectedsFn: () => Dictionary<boolean> | null = null
  ): Observable<OpalGridDataResult<ECertificateViewModel>> {
    return eCertificateObs.pipe(
      map(searchECertificateResult => {
        return <OpalGridDataResult<ECertificateViewModel>>{
          data: searchECertificateResult.items.map(_ =>
            ECertificateViewModel.createFromModel(_, checkAll, selectedsFn != null ? selectedsFn() : {})
          ),
          total: searchECertificateResult.totalCount
        };
      })
    );
  }
}
