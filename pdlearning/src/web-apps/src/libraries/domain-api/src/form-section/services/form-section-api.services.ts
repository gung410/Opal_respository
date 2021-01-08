import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { FormSection, IFormSection } from '../models/form-section';

import { ICreateFormSectionRequest } from '../dtos/create-form-section-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/internal/operators/map';

@Injectable()
export class FormSectionApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.formApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public saveFormSection(request: ICreateFormSectionRequest): Promise<FormSection> {
    return this.post<ICreateFormSectionRequest, IFormSection>(`/form-sections`, request)
      .pipe(map(data => new FormSection(data)))
      .toPromise();
  }

  public getFormSectionsByFormId(formId: string): Promise<FormSection[]> {
    return this.get<IFormSection[]>(`/form-sections/form-id/${formId}`)
      .pipe(map(listSection => listSection.map(section => new FormSection(section))))
      .toPromise();
  }
}
