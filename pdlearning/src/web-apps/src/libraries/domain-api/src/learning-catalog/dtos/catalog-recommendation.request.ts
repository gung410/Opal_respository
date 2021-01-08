import { CatalogResourceType, LogicOperator } from '../models/catalog-search-results.model';
export interface ICatalogSuggestionRequest {
  userId: string;
  page: number;
  limit: number;
  enableHighlight: boolean;
  searchCriteria?: { [propOrTagName: string]: [LogicOperator, ...string[]] };
  includeMetaData?: boolean;
  resourceTypesFilter?: CatalogResourceType[];
  searchText?: string;
}
