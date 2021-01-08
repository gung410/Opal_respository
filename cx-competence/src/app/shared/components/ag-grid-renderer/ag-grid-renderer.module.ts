import { AgGridModule } from '@ag-grid-community/angular';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { IdpStatusBlockModule } from 'app/individual-development/shared/idp-status-block/idp-status-block.module';
import { ProgressComponent } from '../progress/progress.component';
import { TextOverflowTruncatedComponent } from './common/text-overflow-truncated/text-overflow-truncated.component';
import { LinkViewButtonRendererComponent } from './link-view-button/link-view-button-renderer.component';
import { LnaCompletionRateRendererComponent } from './lna-completion-rate-renderer/lna-completion-rate-renderer.component';
import { LNAStatusRendererComponent } from './lna-status-renderer/lna-status-renderer.component';
import { PdPlanStatusRendererComponent } from './pd-plan-status/pdplan-status-renderer.component';
import { NameRendererComponent } from './user/name-renderer/name-renderer.component';
import { RemoveUserRendererComponent } from './user/remove-user-renderer/remove-user-renderer.component';

@NgModule({
  declarations: [
    NameRendererComponent,
    RemoveUserRendererComponent,
    TextOverflowTruncatedComponent,
    LNAStatusRendererComponent,
    ProgressComponent,
    LinkViewButtonRendererComponent,
    PdPlanStatusRendererComponent,
    LnaCompletionRateRendererComponent,
  ],
  imports: [
    CommonModule,
    AgGridModule.withComponents([
      NameRendererComponent,
      TextOverflowTruncatedComponent,
      RemoveUserRendererComponent,
      LNAStatusRendererComponent,
      LinkViewButtonRendererComponent,
      PdPlanStatusRendererComponent,
      LnaCompletionRateRendererComponent,
    ]),
    RouterModule,
    IdpStatusBlockModule,
  ],
  exports: [ProgressComponent],
})
export class AgGridRendererModule {}
