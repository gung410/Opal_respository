import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AgGridModule } from 'ag-grid-angular';
import { MomentModule } from 'angular2-moment';
import { SharedModule } from 'app/shared/shared.module';
import { CellRemoveButtonActionComponent } from 'app/user-accounts/user-list/cell-components/cell-remove-button-action/cell-remove-button-action.component';

import { CellDropdownMenuComponent } from './user-group-list/cell-dropdown-menu/cell-dropdown-menu.component';
import { HeaderCustomComponent } from './user-group-list/header-custom-action/header-custom.component';
import { UserGroupListComponent } from './user-group-list/user-group-list.component';
import { UserGroupModifyFormComponent } from './user-group-list/user-group-modify-form/user-group-modify-form.component';
import { UserSelectComponent } from './user-group-list/user-group-modify-form/user-select/user-select.component';
import { UserGroupsDataService } from './user-groups-data.service';
import { UserGroupsRemoveConfirmationComponent } from './user-groups-remove-confirmation/user-groups-remove-confirmation.component';

@NgModule({
  declarations: [
    UserGroupListComponent,
    CellDropdownMenuComponent,
    CellRemoveButtonActionComponent,
    UserSelectComponent,
    HeaderCustomComponent,
    UserGroupsRemoveConfirmationComponent,
    UserGroupModifyFormComponent
  ],
  imports: [
    CommonModule,
    NgbDropdownModule,
    ReactiveFormsModule,
    CxCommonModule,
    SharedModule,
    MomentModule,
    FormsModule,
    MatTabsModule,
    AgGridModule.withComponents([
      CellDropdownMenuComponent,
      CellRemoveButtonActionComponent,
      HeaderCustomComponent
    ]),
    RouterModule.forChild([{ path: '', component: UserGroupListComponent }])
  ],
  exports: [UserGroupListComponent],
  entryComponents: [UserGroupsRemoveConfirmationComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [UserGroupsDataService]
})
export class UserGroupsModule {}
