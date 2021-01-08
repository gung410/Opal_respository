export interface ISendNominationRequest {
  organisations: number[];
  courseId: string;
  base64Message: string;
  specificOrganisation: boolean;
  userNameTag?: string;
  courseTitleTag?: string;
}
