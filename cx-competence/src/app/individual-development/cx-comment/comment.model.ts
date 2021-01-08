import { UserBasicInfo } from 'app-models/auth.model';
import { v4 as uuid } from 'uuid';

export class CommentData {
  id: string;
  content: string;
  created: string;
  owner: UserBasicInfo;
  lastUpdated?: string;
  tag?: CommentTagTypeEnum;
  allowEdit: boolean;
  allowDelete: boolean;
  constructor(comment?: Partial<CommentData>) {
    if (comment) {
      this.content = comment.content;
      this.created = comment.created;
      this.owner = comment.owner;
      this.tag = comment.tag ? comment.tag : CommentTagTypeEnum.Generic;
      this.id = comment.id ? comment.id : uuid();
      this.allowEdit = comment.allowEdit;
      this.allowDelete = comment.allowDelete;
    } else {
      this.id = uuid();
    }
  }
}

export class CommentChangeData {
  action: CommentActionEnum;
  commentItem: CommentData;
  changeResult: boolean;
  constructor(comment: CommentData, action: CommentActionEnum) {
    this.commentItem = comment;
    this.action = action;
  }
}

export class CommentResultDto extends CommentData {
  resultExtId: string;
}

export enum CommentActionEnum {
  ADD = 'add',
  UPDATE = 'update',
  DELETE = 'delete',
}

export enum CommentTagTypeEnum {
  Generic = 'Generic',
  Approval = 'Approval',
  Rejection = 'Rejection',
}
