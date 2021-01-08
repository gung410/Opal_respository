import { RouterModule, Routes } from '@angular/router';

import { CommonRoutePaths } from './common.config';
import { ErrorPageComponent } from './components/error-page/error-page.component';
import { NgModule } from '@angular/core';
import { WebinarErrorPageComponent } from './components/webinar-error-page/webinar-error-page.component';
import { WebinarSeekingServerPageComponent } from './components/webinar-seeking-server-page/webinar-seeking-server-page.component';

const routes: Routes = [
  {
    path: '',
    component: ErrorPageComponent
  },
  {
    path: CommonRoutePaths.Error,
    component: ErrorPageComponent
  },
  {
    path: CommonRoutePaths.WebinarError,
    component: WebinarErrorPageComponent
  },
  {
    path: CommonRoutePaths.WebinarSeekingServer,
    component: WebinarSeekingServerPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class CommonRoutingModule {}
