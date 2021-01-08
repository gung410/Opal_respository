import { CoursePlanningCycle, ICoursePlanningCycle } from '@opal20/domain-api';

import { IGridDataItem } from '@opal20/infrastructure';

export interface ICoursePlanningCycleViewModel extends ICoursePlanningCycle {
  selected: boolean;
}

// @dynamic
export class CoursePlanningCycleViewModel extends CoursePlanningCycle implements IGridDataItem {
  public id: string;
  public selected: boolean;
  public static createFromModel(
    coursePlanningCycle: CoursePlanningCycle,
    checkAll: boolean = false,
    selecteds: Dictionary<boolean> = {}
  ): CoursePlanningCycleViewModel {
    return new CoursePlanningCycleViewModel({
      ...coursePlanningCycle,
      selected: checkAll || selecteds[coursePlanningCycle.id]
    });
  }

  constructor(data?: ICoursePlanningCycleViewModel) {
    super(data);
    if (data != null) {
      this.selected = data.selected;
    }
  }
}
