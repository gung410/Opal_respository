import { Identity } from './common.model';

export class MyTopHierarchyDepartment {
  identity: Identity;
  departmentName: string;
  parentDepartmentId: number;
  path: string;
  pathName: string;
  defaultHierarchyDepartment: DefaultHierarchyDepartment;
}

/**
 * The default hierarchy department of the currently logged-in user.
 * e.g: SA and UAA has the default is MOE, other users should have the default same as the top accessible one.
 */
export class DefaultHierarchyDepartment {
  departmentId: number;
  hdId: number;
  hierarchyId: number;
  parentHdId: number;
  path: string;
}
