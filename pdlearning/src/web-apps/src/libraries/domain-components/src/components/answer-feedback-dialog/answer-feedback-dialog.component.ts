import { BaseFormComponent, IFormBuilderDefinition, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';

import { DialogRef } from '@progress/kendo-angular-dialog';

// TODO: rename to the component and file to form-question-option-configuration
@Component({
  selector: 'answer-feedback-dialog',
  templateUrl: './answer-feedback-dialog.component.html'
})
export class AnswerFeedbackDialogComponent extends BaseFormComponent {
  @Input() public feedback: string = '';
  @Input() public branchingOptions: IDataItem[] = [];
  @Input() public allowConfigureFeedback: boolean = true;
  @Input() public allowConfigureBranching: boolean = true;
  @Input() public isViewMode: boolean = false;

  public isShowExplanation: boolean = false;
  public nextQuestionId: string = null;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    this.dialogRef.close();
  }

  public onSubmit(): void {
    this.dialogRef.close({ action: DialogFeedbackAction.Submit, nextQuestionId: this.nextQuestionId, feedback: this.feedback });
  }

  public toggleShowExplaination(): void {
    this.isShowExplanation = !this.isShowExplanation;
  }

  protected createFormBuilderDefinition(): IFormBuilderDefinition {
    return {
      formName: 'form',
      controls: {
        feedback: {
          defaultValue: null
        },
        nextQuestionId: {
          defaultValue: null
        }
      }
    };
  }
}

export enum DialogFeedbackAction {
  Submit = 'submit',
  Close = 'close'
}
