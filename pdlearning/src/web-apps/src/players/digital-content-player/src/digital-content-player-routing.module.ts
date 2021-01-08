import { RouterModule, Routes } from '@angular/router';

import { DigitalContentPlayerContainerComponent } from './components/digital-content-player-container.component';
import { DigitalContentPlayerRoutePaths } from './digital-content-player.config';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: DigitalContentPlayerRoutePaths.Default,
    component: DigitalContentPlayerContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class DigitalContentPlayerRoutingModule {}
