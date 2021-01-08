import { MyRegistrationStatus } from '../../learner/models/my-course.model';
export interface IChangeRegistrationStatusByCourseClassRunRequest {
  courseId: string;
  classRunId: string;
  status: MyRegistrationStatus;
}
