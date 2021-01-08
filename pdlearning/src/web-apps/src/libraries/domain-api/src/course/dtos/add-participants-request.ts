export interface IAddParticipantsRequest {
  courseId: string;
  classRunId: string;
  userIds: string[];
  followCourseTargetParticipant: boolean;
  departmentIds: number[];
}
