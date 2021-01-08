import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared.module';
import { LandingPageComponent } from './landing-page.component';
@NgModule({
  declarations: [LandingPageComponent],
  imports: [
    SharedModule,
    RouterModule.forChild([{ path: '', component: LandingPageComponent }]),
  ],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class LandingPageModule {}
