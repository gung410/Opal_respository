import { ModuleWithProviders, NgModule } from '@angular/core';

import { AnnouncementApiService } from './services/announcement-api.service';
import { AnnouncementRepository } from './repositories/announcement.repository';
import { AssessmentAnswerApiService } from './services/assessment-answer-api.service';
import { AssessmentAnswerRepository } from './repositories/assessment-answer.respository';
import { AssignmentApiService } from './services/assignment-api.service';
import { AssignmentRepository } from './repositories/assignment.repository';
import { AttendanceTrackingRepository } from './repositories/attendance-tracking.repository';
import { AttendanceTrackingService } from './services/attendance-tracking-api.service';
import { BlockoutDateRepository } from './repositories/blockout-date.repository';
import { BlockoutDateService } from './services/blockout-date-api.service';
import { ClassRunApiService } from './services/classrun-api.service';
import { ClassRunRepository } from './repositories/classrun.repository';
import { CourseApiService } from './services/course-api.service';
import { CourseCriteriaApiService } from './services/course-criteria-api.service';
import { CourseCriteriaRepository } from './repositories/course-criteria.repository';
import { CoursePlanningCycleApiService } from './services/course-planning-cycle-api.service';
import { CoursePlanningCycleRepository } from './repositories/course-planning-cycle.repository';
import { CourseRepository } from './repositories/course.repository';
import { CourseRepositoryContext } from './course-repository-context';
import { ECertificateApiService } from './services/ecertificate-api.service';
import { ECertificateRepository } from './repositories/ecertificate.repository';
import { LearningContentApiService } from './services/learning-content-api.service';
import { LearningContentRepository } from './repositories/learning-content.repository';
import { LearningPathApiService } from './services/learning-path-api.service';
import { LearningPathRepository } from './repositories/learning-path.repository';
import { ParticipantAssignmentTrackApiService } from './services/participant-assignment-track-api.service';
import { ParticipantAssignmentTrackRepository } from './repositories/participant-assignment-track.repository';
import { RegistrationApiService } from './services/registration-api.service';
import { RegistrationRepository } from './repositories/registration.repository';
import { SessionApiService } from './services/session-api.service';
import { SessionRepository } from './repositories/session.repository';
@NgModule({
  providers: [
    LearningPathApiService,
    LearningPathRepository,
    CourseApiService,
    CourseCriteriaApiService,
    CourseRepository,
    CourseCriteriaRepository,
    ClassRunApiService,
    ClassRunRepository,
    RegistrationApiService,
    RegistrationRepository,
    AssignmentApiService,
    SessionApiService,
    SessionRepository,
    ParticipantAssignmentTrackApiService,
    ParticipantAssignmentTrackRepository,
    AttendanceTrackingService,
    AttendanceTrackingRepository,
    AssignmentRepository,
    CoursePlanningCycleApiService,
    CoursePlanningCycleRepository,
    ECertificateApiService,
    ECertificateRepository,
    LearningContentApiService,
    LearningContentRepository,
    AnnouncementApiService,
    AnnouncementRepository,
    BlockoutDateService,
    BlockoutDateRepository,
    AssessmentAnswerApiService,
    AssessmentAnswerRepository
  ]
})
export class CourseDomainApiModule {
  public static forRoot(): ModuleWithProviders {
    return {
      ngModule: CourseDomainApiModule,
      providers: [CourseRepositoryContext]
    };
  }
}
