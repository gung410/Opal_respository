import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { Validators } from '@angular/forms';

@Component({
  selector: 'question-bank-add-dialog',
  templateUrl: './question-bank-add-dialog.component.html'
})
export class QuestionBankAddDialogComponent extends BaseFormComponent {
  public title: string = '';
  public questionGroupName: string = '';

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public async onSubmit(): Promise<void> {
    this.validate().then(valid => {
      if (valid) {
        this.dialogRef.close({ title: this.title, questionGroupName: this.questionGroupName });
      }
    });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        title: {
          defaultValue: null,
          validators: [
            {
              validator: Validators.required,
              validatorType: 'required'
            }
          ]
        }
      }
    };
  }
}
