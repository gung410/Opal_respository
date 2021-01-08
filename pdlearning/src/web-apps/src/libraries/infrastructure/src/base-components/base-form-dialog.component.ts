import { BaseFormComponent } from './base-form.component';
import { ModuleFacadeService } from '../services/module-facade.service';
import { WindowRef } from '@progress/kendo-angular-dialog';

export abstract class BaseFormDialogComponent extends BaseFormComponent {
  constructor(protected moduleFacadeService: ModuleFacadeService, protected windowRef: WindowRef) {
    super(moduleFacadeService);
  }

  public close(): void {
    this.windowRef.close();
  }
}
