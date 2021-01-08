import { BaseRepositoryContext } from '@opal20/infrastructure';
import { BehaviorSubject } from 'rxjs';
import { BookmarkInfoModel } from './models/bookmark-info.model';
import { Injectable } from '@angular/core';
import { LearnerLearningPath } from './models/my-learning-path.model';
import { MyCourseResultModel } from './models/my-course-result.model';
import { MyDigitalContent } from './models/my-digital-content.model';
import { UserPreferenceModel } from './models/user-preferences.model';
import { UserReviewModel } from './models/user-review.model';

@Injectable()
export class LearnerRepositoryContext extends BaseRepositoryContext {
  public userReviewSubject: BehaviorSubject<Dictionary<UserReviewModel>> = new BehaviorSubject({});
  public myCourseResultSubject: BehaviorSubject<Dictionary<MyCourseResultModel>> = new BehaviorSubject({});
  public bookmarkInfoSubject: BehaviorSubject<Dictionary<BookmarkInfoModel>> = new BehaviorSubject({});
  public userPreferenceSubject: BehaviorSubject<Dictionary<UserPreferenceModel>> = new BehaviorSubject({});
  public myDigitalContentSubject: BehaviorSubject<Dictionary<MyDigitalContent>> = new BehaviorSubject({});
  public learnerLearningPathSubject: BehaviorSubject<Dictionary<LearnerLearningPath>> = new BehaviorSubject({});
}
