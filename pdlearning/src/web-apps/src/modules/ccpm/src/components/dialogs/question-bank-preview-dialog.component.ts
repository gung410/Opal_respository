import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { FormQuestionModel } from '@opal20/domain-api';

@Component({
  selector: 'question-bank-preview-dialog',
  templateUrl: './question-bank-preview-dialog.component.html'
})
export class QuestionBankPreviewDialogComponent extends BaseComponent {
  @Input() public question: FormQuestionModel;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }
}
