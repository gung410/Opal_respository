import { Department } from 'app/department-hierarchical/models/department.model';

export class DepartmentDialogResult {
  constructor(public selectedDepartments: Department[]) {}
}
