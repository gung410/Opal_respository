import { DigitalContentStatus, IPagedInfo } from '@opal20/domain-api';

export interface IDigitalContentSearchRequest {
  searchText: string;
  filterByStatus: DigitalContentStatus[];
  pagedInfo: IPagedInfo;
  withinCopyrightDuration?: boolean;
}
