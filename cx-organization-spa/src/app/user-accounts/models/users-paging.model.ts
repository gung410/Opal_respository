import { UserManagement } from './user-management.model';

export class PagedUsersList {
  totalItems: number;
  pageIndex: number;
  pageSize: number;
  items: UserManagement[];
}
