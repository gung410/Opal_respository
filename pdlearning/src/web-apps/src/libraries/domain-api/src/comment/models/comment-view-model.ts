import { Comment, IComment } from './comment';

import { PublicUserInfo } from '../../share/models/user-info.model';

export interface ICommentViewModel extends IComment {
  user: PublicUserInfo;
}

export class CommentViewModel extends Comment {
  public user: PublicUserInfo;
  public static createFromModel(comment: IComment, user: PublicUserInfo): CommentViewModel {
    return new CommentViewModel({
      ...comment,
      user: user
    });
  }

  constructor(data?: ICommentViewModel) {
    super(data);
    if (data != null) {
      this.user = data.user;
    }
  }
}
