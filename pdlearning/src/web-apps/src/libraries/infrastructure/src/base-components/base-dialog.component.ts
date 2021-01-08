import { BasePageComponent } from './base-page.component';
import { ModuleFacadeService } from '../services/module-facade.service';
import { WindowRef } from '@progress/kendo-angular-dialog';

export abstract class BaseDialogComponent extends BasePageComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected windowRef: WindowRef) {
    super(moduleFacadeService);
  }

  public close(): void {
    this.windowRef.close();
  }
}
