import {
  CUSTOM_ELEMENTS_SCHEMA,
  NgModule,
  NO_ERRORS_SCHEMA,
} from '@angular/core';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { IdpToolbarModule } from 'app/individual-development/shared/idp-toolbar/idp-toolbar.module';
import { PdEvaluationDialogModule } from 'app/individual-development/shared/pd-evalution-dialog/pd-evaluation-dialog.module';
import { SharedModule } from 'app/shared/shared.module';
import { BookmarkOpportunitiesComponent } from '../bookmark-opportunitites-dialog/bookmark-opportunities/bookmark-opportunities.component';
import { BookmarkOpportunititesDialogComponent } from '../bookmark-opportunitites-dialog/bookmark-opportunitites-dialog.component';
import { CatalogFilterFormModule } from '../catalog-filter-form/catalog-filter-form.module';
import { FilterCatalogSlidebarService } from '../catalog-filter-form/services/filter-catalog-slidebar.service';
import { CatalogToolbarComponent } from './catalog-toolbar/catalog-toolbar.component';
import { CatalogueCoursePickerComponent } from './catalogue-course-picker/catalogue-course-picker.component';
import { OpportunitiesCatalogDialogComponent } from './opportunities-catalog-dialog.component';
import { OpportunitiesCatalogComponent } from './opportunities-catalog/opportunities-catalog.component';
import { OpportunityComponent } from './opportunities-catalog/opportunity/opportunity.component';
import { OpportunityDetailDialogComponent } from './opportunity-detail-dialog/opportunity-detail-dialog.component';
import { OpportunityDetailComponent } from './opportunity-detail/opportunity-detail.component';

@NgModule({
  declarations: [
    OpportunitiesCatalogDialogComponent,
    CatalogToolbarComponent,
    OpportunitiesCatalogComponent,
    OpportunityComponent,
    BookmarkOpportunititesDialogComponent,
    BookmarkOpportunitiesComponent,
    OpportunityDetailDialogComponent,
    OpportunityDetailComponent,
    CatalogueCoursePickerComponent,
  ],
  exports: [
    OpportunitiesCatalogDialogComponent,
    CatalogueCoursePickerComponent,
  ],
  imports: [
    SharedModule,
    CxCommonModule,
    CxCommonModule,
    IdpToolbarModule,
    MatTabsModule,
    MatSelectModule,
    MatRadioModule,
    PdEvaluationDialogModule,
    CatalogFilterFormModule,
  ],
  providers: [FilterCatalogSlidebarService],
  entryComponents: [
    OpportunitiesCatalogDialogComponent,
    BookmarkOpportunititesDialogComponent,
    OpportunityDetailDialogComponent,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class OpportunitiesCatalogDialogModule {}
