import { Assignment, IAssignment } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface IAssignmentViewModel extends IAssignment {
  selected: boolean;
}

// @dynamic
export class AssignmentViewModel extends Assignment implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public static createFromModel(
    assignment: Assignment,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {}
  ): AssignmentViewModel {
    return new AssignmentViewModel({
      ...assignment,
      selected: checkAll || selecteds[assignment.id]
    });
  }

  constructor(data?: IAssignmentViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
    }
  }
}
