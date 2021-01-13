import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { TranslateModule } from '@ngx-translate/core';
import { AuthGuardService } from 'app-authguards/auth-guard.service';
import { NotificationDataService } from 'app-services/notification-data.service';
import { TranslateAdapterService } from 'app-services/translate-adapter.service';
import {
  OpalLabelTemplateDirective,
  OpalOptionTemplateDirective
} from './components/select/templates.directives';

import { CellDropdownActionComponent } from './components/cell-dropdown-action/cell-dropdown-action.component';
import { CellHeaderCustomComponent } from './components/cell-header-custom/cell-header-custom.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { DataCheckIndicatorComponent } from './components/data-check-indicator/data-check-indicator.component';
import { IconButtonComponent } from './components/icon-button/icon-button.component';
import { LanguageSelectorComponent } from './components/language-selector/language-selector.component';
import { LoaderComponent } from './components/loader/loader.component';
import { PeoplePickerComponent } from './components/people-picker/people-picker.component';
import { OpalSelectComponent } from './components/select/select.component';
import { OpalTextareaComponent } from './components/textarea/textarea.component';
import { FocusDirective } from './directives/focus.directive';
import { PermissionDirective } from './directives/permission.directive';

@NgModule({
  declarations: [
    LoaderComponent,
    IconButtonComponent,
    LanguageSelectorComponent,
    DataCheckIndicatorComponent,
    PeoplePickerComponent,
    ConfirmDialogComponent,
    OpalSelectComponent,
    OpalTextareaComponent,
    FocusDirective,
    CellHeaderCustomComponent,
    CellDropdownActionComponent,
    PermissionDirective,
    OpalOptionTemplateDirective,
    OpalLabelTemplateDirective
  ],
  imports: [
    NgSelectModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    NgbDropdownModule
  ],
  providers: [
    AuthGuardService,
    TranslateAdapterService,
    NotificationDataService
  ],
  exports: [
    CommonModule,
    OpalSelectComponent,
    OpalTextareaComponent,
    FocusDirective,
    PermissionDirective,
    FormsModule,
    TranslateModule,
    OpalOptionTemplateDirective,
    RouterModule,
    ConfirmDialogComponent,
    LoaderComponent,
    IconButtonComponent,
    LanguageSelectorComponent,
    DataCheckIndicatorComponent,
    PeoplePickerComponent
  ],
  entryComponents: [CellHeaderCustomComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class SharedModule {}
