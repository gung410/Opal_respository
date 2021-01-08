import { EntityCommentType } from '../models/entity-comment-type';

export interface IGetCommentNotSeenRequest {
  objectIds: string[];
  entityCommentType?: EntityCommentType;
}
