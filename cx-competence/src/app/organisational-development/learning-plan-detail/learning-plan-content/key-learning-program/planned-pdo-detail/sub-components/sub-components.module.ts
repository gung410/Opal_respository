import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { SharedModule } from 'app/shared/shared.module';
import { NominationMemberList } from './nomination-member-list/nomination-member-list.component';
import { NominationStatusTabs } from './nomination-status-tabs/nomination-status-tabs.component';

@NgModule({
  declarations: [NominationMemberList, NominationStatusTabs],
  imports: [SharedModule, CxCommonModule, MatRadioModule],
  providers: [],
  exports: [NominationMemberList, NominationStatusTabs],
  entryComponents: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class SubComponentModule {}
