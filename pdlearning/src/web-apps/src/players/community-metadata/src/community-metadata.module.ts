import { AuthService, UrlHelperService } from '@opal20/authentication';
import { BaseRoutingModule, FunctionModule, ModuleFacadeService } from '@opal20/infrastructure';
import { Router, RouterModule } from '@angular/router';

import { ButtonModule } from '@progress/kendo-angular-buttons';
import { CommonComponentsModule } from '@opal20/common-components';
import { CommunityIntergrationService } from './services/community-intergration.service';
import { CommunityMetadataBuilderService } from './services/communitty-metadata-builder.service';
import { CommunityMetadataComponent } from './component/community-metadata.component';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { DomainComponentsModule } from '@opal20/domain-components';
import { NgModule } from '@angular/core';

@NgModule({
  imports: [
    FunctionModule,
    RouterModule.forRoot([]),
    CommonComponentsModule.forRoot(),
    DomainComponentsModule.forRoot(),
    ButtonModule,
    DateInputsModule
  ],
  providers: [AuthService, UrlHelperService, CommunityIntergrationService, CommunityMetadataBuilderService],
  declarations: [CommunityMetadataComponent],
  bootstrap: [CommunityMetadataComponent]
})
export class CommunityMetadataModule extends BaseRoutingModule {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected router: Router) {
    super(moduleFacadeService, router);
  }
}
