import { LogicOperator } from '../models/catalog-search-results.model';

export interface INewlyAddedCoursesRequest {
  sort?: 'desc' | 'asc';
  page: number;
  limit: number;
  searchCriteria?: { [propOrTagName: string]: [LogicOperator, ...string[]] };
  includeMetaData?: boolean;
}
