import { RouterModule, Routes } from '@angular/router';

import { FormGuard } from '@opal20/infrastructure';
import { NgModule } from '@angular/core';
import { POCAppInfoComponent } from './features/app-info/poc-app-info.component';
import { POCBackendComponent } from './features/backend-service/poc-backend.component';
import { POCDialogPageComponent } from './features/dialog/poc-dialog-page.component';
import { POCModalComponent } from './features/modal/poc-modal.component';
import { POCMultiFormComponent } from './features/multi-form/poc-multi-form.component';
import { POCNavigationComponent } from './features/navigation/poc-navigation.component';
import { POCNavigationDataComponent } from './features/navigation/poc-navigation-data.component';
import { POCSingleFormComponent } from './features/single-form/poc-single-form.component';
import { POCSpinnerComponent } from './features/spinner/poc-spinner.component';
import { POCTranslationComponent } from './features/translation/poc-translation.component';

const routes: Routes = [
  {
    path: 'spinner',
    component: POCSpinnerComponent
  },
  {
    path: 'navigation',
    component: POCNavigationComponent
  },
  {
    path: 'navigation-data',
    component: POCNavigationDataComponent
  },
  {
    path: 'translation',
    component: POCTranslationComponent
  },
  {
    path: 'app-info',
    component: POCAppInfoComponent
  },
  {
    path: 'backend-service',
    component: POCBackendComponent
  },
  {
    path: 'modal-service',
    component: POCModalComponent
  },
  {
    path: 'single-form',
    component: POCSingleFormComponent,
    canDeactivate: [FormGuard]
  },
  {
    path: 'multi-form',
    component: POCMultiFormComponent
  },
  {
    path: 'dialog',
    component: POCDialogPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class POCRoutingModule {}
