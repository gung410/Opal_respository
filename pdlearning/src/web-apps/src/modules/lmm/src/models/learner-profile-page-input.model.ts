import { SearchLearnerProfileType } from './search-learner-profile-type.model';

/**
 * Please dont change this file. If you want change it, you must update deeplink for Front-end/Back-end
 */
export class LearnerProfilePageInput {
  public registrationId?: string;
  public attendanceTrackingId?: string;
  public userId?: string;
  public courseId?: string;
  public classRunId?: string;
  public searchType?: SearchLearnerProfileType;
}
