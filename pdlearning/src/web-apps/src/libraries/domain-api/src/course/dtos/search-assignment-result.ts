import { Assignment, IAssignment } from '../models/assignment.model';

export interface ISearchAssignmentResult {
  items: IAssignment[];
  totalCount: number;
}

export class SearchAssignmentResult {
  public items: Assignment[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchAssignmentResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new Assignment(item));
    this.totalCount = data.totalCount;
  }
}
