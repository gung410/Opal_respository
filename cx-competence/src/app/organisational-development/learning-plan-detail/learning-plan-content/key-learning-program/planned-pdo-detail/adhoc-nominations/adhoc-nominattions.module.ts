import { CommonModule } from '@angular/common';
import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { AdhocNominationService } from 'app-services/idp/assign-pdo/adhoc-nomination.service';
import { PdPlannerService } from 'app-services/idp/pd-planner/pd-planner.service';
import { CxSelectModule } from 'app/shared/components/cx-select/cx-select.module';
import { OpportunitiesCatalogDialogModule } from 'app/shared/components/opportunities-catalog-dialog/opportunities-catalog-dialog.module';
import { SharedModule } from 'app/shared/shared.module';
import { SubComponentModule } from '../sub-components/sub-components.module';
import { AdhocNominationsComponent } from './adhoc-nominations.component';

@NgModule({
  declarations: [AdhocNominationsComponent],
  imports: [
    CommonModule,
    CxCommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: AdhocNominationsComponent }]),
    CxSelectModule,
    MatRadioModule,
    SubComponentModule,
    OpportunitiesCatalogDialogModule,
  ],
  providers: [AdhocNominationService, PdPlannerService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class AdhocNominationsModule {}
