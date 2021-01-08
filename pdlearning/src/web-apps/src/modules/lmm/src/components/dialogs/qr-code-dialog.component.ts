import { BaseFormComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { ClassRun, Session, SessionRepository } from '@opal20/domain-api';

import { Component } from '@angular/core';
import { DialogAction } from '@opal20/common-components';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'qr-code-dialog',
  templateUrl: './qr-code-dialog.component.html'
})
export class QRCodeDialogComponent extends BaseFormComponent {
  public sessionCode: string = '';
  public session: Session = new Session();
  public classRun: ClassRun = new ClassRun();
  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef, private sessionRepository: SessionRepository) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  protected onInit(): void {
    this.sessionRepository
      .loadSessionCodeById(this.session.id)
      .pipe(this.untilDestroy())
      .subscribe(session => {
        this.sessionCode = session.sessionCode;
      });
  }
}
