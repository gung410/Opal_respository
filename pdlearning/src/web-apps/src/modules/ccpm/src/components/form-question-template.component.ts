import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input } from '@angular/core';
import { FormType, IQuestionBankSelection, QuestionBankViewModel, QuestionType } from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { OpalDialogService } from '@opal20/common-components';
import { QuestionBankImportDialogComponent } from './dialogs/question-bank-import-dialog.component';
import { QuestionDateOptionSelectionDialogComponent } from './dialogs/question-date-option-selection-dialog.component';
import { QuestionTypeSelectionService } from '../services/question-type-selection.service';

@Component({
  selector: 'form-question-template',
  templateUrl: './form-question-template.component.html'
})
export class FormQuestionTemplateComponent extends BaseComponent {
  public readonly formType: typeof FormType = FormType;
  public readonly questionTypeEnum: typeof QuestionType = QuestionType;

  @Input() public type: FormType = FormType.Quiz;
  @Input() public priority: number;
  @Input() public minorPriority: number;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private questionTypeSelectionService: QuestionTypeSelectionService,
    private opalDialogService: OpalDialogService
  ) {
    super(moduleFacadeService);
  }

  public onDateQuestionTemplateClicked(): void {
    const dateQuestionDialogRef = this.moduleFacadeService.dialogService.open({
      content: QuestionDateOptionSelectionDialogComponent
    });
    dateQuestionDialogRef.content.instance.priority = this.priority;
    dateQuestionDialogRef.content.instance.minorPriority = this.minorPriority;
  }

  public onQuestionTemplateClick(newQuestionType: QuestionType): void {
    if (newQuestionType) {
      this.questionTypeSelectionService.setNewQuestionType(newQuestionType, this.priority, this.minorPriority);
    }
  }

  public showImportQuestionDialog(): void {
    let questionType = [QuestionType.SingleChoice, QuestionType.MultipleChoice];
    const quizSurveyQuestionTypes = [
      QuestionType.ShortText,
      QuestionType.TrueFalse,
      QuestionType.DropDown,
      QuestionType.DatePicker,
      QuestionType.DateRangePicker
    ];

    if (this.type === FormType.Quiz) {
      questionType = [...questionType, ...quizSurveyQuestionTypes, QuestionType.FreeResponse, QuestionType.FillInTheBlanks];
    } else if (this.type === FormType.Survey) {
      questionType = [...questionType, ...quizSurveyQuestionTypes];
    }

    const dialogRef: DialogRef = this.opalDialogService.openDialogRef(
      QuestionBankImportDialogComponent,
      {
        questionType: questionType
      },
      {
        padding: '0px'
      }
    );

    dialogRef.result.toPromise().then((questionBankImport: QuestionBankViewModel[]) => {
      if (questionBankImport && questionBankImport.length) {
        const dataImport: IQuestionBankSelection = {
          listQuestion: questionBankImport,
          priority: this.priority,
          minorPriority: this.minorPriority
        };
        this.questionTypeSelectionService.setQuestionListImport(dataImport);
      }
    });
  }
}
