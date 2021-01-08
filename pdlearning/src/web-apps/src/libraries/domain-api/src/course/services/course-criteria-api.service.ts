import { BaseBackendService, CommonFacadeService } from '@opal20/infrastructure';
import { CourseCriteria, ICourseCriteria } from '../models/course-criteria.model';

import { ISaveCourseCriteriaRequest } from './../dtos/save-course-criteria-request';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable()
export class CourseCriteriaApiService extends BaseBackendService {
  protected get apiUrl(): string {
    return AppGlobal.environment.courseApiUrl;
  }

  constructor(protected commonFacadeService: CommonFacadeService) {
    super(commonFacadeService);
  }

  public getCourseCriteria(id: string, showSpinner?: boolean): Promise<CourseCriteria | null> {
    return this.get<ICourseCriteria>(`/courseCriteria/${id}`, null, showSpinner)
      .pipe(map(data => (data == null ? null : new CourseCriteria(data))))
      .toPromise();
  }

  public saveCourseCriteria(request: ISaveCourseCriteriaRequest): Promise<CourseCriteria> {
    return this.post<ISaveCourseCriteriaRequest, ICourseCriteria>(`/courseCriteria/save`, request)
      .pipe(map(data => new CourseCriteria(data)))
      .toPromise();
  }
}
