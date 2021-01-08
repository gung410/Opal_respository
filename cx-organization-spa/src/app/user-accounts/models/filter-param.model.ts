import { AppConstant } from 'app/shared/app.constant';

export class FilterParamModel {
  organizationSearchKey?: string;
  status?: any[];
  userTypeIds?: any[];
  parentDepartmentId?: any[];
  jsonDynamicData?: any[];
  externallyMastered?: boolean;
  filterOnParentHd?: boolean;
  groupIds?: any[];
  orderBy?: string;
  pageIndex?: number;
  pageSize?: number;
  includeFilterOption?: boolean;
  userIds?: any[];

  constructor(data?: Partial<FilterParamModel>) {
    this.organizationSearchKey =
      data && data.organizationSearchKey ? data.organizationSearchKey : '';
    this.status = data && data.status ? data.status : [];
    this.userTypeIds = data && data.userTypeIds ? data.userTypeIds : [];
    this.jsonDynamicData =
      data && data.jsonDynamicData ? data.jsonDynamicData : [];
    this.externallyMastered =
      data && data.externallyMastered ? data.externallyMastered : null;
    this.parentDepartmentId =
      data && data.parentDepartmentId ? data.parentDepartmentId : [];
    this.filterOnParentHd =
      data && data.filterOnParentHd ? data.filterOnParentHd : undefined;
    this.groupIds = data && data.groupIds ? data.groupIds : [];
    this.orderBy = data && data.orderBy ? data.orderBy : '';
    this.pageIndex = data && data.pageIndex ? data.pageIndex : 1;
    this.pageSize =
      data && data.pageSize ? data.pageSize : AppConstant.ItemPerPage;
    this.includeFilterOption =
      data && data.includeFilterOption ? data.includeFilterOption : false;
    this.userIds = data && data.userIds ? data.userIds : [];
  }
}
