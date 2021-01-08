import { IFilter } from '@opal20/infrastructure';
import { SearchClassRunType } from '../models/search-classrun-type.model';

export interface ISearchClassRunRequest {
  courseId: string;
  searchType?: SearchClassRunType;
  searchText: string;
  notStarted: boolean;
  notEnded: boolean;
  skipCount: number;
  maxResultCount: number;
  loadHasContentInfo?: boolean;
  filter: IFilter;
}
