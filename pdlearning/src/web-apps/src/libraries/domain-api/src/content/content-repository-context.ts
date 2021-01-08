import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { DigitalContent } from './models/digital-content';
import { Injectable } from '@angular/core';

@Injectable()
export class ContentRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<DigitalContent>> = new BehaviorSubject({});
}
