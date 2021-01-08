import { IPersonalFileModel, PersonalFileModel } from '../models/personal-file.model';

import { FileType } from '../models/file-type.enum';
import { PersonalFileSortField } from '../models/personal-file-sort-item.model';
import { SortDirection } from './../../share/dtos/sort-direction';

export class SearchPersonalFileRequest {
  public pagedInfo: { skipCount: number; maxResultCount: number };
  public searchText: string | undefined;
  public filterByExtensions?: string[];
  public filterByType?: FileType[];
  public sortDirection?: SortDirection;
  public sortBy?: PersonalFileSortField;
}

export interface ISearchPersonalFileResponse {
  totalCount: number;
  items: IPersonalFileModel[];
}

export class SearchPersonalFileResponse {
  public totalCount: number;
  public items: PersonalFileModel[];

  constructor(data?: ISearchPersonalFileResponse) {
    if (data != null) {
      this.totalCount = data.totalCount;
      this.items = data.items.map(p => new PersonalFileModel(p));
    }
  }
}
