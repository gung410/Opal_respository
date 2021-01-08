import { DigitalContentQueryMode } from './digital-content-query-mode.model';
import { DigitalContentSortField } from './digital-content-sort-field.model';
import { DigitalContentStatus } from '../../share/models/digital-content-status.enum';
import { IPagedInfo } from '../models/paged-info';
import { SortDirection } from '../../share/dtos/sort-direction';

export interface IDigitalContentSearchRequest {
  searchText: string;
  queryMode: DigitalContentQueryMode;
  sortField?: DigitalContentSortField;
  sortDirection?: SortDirection;
  filterByStatus: DigitalContentStatus[];
  filterByExtensions?: string[];
  withinDownloadableContent?: boolean;
  pagedInfo: IPagedInfo;
  includeContentForImportToCourse?: boolean;
  withinCopyrightDuration?: boolean;
}

export interface IGetPendingApprovalDigitalContentRequest {
  pagedInfo: IPagedInfo;
}
