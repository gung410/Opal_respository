export interface IPagedInfo {
  skipCount: number;
  maxResultCount: number;
}

export interface IAccessRightSearchRequest {
  originalObjectId: string;
  pagedInfo: IPagedInfo;
}

export interface IAccessRightGetAllCollaboratorsIdRequest {
  originalObjectId: string;
}
