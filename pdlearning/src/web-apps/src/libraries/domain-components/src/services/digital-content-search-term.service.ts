import { DigitalContentQueryMode, DigitalContentStatus } from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Injectable()
export class DigitalContentSearchTermService {
  public searchText?: string;
  public searchStatuses: DigitalContentStatus[];
  public state: PageChangeEvent = {
    skip: 0,
    take: 25
  };
  public queryMode?: DigitalContentQueryMode = DigitalContentQueryMode.AllByCurrentUser;
  public digitalContentSortModeId: number = 1;
}
