export interface ISendPlacementLetterRequest {
  ids: string[];
  base64Message: string;
  userNameTag: string;
  courseTitleTag: string;
  courseCodeTag: string;
  courseAdminNameTag: string;
  courseAdminEmailTag: string;
  listSessionTag: string;
  detailUrlTag: string;
}
