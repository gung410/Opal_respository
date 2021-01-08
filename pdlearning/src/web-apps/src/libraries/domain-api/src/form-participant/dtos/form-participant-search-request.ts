export interface IFormParticipantSearchRequest {
  formOriginalObjectId: string;
  pagedInfo: IPagedInfo;
}

export interface IPagedInfo {
  skipCount: number;
  maxResultCount: number;
}
