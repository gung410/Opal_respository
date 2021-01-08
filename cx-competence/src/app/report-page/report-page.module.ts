import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { SharedModule } from 'app/shared/shared.module';
import { RouterModule } from '@angular/router';
import { ReportPageComponent } from './report-page.component';

@NgModule({
  declarations: [ReportPageComponent],
  imports: [
    CommonModule,
    CxCommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: ReportPageComponent }]),
  ],
})
export class ReportPageModule {}
