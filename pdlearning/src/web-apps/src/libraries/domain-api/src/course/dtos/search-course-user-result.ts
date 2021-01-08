import { CourseUser, ICourseUser } from './../models/course-user.model';

export interface ISearchCourseUserResult {
  items: ICourseUser[];
  totalCount: number;
}

export class SearchCourseUserResult implements ISearchCourseUserResult {
  public items: CourseUser[] = [];
  public totalCount: number = 0;

  constructor(data?: ISearchCourseUserResult) {
    if (data === undefined) {
      return;
    }

    this.items = data.items.map(item => new CourseUser(item));
    this.totalCount = data.totalCount;
  }
}
