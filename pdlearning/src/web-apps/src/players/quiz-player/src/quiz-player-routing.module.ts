import { RouterModule, Routes } from '@angular/router';

import { MainQuizPlayerPageComponent } from '@opal20/domain-components';
import { NgModule } from '@angular/core';
import { QuizPlayerRoutePaths } from './quiz-player.config';

const routes: Routes = [
  {
    path: QuizPlayerRoutePaths.Default,
    component: MainQuizPlayerPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class QuizPlayerRoutingModule {}
