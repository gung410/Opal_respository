import { CatalogResourceType, LogicOperator } from '../models/catalog-search-results.model';
import { ResourceStatusFilter, ResourceTypeFilters } from '../models/catalog-search-request.model';

// Document: https://cxtech.atlassian.net/wiki/spaces/MP/pages/1130856651/Search+API
export interface ICatalogSearchRequest {
  searchText: string;
  page: number;
  limit: number;
  searchFields: string[]; // ['title', 'description', 'code', 'externalcode', 'tag']

  searchCriteria?: { [propOrTagName: string]: [LogicOperator, ...string[]] };
  searchCriteriaOr?: { [propOrTagName: string]: [LogicOperator, ...string[]] };

  resourceTypesFilter?: CatalogResourceType[];
  statisticResourceTypes?: CatalogResourceType[];

  includeMetaData?: boolean;
  useFuzzy: boolean;
  useSynonym: boolean;
}

export interface ICatalogSearchV2Request {
  searchText: string;
  page: number;
  limit: number;
  searchFields: string[]; // ['title', 'description', 'code', 'externalcode', 'tag']

  statisticResourceTypes?: CatalogResourceType[];

  resourceType?: CatalogResourceType;
  resourceStatusFilters?: ResourceStatusFilter;
  filters?: ResourceTypeFilters;
  enableStatistics?: boolean;

  includeMetaData?: boolean;
  useFuzzy: boolean;
  useSynonym: boolean;
  attachmentTypeFilter?: string;
}

export const SEARCH_DATE_FORMAT = 'DD/MM/YYYY HH:mm:ss';
