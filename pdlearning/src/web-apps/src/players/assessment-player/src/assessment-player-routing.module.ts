import { RouterModule, Routes } from '@angular/router';

import { AssessmentPlayerRoutePaths } from './assessment-player.config';
import { MainAssessmentPlayerComponent } from '@opal20/domain-components';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: AssessmentPlayerRoutePaths.Default,
    component: MainAssessmentPlayerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AssessmentPlayerRoutingModule {}
