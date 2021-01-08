import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { ScormPlayerContainerComponent } from './components/scorm-player-container.component';
import { ScormPlayerIntegratorRoutePaths } from './scorm-player-integrator.config';

const routes: Routes = [
  {
    path: ScormPlayerIntegratorRoutePaths.Default,
    component: ScormPlayerContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class ScormPlayerIntegratorRoutingModule {}
