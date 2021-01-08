import { StatusTypeEnum } from 'app/shared/constants/user-status-type.enum';

export class DepartmentQueryModel {
  includeParent: boolean;
  includeChildren: boolean;
  departmentTypeIds?: number[];
  countChildren?: boolean;
  maxChildrenLevel?: number;
  departmentEntityStatuses?: string[];
  departmentName: string;
  /* Dummy parameter.
   */
  searchText: string;
  countUser?: boolean;
  includeDepartmentType?: boolean;
  constructor(data?: Partial<DepartmentQueryModel>) {
    if (!data) {
      return;
    }
    this.searchText = data.searchText;
    this.includeChildren = data.includeChildren;
    this.includeParent = data.includeParent;
    this.departmentTypeIds = data.departmentTypeIds
      ? data.departmentTypeIds
      : undefined;
    this.countChildren = data.countChildren;
    this.maxChildrenLevel = data.maxChildrenLevel;
    this.departmentEntityStatuses = data.departmentEntityStatuses
      ? data.departmentEntityStatuses
      : [StatusTypeEnum.Active.code];
    this.countUser = data.countUser;
    this.includeDepartmentType = data.includeDepartmentType;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class DepartmentFilterGroupModel {
  name: string;
  groupConstant: string;
  options: DepartmentFilterOption[] = [];
  constructor(data?: DepartmentFilterGroupModel) {
    if (!data) {
      return;
    }
    this.name = data.name;
    this.options = data.options;
    this.groupConstant = data.groupConstant;
  }
}

// tslint:disable-next-line:max-classes-per-file
export class DepartmentFilterOption {
  isSelected?: boolean;
  value: any;
  objectId?: number;
  constructor(data?: Partial<DepartmentFilterOption>) {
    if (!data) {
      return;
    }
    this.isSelected = data.isSelected ? data.isSelected : false;
    this.value = data.value;
    this.objectId = data.objectId ? data.objectId : undefined;
  }
}
