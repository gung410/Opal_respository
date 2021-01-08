import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SystemAuditLogComponent } from 'app/system-audit-log/system-audit-log.component';

@NgModule({
  declarations: [SystemAuditLogComponent],
  imports: [
    RouterModule.forChild([{ path: '', component: SystemAuditLogComponent }])
  ],
  exports: [SystemAuditLogComponent],
  entryComponents: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class SystemAuditLogModule {}
