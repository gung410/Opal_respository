import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { DialogAction, requiredIfValidator } from '@opal20/common-components';

import { Component } from '@angular/core';
import { DialogRef } from '@progress/kendo-angular-dialog';

@Component({
  selector: 'comment-dialog',
  templateUrl: './comment-dialog.component.html'
})
export class CommentDialogComponent extends BaseFormComponent {
  public title: string = '';
  public comment: string = '';
  public requiredCommentField: boolean = true;
  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onClose(): void {
    this.dialogRef.close({ action: DialogAction.Close });
  }

  public onProceed(): void {
    this.validate().then(valid => {
      if (valid) {
        this.dialogRef.close({ action: DialogAction.OK, comment: this.comment });
      }
    });
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        comment: {
          defaultValue: null,
          validators: [
            {
              validator: requiredIfValidator(() => this.requiredCommentField),
              validatorType: 'required'
            }
          ]
        }
      }
    };
  }
}
