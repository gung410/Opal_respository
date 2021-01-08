import { SurveyQueryModeEnum, SurveyStatus } from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Injectable()
export class StandaloneSurveySearchTermService {
  public searchText?: string;
  public searchStatuses: SurveyStatus[];
  public state: PageChangeEvent = {
    skip: 0,
    take: 25
  };
  public queryMode: SurveyQueryModeEnum = SurveyQueryModeEnum.All;
}
