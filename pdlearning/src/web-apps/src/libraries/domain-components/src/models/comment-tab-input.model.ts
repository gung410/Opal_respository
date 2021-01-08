import { CommentNotification, CommentServiceType, EntityCommentType } from '@opal20/domain-api';

export class CommentTabInput {
  public originalObjectId: string;
  public commentServiceType: CommentServiceType;
  public entityCommentType?: EntityCommentType;
  public actionType?: string;
  public mappingAction?: Dictionary<string>;
  public commentNotification?: CommentNotification;
  public hasReply?: boolean = true;
  public orderByDescendingDate?: boolean;
  public mode?: 'view' | 'edit' = 'edit';
}
