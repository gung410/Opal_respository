import { CourseStatus } from '../../share/models/course-status.enum';
import { IFilter } from '@opal20/infrastructure';
import { SearchCourseType } from '../models/search-course-type.model';

export interface ISearchCourseRequest {
  skipCount: number;
  maxResultCount: number;
  searchText: string;
  searchType: SearchCourseType;
  searchStatus: CourseStatus[];
  checkCourseContent: boolean;
  coursePlanningCycleId?: string;
  filter: IFilter;
}
