export type ResourceStatistics = { type: CatalogResourceType; total: number }[];
export interface ICatalogSearchResult {
  imsx_StatusInfo: ImsxStatusInfo;
  total: number;
  resources: IResource[];
  resourceStatistics: ResourceStatistics;
}

export interface ImsxStatusInfo {
  description: string;
  imsx_codeMajor: string;
}

export interface IResource {
  id: string;
  name: string;
  description: string;
  thumbnailUrl: string;
  publisher: string;
  metadata?: IMetadatum[];
  resourcetype: CatalogResourceType;
  publishdate: string;
  status: string; // published,..
}

export interface IMetadatum {
  name: string;
  id: string;
  value: string;
}

export type CatalogResourceType =
  | 'all'
  | 'course'
  | 'content'
  | 'microlearning'
  | 'learningpath'
  | 'community'
  | 'createdby'
  | 'memberships.id'
  | 'form';
export type LogicOperator = 'gt' | 'gte' | 'lt' | 'lte' | 'between' | 'contains' | 'equals';
