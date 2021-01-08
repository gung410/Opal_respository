import { NgModule } from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { LearningNeedsPreviewAnswerComponent } from './learning-needs-preview-answer.component';
import { CxCommonModule } from '@conexus/cx-angular-common';

@NgModule({
  declarations: [LearningNeedsPreviewAnswerComponent],
  exports: [LearningNeedsPreviewAnswerComponent],
  imports: [SharedModule, CxCommonModule],
})
export class LearningNeedsPreviewAnswerModule {}
