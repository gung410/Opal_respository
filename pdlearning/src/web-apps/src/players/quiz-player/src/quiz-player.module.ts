import { BaseModuleOutlet, BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { NgModule, Type } from '@angular/core';

import { CommonComponentsModule } from '@opal20/common-components';
import { DomainComponentsModule } from '@opal20/domain-components';
import { QuizPlayerComponent } from './quiz-player.component';
import { QuizPlayerOutletComponent } from './quiz-player-outlet.component';
import { QuizPlayerRoutingModule } from './quiz-player-routing.module';
import { Router } from '@angular/router';

@NgModule({
  imports: [FunctionModule, QuizPlayerRoutingModule, CommonComponentsModule, DomainComponentsModule],
  declarations: [QuizPlayerComponent, QuizPlayerOutletComponent],
  providers: [],
  entryComponents: [QuizPlayerOutletComponent],
  bootstrap: [QuizPlayerComponent]
})
export class QuizPlayerModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }

  protected get outletType(): Type<BaseModuleOutlet> {
    return QuizPlayerOutletComponent;
  }
}
