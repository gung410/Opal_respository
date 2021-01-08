import { CatalogResourceType, LogicOperator } from './catalog-search-results.model';

export interface ResourceTypeFilter {
  fieldName: string;
  operator: LogicOperator;
  values: string[];
}

export type ResourceStatusFilter = Record<CatalogResourceType, string[]>;

export type ResourceTypeFilters = Record<CatalogResourceType, ResourceTypeFilter[]>;
