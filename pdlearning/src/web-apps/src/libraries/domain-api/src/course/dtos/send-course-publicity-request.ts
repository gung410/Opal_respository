export interface ISendPublicityRequest {
  userIds: string[];
  teachingSubjectIds: string[];
  teachingLevels: string[];
  courseId: string;
  base64Message: string;
  specificTargetAudience: boolean;
  userNameTag?: string;
  courseTitleTag?: string;
}
