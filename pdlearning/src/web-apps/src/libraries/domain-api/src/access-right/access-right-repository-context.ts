import { AccessRight } from './models/access-right';
import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable()
export class AccessRightRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<AccessRight>> = new BehaviorSubject({});
}
