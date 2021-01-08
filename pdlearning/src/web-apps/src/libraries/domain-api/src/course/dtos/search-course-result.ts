import { Course, ICourse } from '../models/course.model';

export interface ISearchCourseResult {
  items: ICourse[];
  totalCount: number;
}

export class SearchCourseResult implements ISearchCourseResult {
  public items: Course[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchCourseResult) {
    if (data == null) {
      return;
    }

    this.items = data.items.map(item => new Course(item));
    this.totalCount = data.totalCount;
  }
}
