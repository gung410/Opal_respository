export interface IChangeLearnerStatusRequest {
  registrationIds: string[];
  courseId: string;
  classRunId: string;
  isCompleted: boolean;
}
