import {Identity} from '../../../models/identity.model';
export class CxBreadCrumbItem {
  name: string;
  identity: Identity;
  constructor(breadCrumbItem: Partial<CxBreadCrumbItem>) {
      this.name = breadCrumbItem.name;
      this.identity = breadCrumbItem.identity;
  }
}

