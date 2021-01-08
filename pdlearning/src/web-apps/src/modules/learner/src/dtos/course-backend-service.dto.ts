import { Course } from '@opal20/domain-api';

export interface IPagedCourseDetailsModelResult {
  totalCount: number;
  items: Course[];
}

export class PagedCourseDetailsModelResult implements IPagedCourseDetailsModelResult {
  public totalCount: number = 0;
  public items: Course[] = [];

  constructor(data?: IPagedCourseDetailsModelResult) {
    if (data == null) {
      return;
    }
    this.totalCount = data.totalCount;
    this.items = data.items !== undefined ? data.items.map(p => new Course(p)) : [];
  }
}

export interface IGetAllIdsBelongToCourseIdsResult {
  courseId: string;
  listDigitalContentId: string[];
}
