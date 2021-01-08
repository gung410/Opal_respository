import {
  NgModule,
  CUSTOM_ELEMENTS_SCHEMA,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { SharedModule } from 'app/shared/shared.module';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { PdEvaluationDialogComponent } from './pd-evaluation-dialog.component';

@NgModule({
  declarations: [PdEvaluationDialogComponent],
  exports: [PdEvaluationDialogComponent],
  imports: [SharedModule, CxCommonModule],
  entryComponents: [PdEvaluationDialogComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class PdEvaluationDialogModule {}
