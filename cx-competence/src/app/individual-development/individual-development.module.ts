import { CommonModule } from '@angular/common';
import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { TranslateModule } from '@ngx-translate/core';
import { IFrameResizerDirective } from 'app-utilities/iframe-resizer.pipe';
import { SharedModule } from 'app/shared/shared.module';
import { IndividualDevelopmentComponent } from './individual-development.component';
import { LearningNeedsAnalysisModule } from './learning-needs-analysis/learning-needs-analysis.module';
import { LearningNeedsModule } from './learning-needs/learning-needs.module';
import { PdOpportunitiesModule } from './pd-opportunities/pd-opportunities.module';
import { IdpToolbarModule } from './shared/idp-toolbar/idp-toolbar.module';
import { LnaSurveyLinkComponent } from './shared/lna-survey-link/lna-survey-link.component';

@NgModule({
  declarations: [IndividualDevelopmentComponent, IFrameResizerDirective, LnaSurveyLinkComponent],
  imports: [
    FormsModule,
    CommonModule,
    CxCommonModule,
    SharedModule,
    MatTabsModule,
    TranslateModule,
    LearningNeedsAnalysisModule,
    LearningNeedsModule,
    PdOpportunitiesModule,
    IdpToolbarModule,
  ],
  exports: [IndividualDevelopmentComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class IndividualDevelopmentModule {}
