import { EntityCommentType } from '../models/entity-comment-type';

export interface IPagedInfo {
  skipCount: number;
  maxResultCount: number;
}

export interface ISearchCommentRequest {
  objectId: string;
  entityCommentType?: EntityCommentType;
  actionType?: string;
  pagedInfo: IPagedInfo;
}
