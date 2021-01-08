import { CoursePlanningCycle, ICoursePlanningCycle } from '../models/course-planning-cycle.model';

export interface ISearchCoursePlanningCycleResult {
  items: ICoursePlanningCycle[];
  totalCount: number;
}

export class SearchCoursePlanningCycleResult implements ISearchCoursePlanningCycleResult {
  public items: CoursePlanningCycle[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchCoursePlanningCycleResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new CoursePlanningCycle(item));
    this.totalCount = data.totalCount;
  }
}
