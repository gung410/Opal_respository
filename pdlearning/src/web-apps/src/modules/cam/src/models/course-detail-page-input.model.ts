import { CourseCriteriaDetailMode, CourseDetailMode } from '@opal20/domain-components';

/**
 * Please dont change this file. If you want change it, you must update deeplink for Front-end/Back-end
 */
export class CourseDetailPageInput {
  public mode: CourseDetailMode;
  public id?: string;
  public coursePlanningCycleId?: string;
  public courseCriteriaMode: CourseCriteriaDetailMode;
}
