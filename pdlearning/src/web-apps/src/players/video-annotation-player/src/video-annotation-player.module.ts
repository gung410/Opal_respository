import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { NgModule, Type } from '@angular/core';

import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { Router } from '@angular/router';
import { VideoAnnotationPlayerContainerComponent } from './components/video-annotation-player-container.component';
import { VideoAnnotationPlayerOutletComponent } from './video-annotation-player-outlet.component';
import { VideoAnnotationPlayerPageComponent } from './video-annotation-player-page.component';
import { VideoAnnotationPlayerRoutingModule } from './video-annotation-player-routing.module';

@NgModule({
  imports: [FunctionModule, CommonComponentsModule, DomainComponentsModule, VideoAnnotationPlayerRoutingModule],
  declarations: [VideoAnnotationPlayerOutletComponent, VideoAnnotationPlayerPageComponent, VideoAnnotationPlayerContainerComponent],
  providers: [],
  entryComponents: [VideoAnnotationPlayerOutletComponent],
  bootstrap: [VideoAnnotationPlayerPageComponent]
})
export class VideoAnnotationPlayerModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return VideoAnnotationPlayerOutletComponent;
  }
}
