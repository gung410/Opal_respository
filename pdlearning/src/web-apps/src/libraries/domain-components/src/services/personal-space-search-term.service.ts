import { FileType, PersonalFileSortField, SortDirection } from '@opal20/domain-api';

import { Injectable } from '@angular/core';
import { PageChangeEvent } from '@progress/kendo-angular-grid';

@Injectable()
export class PersonalSpaceSearchTermService {
  public searchText?: string;
  public filterByExtensions: string[] = [];
  public filterByType: FileType = FileType.All;
  public state: PageChangeEvent = {
    skip: 0,
    take: 25
  };
  public sortBy: PersonalFileSortField = PersonalFileSortField.CreatedDate;
  public sortDirection: SortDirection = SortDirection.Descending;

  public sortModeId: number = 1;
}
