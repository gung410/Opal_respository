import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AgGridModule } from 'ag-grid-angular';
import { MomentModule } from 'angular2-moment';
import { SharedModule } from 'app/shared/shared.module';

import {
  MAT_DIALOG_DEFAULT_OPTIONS,
  MatDialogModule
} from '@angular/material/dialog';
import { FormBuilderService } from 'app-services/form-builder.service';
import { SystemRolesDataService } from 'app/core/store-data-services/system-roles-data.service';
import { ConfirmDialogComponent } from 'app/shared/components/confirm-dialog/confirm-dialog.component';
import { FocusDirective } from 'app/shared/directives/focus.directive';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { SystemRoleDialogComponent } from './dialogs/system-role-dialog/system-role-dialog.component';
import { SystemRolesShowHideComponent } from './dialogs/system-roles-show-hide-column/system-roles-show-hide-column.component';
import { PermissionsTableComponent } from './permissions-table/permissions-table.component';
import { PermissionsComponent } from './permissions.component';
import { CheckboxRendererComponent } from './renderers/checkbox-renderer/checkbox-renderer.component';
import { DropdownListModulesHeaderComponent } from './renderers/module-header/dropdown-list-modules-header.component';
import { SystemRoleCheckboxComponent } from './renderers/system-role-checkbox/system-role-checkbox.component';
import { SystemRoleHeaderComponent } from './renderers/system-role-header/system-role-header.component';
import { PermissionsApiService } from './services/permissions-api.service';
import { PermissionsColumnService } from './services/permissions-column.service';
import { PermissionsTableService } from './services/permissions-table.service';

@NgModule({
  declarations: [
    PermissionsComponent,
    PermissionsTableComponent,
    CheckboxRendererComponent,
    FocusDirective,
    DropdownListModulesHeaderComponent,
    SystemRoleDialogComponent,
    SystemRolesShowHideComponent,
    SystemRoleHeaderComponent,
    SystemRoleCheckboxComponent
  ],
  imports: [
    CommonModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatRadioModule,
    MatInputModule,
    NgbModule,
    MatMenuModule,
    NgbDropdownModule,
    CxCommonModule,
    MatCheckboxModule,
    FormsModule,
    AgGridModule.withComponents([
      CheckboxRendererComponent,
      DropdownListModulesHeaderComponent,
      SystemRoleHeaderComponent
    ]),
    MatSelectModule,
    MatIconModule,
    MatDatepickerModule,
    SharedModule,
    MomentModule,
    MatDialogModule,
    RouterModule.forChild([{ path: '', component: PermissionsComponent }])
  ],
  exports: [],
  entryComponents: [ConfirmDialogComponent, SystemRolesShowHideComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    UserAccountsDataService,
    FormBuilderService,
    PermissionsApiService,
    PermissionsTableService,
    SystemRolesDataService,
    PermissionsColumnService
    // { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
    // { provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: { hasBackdrop: false } }
  ]
})
export class PermissionsModule {}
