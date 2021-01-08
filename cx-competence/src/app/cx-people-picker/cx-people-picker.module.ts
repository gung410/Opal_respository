import { AgGridModule } from '@ag-grid-community/angular';
import { CommonModule } from '@angular/common';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AgGridRendererModule } from 'app/shared/components/ag-grid-renderer/ag-grid-renderer.module';
import { SharedModule } from 'app/shared/shared.module';
import { CxPeopleListDialogComponent } from './cx-people-list-dialog/cx-people-list-dialog.component';
import { CxPeopleListComponent } from './cx-people-list/cx-people-list.component';
import { CxPeoplePickerDialogComponent } from './cx-people-picker-dialog/cx-people-picker-dialog.component';
import { CxPeoplePickerService } from './cx-people-picker.service';

@NgModule({
  declarations: [
    CxPeoplePickerDialogComponent,
    CxPeopleListComponent,
    CxPeopleListDialogComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    CxCommonModule,
    NgbModule,
    AgGridModule,
    AgGridRendererModule,
  ],
  providers: [CxPeoplePickerService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [CxPeoplePickerDialogComponent, CxPeopleListDialogComponent],
})
export class CxPeoplePickerModule {}
