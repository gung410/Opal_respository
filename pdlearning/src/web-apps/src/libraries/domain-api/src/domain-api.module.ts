import { ModuleWithProviders, NgModule } from '@angular/core';

import { AccessRightDomainApiModule } from './access-right/access-right-domain-api.module';
import { BadgeDomainApiModule } from './badge/badge-domain-api.module';
import { BrokenLinkReportDomainApiModule } from './broken-link-report/broken-link-report-domain-api.module';
import { CalendarDomainApiModule } from './calendar/calendar-domain-api.module';
import { CommentDomainApiModule } from './comment/comment-domain-api.module';
import { ContentDomainApiModule } from './content/content-domain-api.module';
import { CourseDomainApiModule } from './course/course-domain-api.modules';
import { CxCompetenceDomainApiModule } from './cx-competence/cx-competence-domain-api.module';
import { FormDomainApiModule } from './form/form-domain-api.module';
import { FormParticipantDomainApiModule } from './form-participant/form-participant-domain-api.module';
import { FormSectionDomainApiModule } from './form-section/form-section-domain-api.module';
import { LearnerDomainApiModule } from './learner/learner-domain-api.module';
import { LearningCatalogueDomainApiModule } from './learning-catalog/learning-catalogue-domain-api.module';
import { LnaFormDomainApiModule } from './lna-form/lna-form-domain-api.module';
import { NewsfeedDomainApiModule } from './newsfeed/newsfeed-domain-api.module';
import { OrganizationDomainApiModule } from './organization/organization-domain-api.module';
import { PersonalSpaceApiModule } from './personal-space/personal-space-domain-api.module';
import { QuestionBankDomainApiModule } from './question-bank/question-bank-domain-api.module';
import { TaggingDomainApiModule } from './tagging/tagging-domain-api.module';
import { UserDomainApiModule } from './user/user-domain-api.module';
import { VersionTrackingDomainApiModule } from './version-tracking/version-tracking-domain-api.module';
import { WebinarDomainApiModule } from './webinar/webinar-domain-api.module';

@NgModule({
  imports: [
    CommentDomainApiModule,
    ContentDomainApiModule,
    CourseDomainApiModule,
    CxCompetenceDomainApiModule,
    FormDomainApiModule,
    LearnerDomainApiModule,
    LearningCatalogueDomainApiModule,
    OrganizationDomainApiModule,
    TaggingDomainApiModule,
    UserDomainApiModule,
    VersionTrackingDomainApiModule,
    AccessRightDomainApiModule,
    BrokenLinkReportDomainApiModule,
    FormSectionDomainApiModule,
    FormParticipantDomainApiModule,
    NewsfeedDomainApiModule,
    WebinarDomainApiModule,
    CalendarDomainApiModule,
    LnaFormDomainApiModule,
    PersonalSpaceApiModule,
    BadgeDomainApiModule,
    QuestionBankDomainApiModule
  ]
})
export class DomainApiModule {
  public static forRoot(): ModuleWithProviders[] {
    return [
      CommentDomainApiModule.forRoot(),
      ContentDomainApiModule.forRoot(),
      CourseDomainApiModule.forRoot(),
      CxCompetenceDomainApiModule.forRoot(),
      FormDomainApiModule.forRoot(),
      LearnerDomainApiModule.forRoot(),
      LearningCatalogueDomainApiModule.forRoot(),
      OrganizationDomainApiModule.forRoot(),
      TaggingDomainApiModule.forRoot(),
      UserDomainApiModule.forRoot(),
      VersionTrackingDomainApiModule.forRoot(),
      AccessRightDomainApiModule.forRoot(),
      BrokenLinkReportDomainApiModule.forRoot(),
      FormSectionDomainApiModule.forRoot(),
      FormParticipantDomainApiModule.forRoot(),
      NewsfeedDomainApiModule.forRoot(),
      WebinarDomainApiModule.forRoot(),
      CalendarDomainApiModule.forRoot(),
      LnaFormDomainApiModule.forRoot(),
      PersonalSpaceApiModule.forRoot(),
      BadgeDomainApiModule.forRoot(),
      QuestionBankDomainApiModule.forRoot()
    ];
  }
}
