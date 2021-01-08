import { CommentNotification } from '../models/comment-notification';
import { EntityCommentType } from '../models/entity-comment-type';

export interface ICreateCommentRequest {
  objectId: string;
  content: string;
  entityCommentType?: EntityCommentType;
  commentNotification?: CommentNotification;
}

export class CreateCommentRequest implements ICreateCommentRequest {
  public objectId: string;
  public content: string;
  public entityCommentType?: EntityCommentType;
  public commentNotification?: CommentNotification;
}
