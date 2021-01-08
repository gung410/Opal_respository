import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import {
  CompareSide,
  IComparisonQuestionViewModel,
  ItemChangeType,
  QuestionChangeType,
  QuestionOptionChangeType
} from './dialogs/compare-version-form-dialog.component';
import { Component, Input } from '@angular/core';
import { QuestionAnswerValue, QuestionOptionType, QuestionType } from '@opal20/domain-api';

@Component({
  selector: 'form-question-comparison',
  templateUrl: './form-question-comparison.component.html'
})
export class FormQuestionComparisonComponent extends BaseComponent {
  @Input() public comparisonQuestionVM: IComparisonQuestionViewModel;
  @Input() public comparisonSide: CompareSide = CompareSide.Left;
  @Input() public isInSection: boolean = false;

  public QUESTIONTYPE = QuestionType;
  public ITEMCHANGETYPE = ItemChangeType;
  public QUESTIONCHANGETYPE = QuestionChangeType;
  public OPTIONCHANGETYPE = QuestionOptionChangeType;
  public readonly QUESTIONOPTIONTYPEENUM: typeof QuestionOptionType = QuestionOptionType;

  constructor(moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public isBlankArea(changType: QuestionChangeType[]): boolean {
    return (
      (this.comparisonSide === CompareSide.Right && changType.includes(QuestionChangeType.Removed)) ||
      (this.comparisonSide === CompareSide.Left && changType.includes(QuestionChangeType.AddNew))
    );
  }

  public isOptionblankArea(changType: QuestionOptionChangeType[]): boolean {
    return (
      (this.comparisonSide === CompareSide.Right && changType.includes(QuestionOptionChangeType.Removed)) ||
      (this.comparisonSide === CompareSide.Left && changType.includes(QuestionOptionChangeType.AddNew))
    );
  }

  public isMediaOptionblankArea(changType: QuestionOptionChangeType[]): boolean {
    return (
      (this.comparisonSide === CompareSide.Right && changType.includes(QuestionOptionChangeType.MediaOptionRemoved)) ||
      (this.comparisonSide === CompareSide.Left && changType.includes(QuestionOptionChangeType.MediaOptionAddNew))
    );
  }

  public getMediaUrl(meidaUrl: string): string {
    return meidaUrl ? `${AppGlobal.environment.cloudfrontUrl}/${meidaUrl}` : '';
  }

  public getDatePickerAnswer(answer: QuestionAnswerValue): Date {
    const dateAnswer = (answer as unknown) as Date;
    return dateAnswer ? new Date(dateAnswer) : null;
  }

  public getDateRangePickerAnswer(answer: QuestionAnswerValue, index: number): Date {
    if (!answer) {
      return null;
    }
    const dateAnswer = (answer[index] as unknown) as Date;
    return dateAnswer ? new Date(dateAnswer) : null;
  }

  public getQuestionTypeDisplay(type: QuestionType): string {
    switch (type) {
      case QuestionType.FillInTheBlanks:
        return 'Fill In The Blanks';
      case QuestionType.ShortText:
        return 'Free Text';
      case QuestionType.DatePicker:
        return 'Date Picker: One Date';
      case QuestionType.DateRangePicker:
        return 'Date Picker: Date Range';
      case QuestionType.SingleChoice:
        return 'Radio Buttons';
      case QuestionType.DropDown:
        return 'Drop down';
      case QuestionType.TrueFalse:
        return 'True / False';
      case QuestionType.MultipleChoice:
        return 'Checkboxs';
      default:
        return type.toString();
    }
  }
}
