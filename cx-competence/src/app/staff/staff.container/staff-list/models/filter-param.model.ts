import { environment } from 'app-environments/environment';

export class FilterParamModel {
  public idpEmployeeSearchKey: string;
  public userIds?: any[];
  public departmentIds?: any[];
  public assessmentStatusTypeIds?: any[];
  public dueDate?: any[];
  public sortField?: string;
  public sortOrder?: string;
  public pageIndex?: number;
  public pageSize?: number;
  public includeFilterOptions?: {};
  public entityStatuses?: string[];
  public userDynamicAttributes?: any[];
  public externallyMastered?: boolean;
  public multiUserTypeIds?: any[][];
  public multiUserTypeExtIds?: string[][];
  public multiStatusTypeIds?: { LearningNeed: any[]; LearningPlan: any[] };
  public ageRanges?: any[];
  public createdAfter?: string;
  public createdBefore?: string;
  public expirationDateAfter?: string;
  public expirationDateBefore?: string;
  public organizationalUnitTypeIds?: any[];
  public multiUserGroupIds?: any[][];
  public exportOptions: ExportModel;
  public filterOnSubDepartment: boolean;
  public forCurrentUser?: boolean;
  public statusTypeLogs?: { LearningNeed: any[]; LearningPlan: any[] };

  constructor(data?: Partial<FilterParamModel>) {
    if (!data) {
      this.pageIndex = 1;
      this.multiUserTypeIds = [];
      this.multiUserTypeIds.push([]); // Role
      this.multiUserTypeExtIds = [];
      this.multiUserTypeExtIds.push([]); // Service scheme ...
      this.multiUserTypeExtIds.push([]); // Developmental role
      this.multiStatusTypeIds = {
        LearningNeed: [],
        LearningPlan: [],
      };
      this.organizationalUnitTypeIds = [];
      this.pageSize = environment.ItemPerPage;
      this.entityStatuses = [];
      this.multiUserGroupIds = [];
      this.multiUserGroupIds.push([]); // User pool (group of the users)
      this.multiUserGroupIds.push([]); // Approval group (for AO)
      this.userDynamicAttributes = [];
      this.statusTypeLogs = {
        LearningNeed: [],
        LearningPlan: [],
      };

      return;
    }
    this.forCurrentUser = data.forCurrentUser;
    this.idpEmployeeSearchKey = data.idpEmployeeSearchKey
      ? data.idpEmployeeSearchKey
      : '';
    this.userIds = data.userIds ? data.userIds : [];
    this.departmentIds = data.departmentIds ? data.departmentIds : [];
    this.assessmentStatusTypeIds = data.assessmentStatusTypeIds
      ? data.assessmentStatusTypeIds
      : [];
    this.dueDate = data.dueDate ? data.dueDate : [];
    this.sortField = data.sortField ? data.sortField : undefined;
    this.sortOrder = data.sortOrder ? data.sortOrder : undefined;
    this.pageIndex = data.pageIndex !== undefined ? data.pageIndex : 1;
    this.pageSize =
      data.pageSize !== undefined ? data.pageSize : environment.ItemPerPage;
    this.entityStatuses = data.entityStatuses ? data.entityStatuses : [];
    this.userDynamicAttributes = data.userDynamicAttributes
      ? data.userDynamicAttributes
      : [];
    this.externallyMastered =
      data.externallyMastered !== null ? data.externallyMastered : undefined;
    if (data.multiUserTypeIds) {
      this.multiUserTypeIds = data.multiUserTypeIds;
    } else {
      this.multiUserTypeIds = [];
      this.multiUserTypeIds.push([]); // Role
    }

    if (data.multiUserTypeExtIds) {
      this.multiUserTypeExtIds = data.multiUserTypeExtIds;
    } else {
      this.multiUserTypeExtIds = [];
      this.multiUserTypeExtIds.push([]); // Service scheme ...
      this.multiUserTypeExtIds.push([]); // Developmental role
    }
    this.multiStatusTypeIds = data.multiStatusTypeIds
      ? data.multiStatusTypeIds
      : {
          LearningNeed: [],
          LearningPlan: [],
        };
    this.ageRanges = data.ageRanges ? data.ageRanges : [];
    this.createdAfter = data.createdAfter ? data.createdAfter : undefined;
    this.createdBefore = data.createdBefore ? data.createdBefore : undefined;
    this.expirationDateAfter = data.expirationDateAfter
      ? data.expirationDateAfter
      : undefined;
    this.expirationDateBefore = data.expirationDateBefore
      ? data.expirationDateBefore
      : undefined;
    this.organizationalUnitTypeIds = data.organizationalUnitTypeIds
      ? data.organizationalUnitTypeIds
      : undefined;
    this.includeFilterOptions = data.includeFilterOptions
      ? data.includeFilterOptions
      : undefined;
    this.multiUserGroupIds = data.multiUserGroupIds
      ? data.multiUserGroupIds
      : undefined;
    if (data.multiUserGroupIds) {
      this.multiUserGroupIds = data.multiUserGroupIds;
    } else {
      this.multiUserGroupIds = [];
      this.multiUserGroupIds.push([]); // User pool (group of the users)
      this.multiUserGroupIds.push([]); // Approval group (for AO)
    }
    this.exportOptions = data.exportOptions ? data.exportOptions : undefined;
    this.filterOnSubDepartment = data.filterOnSubDepartment
      ? data.filterOnSubDepartment
      : undefined;
    this.statusTypeLogs = data.statusTypeLogs
      ? data.statusTypeLogs
      : {
          LearningNeed: [],
          LearningPlan: [],
        };
  }
}

export class ExportModel {
  public exportAllItems: boolean;
  public exportFields: any;
  public delimiter: string;
  constructor(data?: Partial<ExportModel>) {
    if (!data) {
      return;
    }
    this.exportAllItems = data.exportAllItems ? data.exportAllItems : true;
    this.exportFields = data.exportFields ? data.exportFields : {};
    this.delimiter = data.delimiter ? data.delimiter : undefined;
  }
}

export class DepartmentQueryModel {
  includeParent: boolean;
  includeChildren: boolean;
  countChildren?: boolean;
  maxChildrenLevel?: number;
  searchText: string;
  constructor(data?: Partial<DepartmentQueryModel>) {
    if (!data) {
      return;
    }
    this.includeChildren = data.includeChildren;
    this.includeParent = data.includeParent;
    this.searchText = data.searchText;
    this.countChildren = data.countChildren;
    this.maxChildrenLevel = data.maxChildrenLevel;
  }
}
