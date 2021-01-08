import { FormQueryModeEnum, FormStatus } from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Injectable()
export class FormSearchTermService {
  public searchText?: string;
  public searchStatuses: FormStatus[];
  public state: PageChangeEvent = {
    skip: 0,
    take: 25
  };
  public queryMode: FormQueryModeEnum = FormQueryModeEnum.All;
  public isSurveyTemplate?: boolean;
}
