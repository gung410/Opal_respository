import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'app/shared/shared.module';
import { SocialEngagementComponent } from './social-engagement.component';

@NgModule({
  declarations: [SocialEngagementComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: SocialEngagementComponent }])
  ]
})
export class SocialEngagementModule {}
