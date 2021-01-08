import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { MomentModule } from 'angular2-moment';
import { SharedModule } from 'app/shared/shared.module';
import { ExportConfirmationComponent } from './export-confirmation/export-confirmation.component';
import { ReportsComponent } from './reports.component';

@NgModule({
  declarations: [ReportsComponent, ExportConfirmationComponent],
  imports: [
    CommonModule,
    NgbDropdownModule,
    CxCommonModule,
    SharedModule,
    MomentModule,
    RouterModule.forChild([{ path: '', component: ReportsComponent }])
  ],
  exports: [ReportsComponent],
  entryComponents: [ExportConfirmationComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ReportsModule {}
