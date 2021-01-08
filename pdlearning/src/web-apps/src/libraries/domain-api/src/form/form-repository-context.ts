import { Assessment } from './models/assessment.model';
import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { FormModel } from './models/form.model';
import { Injectable } from '@angular/core';

@Injectable()
export class FormRepositoryContext extends BaseRepositoryContext {
  public formListDataSubject: BehaviorSubject<Dictionary<FormModel>> = new BehaviorSubject({});
  public assessmentSubject: BehaviorSubject<Dictionary<Assessment>> = new BehaviorSubject({});
}
