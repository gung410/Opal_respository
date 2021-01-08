import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { StandaloneSurveyModel } from './models/lna-form.model';

@Injectable()
export class StandaloneSurveyRepositoryContext extends BaseRepositoryContext {
  public formListDataSubject: BehaviorSubject<Dictionary<StandaloneSurveyModel>> = new BehaviorSubject({});
}
