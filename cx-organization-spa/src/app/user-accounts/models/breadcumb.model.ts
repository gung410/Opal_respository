import { findIndexCommon } from 'app/shared/constants/common.const';
import { Identity } from '../../shared/models/identity.model';

export class CxBreadCrumbItem {
  /**
   * Checks whether the specified identity existing in the bread crumb items or not.
   * @param breadCrumbItems The list of bread crumb items.
   * @param checkingIdentity The identity which using for checking exist.
   */
  static checkExistingBreadCrumbItem(
    breadCrumbItems: CxBreadCrumbItem[],
    checkingIdentity: number
  ): boolean {
    return (
      breadCrumbItems &&
      breadCrumbItems.findIndex(
        (a: CxBreadCrumbItem) => a.identity.id === checkingIdentity
      ) !== findIndexCommon.notFound
    );
  }
  name: string;
  identity: Identity;
  constructor(breadCrumbItem: Partial<CxBreadCrumbItem>) {
    this.name = breadCrumbItem.name;
    this.identity = breadCrumbItem.identity;
  }
}
