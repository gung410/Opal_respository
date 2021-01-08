import { RouterModule, Routes } from '@angular/router';

import { AssignmentPlayerRoutePaths } from './assignment-player.config';
import { MainAssignmentPlayerComponent } from '@opal20/domain-components';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: AssignmentPlayerRoutePaths.Default,
    component: MainAssignmentPlayerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AssignmentPlayerRoutingModule {}
