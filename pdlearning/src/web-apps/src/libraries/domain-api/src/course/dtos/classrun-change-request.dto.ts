export interface IClassRunChangeRequest {
  registrationId: string;
  classRunChangeId: string;
  comment: string;
}

export interface IMassClassRunChangeRequest {
  registrationIds: string[];
  classRunChangeId: string;
  comment: string;
}
