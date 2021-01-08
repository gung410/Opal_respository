/**
 * Starting point for looking up users.
 */
export enum StartingHierarchyDepartment {
  /**
   * The top accessible department of the current login user.
   */
  TopAccessible,
  /**
   * The default selected department depending on the system role of the current login user.
   */
  DefaultSelected,
  /**
   * The department of the current login user.
   */
  LoginUserDepartment,
  /**
   * Specify which department should be the starting point.
   */
  SpecifiedDepartment,
}
