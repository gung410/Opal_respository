import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyPdJourneyComponent } from './my-pd-journey.component';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'app/shared/shared.module';
import { IndividualDevelopmentModule } from 'app/individual-development/individual-development.module';

@NgModule({
  declarations: [MyPdJourneyComponent],
  imports: [
    CommonModule,
    SharedModule,
    IndividualDevelopmentModule,
    RouterModule.forChild([{ path: '', component: MyPdJourneyComponent }]),
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class MyPdJourneyModule {}
