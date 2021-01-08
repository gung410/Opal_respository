import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { FormSection } from './models/form-section';
import { Injectable } from '@angular/core';

@Injectable()
export class FormSectionRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<FormSection>> = new BehaviorSubject({});
}
