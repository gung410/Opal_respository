import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { SharedModule } from 'app/shared/shared.module';
import { CxSelectModule } from '../cx-select/cx-select.module';
import { FilterCatalogFormPlaceholderDirective } from './directives/filter-catalog-form-placeholder.derective';
import { PdoFilterFormComponent } from './filter-form/pdo/pdo-filter-form.component';
import { FilterCatalogueSlidebarContainerComponent } from './filter-slidebar/filter-catalog-slidebar-container.component';
import { FilterCatalogSlidebarService } from './services/filter-catalog-slidebar.service';

@NgModule({
  declarations: [
    FilterCatalogueSlidebarContainerComponent,
    PdoFilterFormComponent,
    FilterCatalogFormPlaceholderDirective,
  ],
  exports: [FilterCatalogueSlidebarContainerComponent],
  imports: [SharedModule, CxCommonModule, CxSelectModule],
  providers: [FilterCatalogSlidebarService],
  entryComponents: [PdoFilterFormComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class CatalogFilterFormModule {}
