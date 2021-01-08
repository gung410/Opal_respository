import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import {
  FormAnswerApiService,
  FormAnswerModel,
  FormQuestionModel,
  FormSectionViewModel,
  FormType,
  IUpdateFormAnswerRequest,
  IUpdateFormQuestionAnswerRequest
} from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
@Component({
  selector: 'standalone-form-answer-review-dialog',
  templateUrl: './standalone-form-answer-review-dialog.component.html'
})
export class StandaloneFormAnswerReviewDialogComponent extends BaseComponent {
  @Input() public formQuestion: FormQuestionModel[];
  @Input() public formAnswer: FormAnswerModel;
  @Input() public formDataType: FormType;
  @Input() public sectionsQuestion: FormSectionViewModel[];

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    public dialogRef: DialogRef,
    private formAnswerApiService: FormAnswerApiService
  ) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    if (this.isScoreChanged()) {
      this.moduleFacadeService.modalService.showConfirmMessage(
        'There are some changes in the score. You will lose the changes. Are you sure you want to close?',
        () => {
          this.dialogRef.close();
        },
        null,
        null
      );
    } else {
      this.dialogRef.close();
    }
  }

  public onSave(): void {
    this.processUpdateScore();
  }

  public canShowSaveButton(): boolean {
    return this.formDataType === FormType.Quiz && this.formAnswer.isCompleted && this.formQuestion.filter(x => x.isScoreEnabled).length > 0;
  }

  public isScoreChanged(): boolean {
    return this.formAnswer.questionAnswers.filter(x => x.markedScore !== undefined && x.score !== x.markedScore).length > 0;
  }

  private processUpdateScore(): void {
    if (this.isScoreChanged()) {
      const questionAnswers: IUpdateFormQuestionAnswerRequest[] = [];
      this.formAnswer.questionAnswers
        .filter(x => x.markedScore !== undefined && x.score !== x.markedScore)
        .forEach(item => {
          questionAnswers.push({
            formQuestionId: item.formQuestionId,
            markedScore: item.markedScore
          });
        });
      const requestDto: IUpdateFormAnswerRequest = {
        formAnswerId: this.formAnswer.id,
        questionAnswers: questionAnswers
      };
      this.formAnswerApiService.updateFormAnswerScore(requestDto).subscribe(formAnswer => {
        this.formAnswer = formAnswer;
        this.showNotification(`The scores have been updated`);
      });
    }
  }
}
