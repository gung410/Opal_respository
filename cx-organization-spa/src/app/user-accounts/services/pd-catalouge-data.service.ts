import { Injectable } from '@angular/core';
import { HttpHelpers } from 'app-utilities/http-helpers';
import { AppConstant } from 'app/shared/app.constant';
import { Observable } from 'rxjs';

import { PDCatalogueEnumerationDto } from '../models/pd-catalogue.model';

@Injectable()
export class PDCatalogueDataService {
  constructor(private httpHelper: HttpHelpers) {}

  getPDCatalogue(): Observable<PDCatalogueEnumerationDto[]> {
    return this.httpHelper.get<PDCatalogueEnumerationDto[]>(
      `${AppConstant.api.learningCatalog}/enumeration`
    );
  }

  getPDCategory(
    categoryId: string,
    hierarchyLevel?: number,
    associationType?: string
  ): Observable<PDCatalogueEnumerationDto[]> {
    return this.httpHelper.get<PDCatalogueEnumerationDto[]>(
      `${AppConstant.api.learningCatalog}/catalogentries/explorer/${categoryId}`
    );
  }
}
