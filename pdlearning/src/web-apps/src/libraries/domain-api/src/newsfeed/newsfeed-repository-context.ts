import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { IGetNewsfeedRequest } from './dtos/get-newsfeed-request.dto';
import { IPagedResultDto } from '../share/dtos/paged-result.dto';
import { Injectable } from '@angular/core';
import { Newsfeed } from './models/newsfeed.model';

@Injectable()
export class NewsfeedRepositoryContext extends BaseRepositoryContext {
  public loadedNewsfeedIdsResultSubject: BehaviorSubject<Dictionary<LoadedNewsfeedIdsResult>> = new BehaviorSubject({});
  public currentNewsfeedRequestSubject: BehaviorSubject<IGetNewsfeedRequest> = new BehaviorSubject({});
  public newsfeedSubject: BehaviorSubject<Dictionary<Newsfeed>> = new BehaviorSubject({});
}

export interface LoadedNewsfeedIdsResult extends IPagedResultDto<string> {}
