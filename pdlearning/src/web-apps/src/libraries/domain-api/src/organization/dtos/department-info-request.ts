export interface IDepartmentInfoRequest {
  /**
   * This is the top parent department node id of the returned result.
   * The returned result will include this node with departmentId and its children
   */
  departmentId: number;
  includeChildren?: boolean;
  includeDepartmentType?: boolean;
  getParentDepartmentId?: boolean;
  departmentTypeIds?: number[];
  pageIndex?: number;
  pageSize?: number;
}
