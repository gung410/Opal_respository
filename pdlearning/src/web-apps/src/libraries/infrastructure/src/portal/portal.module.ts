import { CustomPortal, PortalOutlet } from './portal-directives';
import { NgModule, Type } from '@angular/core';

const PORTAL_COMPONENTS: Type<unknown>[] = [CustomPortal, PortalOutlet];

@NgModule({
  declarations: [...PORTAL_COMPONENTS],
  exports: [...PORTAL_COMPONENTS]
})
export class PortalModule {}
