import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { SurveySection } from './models/form-section';

@Injectable()
export class LnaFormSectionRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<SurveySection>> = new BehaviorSubject({});
}
