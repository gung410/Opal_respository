import { EntityStatus } from './membership.model';
import { LocalizedDataItem } from 'app/core/models/system-role';
import { Identity } from './common.model';

export class XCategoryTypeQueryModel {
  xCategoryTypeIds: number;
  xCategoryTypeExtIds: string;
  createdAfter: string;
  createdBefore: string;
  includeLocalizedData: string;
  orderBy: string;
}

export class XCategoryType {
  parentId: number;
  type: number;
  status: number;
  no: number;
  created: Date;
  localizedData: LocalizedDataItem[];
  identity: Identity;
  dynamicAttributes: any[];
  entityStatus: EntityStatus;
}

export class XCategoryQueryModel {
  xCategoryIds: number;
  xCategoryExtIds: string;
  activityIds: string;
  pageIds: string;
  questionIds: string;
  types: string;
  parentXCategoryIds: number;
  createdAfter: string;
  createdBefore: string;
  xCategoryName: string;
  includeLocalizedData: string;
  includeDynamicAttributes: string;
  orderBy: string;
}

export class XCategory {
  parentId: number;
  type: number;
  status: number;
  no: number;
  created: Date;
  localizedData: LocalizedDataItem[];
  identity: Identity;
  dynamicAttributes: any[];
  entityStatus: EntityStatus;
}
