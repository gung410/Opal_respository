import { RouterModule, Routes } from '@angular/router';

import { MainFormStandalonePlayerPageComponent } from '@opal20/domain-components';
import { NgModule } from '@angular/core';

const routes: Routes = [
  {
    path: 'detail',
    component: MainFormStandalonePlayerPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class FormStandaloneRoutingModule {}
