import { ModuleWithProviders, NgModule } from '@angular/core';

import { CourseLearnerRepository } from '././repositories/course-learner.repository';
import { LearnerRepositoryContext } from './learner-repository-context';
import { MyAchievementAPIService } from './services/my-achievement-backend.service';
import { MyAssignmentApiService } from './services/my-assignment-backend.service';
import { MyBookmarkApiService } from './services/my-bookmark-backend.service';
import { MyBookmarkRepository } from './repositories/my-bookmark.repository';
import { MyCourseApiService } from './services/my-course-backend.service';
import { MyCourseRepository } from './repositories/my-course.repository';
import { MyDigitalContentApiService } from './services/my-digital-content-backend.service';
import { MyDigitalContentRepository } from './repositories/my-digital-content.repository';
import { MyLearningBackendService } from './services/my-learning-backend.service';
import { MyLearningPathApiService } from './services/my-learning-path.service';
import { MyOutstandingTaskApiService } from './services/my-outstanding-task-backend.service';
import { UserPreferenceAPIService } from './services/user-preferences-backend.service';
import { UserPreferenceRepository } from './repositories/user-preference.repository';
import { UserReviewApiService } from './services/user-review-backend.service';
import { UserSharingAPIService } from './services/user-sharing-backend.service';
import { UserTrackingAPIService } from './services/user-tracking-backend.service';

@NgModule({
  providers: [
    LearnerRepositoryContext,
    CourseLearnerRepository,
    UserPreferenceRepository,
    MyCourseRepository,
    MyDigitalContentRepository,
    MyBookmarkRepository,
    MyCourseApiService,
    MyLearningBackendService,
    MyDigitalContentApiService,
    UserReviewApiService,
    MyBookmarkApiService,
    MyAssignmentApiService,
    MyLearningPathApiService,
    UserSharingAPIService,
    UserTrackingAPIService,
    UserPreferenceAPIService,
    MyOutstandingTaskApiService,
    MyAchievementAPIService
  ]
})
export class LearnerDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: LearnerDomainApiModule,
      providers: []
    };
  }
}
