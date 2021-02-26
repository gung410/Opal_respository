import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTreeModule } from '@angular/material/tree';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridModule } from 'ag-grid-angular';
import { MomentModule } from 'angular2-moment';
import { FormBuilderService } from 'app-services/form-builder.service';
import { SharedModule } from 'app/shared/shared.module';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import {
  PERFECT_SCROLLBAR_CONFIG,
  PerfectScrollbarConfigInterface,
  PerfectScrollbarModule
} from 'ngx-perfect-scrollbar';
import { CellDropdownMetadataListActionsComponent } from './cell-components/cell-dropdown-metadata-list-actions/cell-dropdown-metadata-list-actions.component';
import { CellRequestStatusComponent } from './cell-components/cell-request-status/cell-request-status.component';
import { CellRequestedDateComponent } from './cell-components/cell-requested-date/cell-requested-date.component';
import { TaxonomyCellUserInfoComponent } from './cell-components/cell-user-info/taxonomy-cell-user-info.component';
import { MetadataRequestApiService } from './services/metadata-request-api.services';
import { MetadataRequestDataListSerivce } from './services/metadata-request-data-list.service';
import { MetadataRequestDialogService } from './services/metadata-request-dialog.service';
import { MetadataConfirmDialogComponent } from './taxonomy-dialog/metadata-confirm-dialog/metadata-confirm-dialog.component';
import { TaxonomyRequestDialogComponent } from './taxonomy-dialog/metadata-request/metadata-request/taxonomy-request-dialog.component';
import { MetadataTypeDialogComponent } from './taxonomy-dialog/metadata-type-dialog/metadata-type-dialog.component';
import { TaxonomyManagementListComponent } from './taxonomy-management-list/taxonomy-management-list.component';
import { TaxonomyManagementComponent } from './taxonomy-management.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};
@NgModule({
  declarations: [
    TaxonomyManagementComponent,
    TaxonomyManagementListComponent,
    TaxonomyCellUserInfoComponent,
    CellRequestStatusComponent,
    CellRequestedDateComponent,
    CellDropdownMetadataListActionsComponent,
    TaxonomyRequestDialogComponent,
    MetadataTypeDialogComponent,
    MetadataConfirmDialogComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbDropdownModule,
    CxCommonModule,
    NgSelectModule,
    NgbModule,
    MatRadioModule,
    MatIconModule,
    MatButtonModule,
    SharedModule,
    MomentModule,
    MatTreeModule,
    AgGridModule.withComponents([
      TaxonomyCellUserInfoComponent,
      CellRequestStatusComponent,
      CellRequestedDateComponent,
      CellDropdownMetadataListActionsComponent
    ]),
    RouterModule.forChild([
      { path: '', component: TaxonomyManagementComponent }
    ]),
    MatTabsModule,
    PerfectScrollbarModule
  ],
  exports: [
    TaxonomyManagementComponent,
    TaxonomyManagementListComponent,
    TranslateModule
  ],
  entryComponents: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    MetadataRequestApiService,
    MetadataRequestDialogService,
    FormBuilderService,
    UserAccountsDataService,
    MetadataRequestDataListSerivce,
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class TaxonomyManagementModule {}
