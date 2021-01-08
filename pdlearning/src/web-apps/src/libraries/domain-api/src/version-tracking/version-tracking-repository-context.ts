import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { VersionTracking } from './models/version-tracking';

@Injectable()
export class VersionTrackingRepositoryContext extends BaseRepositoryContext {
  public contentsSubject: BehaviorSubject<Dictionary<VersionTracking>> = new BehaviorSubject({});
}
