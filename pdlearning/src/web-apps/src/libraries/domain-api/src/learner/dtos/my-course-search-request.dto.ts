import { IPagedResultRequestDto } from '../../share/dtos/paged-request.dto';
import { LearningCourseType } from './my-course-backend-service.dto';
import { MyLearningStatisticType } from '../models/search-filter-statistic-type.dto';

export interface IMyCoursesSearchRequest extends IPagedResultRequestDto {
  searchText: string;
  statusFilter?: MyLearningStatisticType;
  orderBy: string;
  courseType?: LearningCourseType;
}
