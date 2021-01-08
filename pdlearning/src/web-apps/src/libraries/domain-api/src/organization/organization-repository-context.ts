import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { DepartmentInfoModel } from './models/department-info.model';
import { DepartmentLevelModel } from './models/department-level.model';
import { Injectable } from '@angular/core';

@Injectable()
export class OrganizationRepositoryContext extends BaseRepositoryContext {
  public departmentsSubject: BehaviorSubject<Dictionary<DepartmentInfoModel>> = new BehaviorSubject({});
  public departmentLevelsSubject: BehaviorSubject<Dictionary<DepartmentLevelModel>> = new BehaviorSubject({});
}
