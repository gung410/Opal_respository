export interface ISeenCommentModel {
  objectId: string;
  commentNotSeenIds: string[];
}

export class SeenCommentModel implements ISeenCommentModel {
  public objectId: string;
  public commentNotSeenIds: string[];

  constructor(data: ISeenCommentModel) {
    if (data == null) {
      return;
    }

    this.objectId = data.objectId;
    this.commentNotSeenIds = data.commentNotSeenIds;
  }
}
