import { Injectable } from '@angular/core';
import { AssignPDOHelper } from 'app-services/idp/assign-pdo/assign-pdo.helper';
import { UserService } from 'app-services/user.service';
import { CxSelectItemModel } from 'app/shared/components/cx-select/cx-select.model';
import { Staff } from 'app/staff/staff.container/staff-list/models/staff.model';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UserFilterService {
  constructor(private userService: UserService) {}
  filterLearners = (
    searchText?: string,
    departmentId?: number,
    pageIndex?: number
  ): Observable<CxSelectItemModel<Staff>[]> => {
    const filterParams = AssignPDOHelper.filterParamLearnerBuilder(
      searchText,
      departmentId,
      pageIndex
    );

    return this.userService
      .getListEmployee(filterParams)
      .pipe(map(AssignPDOHelper.mapPagedStaffsToCxSelectItems));
  };
}
