import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { CommunityResultModel } from './models/csl-community.model';
import { Injectable } from '@angular/core';

@Injectable()
export class CslRepositoryContext extends BaseRepositoryContext {
  public communityResultSubject: BehaviorSubject<Dictionary<CommunityResultModel>> = new BehaviorSubject({});
}
