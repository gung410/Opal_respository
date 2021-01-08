import { RouterModule, Routes } from '@angular/router';

import { NgModule } from '@angular/core';
import { VideoAnnotationPlayerContainerComponent } from './components/video-annotation-player-container.component';
import { VideoAnnotationPlayerRoutePaths } from './video-annotation-player.config';

const routes: Routes = [
  {
    path: VideoAnnotationPlayerRoutePaths.Default,
    component: VideoAnnotationPlayerContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class VideoAnnotationPlayerRoutingModule {}
