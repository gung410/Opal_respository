import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CxCommonModule } from '@conexus/cx-angular-common';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from 'app/shared/shared.module';
import { MatSidenavModule } from '@angular/material/sidenav';

import { DepartmentHierarchicalComponent } from './department-hierarchical.component';
import { FilterDepartmentComponent } from './filter-department/filter-department.component';
import { SearchDepartmentHierarchicalComponent } from './search-department-hierarchical-dialog/search-department-hierarchical.component';

@NgModule({
  declarations: [
    DepartmentHierarchicalComponent,
    SearchDepartmentHierarchicalComponent,
    FilterDepartmentComponent
  ],
  imports: [
    SharedModule,
    CxCommonModule,
    NgbDropdownModule,
    MatSidenavModule,
    RouterModule.forChild([
      { path: '', component: DepartmentHierarchicalComponent },
      { path: '', component: SearchDepartmentHierarchicalComponent }
    ])
  ],
  entryComponents: [FilterDepartmentComponent],
  exports: [DepartmentHierarchicalComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class DepartmentHierarchicalModule {}
