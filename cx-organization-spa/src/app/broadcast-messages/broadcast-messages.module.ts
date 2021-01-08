import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AgGridModule } from 'ag-grid-angular';
import { MomentModule } from 'angular2-moment';
import { SharedModule } from 'app/shared/shared.module';

import { BroadcastMessagesDetailComponent } from './broadcast-messages-detail/broadcast-messages-detail.component';
import { BroadcastMessagesListComponent } from './broadcast-messages-list/broadcast-messages-list.component';
import { CellBroadcastDropdownMenuComponent } from './broadcast-messages-list/cell-dropdown-menu/cell-broadcast-dropdown-menu.component';
import { HeaderCustomComponent } from './broadcast-messages-list/header-custom-action/header-custom.component';
import { BroadcastMessagesComponent } from './broadcast-messages.component';

import { MAT_DATE_LOCALE } from '@angular/material/core';
import {
  MAT_DIALOG_DEFAULT_OPTIONS,
  MatDialogModule
} from '@angular/material/dialog';
import { FormBuilderService } from 'app-services/form-builder.service';
import { DepartmentHierarchicalService } from 'app/department-hierarchical/department-hierarchical.service';
import { ConfirmDialogComponent } from 'app/shared/components/confirm-dialog/confirm-dialog.component';
import { HighlightDirective } from 'app/shared/directives/highlight.directive';
import { UserAccountsDataService } from 'app/user-accounts/user-accounts-data.service';
import { UserGroupsDataService } from 'app/user-groups/user-groups-data.service';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { BroadcastMessageStatusComponent } from './broadcast-message-status/broadcast-message-status.component';
import { RecurringMessageComponent } from './broadcast-messages-detail/recurring-message/recurring-message.component';
import { BroadcastMessageRecurringDialogComponent } from './broadcast-messages-recurring/broadcast-messages-recurring.component';
import { CellBroadcastMessageStatusComponent } from './cell-components/cell-user-status/cell-broadcast-message-status.component';
import { DepartmentsDialogComponent } from './departments-dialog/departments-dialog.component';
import { BroadcastMessagesApiService } from './services/broadcast-messages-api.service';
import { BroadcastMessagesService } from './services/broadcast-messages.service';

@NgModule({
  declarations: [
    BroadcastMessagesComponent,
    CellBroadcastDropdownMenuComponent,
    HeaderCustomComponent,
    BroadcastMessagesListComponent,
    CellBroadcastMessageStatusComponent,
    BroadcastMessageStatusComponent,
    BroadcastMessagesDetailComponent,
    BroadcastMessageRecurringDialogComponent,
    RecurringMessageComponent,
    DepartmentsDialogComponent,
    HighlightDirective
  ],
  imports: [
    CommonModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatRadioModule,
    MatInputModule,
    NgxMaterialTimepickerModule,
    NgbDropdownModule,
    CxCommonModule,
    FormsModule,
    AgGridModule.withComponents([
      CellBroadcastDropdownMenuComponent,
      HeaderCustomComponent
    ]),
    MatSelectModule,
    MatIconModule,
    MatDatepickerModule,
    SharedModule,
    MomentModule,
    MatDialogModule,
    RouterModule.forChild([
      { path: '', component: BroadcastMessagesComponent },
      {
        path: 'detail/:broadcastMessageId',
        component: BroadcastMessagesDetailComponent
      }
    ])
  ],
  exports: [BroadcastMessagesComponent],
  entryComponents: [
    BroadcastMessageRecurringDialogComponent,
    DepartmentsDialogComponent,
    ConfirmDialogComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  providers: [
    BroadcastMessagesApiService,
    BroadcastMessagesService,
    DepartmentHierarchicalService,
    UserAccountsDataService,
    UserGroupsDataService,
    FormBuilderService,
    { provide: MAT_DATE_LOCALE, useValue: 'en-GB' },
    { provide: MAT_DIALOG_DEFAULT_OPTIONS, useValue: { hasBackdrop: false } }
  ]
})
export class BroadcastMessagesModule {}
