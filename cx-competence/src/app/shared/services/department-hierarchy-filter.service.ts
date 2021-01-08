import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DepartmentHierarchyFilterService {
  private departmentSessionStorageLabel = 'current-global-department-id';

  constructor() {}
  public getCurrentGlobalDepartmentId(): number {
    return +sessionStorage.getItem(this.departmentSessionStorageLabel);
  }

  public setCurrentGlobalDepartmentId(departmentId: number) {
    return sessionStorage.setItem(
      this.departmentSessionStorageLabel,
      departmentId.toString()
    );
  }
}
