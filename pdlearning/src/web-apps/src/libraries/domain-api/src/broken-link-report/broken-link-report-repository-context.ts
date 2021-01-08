import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { BrokenLinkReport } from './model/broken-link-report';
import { Injectable } from '@angular/core';

@Injectable()
export class BrokenLinkReportRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<BrokenLinkReport>> = new BehaviorSubject({});
}
