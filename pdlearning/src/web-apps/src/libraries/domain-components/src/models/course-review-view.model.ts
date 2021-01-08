import { IUserReviewModel, PublicUserInfo, UserReviewModel } from '@opal20/domain-api';

export interface ICourseReviewViewModel extends IUserReviewModel {
  user: PublicUserInfo;
}

export class CourseReviewViewModel extends UserReviewModel {
  public user: PublicUserInfo;
  public static createFromModel(review: IUserReviewModel, user: PublicUserInfo): CourseReviewViewModel {
    return new CourseReviewViewModel({
      ...review,
      user: user
    });
  }

  constructor(data?: ICourseReviewViewModel) {
    super(data);
    if (data != null) {
      this.user = data.user;
    }
  }
}
