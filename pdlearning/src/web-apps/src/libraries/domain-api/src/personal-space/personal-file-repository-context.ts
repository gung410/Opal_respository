import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { PersonalFileModel } from './models/personal-file.model';

@Injectable()
export class PersonalFileRepositoryContext extends BaseRepositoryContext {
  public personalFileSubject: BehaviorSubject<Dictionary<PersonalFileModel>> = new BehaviorSubject({});
}
